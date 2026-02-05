using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public string sceneName;
    public float[] playerPosition; // x, y, z
    public List<ItemData> inventory;
}