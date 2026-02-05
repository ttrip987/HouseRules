using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string savePath;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        GameSaveData data = new GameSaveData();
        data.sceneName = SceneManager.GetActiveScene().name;

        Vector3 pos = player.transform.position;
        data.playerPosition = new float[] { pos.x, pos.y, pos.z };

        data.inventory = InventoryManager.Instance.items;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No save file found");
            return;
        }

        string json = File.ReadAllText(savePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name != data.sceneName) return;

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                player.transform.position = new Vector3(
                    data.playerPosition[0],
                    data.playerPosition[1],
                    data.playerPosition[2]
                );
            }

            InventoryManager.Instance.items = data.inventory;
        };

        SceneManager.LoadScene(data.sceneName);
    }
}
