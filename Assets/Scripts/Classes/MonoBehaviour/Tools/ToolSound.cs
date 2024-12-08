using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSound : MonoBehaviour
{
    private ToolBase tool;
    [SerializeField] private ToolWorkerAnimationController anim;
    private ToolController controller;
    [SerializeField] private string soundImpact = "Impact";
    [SerializeField] private string soundWakeup = "Scream";
    private void Start()
    {
        tool = GetComponent<ToolBase>();
        tool.OnWakeup += PlayWakeup;
        controller = LevelManager.Default.CurrentLevel.ToolController;
        anim.OnAnimationAction += PlayImpact;
    }
    private void OnDestroy()
    {
        tool.OnWakeup -= PlayWakeup;
        anim.OnAnimationAction -= PlayImpact;
    }
    private void PlayImpact()
    {
        PlaySound(soundImpact);
    }
    private void PlayWakeup()
    {
        PlaySound(soundWakeup);
    }
    private void PlaySound(string sound)
    {
        if (controller.ActiveIndex == tool.Index)
        {
            SoundHolder.Default.PlayFromSoundPack(sound);
        }
        else if (controller.NextIndex == tool.Index || controller.PrevIndex == tool.Index)
        {
            var s = SoundHolder.Default.PlayFromSoundPack(sound);
            s.volume = s.volume * 0.5f;
        }
    }
}
