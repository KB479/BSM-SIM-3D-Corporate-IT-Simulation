using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; 
using System.Collections.Generic;

public class IDE_ManagerUI : MonoBehaviour
{
    [Header("UI Referanslarý - Editor")]
    [SerializeField] private GameObject codePanelEmpty;
    [SerializeField] private GameObject codePanelFull;
    [SerializeField] private TextMeshProUGUI codeText;
    
    [SerializeField] private GameObject tabPage;
    [SerializeField] private Button pageButton;
    [SerializeField] private Button closePageButton;

    [Header("Terminal UI Referanslarý")]
    [SerializeField] private TMP_InputField terminalInput;
    [SerializeField] private TextMeshProUGUI terminalHistoryText;
    [SerializeField] private ScrollRect terminalScrollRect;

    private string terminalPrefix = "PS C:\\Users\\kaanb\\BSM_Core> ";

    [Header("UI Referanslarý - Sidebar (Dinamik)")]
    [SerializeField] private Transform fileListContainer; 
    [SerializeField] private GameObject fileButtonPrefab; 

    [Header("UI Referanslarý - Git Panel")]
    [SerializeField] private GameObject gitPanelContainer;

    [Header("Commit A Kartý")]
    [SerializeField] private Button commitA_Button;
    [SerializeField] private TextMeshProUGUI commitA_AuthorText;
    [SerializeField] private TextMeshProUGUI commitA_MessageText;

    [Header("Commit B Kartý")]
    [SerializeField] private Button commitB_Button;
    [SerializeField] private TextMeshProUGUI commitB_AuthorText;
    [SerializeField] private TextMeshProUGUI commitB_MessageText;

    /*
    [Header("ESKÝ! MOCK Terminal & Kontrol")]
    [SerializeField] private Button mockPullButton;
    [SerializeField] private Button mockPushButton;*/
    [SerializeField] private TextMeshProUGUI terminalInfoText; 

    private AppUI parentApp;
    private int selectedCommitIndex = -1;

    private TaskSO currentActiveTask; 
    private TaskSO currentlyViewedTask;

    private List<TaskSO> pulledTasks = new List<TaskSO>();
    private Dictionary<TaskSO, string> completedTasksCode = new Dictionary<TaskSO, string>();
    private Dictionary<TaskSO, bool> completedTasksResult = new Dictionary<TaskSO, bool>();

    private void Awake()
    {
        //if (mockPullButton != null) mockPullButton.onClick.AddListener(MockTerminal_Pull);
        //if (mockPushButton != null) mockPushButton.onClick.AddListener(MockTerminal_Push);

        if (closePageButton != null) closePageButton.onClick.AddListener(ShowEmptyState);

        if (commitA_Button != null) commitA_Button.onClick.AddListener(() => SelectCommit(0));
        if (commitB_Button != null) commitB_Button.onClick.AddListener(() => SelectCommit(1));

        if (terminalInput != null)
            terminalInput.onSubmit.AddListener(OnTerminalCommandEntered);

    }

    public void InitializeIDE(TaskSO task, AppUI appRef)
    {
        currentActiveTask = task;
        parentApp = appRef;
        selectedCommitIndex = -1;

        ShowEmptyState();
        UpdateTerminalLog("Sistem Hazýr. Yeni görev e-postasý alýndý.\n> Bekleyen komut: git pull");
    }

    private void ShowEmptyState()
    {
        codePanelFull.SetActive(false);
        codePanelEmpty.SetActive(true);
        gitPanelContainer.SetActive(false);
        tabPage.SetActive(false);

        //mockPushButton.interactable = false; // Boþtayken push yapýlamaz
        
        // Terminali baþlangýç durumuna getir
        terminalHistoryText.text = "BSM-OS Terminal v1.0.0 baþlatýldý. Komut bekleniyor...\n";
        terminalInput.interactable = true;
        terminalInput.text = "";

    }

    private void OnTerminalCommandEntered(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            RefocusTerminal();
            return;
        }

        AddLogToTerminal($"{terminalPrefix}<color=#E5E510>{command}</color>");
        terminalInput.text = ""; // Alt satýrý temizle

        command = command.Trim().ToLower();

