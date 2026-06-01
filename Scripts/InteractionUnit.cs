using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class InteractionUnit : MonoBehaviour, IInteractable
{
    // ARTIK BASE CLASSTIR BU!! ABSTRAC KOYMADIM ÞÝMDÝLÝK DÝKKATLÝ KULLAN
    // Bütün iþlevler her euda olacak, özel taraflarý interact ile tetikledikleri görev çözme arayüzleri

    [Header("Kimlik")]
    public TaskType type;
    public string unitName;

    [Header("Arayüz")]
    public TextMeshProUGUI taskTextUI; 

    // Görevleri kuyruk ile diziyoruz, ilerde oyuncu istediði görevden baþlayabilsin, dictionray yapýsý belki? 
    private Queue<TaskSO> taskQueue = new Queue<TaskSO>();

    private TaskSO currentTask;
    private bool isProcessingResult = false;

    private void Start()
    {
        TaskManager.Instance.OnDailyTasksReady += TaskManager_OnDailyTasksReady;

        UpdateUI("Yeni gün bekleniyor..."); // ne olur ne olmaz
    }

    // data leak önlemek için güvelik açýsýndan: 
    private void OnDestroy()
    {
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.OnDailyTasksReady -= TaskManager_OnDailyTasksReady;
        }
    }

    private void TaskManager_OnDailyTasksReady(object sender, EventArgs e)
    {
        // O günkü tüm aktif görevleri TM'den istenir, event argümaný ile taþýnamdý þimdilik
        List<TaskSO> allDailyTasks = TaskManager.Instance.GetActiveDailyTasks();

        // Kuyruk temizlenir ne olur ne olmaz
        taskQueue.Clear();

        foreach (TaskSO task in allDailyTasks)
        {
            if (task.type == type)
            {
                taskQueue.Enqueue(task);
            }
        }

        // Kuyruktaki ilk görevi ekrana yansýt - bu kuyruk yapýsý deðiþebilir ilerde
        LoadNextTask();
    }

    private void LoadNextTask()
    {
        if (taskQueue.Count > 0)
        {
            currentTask = taskQueue.Dequeue();
            UpdateUI($"<b>{unitName}</b>\nGörev: {currentTask.taskName}\n<size=70%>[E] Çöz - [R] Hata Bildir</size>");
        }
        else
        {
            // Kuyruk boþsa EU boþtadýr
            currentTask = null;
            UpdateUI($"<b>{unitName}</b>\nBekleyen iþ yok.");
        }
    }


    // Yani Interact için güvenlik kontrolünü burda tuttuk, asýl interact iþlevini ExecuteInteraction ile overrideladýk

    public void Interact()
    {
        if (isProcessingResult) return;

        ExecuteInteraction();
    }

    // mirasçýlarýn overridelayacaðý asýl interact iþlevi
    protected virtual void ExecuteInteraction()
    {
        // Eðer bir objeye yanlýþlýkla doðrudan InteractionUnit atýlýrsa, konsola uyarý bas
        Debug.LogWarning($"<color=orange>DÝKKAT:</color> {gameObject.name} üzerindeki " +
            $"{unitName} doðrudan ana sýnýfý kullanýyor! Lütfen SoftwareUnit, HRUnit gibi miras alan bir script atayýn.");
    }


    public TaskSO GetCurrentTask()
    {
        return currentTask;
    }

    public bool IsProcessing()
    {
        return isProcessingResult;
    }
    
    // AppUI sonuca göre bunu çaðýrýr
    public void ResolveCurrentTask(bool isSuccess)
    {
        if (currentTask == null || isProcessingResult) return;

        StartCoroutine(ShowResultRoutine(isSuccess));
    }

    private IEnumerator ShowResultRoutine(bool isSuccess)
    {
        isProcessingResult = true;

        // Önce TM'e sonucu bildir ki oyunun backendi kredi vs. güncellensin
        TaskManager.Instance.ProcessTaskResult(currentTask, isSuccess);

        // Ekrana görsel feedback
        string resultMark = isSuccess ? "<color=green> BAÞARILI</color>" : "<color=red> BAÞARISIZ</color>";
        UpdateUI($"<b>{unitName}</b>\n{currentTask.taskName}\n{resultMark}");

        // Oyuncunun bu geri bildirimi okuyabilmesi için 1.5 saniye bekle
        yield return new WaitForSeconds(1.5f);

        // Kilidi aç ve sýradaki göreve geç
        isProcessingResult = false;
        LoadNextTask();
    }

    private void UpdateUI(string message)
    {
        if (taskTextUI != null)
        {
            taskTextUI.text = message;
        }
    }
}