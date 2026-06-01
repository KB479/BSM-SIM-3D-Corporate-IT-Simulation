using UnityEngine;
using UnityEngine.UI;

public class VirtualPC_UI : MonoBehaviour
{
    public static VirtualPC_UI Instance { get; private set; }

    [Header("Masaüstü Elemanlarý")]
    [SerializeField] private Button shutdownButton; 
    [SerializeField] private Button appIconButton; 
    [SerializeField] private Button adminAppIconButton; 
    [SerializeField] private Button mailAppIconButton; 

    [Header("Uygulama Referanslarý")]
    [SerializeField] private AppUI appUI;
    [SerializeField] private AdminAppUI adminAppUI;
    [SerializeField] private MailAppUI mailAppUI;

    private InteractionUnit connectedEU; // Þu an hangi eu-kasaya baðlýyý tutarýz

    private void Awake()
    {
        if (Instance != null) { Debug.LogError("Birden fazla VirtualPC_UI var!"); return; }
        Instance = this;

        shutdownButton.onClick.AddListener(TurnOffOS);
        appIconButton.onClick.AddListener(OpenTaskApp);
        adminAppIconButton.onClick.AddListener(OpenAdminTaskApp);
        mailAppIconButton.onClick.AddListener(OpenMailApp);

        gameObject.SetActive(false);
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
        }
    }

    private void GameManager_OnStateChanged(object sender, GameManager.OnStateChangedEventArgs e)
    {
        // Pcde iken interacting stateti deðiþirse, oyun biterse, direkt game over ekraný için
        if (e.currentGameState != GameState.Interacting)
        {
            HideAll(); 
        }
    }


    // Ýlgili eu, os'i açar
    public void TurnOnOS(InteractionUnit eu)
    {
        // eudan datalarý, görevleri alýcaz
        connectedEU = eu;
        GameManager.Instance.StartInteractionRequest(); // Karakteri dondur, imleci sal

        // Uygulamalar kapalý baþlat 
        if (appUI != null) appUI.CloseApp(); 
        if (mailAppUI != null) mailAppUI.CloseMailApp();

        // admin app için window controller vs yok, direkt setactive ile kapat þimdilik
        adminAppUI.gameObject.SetActive(false);

        // sadece desktop görünür
        gameObject.SetActive(true);
    }

    private void OpenTaskApp()
    {
        // Uygulamayý aç ve ona hangi kasaya baðlý olduðumuzu ilet
        appUI.OpenApp(connectedEU);
    }

    private void OpenMailApp()
    {
        mailAppUI.OpenMailApp(connectedEU);
    }

    private void OpenAdminTaskApp()
    {
        // Uygulamayý aç ve ona hangi kasaya baðlý olduðumuzu ilet
        adminAppUI.OpenApp(connectedEU); 
    }

    

    private void TurnOffOS()
    {
        // Uygulamalara kapanma emrini veriyoruz. Onlar hem pencerelerini hem kendilerini uyutacaklar.
        if (appUI != null) appUI.CloseApp();
        if (mailAppUI != null) mailAppUI.CloseMailApp();

        // admini için þimdilik direkt kapat de
        if (adminAppUI != null) adminAppUI.gameObject.SetActive(false);

        // Masaüstünü kapat ve 3D dünyaya dön
        gameObject.SetActive(false);
        GameManager.Instance.EndInteractionRequest();
    }

    // fail safe
    private void HideAll()
    {
        gameObject.SetActive(false);
    }

}
