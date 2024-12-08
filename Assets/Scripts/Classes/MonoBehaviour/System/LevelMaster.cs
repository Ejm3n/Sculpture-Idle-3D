using BG.UI.Camera;
using BG.UI.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaster : MonoBehaviour
{
    public enum LevelState
    {
        none,
        start,
        process,
        win,
        over
    }

    private LevelState state;

    [SerializeField] private ResourcePrefabReference pillarPrefab;
    [SerializeField] private GameObject sculpturePrefab;
    [SerializeField] private CameraSystem cameras;
    [SerializeField] private ToolController toolController;
    private bool hasPillar, hasTool;
    private PillarObject pillar;
    [SerializeField,HideInInspector] private GameObject sculpture;
    [SerializeField,HideInInspector] private Vector3 startCameraPos;
    public LevelState State => state;
    public ToolController ToolController => toolController;
    public GameObject Sculpture => sculpture;
    public System.Action OnWin;
    private void Start()
    {

    }
    private void OnDestroy()
    {
        if(hasPillar)
        {
            pillar.OnPillarFinish -= GameWin;
            pillar.OnHurt -= OnPillarHurt;

        }
        toolController.OnAdd -= OnToolAdd;

    }
    public void Initialize()
    {
        if (LoadPrefabFromResources())
        {
            if (Application.isPlaying)
            {
                toolController.SetPillar(pillar);
                toolController.Initialize();
                toolController.OnAdd += OnToolAdd;
                if (PartyManager.Default != null)
                {
                    PartyManager.Default.LoadParty(toolController);
                    PartyManager.Default.LoadPillar(pillar);
                }
                toolController.Selected.Select();
            }
            SetupSculpture();
            startCameraPos = cameras.StartCamera.transform.position;
            SetupStartCamera();
        }
        
    }
    public void ForceInitialize()
    {
        pillar = GetComponentInChildren<PillarObject>();
        hasPillar = pillar != null;
        pillar.AddHealth((LevelManager.Default.CurrentLevelCount - 1) * GameData.Default.pillarHealthIncrease);
        pillar.Init();
        pillar.OnPillarFinish += GameWin;
        pillar.OnHurt += OnPillarHurt;
        toolController.SetPillar(pillar);
        toolController.Initialize();
        toolController.OnAdd += OnToolAdd;
        if (PartyManager.Default != null)
        {          
            PartyManager.Default.LoadParty(toolController);
            PartyManager.Default.LoadPillar(pillar);
        }
        toolController.Selected.Select();
        sculpture.transform.position = pillar.GetTopSegmentPosition();
        SetupStartCamera();

    }
    private bool LoadPrefabFromResources()
    {
        pillar = Instantiate(pillarPrefab.Get<PillarObject>(),transform);
        if (Application.isPlaying)
            pillar.AddHealth((LevelManager.Default.CurrentLevelCount - 1) * GameData.Default.pillarHealthIncrease);
        pillar.Init();
        pillar.OnPillarFinish += GameWin;
        pillar.OnHurt += OnPillarHurt;
        return hasPillar = pillar != null;
    }
    private void SetupStartCamera()
    {
        Transform startCamera = cameras.StartCamera.transform;
        Vector3 pos = startCameraPos;
        pos.y += pillar.GetTopSegmentPosition().y;
        startCamera.position = pos;
    }
    private void SetupSculpture()
    {
        sculpture = Instantiate(sculpturePrefab, pillar.transform);
        sculpture.transform.position = pillar.GetTopSegmentPosition();
    }
    public bool GetPillar(out PillarObject pillar)
    {
        pillar = this.pillar;
        return hasPillar;
    }


    public void GameStart()
    {
        state = LevelState.start;
        toolController.enabled = false;
        enabled = true;
        PartyManager.Default.SaveParty(toolController);
        PartyManager.Default.SavePillar(pillar);
        PartyManager.Default.SaveTime();
        //if (PartyManager.Default != null)
        //{
        //    PartyManager.Default.LoadPillar(pillar);
        //}

    }
    public void GameWin()
    {
        state = LevelState.win;
        UIManager.Default[UIState.Process].HidePanel();
        cameras.CurentState = CameraState.Win;
        toolController.Finish();
        PartyManager.Default.ClearPillar();
        OnWin?.Invoke();
    }
    public void OnPillarHurt()
    {
        PartyManager.Default.SavePillar(pillar);
        PartyManager.Default.SaveIncome(toolController);
        PartyManager.Default.SaveTime();
    }
    public void OnToolChange()
    {
        PartyManager.Default.SaveParty(toolController);
        PartyManager.Default.SaveIncome(toolController);

    }
    public void OnToolAdd(ToolBase tool)
    {
        PartyManager.Default.SaveParty(toolController);
        PartyManager.Default.SaveIncome(toolController);

    }
    //public void GameOver()
    //{
    //    state = LevelState.over;
    //    UIManager.Default.CurentState = UIState.Fail;
    //    cameras.CurentState = CameraState.Fail;
    //    toolController.enabled = false;

    //}
    private bool CheckStart()
    {
        Vector3 dist =
            cameras[CameraState.Process].
            transform.position -
            CinemachineBrain.Default.transform.position;
        if (dist.sqrMagnitude <= 0.1f)
        {
            state = LevelState.process;
            LevelManager.Default.StartLevel(pillar.Percent == 1f);
            toolController.enabled = true;
            return true;
        }
        return false;
    }
    private void LateUpdate()
    {
        if (state != LevelState.start || CheckStart())
            enabled = false;
    }
}
