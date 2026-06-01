using UnityEngine;

// Projedeki tüm enumlar burada tutulabilir þimdilik
public enum TaskType
{
    Software,   
    Hardware,
    Organization
}

public enum TaskDifficulty
{
    Easy,
    Medium,
    Hard
}

public enum GameState
{
    Tutorial,
    NewGameDay,
    DayInProgress,
    Interacting,
    EndGameDay,
    EndGame
}