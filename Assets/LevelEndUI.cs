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
    [SerializeField] private UnlockedLeves _unlockedLevelsSO;
    private void OnEnable()
    {
        levelEndBottomText.text = "Rotações: " + totalRotationsSO.value + $"\nTempo Total: {totalTimeSO.value:F2}s\nPressione Enter para continuar";
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
            _unlockedLevelsSO.unlockedLevels++;
            Time.timeScale = 1f;
        }
    }

}
