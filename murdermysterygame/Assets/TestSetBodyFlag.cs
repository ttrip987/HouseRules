using UnityEngine;

public class TestSetBodyFlag : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B key pressed");

            if (GameProgress.Instance != null)
            {
                Debug.Log("GameProgress exists");
                GameProgress.Instance.SetFlag("FoundBody");
                Debug.Log("Manual test set FoundBody");
            }
            else
            {
                Debug.LogError("GameProgress.Instance is NULL");
            }
        }
    }
}