        if (command == "git pull")
        {
            StartCoroutine(SimulateGitPullRoutine());
        }
        else if (command == "git push")
        {
            StartCoroutine(SimulateGitPushRoutine());
        }
        else if (command == "clear" || command == "cls")
        {
            terminalHistoryText.text = "";
            RefocusTerminal();
        }
        else
        {
            string errorMsg = $"<color=#F22C3D>{command} : The term '{command}' is not recognized as the name of a cmdlet, function, script file, or operable program. Check the spelling of the name, or if a path was included, verify that the path is correct and try again.\nAt line:1 char:1\n+ {command}\n+ ~~~~</color>";
            AddLogToTerminal(errorMsg);
            RefocusTerminal();
        }
    }

    private IEnumerator SimulateGitPullRoutine()
    {
        terminalInput.interactable = false; 

        if (currentActiveTask == null)
        {
            AddLogToTerminal("<color=yellow>Çekilecek (pull) yeni bir görev yok.</color>");
            terminalInput.interactable = true;
            yield break;
        }

        if (!MailManagerUI.readTasks.Contains(currentActiveTask))
        {
            AddLogToTerminal("<color=red>FATAL ERROR: Ýþ isteri onaylanmadý. Lütfen önce Mail uygulamasýndan yönergeleri okuyunuz.</color>");
            terminalInput.interactable = true;
            RefocusTerminal();
            yield break;
        }

        yield return new WaitForSeconds(0.4f);
        AddLogToTerminal("Enumerating objects: 5, done.");

        yield return new WaitForSeconds(0.6f);
        AddLogToTerminal("Counting objects: 100% (5/5), done.");

        yield return new WaitForSeconds(0.8f);
        AddLogToTerminal($"Unpacking objects: 100% (3/3), done.\n> {currentActiveTask.fileName} baþarýyla çekildi. <color=yellow>Merge çakýþmalarý (conflict) tespit edildi.</color>");

        if (!pulledTasks.Contains(currentActiveTask))
        {
            pulledTasks.Add(currentActiveTask);
            CreateFileButtonForTask(currentActiveTask);
        }
        OpenFile(currentActiveTask);

        terminalInput.interactable = true;
        RefocusTerminal();
    }

    private IEnumerator SimulateGitPushRoutine()
    {
        if (currentActiveTask == null || currentlyViewedTask != currentActiveTask || selectedCommitIndex == -1)
        {
            AddLogToTerminal("<color=red>Hata: Push yapýlacak geçerli bir commit seçilmedi.</color>");
            RefocusTerminal();
            yield break;
        }

        terminalInput.interactable = false; 
        bool isCorrect = (selectedCommitIndex == currentActiveTask.correctCommitIndex);

        yield return new WaitForSeconds(0.4f);
        AddLogToTerminal("Pushing to origin main...");

        yield return new WaitForSeconds(0.8f);
        AddLogToTerminal("Writing objects: 100% (3/3), 324 bytes, done.");

        yield return new WaitForSeconds(1.2f);
        AddLogToTerminal("> CI/CD Pipeline tetiklendi. Sunucuya gönderiliyor...");

        completedTasksCode[currentActiveTask] = codeText.text;
        completedTasksResult[currentActiveTask] = isCorrect;

        if (gitPanelContainer != null) gitPanelContainer.SetActive(false);

        parentApp.SubmitTaskResult(isCorrect);
        currentActiveTask = null;

    }

    private void AddLogToTerminal(string message)
    {
        terminalHistoryText.text += message + "\n";
        Canvas.ForceUpdateCanvases();
        if (terminalScrollRect != null)
        {
            terminalScrollRect.verticalNormalizedPosition = 0f; // Scroll'u en aþaðý çek, geldikçe aksýn 
        }
    }

    private void RefocusTerminal()
    {
        if (terminalInput.interactable)
        {
            terminalInput.Select();
            terminalInput.ActivateInputField();
        }
    }

    private void CreateFileButtonForTask(TaskSO task)
    {
        GameObject newBtnObj = Instantiate(fileButtonPrefab, fileListContainer);

        TextMeshProUGUI btnText = newBtnObj.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null) btnText.text = ">" + task.fileName;

        Button btn = newBtnObj.GetComponent<Button>();
        btn.onClick.AddListener(() => OpenFile(task));
    }

    public void OpenFile(TaskSO taskToOpen)
    {
        currentlyViewedTask = taskToOpen;

        codePanelEmpty.SetActive(false);
        codePanelFull.SetActive(true);

        // Tab page güncellemesi
        tabPage.SetActive(true);
        
        TextMeshProUGUI pageText = pageButton.GetComponentInChildren<TextMeshProUGUI>();
        if(pageText != null) pageText.text = taskToOpen.fileName;

        if (taskToOpen == currentActiveTask)
        {
            gitPanelContainer.SetActive(true);
            //mockPushButton.interactable = true;

            FillGitPanelData(taskToOpen);
            SelectCommit(selectedCommitIndex == -1 ? 0 : selectedCommitIndex); // Varsayýlan olarak A'yý göster
        }
        else if (completedTasksCode.ContainsKey(taskToOpen))
        {
            gitPanelContainer.SetActive(false); // Git panelini kapat
            //mockPushButton.interactable = false; // Push butonunu kilitle

            // O zamanlar nasýl pushladýysa o kodu göster
            codeText.text = completedTasksCode[taskToOpen];

            bool wasSuccess = completedTasksResult[taskToOpen];
            string resultText = wasSuccess ? "<color=green>[BAÞARILI]</color>" : "<color=red>[BAÞARISIZ - REVERTED]</color>";
            UpdateTerminalLog($"> {taskToOpen.fileName} dosyasý (Geçmiþ Görev) salt okunur modda açýldý.\n> Durum: {resultText}");
        }
    }

    private void FillGitPanelData(TaskSO task)
    {
        if (commitA_AuthorText != null) commitA_AuthorText.text = task.commitA_Card.author;
        if (commitA_MessageText != null) commitA_MessageText.text = task.commitA_Card.message;

        if (commitB_AuthorText != null) commitB_AuthorText.text = task.commitB_Card.author;
        if (commitB_MessageText != null) commitB_MessageText.text = task.commitB_Card.message;
    }

    public void SelectCommit(int index)
    {
        if (currentlyViewedTask != currentActiveTask) return; 

        selectedCommitIndex = index;
        RenderCodeWithSelection(index);
    }

    private void RenderCodeWithSelection(int index)
    {
        string currentCode = currentActiveTask.baseCodeTemplate;

        foreach (var zone in currentActiveTask.conflictZones)
        {
            string codeToInject = (index == 0) ? zone.commitACode : zone.commitBCode;
            string highlightedCode = $"<mark=#33669988>{codeToInject}</mark>";
            currentCode = currentCode.Replace(zone.zoneID, highlightedCode);
        }

        codeText.text = currentCode;
    }

    private void UpdateTerminalLog(string message)
    {
        if (terminalInfoText != null)
        {
            terminalInfoText.text = message;
        }
    }
}