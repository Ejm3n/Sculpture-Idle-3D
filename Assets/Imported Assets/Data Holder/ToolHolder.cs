using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/ToolHolder")]
public class ToolHolder : DataHolder
{
    //const string PREFS_KEY_CURRENT_TOOL = "CurrentToolCount";



    [System.Serializable]
    public class Tool 
    {
        public ulong price;
        public ToolBase prefab;
        public GameObject ghost;
        [DisplayScriptableObjectProperties] public ToolUpgrade upgrade;

    }
    #region Singleton

    private static ToolHolder _default;
    public static ToolHolder Default => _default;

    public override void Init()
    {
        _default = this;
    }

    #endregion
    
    public List<Tool> tools = new List<Tool>();
    //public Tool CurrentTool => tools[GetToolCount()];
    public Tool GetTool(int index)
    {
       return tools[index];
    }
    public int GetCorrectIndex(int index)
    {

        return Mathf.Min(index+1, tools.Count-1);
        //return PlayerPrefs.GetInt(PREFS_KEY_CURRENT_TOOL, 1);
    }
    //public int GetToolCount()
    //{
    //    return PlayerPrefs.GetInt(PREFS_KEY_CURRENT_TOOL,1);
    //}
    //public void IncreaseToolCount()
    //{
    //    int count = GetToolCount();
    //    count = Mathf.Min(count + 1,tools.Count-1);
    //    PlayerPrefs.SetInt(PREFS_KEY_CURRENT_TOOL, count);
    //}
    //public void DecreaseToolCount()
    //{
    //    int count = GetToolCount();
    //    count = Mathf.Max(count - 1, 0);
    //    PlayerPrefs.SetInt(PREFS_KEY_CURRENT_TOOL, count);
    //}
}
