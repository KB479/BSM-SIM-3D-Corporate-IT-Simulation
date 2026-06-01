using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskDatabaseSO", menuName = "ScriptableObjects/TaskDatabaseSO")]
public class TaskDatabaseSO : ScriptableObject
{
    [Header("All Tasks Pool")]
    public List<TaskSO> allTasks = new List<TaskSO>();
}