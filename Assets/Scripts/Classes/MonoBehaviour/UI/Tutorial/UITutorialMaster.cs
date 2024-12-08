using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UITutorialMaster : MonoBehaviour
{
    [SerializeField] private UITutorialCursor cursor;
    [SerializeField] private UITutorialBase[] tutorials = new UITutorialBase[0];
    private int activeTutorial = 0;
    private const string PREF_INDEX = "TutorIndex";
    private const string PREF_STATE = "TutorState";
    public UITutorialBase Active => tutorials[activeTutorial];

    private void Start()
    {
        activeTutorial = PlayerPrefs.GetInt(PREF_INDEX, 0);
        if (LevelManager.Default.CurrentLevelCount <= 1 &&
                LevelManager.Default.CurrentLevel.ToolController.Grid.GetToolCount() < 2 &&
                activeTutorial < tutorials.Length &&
                GameData.Default.enableTutorial)
        {
            cursor.Hide();
            for (int i = 0; i < tutorials.Length; i++)
            {
                tutorials[i].enabled = false;
                tutorials[i].Init(cursor);

            }
            LevelManager.Default.OnLevelStarted += OnLevelStarted;
        }
        else
        {
            gameObject.SetActive(false);
        }
        enabled = false;
    }
    private void OnDestroy()
    {
        LevelManager.Default.OnLevelStarted -= OnLevelStarted;
    }
    private void OnLevelStarted()
    {
        enabled = true;
        if (Active.enabled = PlayerPrefs.GetInt(PREF_STATE, 1) == 1)
        {
            Active.Select();
            Active.Show();
        }
    }

    private void Next()
    {
        Active.enabled = false;
       
        Active.Hide();
        activeTutorial++;
        Active.enabled = true;
        Active.Select();
        Active.Show();
        PlayerPrefs.SetInt(PREF_INDEX, activeTutorial);
    }

    private void LateUpdate()
    {
        if(activeTutorial < tutorials.Length)
            PlayerPrefs.SetInt(PREF_STATE, tutorials[activeTutorial].enabled ? 1 : 0);

        if (activeTutorial +1 < tutorials.Length)
        {
            if (tutorials[activeTutorial + 1].ShowCondition())
                Next();
        }
        else
            enabled = false;
    }
}
