using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewIntVariable", menuName = "ScriptableObjects/IntVariable", order = 1)]
public class IntVariable : ScriptableObject
{
    public int value;
}
