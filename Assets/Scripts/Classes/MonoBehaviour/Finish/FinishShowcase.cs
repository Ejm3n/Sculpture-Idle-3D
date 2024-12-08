using BG.UI.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BG.UI.Main;

public class FinishShowcase : MonoBehaviour
{
    private bool inAction;
    private LevelMaster levelMaster;
    private Transform sculpture;
    private Transform winCam;
    [SerializeField] private FinishObject finishObject;
    [SerializeField] private string sound = "Win";
    private Sequence sequence;
    //private float angular;

    private void Start()
    {
        LevelManager.Default.OnLevelPreLoad += OnLevelPreLoad;
        levelMaster = GetComponent<LevelMaster>();
        levelMaster.OnWin += OnWin;
        sculpture = levelMaster.Sculpture.transform;
        winCam = CameraSystem.Default.WinCamera.transform;
    }
    private void OnDestroy()
    {
        if(levelMaster != null)
        levelMaster.OnWin -= OnWin;
        LevelManager.Default.OnLevelPreLoad -= OnLevelPreLoad;

    }
    private void OnWin()
    {
        if (!inAction)
        {
            CinemachineBrain.Default.Brain.enabled = true;
            CinemachineBrain.Default.SetBlendTime(GameData.Default.finishCameraBlendTime);
            inAction = true;
            Vector3 finishPoint = GameData.Default.finishPosition;
            Vector3 ssp = sculpture.position;
            float offset = 3f;
            Vector3 swcp = winCam.position + Vector3.up * offset;
            Vector3 twcp = swcp;
            twcp.y = swcp.y + finishPoint.y- offset;
            Quaternion ssr = sculpture.rotation;
            Quaternion tsr = Quaternion.LookRotation(winCam.forward, Vector3.up)*GameData.Default.finishSculptEndRotation;
            sequence?.Kill();
            sequence = DOTween.Sequence();
            //sequence.Append(DOTween.To(() => 0f, (v) => { }, 0f, GameData.Default.finishWaitForScaleTime));
            sequence.Append(winCam.DOMove(swcp, GameData.Default.finishWaitForScaleTime));
            sequence.Append(sculpture.DOScale(0.8f,0f));
            sequence.Append(sculpture.DOScale(1.0f, 1.25f).SetEase(Ease.OutElastic));
            sequence.Append(DOTween.To(() => 0f, (v) => 
            {
                sculpture.position = Vector3.Lerp(ssp,finishPoint,v);
                winCam.position = Vector3.Lerp(swcp, twcp, v);
                sculpture.rotation = Quaternion.Lerp(ssr, tsr,v);
            }
            , 1f, GameData.Default.finishMoveTime).SetEase(Ease.InSine));
            sequence.OnComplete(() =>
            {   
                finishObject.transform.position = finishPoint;
                finishObject.transform.rotation = Quaternion.LookRotation(winCam.forward, Vector3.up);
                finishObject.Show();
                UIManager.Default.CurentState = UIState.Win;
                SoundHolder.Default.PlayFromSoundPack(sound);
            });
        }
    }
    private void OnLevelPreLoad()
    {
        CinemachineBrain.Default.DefaultBlendTime();
    }
    //private void LateUpdate()
    //{
    //    if (inAction)
    //    {
    //        if ((sculpture.position - finishPoint).sqrMagnitude > 0.1f)
    //        {
    //            Vector3 pos = Vector3.MoveTowards(
    //            sculpture.position,
    //            finishPoint,
    //            GameData.Default.finishMoveSpeed * Time.deltaTime);
    //            sculpture.position = pos;
    //            Vector3 cp = winCam.position;
    //            cp.y = startWinCamPos + pos.y;
    //            winCam.position = cp;
    //            angular = GameData.Default.finishSculptStartRot;
    //            if ((sculpture.position - finishPoint).sqrMagnitude <= 0.1f)
    //            {
    //                finishObject.transform.position = pos;
    //                finishObject.transform.rotation = Quaternion.LookRotation(winCam.forward,Vector3.up);
    //                finishObject.Show();
    //                SoundHolder.Default.PlayFromSoundPack(sound);

    //            }
    //        }
    //        else
    //        {
    //            angular = Mathf.MoveTowards(angular,GameData.Default.finishSculptEndRot, GameData.Default.finishSculptStartRot * Time.deltaTime);
    //        }
    //        sculpture.rotation *= Quaternion.AngleAxis(angular * Time.deltaTime, Vector3.up);

    //    }
    //    else
    //        enabled = false;
    //}
}
