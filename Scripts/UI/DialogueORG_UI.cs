using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class DialogueORG_UI : MonoBehaviour
{
    public static DialogueORG_UI Instance { get; private set; }

    [Header("Ana Paneller")]
    [SerializeField] private GameObject dialogueFullPanel;
    [SerializeField] private Transform historyContentArea;
    [SerializeField] private Transform choiceArea;

    [Header("Arayüz Elementleri")]
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private Button exitButton; 

    [Header("Prefablar")]
    [SerializeField] private GameObject dialogueItemPrefab;
    [SerializeField] private GameObject choiceButtonPrefab;

    private InteractionUnit connectedEU;
    private TaskSO currentTask;

    private DialogueState currentState;
    private enum DialogueState { Investigation, Decision }

    private List<GameObject> investigationButtons = new List<GameObject>();
    private List<GameObject> decisionButtons = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialogueFullPanel.SetActive(false);

        exitButton.onClick.AddListener(CloseDialogue);
    }


    public void StartDialogue(InteractionUnit eu)
    {
        connectedEU = eu;
        currentTask = connectedEU.GetCurrentTask();

        if (currentTask == null) return;

        /*  OS açýnca eu arayüzü açýyorduk ve bunun stati vardý, her diyalog aslýnda buna girmeyebilir ama þu an diyalog da bir
        // Eu arayüzü, o yüzden gm'deki stati kullanabiliriz. Ekstra cursor managera giriþmeye de gerek kalmaz, zaten gm stati gerekeni yapar */
        GameManager.Instance.StartInteractionRequest();

        ClearUI();

        exitButton.interactable = false;

        npcNameText.text = currentTask.npcName;
        dialogueFullPanel.SetActive(true);

        AddLogToHistory(currentTask.npcName, currentTask.initialCrisisDialogue);

        ChangeState(DialogueState.Investigation);
    }

    private void ChangeState(DialogueState newState)
    {
        currentState = newState;

        HideAllButtons();

        if (currentState == DialogueState.Investigation)
        {
            if (investigationButtons.Count == 0) GenerateInvestigationButtons();
            else ShowButtons(investigationButtons);
        }
        else if (currentState == DialogueState.Decision)
        {
            if (decisionButtons.Count == 0) GenerateDecisionButtons();
            else ShowButtons(decisionButtons);
        }
    }

    private void HideAllButtons()
    {
        foreach (var btn in investigationButtons) btn.SetActive(false);
        foreach (var btn in decisionButtons) btn.SetActive(false);
    }

    private void ShowButtons(List<GameObject> buttonList)
    {
        foreach (var btn in buttonList) btn.SetActive(true);
    }


    private void GenerateInvestigationButtons()
    {
        if (currentTask.investigationOptions != null)
        {
            for (int i = 0; i < currentTask.investigationOptions.Count; i++)
            {
                InvestigationOption option = currentTask.investigationOptions[i];

                GameObject btnObj = Instantiate(choiceButtonPrefab, choiceArea);
                investigationButtons.Add(btnObj);

                btnObj.GetComponentInChildren<TextMeshProUGUI>().text = option.buttonPreviewText;

                Button btn = btnObj.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    btn.interactable = false;
                    AddLogToHistory("SEN", option.playerFullQuestion);
                    AddLogToHistory(currentTask.npcName, option.npcResponse);
                });
            }
        }

        GameObject changeStateBtn = Instantiate(choiceButtonPrefab, choiceArea);
        investigationButtons.Add(changeStateBtn);
        changeStateBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Bence þöyle yapmalýyýz...";
        changeStateBtn.GetComponent<Button>().onClick.AddListener(() => ChangeState(DialogueState.Decision));
    }

    private void GenerateDecisionButtons()
    {
        if (currentTask.dialogueOptions != null)
        {
            for (int i = 0; i < currentTask.dialogueOptions.Count; i++)
            {
                DialogueOption option = currentTask.dialogueOptions[i];

                GameObject btnObj = Instantiate(choiceButtonPrefab, choiceArea);
                decisionButtons.Add(btnObj);

                btnObj.GetComponentInChildren<TextMeshProUGUI>().text = option.buttonPreviewText;

                Button btn = btnObj.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    LockAllCurrentButtons(decisionButtons);

                    AddLogToHistory("SEN", option.fullLogText);
                    AddLogToHistory(currentTask.npcName, option.npcReaction);

                    SubmitTaskResult(option.isCorrectChoice);
                });
            }
        }

        GameObject changeStateBtn = Instantiate(choiceButtonPrefab, choiceArea);
        decisionButtons.Add(changeStateBtn);
        changeStateBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Bir saniye, sormak istediðim baþka þeyler var...";
        changeStateBtn.GetComponent<Button>().onClick.AddListener(() => ChangeState(DialogueState.Investigation));
    }


    private void AddLogToHistory(string speakerName, string message)
    {
        GameObject newLog = Instantiate(dialogueItemPrefab, historyContentArea);

        newLog.transform.Find("SpeakerName_Text").GetComponent<TextMeshProUGUI>().text = speakerName;
        newLog.transform.Find("MessageBody_Text").GetComponent<TextMeshProUGUI>().text = message;
    }

    private void SubmitTaskResult(bool isSuccess)
    {
        if (connectedEU != null && !connectedEU.IsProcessing())
        {
            connectedEU.ResolveCurrentTask(isSuccess);
        }

        exitButton.interactable = true;
    }

    private void CloseDialogue()
    {
        dialogueFullPanel.SetActive(false);

        GameManager.Instance.EndInteractionRequest();

    }


    private void ClearUI()
    {
        foreach (var btn in investigationButtons) Destroy(btn);
        foreach (var btn in decisionButtons) Destroy(btn);
        investigationButtons.Clear();
        decisionButtons.Clear();

        foreach (Transform child in historyContentArea) Destroy(child.gameObject);
    }

    private void LockAllCurrentButtons(List<GameObject> buttonList)
    {
        foreach (GameObject btnObj in buttonList)
        {
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null) btn.interactable = false;
        }
    }
}