using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FinishObject : MonoBehaviour
{
   [SerializeField] private Transform shine;
    [SerializeField] private ParticleSystem[] particles;
    private float angle;

    private void Start()
    {
        shine.gameObject.SetActive(false);
        shine.localScale = Vector3.one * 0.1f;
        enabled = false;
    }
    public void Show()
    {
        enabled = true;
        shine.gameObject.SetActive(true);

        shine.DOScale(1f, 1.0f);
        for (int i = 0; i < particles.Length; i++)
            particles[i].Play();
    }
    private void LateUpdate()
    {
        angle += Time.deltaTime * GameData.Default.finishShineSpeed;
        shine.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

}
