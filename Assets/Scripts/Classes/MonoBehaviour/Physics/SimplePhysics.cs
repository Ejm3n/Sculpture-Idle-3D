using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePhysics
{
    private Transform target;
    private float deceleration = 5f;
    private float garivtyScale = 1f;
    private Vector3 force;
    private Vector3 garivty;
    private Vector3 torque;
    private int bounceCount;
    private bool hasTimer;
    private float nextTime;
    private bool canCollide = true;
    public Transform Target => target;
    public SimplePhysics(Transform target, float deceleration, float garivtyScale)
    {
        this.target = target;
        this.deceleration = deceleration;
        this.garivtyScale = garivtyScale;
    }

    public virtual void AddForce(Vector3 force, Vector3 torque)
    {
        this.force = force;
        this.torque = torque;
    }
    public void Simulate()
    {
        force = Vector3.MoveTowards(force, Vector3.zero, deceleration * Time.deltaTime);
        garivty += Physics.gravity * garivtyScale * Time.deltaTime;
        target.rotation = target.rotation * Quaternion.Euler(Time.deltaTime * torque);
        target.Translate((force + garivty) * Time.deltaTime, Space.World);
        Vector3 pos = target.position;
        if (pos.y <= GameData.Default.fragFloorYPosSimplePhysics && canCollide)
        {
            if (bounceCount < GameData.Default.fragMaxBounceCountSimplePhysics)
            {
                pos.y = GameData.Default.fragFloorYPosSimplePhysics + 0.05f;
                bounceCount++;              
                force.y = -garivty.y * 0.5f;
                garivty.y = 0;
                target.position = pos;
            }
            else
            {
                force.y = garivty.y = 0;
                pos.y = GameData.Default.fragFloorYPosSimplePhysics;
                target.position = pos;
                torque = Vector3.MoveTowards(torque, Vector3.zero, deceleration * 360f * Time.deltaTime);
            
                if(!hasTimer)
                {
                    hasTimer = true;
                    nextTime = Time.time + GameData.Default.fragTimeToDisableSimplePhysics;
                }
                else
                {
                    if (Time.time >= nextTime)
                    {
                        canCollide = false;
                    }
                }
            
            }
        }
    }
}
