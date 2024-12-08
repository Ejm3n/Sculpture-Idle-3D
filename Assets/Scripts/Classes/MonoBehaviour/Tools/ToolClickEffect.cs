using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolClickEffect : MonoBehaviour
{
    [SerializeField] private ToolCursor tool;
    [SerializeField] private EffectDynamic dynamic;
    [SerializeField] private EffectInfoDynamic icome;
    [SerializeField] private string sound = "ImpactClick";

    private void Start()
    {
        tool.OnAction += Spawn;
    }
    private void OnDestroy()
    {
        tool.OnAction -= Spawn;
    }
    public void Spawn()
    {
       var ipo = PoolManager.Default.Pop(dynamic,null);
        var ipo2 = PoolManager.Default.Pop(icome, null) as EffectInfoDynamic;
        Camera cam = CinemachineBrain.Default.Camera;
        Vector3 pos2 = cam.ScreenToWorldPoint(Input.mousePosition+ Vector3.forward* tool.input) + cam.transform.forward;
        ipo.SetPosition(pos2);
        ipo2.SetPosition(pos2);
        ipo2.SetText(MoneyService.AmountToString(tool.Property.Income));

        SoundHolder.Default.PlayFromSoundPack(sound);

    }
}
