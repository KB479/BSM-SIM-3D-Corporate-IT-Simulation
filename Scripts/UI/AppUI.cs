using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AppUI : MonoBehaviour
{
    [SerializeField] private WindowControllerUI myWindowController;

    [Header("App Yöneticileri")]
    [SerializeField] private IDE_ManagerUI ideManager; 

    private InteractionUnit myEU;

    public void OpenApp(InteractionUnit eu)
    {
        myEU = eu;

        gameObject.SetActive(true);

        if (myWindowController != null)
        {
            myWindowController.OpenWindow();
        }

        if (myEU != null && ideManager != null)
        {
            TaskSO currentTask = myEU.GetCurrentTask();

            if (currentTask != null)
            {
                ideManager.InitializeIDE(currentTask, this);
            }
        }
    }

    public void CloseApp()
    {
        gameObject.SetActive(false);

        if (myWindowController != null)
        {
            myWindowController.CloseWindow();
        }
    }

    public void SubmitTaskResult(bool isSuccess)
    {
        if (myEU != null && !myEU.IsProcessing())
        {
            myEU.ResolveCurrentTask(isSuccess);
        }
    }
}