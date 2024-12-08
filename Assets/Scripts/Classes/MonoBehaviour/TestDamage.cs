using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamage : MonoBehaviour
{
    private ToolController controller;
    private void Start()
    {
        controller = LevelManager.Default.CurrentLevel.ToolController;
        
    }
    private void LateUpdate()
    {
        controller.Grid.HasSleepTool();
    }
}
