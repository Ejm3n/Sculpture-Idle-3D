using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    #region Singletone
    private static LevelManager _default;
    public static LevelManager Default { get => _default; }
    public LevelManager() => _default = this;
    #endregion

    const string PREFS_KEY_LEVEL_ID = "CurrentLevelCount";
    const string PREFS_KEY_LAST_INDEX = "LastLevelIndex";

    public bool editorMode = false;
    public int startRandomFrom = 0;

    private int localCounter = 0;

    public int CurrentLevelCount => PlayerPrefs.GetInt(PREFS_KEY_LEVEL_ID, 0) + 1;
    public int CurrentLevelIndex;
    public int LocalCounter => localCounter;
    public LevelMaster CurrentLevel { get; private set; }
    public List<Level> Levels = new List<Level>();
    public Action OnLevelPrepare;
    public Action OnLevelStarted;
    public Action OnLevelLoaded;
    public Action OnLevelPreLoad;

    private bool isPlaying;
    public bool IsPlaying => isPlaying;

    public void Start()
    {
#if !UNITY_EDITOR
        editorMode = false;
#endif
        if (!editorMode)
            SelectLevel(PlayerPrefs.GetInt("LastLevelIndex"), true);
        else
        {
            PlayerPrefs.SetInt(PREFS_KEY_LEVEL_ID, CurrentLevelIndex);
            CurrentLevel = GetComponentInChildren<LevelMaster>();
            CurrentLevel.ForceInitialize();
            OnLevelLoaded?.Invoke();
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(PREFS_KEY_LAST_INDEX, CurrentLevelIndex);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(PREFS_KEY_LAST_INDEX, CurrentLevelIndex);
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            Application.Quit();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            Application.Quit();
    }
    public void PrepareLevel()
    {
        OnLevelPrepare?.Invoke();
    }
    public void StartLevel(bool send = false)
    {
        if (send)
            SendStart();
        isPlaying = true;
        OnLevelStarted?.Invoke();
    }

    public void RestartLevel()
    {
        SendRestart();
        isPlaying = false;
        SelectLevel(CurrentLevelIndex, false);
    }

    public void NextLevel()
    {
        SendComplete();
        isPlaying = false;
        PlayerPrefs.SetInt(PREFS_KEY_LEVEL_ID, (PlayerPrefs.GetInt(PREFS_KEY_LEVEL_ID) + 1));
        SelectLevel(CurrentLevelIndex + 1);
        localCounter++;
    }

    public void ClearListAtIndex(int levelIndex)
    {
        Levels[levelIndex].LevelPrefab = null;
    }

    public void SelectLevel(int levelIndex, bool indexCheck = true)
    {
        if (indexCheck)
            levelIndex = GetCorrectedIndex(levelIndex);

        if (Levels[levelIndex].LevelPrefab == null)
        {
            Debug.Log("<color=red>There is no prefab attached!</color>");
            return;
        }

        var level = Levels[levelIndex];

        if (level.LevelPrefab)
        {
            SelLevelParams(level);
            CurrentLevelIndex = levelIndex;
        }
        PlayerPrefs.SetInt(PREFS_KEY_LAST_INDEX, CurrentLevelIndex);

    }

    public void PrevLevel() =>
        SelectLevel(CurrentLevelIndex - 1);

    private int GetCorrectedIndex(int levelIndex)
    {
        if (editorMode)
            return levelIndex > Levels.Count - 1 || levelIndex <= 0 ? 0 : levelIndex;
        else
        {
            int levelId = PlayerPrefs.GetInt(PREFS_KEY_LEVEL_ID);
            if (levelId > Levels.Count - 1)
            {
                if (Levels.Count > 1)
                {
                    while (true)
                    {
                        levelId = UnityEngine.Random.Range(startRandomFrom, Levels.Count);
                        if (levelId != CurrentLevelIndex) return levelId;
                    }
                }
                else return UnityEngine.Random.Range(0, Levels.Count);
            }
            return levelId;
        }
    }

    private void SelLevelParams(Level level)
    {
        OnLevelPreLoad?.Invoke();
        if (level.LevelPrefab)
        {
            ClearChilds();
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                CurrentLevel = Instantiate(level.LevelPrefab, transform);
            }
            else
            {
                CurrentLevel = PrefabUtility.InstantiatePrefab(level.LevelPrefab, transform) as LevelMaster;
            }
            foreach (IEditorModeSpawn child in GetComponentsInChildren<IEditorModeSpawn>())
                child.EditorModeSpawn();
#else
            CurrentLevel = Instantiate(level.LevelPrefab, transform);
#endif
        }

        if (level.SkyboxMaterial)
        {
            RenderSettings.skybox = level.SkyboxMaterial;
        }
        CurrentLevel.Initialize();
        OnLevelLoaded?.Invoke();
    }

    private void ClearChilds()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject destroyObject = transform.GetChild(i).gameObject;
            DestroyImmediate(destroyObject);

            //#if UNITY_EDITOR
            //            if (Application.isPlaying)
            //                Destroy(destroyObject);
            //            else
            //                DestroyImmediate(destroyObject);
            //#else
            //                Destroy(destroyObject);

            //#endif

        }
    }



    #region Analitics Events

    public void SendStart()
    {
        int level = CurrentLevelCount;
        Debug.Log($"GL Level Start {level}");
        Dictionary<string, object> param = new Dictionary<string, object>();


    }

    public void SendRestart()
    {
        int level = CurrentLevelCount;
        Debug.Log($"GL Level Fail {level}");
        Dictionary<string, object> param = new Dictionary<string, object>();


    }

    public void SendComplete()
    {
        int level = CurrentLevelCount;
        Debug.Log($"GL Level Complete {level}");
        Dictionary<string, object> param = new Dictionary<string, object>();


    }
    #endregion
}

[System.Serializable]
public class Level
{
    public LevelMaster LevelPrefab;
    public Material SkyboxMaterial;
}