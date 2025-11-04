using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private FloatVariable totalTimeSO;

    private void Start()
    {
        totalTimeSO.value = 0f;
    }

    private void Update()
    {
        totalTimeSO.value += Time.deltaTime;
    }
}
