using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EndDayUI : MonoBehaviour
{
    [SerializeField] private Button startNewDayButton; 
    [SerializeField] private Button saveAndExitButton;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    private void GameManager_OnStateChanged(object sender, GameManager.OnStateChangedEventArgs e)
    {
        if(e.currentGameState == GameState.EndGameDay)
        {
            Show();
            ShowCursor(true);

        }
    }

    public void ButtonClicked()
    {

        GameManager.Instance.StartNextDayRequest();
        Hide();

    }

    // cursor manager ilgileniyor artýk, kaldýr
    private void ShowCursor(bool show)
    {
        if (show)
        {
            // imleci sal
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // imleci kitle
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true); 
    }

}
