using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startGameButton; 
    [SerializeField] private Button optionsButton; 
    [SerializeField] private Button archiveButton; 
    [SerializeField] private Button quitGameButton; 



    public void StartGame()
    {
        SceneManager.LoadScene(1); 
    }


    public void QuiitGame()
    {
        Application.Quit();
    }
    
    


}
