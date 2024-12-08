using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PillarFragRigidbodyHandler : PillarFragDetachHandler
{


    protected class TimeHolder
    {
       private Rigidbody go;
        private float nextHide;

        public TimeHolder(Rigidbody go, float time)
        {
            this.go = go;
            nextHide = Time.time + time;
        }

        public bool Check()
        {
            return Time.time >= nextHide;
        }
        public void Hide()
        {
            go.isKinematic = true;
            go.detectCollisions = false;
            go.transform.DOScale(0.1f, 0.25f).OnComplete(() => go.gameObject.SetActive(false));
            //go.SetActive(false);
        }
    }

    [SerializeField] private float power = 1f;
    private LinkedList<TimeHolder> physics = new LinkedList<TimeHolder>();
    protected override void FragDetach(Rigidbody frag)
    {

        if (LevelManager.Default.CurrentLevel.State == LevelMaster.LevelState.process)
        {
            TimeHolder th = new TimeHolder(frag, GameData.Default.fragTimeToDisableSimplePhysics);
            frag.transform.localScale = frag.transform.localScale * 0.5f;
            Vector3 direction = (frag.transform.position - transform.position);
            direction.y = frag.transform.position.y;
            direction.Normalize();
            direction = (direction + Random.onUnitSphere + Vector3.up * 0.75f).normalized;

            frag.isKinematic = false;
            frag.AddForce(direction * power * Random.Range(0.8f, 1.2f),ForceMode.VelocityChange);
            frag.AddTorque(Random.insideUnitSphere * 720.0f,ForceMode.VelocityChange);
            
            physics.AddLast(th);

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
                counter++;
                if (iter.Value.Check())
                {
                    iter.Value.Hide();
                    var tmp = iter;
                    iter = iter.Next;
                    physics.Remove(tmp);
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
