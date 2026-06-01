using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [Header("Database")]
    public TaskDatabaseSO taskDatabase;
    
    [Header("Task Pools")]
    [SerializeField] private List<TaskSO> availableTasksPool = new List<TaskSO>();
    [SerializeField] private List<TaskSO> activeDailyTasks = new List<TaskSO>();

    [Header("Past Tasks")]
    [SerializeField] private List<CompletedTaskRecords> completedTaskRecords = new List<CompletedTaskRecords>();


    public event EventHandler OnDailyTasksReady; 


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are more than one Task Manager Instance!"); 
        }

        Instance = this;


        // Oyun baþladýðýnda depodaki orijinal görevleri bozmamak için kopyasýný alýyoruz
        if (taskDatabase != null)
        {
            availableTasksPool = new List<TaskSO>(taskDatabase.allTasks);
        }
        else
        {
            Debug.LogError("TaskDatabase atanmamýþ!");
        }

    }

    private void Start()
    {
        GameManager.Instance.OnNewDayStarted += GameManager_OnNewDayStarted;
    }

    private void OnDestroy()
    {
        // data leak kontrol
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnNewDayStarted -= GameManager_OnNewDayStarted;
        }
    }
    


    private void GameManager_OnNewDayStarted(object sender, GameManager.OnNewDayStartedEventArgs e)
    {

        // DaySo parametresi ile menüyü hazýrla
        GenerateDailyTasks(e.currentDaySO);
    }

    private void GenerateDailyTasks(DaySO daySO)
    {
        activeDailyTasks.Clear(); 

        if (daySO == null) return;

        // DaySO içindeki her bir kurala (TaskRequirement) sýrayla bakar
        foreach (TaskRequirement req in daySO.taskRequirements)
        {
            // Havuzda bu kurala uyan tüm görevleri filtrele
            List<TaskSO> matchingTasks = availableTasksPool.FindAll(t => t.type == req.type && t.difficulty == req.difficulty);

            // Ýstenen sayý (req.count) kadar görev seç
            for (int i = 0; i < req.count; i++)
            {
                if (matchingTasks.Count > 0)
                {
                    // Þimdilik listedeki ilk elemaný alýyoruz. (Ýleride buraya Random seçim ekleyebiliriz)
                    // zaten þu an her tipten her zorluktan bir görev var
                    TaskSO selectedTask = matchingTasks[0];

                    activeDailyTasks.Add(selectedTask);

                    // seçilen görev havuzdan silinir, hem total pooldan, hem de filtresi uyanlar listesinden çýkarýyoruz!
                    
                    availableTasksPool.Remove(selectedTask);
                    matchingTasks.Remove(selectedTask);
                }
                else
                {
                    Debug.LogWarning($"Havuzda yeterli {req.difficulty} seviye {req.type} görevi kalmadý!");
                }
            }
        }

        Debug.Log($"Görevler hazýrlandý! O gün için toplam: {activeDailyTasks.Count} görev listelendi.");

        // EU'lara (Etkileþim Arayüzlerine) "Görevler menüde hazýr, gelip çekebilirsiniz" duyurusu yapýyoruz
        OnDailyTasksReady?.Invoke(this, EventArgs.Empty);
    }


    public void ProcessTaskResult(TaskSO completedTask, bool isSuccess)
    {
        // Sonuçlanan görev, geçmiþ kayýt listesine eklenir (Tab-raporUI'lar için)
        completedTaskRecords.Add(new CompletedTaskRecords
        {
            task = completedTask,
            isSuccess = isSuccess
        });

        // Sonuca göre kazanýlacak veya kaybedilecek krediyi hesapla ve GM'e ilet
        int creditChange = isSuccess ? completedTask.successCreditReward : -completedTask.failCreditPenalty;
        GameManager.Instance.ModifyCredit(creditChange);

        // Görevi to-do'dan  çýkar
        activeDailyTasks.Remove(completedTask);

        Debug.Log($"Görev Sonuçlandý: {completedTask.taskName} | Baþarý: {isSuccess} | Kalan Görev: {activeDailyTasks.Count}");

        // Gün bitti mi kontrolü
        if (activeDailyTasks.Count == 0)
        {
            GameManager.Instance.OnAllTasksCompletedForToday();
        }
    }


    // Singleton, direkt eriþim için public get metotlarý koydum, event ateþlemedim

    // EU'larýn o anki aktif görevleri görebilmesi için dýþarýya açýk yardýmcý bir metod
    public List<TaskSO> GetActiveDailyTasks()
    {
        return activeDailyTasks;
    }

    // Sonuçlanan görevlere dýþardan eriþim için
    public List<CompletedTaskRecords> GetCompletedTaskRecords()
    {
        return completedTaskRecords;
    }


}
