using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneGate : MonoBehaviour
{
    public string requiredFlag = "Puzzle1Complete";
    public string sceneToLoad;

    private bool playerInRange;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            TryEnter();
        }
    }

    void TryEnter()
    {
        if (GameProgress.Instance.HasFlag(requiredFlag))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.Log("You can't go here yet.");
            // Hook into your dialogue system here instead
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}