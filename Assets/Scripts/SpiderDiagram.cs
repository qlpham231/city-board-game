using UnityEngine;
using UnityEngine.UI.Extensions; // UI Extensions for UILineRenderer

public class SpiderDiagram : MonoBehaviour
{
    public GameObject linePrefab; // Assign a LineRenderer prefab in Unity
    public int steps = 10; // Scale divided into 10 steps

    Vector2[] points;

    float[] angles = { 90f, 30f, -30f, -90f, -150f, 150f }; // Fixed angles
    Color[] colors = { Color.red, Color.blue, Color.green, Color.magenta, Color.cyan};

    void Start()
    {
        DrawLine(5, 7, 6, 8, 4, 9);
    }

    void DrawLine(float transport, float ecological, float waterResources, float energy, float airQuality, float economy)
    {
        // Get the current scale factor based on the image width
        float scaleFactor = GetComponent<RectTransform>().rect.width / 460f;
        float radius = 144f * scaleFactor; // Adjust the radius dynamically

        float[] values = { transport, ecological, waterResources, energy, airQuality, economy };

        GameObject newLine = Instantiate(linePrefab, transform); // Create a new line
        UILineRenderer uiLineRenderer = newLine.GetComponent<UILineRenderer>();

        // Assign color according to round
        Color roundColor = colors[GameManager.Instance.currentRound - 1];
        uiLineRenderer.color = roundColor;

        points = new Vector2[angles.Length + 1];

        for (int i = 0; i < angles.Length; i++)
        {
            float angle = angles[i] * Mathf.Deg2Rad;
            float normalizedValue = values[i] / steps;
            float x = Mathf.Cos(angle) * (normalizedValue * radius);
            float y = Mathf.Sin(angle) * (normalizedValue * radius);
            points[i] = new Vector2(x, y);
        }

        points[angles.Length] = points[0]; // Close the loop
        uiLineRenderer.Points = points;
        uiLineRenderer.SetVerticesDirty(); // Force UI update
    }
}
