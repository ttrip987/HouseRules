using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string sceneName;

    public float playerX;
    public float playerY;
    public float playerZ;

    public int credits;

    public List<string> inventoryItems = new List<string>();
}