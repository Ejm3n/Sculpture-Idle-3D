using BG.UI.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BG.UI.Camera;

public class WhipSystem : MonoBehaviour
{
    protected class Target
    {
        private bool inAction, canMove = true;
        private WhipSystem whip;
        private ToolGrid.Cell cell;
        private WhipObject obj;
        private Vector3 uiPos;
        private Camera camera;
        private Vector3 offset;
        public Target(WhipSystem whip, ToolGrid.Cell cell, WhipObject obj, Vector3 offset, Vector3 uiButtonPosition)
        {
            this.whip = whip;
            this.cell = cell;
            this.obj = obj;
            uiPos = uiButtonPosition;
            camera = CinemachineBrain.Default.Camera;
            this.obj.gameObject.SetActive(false);
            this.obj.transform.localScale = Vector3.one * 0.15f;
            this.offset = offset;
        }

        public void Play()
        {
            if (!inAction && cell.hasTool)
            {
                cell.tool.PrepareWakeup();
                inAction = true;

                Transform tt = cell.tool.transform;
                Vector3 extents = cell.tool.Extents;

                obj.transform.DOKill();
                obj.gameObject.SetActive(true);
                obj.transform.localScale = Vector3.one;
                Vector3 sp = camera.ScreenToWorldPoint(uiPos) + camera.transform.up * offset.y + camera.transform.right * offset.x;
                Quaternion sr = 
                    Quaternion.LookRotation(camera.transform.forward, camera.transform.up) *
                    Quaternion.AngleAxis(-90f, Vector3.right) * Quaternion.AngleAxis(22.5f, Vector3.up);



                // Vector3 sp = obj.transform.position;
                //Quaternion sr = obj.transform.rotation;
                Vector3 tp = tt.position - tt.forward* Mathf.Min(extents.x, extents.z)*4.5f + Vector3.up * extents.y * 0.4f;
                Quaternion tr = Quaternion.LookRotation(camera.transform.forward, camera.transform.up) * obj.Rotation;
                Sequence sequence = DOTween.Sequence();
                //sequence.Append(obj.transform.DORotateQuaternion(tr, 0.2f));
                //sequence.Append(obj.transform.DOMove(tp,0.25f).OnComplete(() => { obj.Play(); }));

                sequence.Append(DOTween.To(() => 0f, (v) => 
                {
                    //obj.transform.localScale = Vector3.Lerp(Vector3.one*0.5f, Vector3.one, v);
                    obj.transform.position = Vector3.Lerp(sp,tp,v);
                    obj.transform.rotation = Quaternion.Lerp(sr, tr, v);
                }
                , 1f, 0.25f).OnComplete(() => { obj.Play(); }));

                sequence.Append(DOTween.To(() => 0f, (v) => { }, 0f, 0.45f).OnComplete(() => { Whip(); }));

                //sequence.Append(obj.transform.DORotateQuaternion(sr*Quaternion.AngleAxis(-85f,Vector3.up),0.3f));
                // sequence.Append(obj.transform.DORotateQuaternion(sr*Quaternion.AngleAxis(85f, Vector3.up), 0.10f).OnComplete(() => { Whip(); }));
                sequence.Append(DOTween.To(() => 0f, (v) => { }, 0f, 0.5f));
                sequence.OnComplete(() =>
                {
                    canMove = false;
                    inAction = false;
                   // if (cell.tool.InSleep)
                   //     Select();
                   // else
                        Deselect();
                }
                );
            }
        }
        private void Whip()
        {
            if (cell.hasTool)
            {
                cell.tool.Wakeup();
                whip.PlayEmojiEffects(cell.tool);
                whip.PlayPunchEffects(cell.tool);
                whip.PlaySound(cell.tool);
            }
            
        }

