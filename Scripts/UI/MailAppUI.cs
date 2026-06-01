using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MailAppUI : MonoBehaviour
{
    [SerializeField] private WindowControllerUI myWindowController;

    [Header("App Yöneticileri")]
    [SerializeField] private MailManagerUI mailManager; 
    private InteractionUnit myEU;

    public void OpenMailApp(InteractionUnit eu)
    {
        myEU = eu;

        gameObject.SetActive(true);

        if (myWindowController != null)
        {
            myWindowController.OpenWindow();
        }

        if (myEU != null && mailManager != null)
        {
            TaskSO currentTask = myEU.GetCurrentTask();
            mailManager.InitializeMailApp(currentTask);
        }
    }

    public void CloseMailApp()
    {
        gameObject.SetActive(false);

        if (myWindowController != null)
        {
            myWindowController.CloseWindow();
        }
    }
}