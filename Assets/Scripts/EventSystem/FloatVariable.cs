using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewFloatVariable", menuName = "ScriptableObjects/FloatVariable", order = 1)]
public class FloatVariable : ScriptableObject
{
   public float value;
}