        public void Select()
        {
          //  if (cell.hasTool && cell.tool.InSleep)
           // {
          //      obj.transform.DOKill();
           //     obj.gameObject.SetActive(true);
           //     obj.transform.DOScale(0.33f, 0.2f);
           // }
        }
        public void Deselect()
        {
            if (!inAction )
            {
                //obj.transform.localScale = Vector3.one*0.5f;
                obj.transform.DOScale(0.15f, 0.2f).OnComplete(() => 
                { 
                    obj.gameObject.SetActive(false); 
                    canMove = true; 
                    if(cell.hasTool && !cell.tool.InSleep && cell.tool.InPrepareWakeup)
                        cell.tool.PrepareWakeup(false);

                });
            }
        }
        public void Update()
        {
            if (!inAction && canMove)
            {
                obj.transform.rotation = 
                    Quaternion.LookRotation(camera.transform.forward, camera.transform.up)*
                    Quaternion.AngleAxis(-90f,Vector3.right) * Quaternion.AngleAxis(22.5f, Vector3.up);
                obj.transform.position = Vector3.Lerp(obj.transform.position, camera.ScreenToWorldPoint(uiPos)+ camera.transform.up*offset.y + camera.transform.right * offset.x, 35f * Time.deltaTime);
            }
        }
    }

    private LevelMaster levelMaster;
    private ToolController toolController;
    private UISleepSubPanel sleepSubPanel;
    //[SerializeField] private Vector3 offset;
    [SerializeField] private WhipObject[] whipVariants;
    private WhipObject whipPrefab;
    //[SerializeField] private string sound = "Whip";
    [SerializeField] private EffectDynamic[] effects;

    private Target[] targets;
    private int active;

    void Start()
    {
        whipPrefab = whipVariants[GameData.Default.useAltWhip ? 1 : 0];

        var process = UIManager.Default[UIState.Process] as UIProcessPanel;
        sleepSubPanel = process.ToolPanel.SleepPanel;
        sleepSubPanel.CanWakeup = false;
        sleepSubPanel.OnWakeup += OnWakeup;
        sleepSubPanel.OnSleep += OnSleep;
        levelMaster = LevelManager.Default.CurrentLevel;
        toolController = levelMaster.ToolController;
        toolController.OnChange += OnChange;
        var grid = toolController.Grid;
        targets = new Target[grid.Count];
        for (int i = 0; i < grid.Count; i++)
        {
            targets[i] = new Target(
                this, 
                grid[i],
                Instantiate(whipPrefab,transform),
                whipPrefab.Offset,
                sleepSubPanel.WakeupButton.transform.position + Vector3.forward * whipPrefab.Offset.z);
        }


        OnChange(toolController.Selected);
    }
    private void OnDestroy()
    {
        if (sleepSubPanel != null)
            sleepSubPanel.OnWakeup -= OnWakeup;
        if (toolController != null)
            toolController.OnChange -= OnChange;

    }
    public void PlayEmojiEffects(ToolBase tool)
    {
        int rand = Random.Range(0, effects.Length);
        var p = PoolManager.Default.Pop(effects[rand], null);
        p.SetPosition(Vector3.up * tool.Extents.y*2.25f + tool.transform.position);

    }
    public void PlayPunchEffects(ToolBase tool)
    {
        Vector3 extents = tool.Extents;
        var p = PoolManager.Default.Pop(whipPrefab.Effect, null);
        p.SetPosition(Vector3.up * extents.y*0.4f -
            tool.transform.forward* Mathf.Min(extents.x, extents.z)*0.5f + 
            tool.transform.position);

    }
    private void PlaySound(ToolBase tool)
    {
        if (toolController.ActiveIndex == tool.Index)
        {
            SoundHolder.Default.PlayFromSoundPack(whipPrefab.Sound);
        }
        else if (toolController.NextIndex == tool.Index || toolController.PrevIndex == tool.Index)
        {
            var s = SoundHolder.Default.PlayFromSoundPack(whipPrefab.Sound);
            s.volume = s.volume * 0.5f;
        }
    }
    private void OnWakeup()
    {
        targets[active].Play();
    }
    private void OnSleep()
    {
        targets[active].Select();

    }
    private void OnChange(ToolGrid.Cell cell)
    {
        if (active != toolController.ActiveIndex)
        {
            targets[active].Deselect();
            active = toolController.ActiveIndex;
            targets[active].Select();
        }
    }
    private void LateUpdate()
    {
        //if (levelMaster.State == LevelMaster.LevelState.process) 
        //{
        //    for (int i = 0; i < targets.Length; i++)
        //    {
        //        targets[i].Update();
        //    }
        //}
        //else 
        if(levelMaster.State == LevelMaster.LevelState.win)
        {
            for (int i = 0; i < targets.Length; i++)
                targets[i].Deselect();
            enabled = false;
        }
    }
}
