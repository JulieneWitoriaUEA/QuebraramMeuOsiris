using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu"); 
    }
}
