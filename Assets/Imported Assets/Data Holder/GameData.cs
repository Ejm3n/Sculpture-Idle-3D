using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/GameData")]
public class GameData : DataHolder
{

    public static string PrefabResourcesPath = "Prefabs/";

    #region Singleton

    private static GameData _default;
    public static GameData Default => _default;

    #endregion

    [Header("Pillar")]
    public float pillarHealth = 5000f;
    public float pillarHealthIncrease = 1875f;
    public float pillarSegmentHealthPercentToChange = 0.2f;
    public float pillarSegmentsMoveSpeed = 5f;
    public Material platformSelectedMatrial;
    [Header("Tools")]
    public float cursorUpgradeMultiplier = 1f;
    [Min(0.01f)] public float cameraMoveDurationOnChangeTool = 0.5f;
    public int maxSpecialHitCount = 1;
    public float specialHitScaleChange = 0.95f;
    public float damageScale = 1f;
    [Header("Cameras")]
    public float cameraShakeDuration = 0.25f;
    public float cameraShakeStrength = 0.25f;
    public float cameraRotationSensitivity = -1;
    public float cameraRotationSnapping = 5;
    public float cameraRotationDamping = 0.1f;
    public float cameraRotationMinVelocityToStop = 0.2f;
    [Header("Input")]
    public float inputDeltaMinToRotate = 1.0f;
    public float inputDeltaLessToClick = 1.0f;
    public Vector2 inputClickZoneSizeScale = Vector2.one;
    public Vector2 inputClickZoneOffsetScale = Vector2.zero;
    public Vector2 inputWakeupZoneSizeScale = Vector2.one;
    public Vector2 inputWakeupZoneOffsetScale = Vector2.zero;
    [Header("Offline")]
    public int minOfflineTime = 60;
    public int maxOfflineTime = 30 * 60;
    [Header("Finish")]
    //public float finishMoveSpeed =5f;
    public Quaternion finishSculptEndRotation = Quaternion.identity;
    public Vector3 finishPosition = Vector3.up * 40f;
    public float finishWaitForScaleTime = 2f;
    public float finishMoveTime = 2f;
    public float finishCameraBlendTime = 0.75f;
    public float finishShineSpeed = 60f;
    //public float finishSculptStartRot = 360f;
    //public float finishSculptEndRot = 360f;
    [Header("Physics")]
    public float fragFloorYPosSimplePhysics = 0.25f;
    public float fragYPosToDisableSimplePhysics = -10f;
    public float fragTimeToDisableSimplePhysics = 3f;
    public int fragMaxBounceCountSimplePhysics = 3;
    [Header("Other")]
    public bool enableTutorial = true;
    public bool enableCursor = false;
    public float cursorAnimDurationScale = 1f;
    public bool enableLazyCursor = false;
    public bool lazyCursorUseLerp = true;
    public float lazyCursorSpeed = 5f;
    public bool useAltWhip = false;
    [Header("UI")]
    public Color UIColor;
    public Color UITitleColor;

    public override void Init()
    {
        _default = this;
    }
}
