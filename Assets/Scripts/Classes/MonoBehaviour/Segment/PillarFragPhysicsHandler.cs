using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarFragPhysicsHandler : PillarFragDetachHandler
{
    [SerializeField] private float power = 1f;
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private float garivtyScale = 1f;
    private LinkedList<SimplePhysics> physics = new LinkedList<SimplePhysics>();
    protected override void FragDetach(Rigidbody frag)
    {

        if (LevelManager.Default.CurrentLevel.State == LevelMaster.LevelState.process)
        {
            SimplePhysics phy = new SimplePhysics(frag.transform, deceleration, garivtyScale);

            Vector3 direction = (frag.transform.position - transform.position);
            direction.y = frag.transform.position.y;
            direction.Normalize();
            direction = (direction + Random.onUnitSphere + Vector3.up * 0.75f).normalized;
            phy.AddForce(direction * power * Random.Range(0.8f, 1.2f), Random.insideUnitSphere * 720.0f);
            physics.AddLast(phy);

            if (!enabled)
            {
                enabled = true;
            }
        }
        else
        {
            base.FragDetach(frag);
        }
    }

    private void LateUpdate()
    {
        if (physics.Count > 0)
        {

            int counter = 0;
            var iter = physics.First;
            while (counter < physics.Count)
            {
                iter.Value.Simulate();
                counter++;            
                if (iter.Value.Target.position.y <= GameData.Default.fragYPosToDisableSimplePhysics)
                {
                    Transform frag = iter.Value.Target;
                    var tmp = iter;
                    iter = iter.Next;
                    physics.Remove(tmp);
                    frag.gameObject.SetActive(false);
                }
                else
                {
                    iter = iter.Next;
                }
            }
        }
        else
            enabled = false;

    }
}
