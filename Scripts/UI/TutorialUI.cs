using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{

    [SerializeField] private Button startDayOneButton;


    private void Start()
    {
        Show();
        ShowCursor(true);
    }

    public void StartDayOneButtonClicked()
    {
        GameManager.Instance.StartFirstDayRequest();
        Hide();
        ShowCursor(false);

    }

    // Cursor managre ilgelinoy artýk, kaldýrýcam
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
