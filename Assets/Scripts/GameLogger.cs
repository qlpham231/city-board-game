using System.IO;
using UnityEngine;

public class GameLogger : MonoBehaviour
{
    private string filePath;
    public static GameLogger Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Define the file path in persistent data path
        filePath = Path.Combine(Application.persistentDataPath, "GameLog.txt");

        // Create a new log file or clear existing one
        File.WriteAllText(filePath, "Game Log Started:\n");
    }

    public void LogEvent(string message)
    {
        string logEntry = $"{System.DateTime.Now}: {message}\n";

        // Append log entry to file
        File.AppendAllText(filePath, logEntry);

        Debug.Log($"Event Logged: {message}");
    }
}
