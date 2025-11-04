using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Standard Scores", menuName = "ScriptableObjects/Level Standard Scores SO", order = 1)]
public class LevelStandardScoresSO : ScriptableObject
{
    [Serializable]
    public struct LevelScores
    {
        public int totalRotations;
        public float totalTime;
    }

    public List<LevelScores> standardScores;
}