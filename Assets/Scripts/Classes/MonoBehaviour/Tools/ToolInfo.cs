using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolInfo : MonoBehaviour
{
    [SerializeField] private ToolBase tool;
    [SerializeField] private TextMeshPro info;
    private void Start()
    {
        UpdateInfo(tool.Level);
        tool.OnUpgrade += UpdateInfo;
        transform.position = tool.transform.position + Vector3.up * tool.Extents.y * 2.25f + tool.transform.forward * transform.localPosition.z;
        LevelManager.Default.CurrentLevel.OnWin += OnWin;
    }
    private void OnDestroy()
    {
        tool.OnUpgrade -= UpdateInfo;
        LevelManager.Default.CurrentLevel.OnWin -= OnWin;

    }
    private void UpdateInfo(int level)
    {
        info.SetText((level+1).ToString());
    }
    private void OnWin()
    {
        gameObject.SetActive(false);
    }
}
