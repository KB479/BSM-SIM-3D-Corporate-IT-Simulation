using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class TabMenuUI : MonoBehaviour
{
    public static TabMenuUI Instance { get; private set; }

    // PlayerMovement direkt getlesin diye bu, cursor manager içine argüman taþýmak. Biraz kirli çözüm ama iþ görür   
    public bool IsOpen { get; private set; }

    public event EventHandler<OnMenuToggledEventArgs> OnMenuToggled;
    public class OnMenuToggledEventArgs : EventArgs
    {
        public bool isOpen;
    }

    [Header("Ana Menü Kontrolü")]
    [SerializeField] private GameObject tabMenuPanel; 

    [Header("Üst Bilgi Paneli")]
    [SerializeField] private TextMeshProUGUI gameDayIntoText;
    [SerializeField] private TextMeshProUGUI currentCreditText;
    [SerializeField] private TextMeshProUGUI taskInfoText;

    [Header("Detay Paneli (Sað Taraf)")]
    [SerializeField] private TextMeshProUGUI taskTitleText;
    [SerializeField] private TextMeshProUGUI taskDescriptionText;
    [SerializeField] private TextMeshProUGUI taskOutputInfoText; 

    [Header("Liste Panelleri (Sol Taraf)")]
    [SerializeField] private Transform activeTaskListContainer; 
    [SerializeField] private Transform pastTaskListContainer;   
    [SerializeField] private GameObject taskPageButtonPrefab;   


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Sahnede birden fazla TabMenuUI var!");
        }
        Instance = this;
    }

    private void Start()
    {
        tabMenuPanel.SetActive(false);
    }


    // Bunu evente baðlamak lazým! Tab ile basýlýnca bu tetiklenir, UI'da Update kullanma iþi çok sýkýntýlý
    private void Update()
    {
        // Sadece DayInProgress durumunda menü açýlabilir veya kapanabilir
        if (GameManager.Instance.CurrentGameState != GameState.DayInProgress)
        {
            // Eðer aktif gün bittiyse veya baþka bir state'e geçildiyse menüyü zorla kapat
            if (IsOpen)
            {
                ToggleMenu();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        IsOpen = !IsOpen;
        tabMenuPanel.SetActive(IsOpen);

        if (IsOpen)
        {
            OpenMenu();
        }

        OnMenuToggled?.Invoke(this, new OnMenuToggledEventArgs { isOpen = this.IsOpen });
    }

    private void OpenMenu()
    {

        RefreshHeaderInfo();
        PopulateTaskLists();
        ClearDetailsPanel(); 
    }


    private void RefreshHeaderInfo()
    {
        // GM'den index geliyor, + 1 kaçýncý günse
        int currentDay = GameManager.Instance.CurrentDayIndex + 1;
        gameDayIntoText.text = $"{currentDay}. GÜN | MESAYÝ DEVAM EDÝYOR";

        int currentCredit = GameManager.Instance.CurrentCredit;
        currentCreditText.text = $"GÜNCEL KREDÝ: {currentCredit}";

        int activeTaskCount = TaskManager.Instance.GetActiveDailyTasks().Count;
        taskInfoText.text = $"KALAN GÖREV: {activeTaskCount}";
    }

    private void PopulateTaskLists()
    {
        // Önce eski butonlarý temizle
        foreach (Transform child in activeTaskListContainer) Destroy(child.gameObject);
        foreach (Transform child in pastTaskListContainer) Destroy(child.gameObject);

        // Aktif Görevleri Doldur
        List<TaskSO> activeTasks = TaskManager.Instance.GetActiveDailyTasks();
        foreach (TaskSO task in activeTasks)
        {
            CreateTaskButton(task, activeTaskListContainer, true, false);
        }

        // Geçmiþ Görevleri Doldur
        List<CompletedTaskRecords> pastTasks = TaskManager.Instance.GetCompletedTaskRecords();
        foreach (CompletedTaskRecords record in pastTasks)
        {
            CreateTaskButton(record.task, pastTaskListContainer, false, record.isSuccess);
        }
    }

    private void CreateTaskButton(TaskSO task, Transform container, bool isActiveTask, bool isSuccess = false)
    {
        GameObject btnObj = Instantiate(taskPageButtonPrefab, container);

        TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null) btnText.text = task.taskName;

        Button btn = btnObj.GetComponent<Button>();

        btn.onClick.AddListener(() => DisplayTaskDetails(task, isActiveTask, isSuccess));
    }

    private void DisplayTaskDetails(TaskSO task, bool isActiveTask, bool isSuccess)
    {
        taskTitleText.text = $"{task.taskName}";
        taskDescriptionText.text = task.description;

        if (isActiveTask)
        {
            taskOutputInfoText.text = $"<b>Ödül:</b> <color=green>+{task.successCreditReward} CR</color>\n" +
                                     $"<b>Ceza:</b> <color=red>-{task.failCreditPenalty} CR</color>\n\n" +
                                     $"<color=yellow>Durum: Çözüm Bekliyor</color>";
        }
        else
        {
            string resultStr = isSuccess ?
                $"<color=green>BAÞARILI (+{task.successCreditReward} CR)</color>" :
                $"<color=red>BAÞARISIZ (-{task.failCreditPenalty} CR) </color>";

            taskOutputInfoText.text = $"<b>Görev Sonucu:</b>\n{resultStr}\nSAU Müfredat Referansý: {task.curriculumReference} ";

        }
    }

    private void ClearDetailsPanel()
    {
        taskTitleText.text = "GÖREV SEÇÝNÝZ";
        taskDescriptionText.text = "Detaylarý görmek için sol taraftaki listeden bir göreve týklayýn.";
        taskOutputInfoText.text = "";
    }
}