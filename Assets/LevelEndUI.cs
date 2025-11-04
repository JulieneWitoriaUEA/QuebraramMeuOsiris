using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEndUI : MonoBehaviour
{
    [SerializeField] private int currentLevel;
    [SerializeField] private LevelStandardScoresSO standardScoresSO;
    [SerializeField] private IntVariable totalRotationsSO;
    [SerializeField] private FloatVariable totalTimeSO;
    [SerializeField] private Image SecondStar;
    [SerializeField] private Image ThirdStar;
    [SerializeField] private TextMeshProUGUI levelEndBottomText;
    private void OnEnable()
    {
        levelEndBottomText.text = "Rotations: " + totalRotationsSO.value + $"\nTotal Time: {totalTimeSO.value:F2}s\nPress Enter for next stage";
        if(totalRotationsSO.value > standardScoresSO.standardScores[currentLevel].totalRotations 
            && totalTimeSO.value > standardScoresSO.standardScores[currentLevel].totalTime)
            SecondStar.color = Color.black;
        if (totalRotationsSO.value > standardScoresSO.standardScores[currentLevel].totalRotations
            || totalTimeSO.value > standardScoresSO.standardScores[currentLevel].totalTime)
            ThirdStar.color = Color.black;
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)||Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Time.timeScale = 1f;
        }
    }

}
