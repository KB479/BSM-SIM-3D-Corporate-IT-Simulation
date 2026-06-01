using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameManager;
using static UnityEngine.CullingGroup;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    // readonly yeni sytax, readonly hýzlý global get
    public GameState CurrentGameState => currentGameState;
    public int CurrentCredit => currentCredit;
    public int CurrentDayIndex => currentDayIndex;


    [Header("Oyun Ayarlarý")]
    public int startingCredit = 100;

    [Header("Gün Verileri (DaySO Listesi)")]
    public List<DaySO> daysList;

    [Header("Oyun Durumu (Read Only!)")]
    [SerializeField] private GameState currentGameState;
    [SerializeField] private int currentCredit;
    [SerializeField] private int currentDayIndex = 0;

    [SerializeField] private bool areDailyTasksDone = false;

    // Yeni gün duyurusu
    public event EventHandler<OnNewDayStartedEventArgs> OnNewDayStarted;
    public class OnNewDayStartedEventArgs: EventArgs
    {
        public int currentDayIndex; // TM'in buna ihtiyacý yok gibi fakat UI için lazým olabilir, þimdilik tutuyorum.
        public DaySO currentDaySO; // direkt ilgili günü TM'e iletir
    }

    // Player kredi duyurusu 
    public event EventHandler<OnCreditChangedEventArgs> OnCreditChanged;
    public class OnCreditChangedEventArgs : EventArgs
    {
        public int currentCredit; 
    }

    // State deðiþim duyurusu
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public GameState currentGameState; 
    }

    // EndGameUI için ateþlenir
    public event EventHandler<OnEndGameEventArgs> OnEndGame;
    public class OnEndGameEventArgs : EventArgs
    {
        public bool isGameWin; 
    }


    private void Awake()
    {

        if (Instance != null)
        {
            Debug.LogError("There are more than one Game Manager Instance!"); 
        }

        Instance = this;
    }

    private void Start()
    {
        ChangeState(GameState.Tutorial); 
    }

    // public, tutorial UI için, day 0 gelince direkt StartNewDayRequest ile çözülebilir?
    public void StartFirstDayRequest()
    {
        if (currentGameState != GameState.Tutorial) return;

        currentCredit = startingCredit;
        currentDayIndex = 0;

        // UI'ýn haberi olsun diye oyun baþý kredisi duyurulur
        OnCreditChanged?.Invoke(this, new OnCreditChangedEventArgs
        {
            currentCredit = currentCredit
        });

        StartNewDay();
    }

    private void StartNewDay()
    {
        // yeni gün stete duyur
        ChangeState(GameState.NewGameDay);
        areDailyTasksDone = false;


        // günler bitti mi kontrolü 
        if (currentDayIndex < daysList.Count)
        {
            Debug.Log($" {currentDayIndex + 1}. GÜN BAÞLIYOR");

            // TM dinliyor, index ve DaySO argüman taþýnýr, TM DaySO'ya göre menüyü hazýrlar
            OnNewDayStarted?.Invoke(this, new OnNewDayStartedEventArgs
            {
                currentDayIndex = currentDayIndex,
                currentDaySO = daysList[currentDayIndex]
                

            });

            ChangeState(GameState.DayInProgress);
        }
        else
        {
            // Liste bittiyse tüm günler oynandý demektir, endgame (win) geçilir
            ChangeState(GameState.EndGame);
            
            OnEndGame?.Invoke(this, new OnEndGameEventArgs{

             isGameWin = true
            
            }); 

            Debug.Log("TEBRÝKLER! TÜM GÜNLERÝ TAMAMLADINIZ!");
        }

    }

    // TM günlük görevler sonuçlanýnca bunu çaðýrýr
    public void OnAllTasksCompletedForToday()
    {
        areDailyTasksDone = true;
        Debug.Log("Günün tüm görevleri bitti. Oyuncu günü bitirebilir.");
    }


    // oyuncu günü bitir diyince, þimdilik f tuþuna basýnca, çaðrýlýr.
    public void TryEndDay()
    {
        if (currentGameState != GameState.DayInProgress) return; 

        if (areDailyTasksDone)
        {
            ChangeState(GameState.EndGameDay);
            Debug.Log("Gün Baþarýyla Kapatýldý. Gün Sonu Raporu Gösteriliyor...");

        }
        else
        {
            Debug.LogWarning("Daha bitirmemiþ olduðun görevler var! Günü bitiremezsin.");
        }
    }

    // public metot, yeni güne geç butonuna basýlýnca bu talep fonksiyonu çaðýrýlacak
    public void StartNextDayRequest()
    {
        if (currentGameState != GameState.EndGameDay) return;

        currentDayIndex++;
        StartNewDay();
    }


    public void StartInteractionRequest()
    {
        // Oyuncu sadece ve sadece ofiste özgürce gezerken (DayInProgress) PC'ye oturabilir
        if (currentGameState != GameState.DayInProgress) return;

        ChangeState(GameState.Interacting);

    }

    public void EndInteractionRequest()
    {
        if (currentGameState == GameState.Interacting)
        {
            ChangeState(GameState.DayInProgress); 
        }

    }

    // TM için public, kredi  managerýný netleþtirince düzenle, private olmasý daha uygun olur
    public void ModifyCredit(int amount)
    {
        currentCredit += amount;

        OnCreditChanged?.Invoke(this, new OnCreditChangedEventArgs
        {
            currentCredit = currentCredit
        });

        if (currentCredit <= 0)
        {
            currentCredit = 0; // Eksiye düþmesin 
            ChangeState(GameState.EndGame);
            OnEndGame?.Invoke(this, new OnEndGameEventArgs
            {
                isGameWin = false
            });
            Debug.Log("GAME OVER! Kredi Sýfýrlandý.");
        }
    }

    // TM için o gün hangi DaySO'yu kullanacaðýný bilsin diye bir yardýmcý metot
    public DaySO GetCurrentDayConfig()
    {
        if (currentDayIndex >= 0 && currentDayIndex < daysList.Count)
        {
            return daysList[currentDayIndex];
        }
        return null; // Gün kalmadýysa null döner
    }


    // State deðiþimini koda gömmektense merkezi yönetmek
    private void ChangeState(GameState newState)
    {
        if (currentGameState == newState) return;

        currentGameState = newState;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            currentGameState = currentGameState
        }); 
    }



}
