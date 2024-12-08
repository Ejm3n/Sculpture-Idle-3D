using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarSegmentMover : MonoBehaviour
{
    [SerializeField] private Rigidbody rig;
    private PillarObject pillar;
    private bool hasMove;
    private Vector3 target;
    private bool hasPillar;
    private void Start()
    {
        pillar = GetComponent<PillarObject>();
        pillar.OnSegmentPositionChange += StarMove;
        enabled = false;
        hasPillar = true;
    }
    private void OnDestroy()
    {
        if(hasPillar)
        pillar.OnSegmentPositionChange -= StarMove;
    }
    protected virtual void StarMove(Vector3 from, Vector3 to)
    {
        Vector3 delta = from - to;
        target = rig.position + delta;
        if (LevelManager.Default.CurrentLevel.State == LevelMaster.LevelState.process)
        {
            hasMove = true;
            enabled = true;
            pillar.Damageable = false;
        }
        else
        {
            rig.position = target;
        }
    }
    private void OnMoveFinish()
    {
        rig.position = target;
        hasMove = false;
        enabled = false;
        pillar.Damageable = true;
    }
    private void Move()
    {
        if (hasMove)
        {
            Vector3 pos = rig.position;
            rig.position = Vector3.MoveTowards(
                pos,
                target,
                GameData.Default.pillarSegmentsMoveSpeed * Time.deltaTime);
            if ((pos - target).sqrMagnitude <= 0.01)
            {
                OnMoveFinish();
            }
        }
    }
    private void LateUpdate()
    {
        Move();
    }
}

