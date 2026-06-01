using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminAppUI : MonoBehaviour
{
    [Header("App Elemanlarý")]
    [SerializeField] private TextMeshProUGUI infoText; 
    [SerializeField] private Button solveButton; 
    [SerializeField] private Button failButton; 
    [SerializeField] private Button closeAppButton; 

    private InteractionUnit myEU;


    private void Awake()
    {
        solveButton.onClick.AddListener(() => ResolveTask(true));
        failButton.onClick.AddListener(() => ResolveTask(false));

        closeAppButton.onClick.AddListener(CloseApp);
    }

    public void OpenApp(InteractionUnit eu)
    {
        myEU = eu;
        gameObject.SetActive(true);
    }

    public void CloseApp()
    {
        gameObject.SetActive(false);
    }

    public void ResolveTask(bool isSuccess)
    {
        // Ekstra bir güvenlik katmaný, zaten eu açýyor bunu ve IsProcessing iken butanlar aktif deðil, yine de kalsýn
        if (myEU != null && !myEU.IsProcessing())
        {
            myEU.ResolveCurrentTask(isSuccess);
        }
    }

    private void Update()
    {
        if (myEU == null) return;

        if (myEU.IsProcessing())
        {
            infoText.text = "Sistem Ýþleniyor...\nLütfen Bekleyiniz.";
            solveButton.interactable = false;
            failButton.interactable = false;
        }
        else
        {
            TaskSO currentTask = myEU.GetCurrentTask();

            if (currentTask != null)
            {
                infoText.text = $"<b>Görev:</b> {currentTask.taskName}\n<b>Zorluk:</b> {currentTask.difficulty}";
                solveButton.interactable = true;
                failButton.interactable = true;
            }
            else
            {
                infoText.text = "Sistemde bekleyen iþ bulunmamaktadýr.";
                solveButton.interactable = false;
                failButton.interactable = false;
            }
        }
    }


}
