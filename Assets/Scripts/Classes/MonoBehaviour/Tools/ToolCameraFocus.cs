using BG.UI.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolCameraFocus : MonoBehaviour
{
    protected class Target
    {
        private Transform transform;
        private Vector3 direction;
        private Quaternion startRotation = Quaternion.identity;
        private float distance;

        public Target(Transform transform,Vector3 point)
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

    private bool inAction;
    private PillarObject pillar;
    private ToolController toolController;
    private Quaternion from = Quaternion.identity;
    private Quaternion to = Quaternion.identity;
    private float currentTime;
    private Target[] targets = new Target[2];
    private ToolGrid.Cell lastCell;
    private void Start()
    {
        LevelMaster levelMaster = LevelManager.Default.CurrentLevel;
        toolController = levelMaster.ToolController;
        toolController.OnChange += OnToolChange;
        levelMaster.GetPillar(out pillar);
        var cameras = CameraSystem.Default;
        targets[0] = new Target(cameras[CameraState.Process].transform, pillar.transform.position);
        targets[1] = new Target(cameras[CameraState.Win].transform, pillar.transform.position);
    }

    private void OnDestroy()
    {
        toolController.OnChange -= OnToolChange;
    }

    private void OnToolChange(ToolGrid.Cell cell)
    {
        if (!inAction && lastCell != cell)
        {
            lastCell = cell;
            toolController.CanChange = false;
            inAction = true;

            to = cell.rotation;
            currentTime = 0.0f;
            enabled = true;
        }
    }
    private void OnFinish()
    {
        toolController.CanChange = true;
        inAction = false;
        enabled = false;
        from = to;
    }
    private void Evaluate(float percent)
    {
        Quaternion rotation = Quaternion.Slerp(from, to, percent);
        for (int i = 0; i < targets.Length; i++)
            targets[i].Rotate(rotation);
    }
    private void LateUpdate()
    {
        if (inAction)
        {
            currentTime += Time.deltaTime;
            float percent = Mathf.Clamp01(currentTime / GameData.Default.cameraMoveDurationOnChangeTool);
            Evaluate(percent);
            if (percent >= 1f)
            {
                OnFinish();
                return;
            }
        }
        else
            enabled = false;
    }
}
