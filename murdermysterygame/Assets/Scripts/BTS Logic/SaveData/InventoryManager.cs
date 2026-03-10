using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<string> items = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(string itemName)
    {
        if (!items.Contains(itemName))
            items.Add(itemName);
    }

    public void RemoveItem(string itemName)
    {
        if (items.Contains(itemName))
            items.Remove(itemName);
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public void SetInventory(List<string> loadedItems)
    {
        items = new List<string>(loadedItems);
    }
}