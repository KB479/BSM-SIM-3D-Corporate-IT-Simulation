using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MailManagerUI : MonoBehaviour
{
    [Header("Mail Message Box")]
    [SerializeField] private Transform messageBox;
    [SerializeField] private GameObject pageButtonPrefab;

    [Header("Mail Message Content Page")]
    [SerializeField] private GameObject messagePage;
    [SerializeField] private TextMeshProUGUI subjectText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Button goBackButton;

    private TaskSO activeTask;
    private Dictionary<TaskSO, GameObject> spawnedMailButtons = new Dictionary<TaskSO, GameObject>();

    // Bunu kaldýrmayý kaldýracam, GDES olarak saçma, clean code olarak yanlýþ: 
    // IDE'nin okuma durumunu kontrol edebilmesi için statik liste
    public static HashSet<TaskSO> readTasks = new HashSet<TaskSO>();

    public void InitializeMailApp(TaskSO task)
    {
        activeTask = task;

        if (messagePage != null) messagePage.SetActive(false);
        if (messageBox != null) messageBox.gameObject.SetActive(true);

        if (activeTask != null && !spawnedMailButtons.ContainsKey(activeTask))
        {
            CreateMailButton(activeTask);
        }
    }

    private void CreateMailButton(TaskSO task)
    {
        GameObject pageButtonGameObject = Instantiate(pageButtonPrefab, messageBox);
        spawnedMailButtons.Add(task, pageButtonGameObject);

        TextMeshProUGUI buttonText = pageButtonGameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            string sender = string.IsNullOrEmpty(task.emailSender) ? "Lead Developer" : task.emailSender;
            string subject = string.IsNullOrEmpty(task.emailSubject) ? task.taskName : task.emailSubject;

            buttonText.text = $"<b>{sender}</b> - {subject}";
        }

        Button pageButton = pageButtonGameObject.GetComponent<Button>();
        if (pageButton != null)
        {
            pageButton.onClick.AddListener(() => OpenMail(task));
        }
    }

    public void OpenMail(TaskSO task)
    {
        if (messagePage != null) messagePage.SetActive(true);
        if (messageBox != null) messageBox.gameObject.SetActive(false); 


        if (subjectText != null) subjectText.text = string.IsNullOrEmpty(task.emailSubject) ? task.taskName : task.emailSubject;
        if (bodyText != null) bodyText.text = task.emailContent;

        // BUNU KALDIRICAM! 
        // Görevi okundu olarak iþaretle
        if (!readTasks.Contains(task))
        {
            readTasks.Add(task);
            Debug.Log($"[MailApp] {task.taskName} okundu. IDE PULL kilidi açýldý.");
        }
    }

    // OnClick buton editörde atandý! 

    public void BackToInbox()
    {
        if (messagePage != null) messagePage.SetActive(false);
        if (messageBox != null) messageBox.gameObject.SetActive(true);
    }


}