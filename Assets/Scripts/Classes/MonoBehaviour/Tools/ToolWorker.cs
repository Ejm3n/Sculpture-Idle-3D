using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolWorker : ToolBase
{
    [SerializeField] private ToolWorkerAnimationController animationController;
    [SerializeField] private Transform specialHitPosition;

    public override void Setup(PillarObject pillar, ToolUpgrade upgrade, int index, int globalIndex)
    {
        base.Setup(pillar, upgrade, index, globalIndex);
        //animationController.SetTimeToAction(property.TimeToAction);
        animationController.PlayIdle();
        animationController.OnAnimationAction += SpecialAction;
        pillar.OnSegmentPositionChange += OnSegmentChange;
    }
    private void OnDestroy()
    {
        animationController.OnAnimationAction -= SpecialAction;
        pillar.OnSegmentPositionChange -= OnSegmentChange;
    }
    private void OnSegmentChange(Vector3 from, Vector3 to)
    {
        specialHitCount = 0;
    }
    //protected override Vector3 SpecialActionPosition()
    //{
    //    return specialHitPosition.position;
    //}
    public override void Install()
    {
        base.Install();
        animationController.PlayAppear();
        //float addTime = animationController.GetAppearLength() + animationController.GetTimeForActionEvent();
        //SetActionTimer(addTime);
    }
    public override void Execute() 
    {
        base.Execute();
        animationController.PlayAction();
        //float addTime = animationController.GetTimeForActionEvent();
        //SetActionTimer(addTime);
    }
    public override void Finish()  
    {
        base.Finish();
        animationController.PlayWin();
    }
    //public override void Upgrade()
    //{
    //    base.Upgrade();
    //    //animationController.SetTimeToAction(property.TimeToAction);
    //}
    public override void Wakeup()
    {
        if (inSleep)
        {
            base.Wakeup();
            animationController.PlayAppear();
        }
    }
    protected override bool Sleep()
    {       
        if (base.Sleep())
        {
            animationController.PlaySleep();
            return true;
        }
        return false;
    }
}
