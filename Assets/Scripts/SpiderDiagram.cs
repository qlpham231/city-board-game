using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions; // UI Extensions for UILineRenderer

public class SpiderDiagram : MonoBehaviour
{
    public static SpiderDiagram Instance;
    public Canvas spiderDiagramCanvas;
    public GameObject linePrefab; // Assign a LineRenderer prefab in Unity
    public int steps = 10; // Scale divided into 10 steps

    Vector2[] points;

    float[] angles = { 90f, 30f, -30f, -90f, -150f, 150f }; // Fixed angles
    Color[] colors = { Color.red, Color.magenta, Color.blue, Color.green, Color.cyan };

    int[] parameters = { 4, 4, 5, 5, 4, 5 }; // Start parameter values for the city

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

    private void Start()
    {
        DrawBaseLine();
    }

    void DrawBaseLine()
    {
        //float scaleFactor = GetComponent<RectTransform>().rect.width / 460f;
        //float radius = 144f * scaleFactor;
        float radius = GetComponent<RectTransform>().rect.width / 2;

        float[] values = { parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5] };

        GameObject baseLine = Instantiate(linePrefab, transform);
        baseLine.transform.localPosition = Vector3.zero;
        UILineRenderer uiLineRenderer = baseLine.GetComponent<UILineRenderer>();

        uiLineRenderer.color = Color.grey;
        //uiLineRenderer.LineThickness = 2f;

        Vector2[] basePoints = new Vector2[angles.Length + 1];

        for (int i = 0; i < angles.Length; i++)
        {
            float angle = angles[i] * Mathf.Deg2Rad;
            float normalizedValue = values[i] / steps;
            float x = Mathf.Cos(angle) * (normalizedValue * radius);
            float y = Mathf.Sin(angle) * (normalizedValue * radius);
            basePoints[i] = new Vector2(x, y);
        }

        basePoints[angles.Length] = basePoints[0];
        uiLineRenderer.Points = basePoints;
        uiLineRenderer.SetVerticesDirty();
    }


    void DrawLine(float transport, float ecological, float waterResources, float energy, float airQuality, float economy)
    {
        // Get the current scale factor based on the image width
        //float scaleFactor = GetComponent<RectTransform>().rect.width / 460f;
        //float radius = 144f * scaleFactor; // Adjust the radius dynamically
        float radius = GetComponent<RectTransform>().rect.width / 2;

        float[] values = { transport, ecological, waterResources, energy, airQuality, economy };

        GameObject newLine = Instantiate(linePrefab, transform); // Create a new line
        newLine.transform.localPosition = Vector3.zero;
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

    public void UpdateSpiderDiagram(List<Solution> acceptedSolutions)
    {
        foreach(Solution s in acceptedSolutions)
        {
            for (int i = 0; i < s.ParameterChanges.Length; i++)
            {
                parameters[i] += s.ParameterChanges[i];
                parameters[i] = Mathf.Clamp(parameters[i], 0, 10);
            }
        }
        DrawLine(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]);
    }
}
