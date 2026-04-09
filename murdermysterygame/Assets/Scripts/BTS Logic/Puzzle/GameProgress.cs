using System;
using System.Collections.Generic;
using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance;

    private HashSet<string> completedFlags = new HashSet<string>();
    public event Action OnProgressUpdated;

    void Awake()
    {
        Debug.Log("GameProgress Awake called on: " + gameObject.name);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameProgress Instance assigned");
        }
        else
        {
            Debug.LogWarning("Duplicate GameProgress destroyed on: " + gameObject.name);
            Destroy(gameObject);
        }
    }

    public void SetFlag(string flag)
    {
        Debug.Log("SetFlag called with: " + flag);

        if (string.IsNullOrEmpty(flag))
            return;

        if (completedFlags.Add(flag))
        {
            Debug.Log("Flag set: " + flag);
            OnProgressUpdated?.Invoke();
        }
        else
        {
            Debug.Log("Flag already set: " + flag);
        }
    }

    public bool HasFlag(string flag)
    {
        bool hasIt = completedFlags.Contains(flag);
        Debug.Log("Checking flag: " + flag + " = " + hasIt);
        return hasIt;
    }
}