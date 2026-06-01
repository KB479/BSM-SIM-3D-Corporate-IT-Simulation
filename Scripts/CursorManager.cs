using UnityEngine;
using System;

public class CursorManager : MonoBehaviour
{
    private void Start()
    {
        UpdateCursorState(GameManager.Instance.CurrentGameState);

        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        if (TabMenuUI.Instance != null)
        {
            TabMenuUI.Instance.OnMenuToggled += TabMenuUI_OnMenuToggled;
        }
    }

    // data leak kontrolü 
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
        }

        if (TabMenuUI.Instance != null)
        {
            TabMenuUI.Instance.OnMenuToggled -= TabMenuUI_OnMenuToggled;
        }
    }

    private void GameManager_OnStateChanged(object sender, GameManager.OnStateChangedEventArgs e)
    {
        UpdateCursorState(e.currentGameState);
    }

    private void TabMenuUI_OnMenuToggled(object sender, TabMenuUI.OnMenuToggledEventArgs e)
    {
        if (e.isOpen)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            UpdateCursorState(GameManager.Instance.CurrentGameState);
        }
    }

    private void UpdateCursorState(GameState state)
    {
        if (TabMenuUI.Instance != null && TabMenuUI.Instance.IsOpen) return;

        if (state == GameState.DayInProgress)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
}