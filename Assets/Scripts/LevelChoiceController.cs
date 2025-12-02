using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChoiceController : MonoBehaviour
{
    [SerializeField] private UnlockedLeves _unlockedLeves;
    [SerializeField] private Button[] _levelButtons;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(SceneManager.GetActiveScene().name != "Menu")
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }

    private void Start()
    {
        if(SceneManager.GetActiveScene().name != "Menu") 
            return;
        for(int i = 0; i < _unlockedLeves.unlockedLevels; i++)
        {
            _levelButtons[i].interactable = true;
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
