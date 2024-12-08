using BG.UI.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraVelocity : MonoBehaviour
{
    protected class Target
    {
        private Transform transform;
        private Vector3 direction;
        private Quaternion startRotation = Quaternion.identity;
        private float distance;
        public Target(Transform transform, Vector3 point)
        {
            this.transform = transform;
            startRotation = transform.rotation;
            Vector3 delta = (transform.position - point);
            direction = delta.normalized;
            distance = delta.magnitude;
        }

        public void Rotate(Quaternion rotation)
        {
            Vector3 position = (rotation * (direction * distance));
            transform.position = position;
            transform.rotation = rotation * startRotation;
        }
    }
    private bool hasLevelMaster;
    private LevelMaster levelMaster;
    private ToolController toolController;
    private PillarObject pillar;

    private bool inDirectRotation;
    private float velocity;
    private int currentCell;
    private float currentCellFloat;
    private float currentAngle;
    private bool hasLimit;
    private int limit;
    private bool block;

    Quaternion rotation;
    private Target[] targets = new Target[2];

    private void Start()
    {
        enabled = false;
    }
    public void SetLevelMaster(LevelMaster levelMaster)
    {
        this.levelMaster = levelMaster;
        hasLevelMaster = true;
        levelMaster = LevelManager.Default.CurrentLevel;
        toolController = levelMaster.ToolController;
        levelMaster.GetPillar(out pillar);
        var cameras = CameraSystem.Default;
        targets[0] = new Target(cameras[CameraState.Process].transform, levelMaster.transform.position);
        targets[1] = new Target(cameras[CameraState.Win].transform, levelMaster.transform.position);
        enabled = false;
        currentCell = 0;
        currentCellFloat = 0f;
        currentAngle = 0f;
    }
    public void DirectRotate(float deltaX)
    {

        if (!block)
        {
            inDirectRotation = true;
            velocity = deltaX * GameData.Default.cameraRotationSensitivity;
            velocity = Mathf.Clamp(velocity, -1.0f, 1.0f);
            enabled = true;
        }
    }
    public void Unblock()
    {
        block = false;
    }
    public void Limit(int id)
    {
        limit = id;
        hasLimit = id >= 0;
    }
    public void SetCellIndex(int id)
    {
        currentCellFloat = currentCell = id;
    }
    private void Update()
    {
        if (hasLevelMaster && levelMaster.State == LevelMaster.LevelState.process)
        {
            if (!InputController.Default.IsTouch)
                inDirectRotation = false;
            currentCellFloat += velocity;

            currentCellFloat = Mathf.Repeat(currentCellFloat, toolController.CellsCount);
            currentCell = (int)Mathf.Repeat(Mathf.Round(currentCellFloat), toolController.CellsCount);
            if(currentCell != toolController.ActiveIndex)
            {
                block = true;
                inDirectRotation = false;
                velocity = 0.0f;
            }

            if (hasLimit)
            {
                currentCellFloat = Mathf.Clamp(currentCellFloat, 0f, limit);
                currentCell = Mathf.Clamp(currentCell, 0, limit);

            }



            toolController.Select(currentCell);



            float angle = 0f;
            if (inDirectRotation || Mathf.Abs(velocity) > GameData.Default.cameraRotationMinVelocityToStop)
                angle = -currentCellFloat * toolController.CellStep;
            else
            {
                currentCellFloat = currentCell;
                angle = -currentCell * toolController.CellStep;
            }
            currentAngle = Mathf.LerpAngle(currentAngle, angle, GameData.Default.cameraRotationSnapping * Time.deltaTime);
            rotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
            for (int i = 0; i < targets.Length; i++)
                targets[i].Rotate(rotation);
            if (!inDirectRotation)
                velocity = Mathf.MoveTowards(velocity, 0f, GameData.Default.cameraRotationDamping * Time.deltaTime);
        }
    }
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(CameraSystem.Default[CameraState.Process].transform.position, toolController.Selected.point);
        }
    }
}
