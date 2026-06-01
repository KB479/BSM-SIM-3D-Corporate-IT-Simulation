using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TaskRequirement
{
    public TaskType type;
    public TaskDifficulty difficulty;
    public int count; 

[CreateAssetMenu(fileName = "DayScriptableObjects", menuName = "ScriptableObjects/DayScriptableObjects")]

public class DaySO : ScriptableObject
{
    public int dayIndex; 
    public string dayTitle; 

    [Header("Günün Görev Havuzu Kurallarý")]
    public List<TaskRequirement> taskRequirements;
}