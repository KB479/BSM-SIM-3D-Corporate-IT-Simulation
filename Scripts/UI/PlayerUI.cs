using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerCreditText;


    private void Start()
    {
        GameManager.Instance.OnCreditChanged += GameManager_OnCreditChanged;
    }

    private void GameManager_OnCreditChanged(object sender, GameManager.OnCreditChangedEventArgs e)
    {
        playerCreditText.text = e.currentCredit.ToString();
    }




}
