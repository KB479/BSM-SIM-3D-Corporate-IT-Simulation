using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI endGameText;
    [SerializeField] private Button restartButton; 
    [SerializeField] private Button quitToMenuButton; 
    [SerializeField] private Button quitToDesktopButton; 


    private void Start()
    {
        GameManager.Instance.OnEndGame += GameManager_OnEndGame;
        Hide();
    }

    private void GameManager_OnEndGame(object sender, GameManager.OnEndGameEventArgs e)
    {
        Show(); 
        ShowCursor(true); 

        if (e.isGameWin)
        {
            EndGameVisual("You Win!", Color.green); 
        }
        else
        {
            EndGameVisual("You Lose!", Color.red); 
        }
    }

    public void RestartButtonClicked()
    {
        // scene yüklemeyi kontrol eden classa request göndermeli, þimdilik direkt burdan yükliycem, refactor et ilerde
        SceneManager.LoadScene(1); 

    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEndGame -= GameManager_OnEndGame;
        }
    }

    public void QuitToMenuButtonClicked()
    {
        SceneManager.LoadScene(0);

    }

    public void QuitToDesktopButtonClicked()
    {
        Application.Quit();

        Debug.Log("Quit!"); 
    }

    // cursor manager kontrol ediyor artýk, kaldýr
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

    private void EndGameVisual(string endGameText, Color textColor)
    {
        this.endGameText.text = endGameText;
        this.endGameText.color = textColor;
    }

}
