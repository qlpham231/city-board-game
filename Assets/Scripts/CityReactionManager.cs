using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CityReactionManager : MonoBehaviour
{
    public static CityReactionManager Instance;

    [Header("UI Refs")]
    //public GameObject reactionPopup;
    public TextMeshProUGUI narrativeTextUI;
    public GameObject reactionImage;
    public AudioSource audioSource;
    public GameObject narrativePanel;

    [Header("Transport")]
    public Material roadMaterial;
    public Color32 badRoadColor;
    public Color32 goodRoadColor;
    float colorChangeSpeed = 2f;
    public GameObject[] cars;

    [Header("Ecological")]
    public Material grassMaterial;
    public Color32 badGrassColor;
    public Color32 goodGrassColor;
    public GameObject[] treeObjects;

    [Header("Water")]
    public SpriteRenderer skySpriteRenderer;
    public Color32 drySkyColor;
    public Color32 goodSkyColor;

    [Header("Energy")]
    public ParticleSystem[] smokeParticleSystems;

    [Header("Air Quality")]
    public Color32 pollutedSkyColor;
    public Material fogMaterial;
    public GameObject[] windRotators;

    [Header("Economy")]
    public GameObject[] tents;
    public GameObject[] highRises;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        narrativePanel.SetActive(false);

        foreach (ParticleSystem p in smokeParticleSystems)
        {
            if (p.isPlaying)
                p.Stop();
        }

        ShowCityReaction();
    }

    public void PlayReaction(Challenge challenge, ChallengeReactionLevel level)
    {
        CityReaction reaction = GameManager.Instance.allReactions.Find(r =>
            r.Challenge.Name == challenge.Name && r.ReactionLevel == level);

        if (reaction == null)
        {
            Debug.LogWarning("No matching city reaction found!");
            return;
        }

        ShowReaction(reaction);
    }

    private void ShowReaction(CityReaction reaction)
    {
        narrativePanel.SetActive(true);
        narrativeTextUI.text = reaction.NarrativeText;
        reactionImage.GetComponent<Image>().sprite = reaction.VisualCue;
        //reactionImage.sprite = reaction.visualCue;

        if (reaction.ReactionSound)
        {
            audioSource.PlayOneShot(reaction.ReactionSound);
        }

        reactionImage.SetActive(true);
        ShowCityReaction();
    }

    public void ShowCityReaction()
    {
        int[] parameters = SpiderDiagram.Instance.parameters;

        // Transport
        int transport = SpiderDiagram.Instance.parameters[0];

        // Road material transition
        Color targetRoadColor = Color.Lerp(badRoadColor, goodRoadColor, transport / 10f);
        roadMaterial.color = Color.Lerp(roadMaterial.color, targetRoadColor, Time.deltaTime * colorChangeSpeed);

        // Toggle cars
        //for (int i = 0; i < cars.Length; i++)
        //    cars[i].SetActive(i < Mathf.RoundToInt(maxCars * (transport / 10f)));
        for (int i = 0; i < cars.Length; i++)
        {
            MeshRenderer renderer = cars[i].GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.enabled = (i < Mathf.RoundToInt(cars.Length * (transport / 10f)));
        }

        // Ecological
        int ecological = SpiderDiagram.Instance.parameters[1];
        // Grass color
        Color targetGrassColor = Color.Lerp(badGrassColor, goodGrassColor, ecological / 10f);
        grassMaterial.color = Color.Lerp(grassMaterial.color, targetGrassColor, Time.deltaTime * colorChangeSpeed);

        // Tree density (e.g., enabling/disabling tree prefabs)
        int activeTrees = Mathf.RoundToInt(treeObjects.Length * (ecological / 10f));
        for (int i = 0; i < treeObjects.Length; i++)
            treeObjects[i].SetActive(i < activeTrees);

        //trash

        // Water resources
        int water = SpiderDiagram.Instance.parameters[2];
        // Grass color
        targetGrassColor = Color.Lerp(badGrassColor, goodGrassColor, water / 10f);
        grassMaterial.color = Color.Lerp(grassMaterial.color, targetGrassColor, Time.deltaTime * colorChangeSpeed);

        // Sky color
        Color targetSkyColor = Color.Lerp(drySkyColor, goodSkyColor, water / 10f);
        skySpriteRenderer.color = Color.Lerp(skySpriteRenderer.color, targetSkyColor, Time.deltaTime * colorChangeSpeed);

        // Water signs/fountains
        //EnableAmount(waterSigns, Mathf.RoundToInt(maxSigns * (1 - water / 10f)));
        //EnableAmount(waterFountains, Mathf.RoundToInt(maxFountains * (water / 10f)));


        // Energy
        int energy = SpiderDiagram.Instance.parameters[3];

        // Air Quality
        int airQuality = SpiderDiagram.Instance.parameters[4];
        // Smoke
        foreach (ParticleSystem p in smokeParticleSystems)
        {
            if (airQuality < 4 || energy < 4)
            {
                if (!p.isPlaying)
                    p.Play();
            }
            else
            {
                if (p.isPlaying)
                    p.Stop();
            }
        }
         
        // Sky color
        targetSkyColor = Color.Lerp(pollutedSkyColor, goodSkyColor, airQuality / 10f);
        skySpriteRenderer.color = Color.Lerp(skySpriteRenderer.color, targetSkyColor, Time.deltaTime * colorChangeSpeed);

        // Fog
        UpdateFogSmooth(airQuality);

        // Windmills
        UpdateWindmills(airQuality);


        // Economy
        int economy = SpiderDiagram.Instance.parameters[4];

        // Tents
        float visibilityPercentage = MapThreshold(economy, 4f, inverse: true);
        EnableAmount(tents, Mathf.RoundToInt(tents.Length * visibilityPercentage));
        
        // Highrise buildings
        visibilityPercentage = MapThreshold(economy, 6f);
        EnableAmount(highRises, Mathf.RoundToInt(highRises.Length * visibilityPercentage));

        //EnableAmount(pedestrians, Mathf.RoundToInt(maxPeople * (economy / 10f)));

    }


    private void EnableAmount(GameObject[] objects, int amountToEnable)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(i < amountToEnable);
        }
    }

    float currentIntensity;
    private void UpdateFogSmooth(float airQuality)
    {
        float targetIntensity = MapThreshold(airQuality, 4f, inverse: true);
        float clampedFogIntensity = Mathf.Lerp(0f, 0.55f, targetIntensity);
        StartCoroutine(SmoothFogTransition(clampedFogIntensity));

        //currentIntensity = Mathf.Lerp(currentIntensity, clampedFogIntensity, Time.deltaTime * 2f); // Smooth fade
        //fogMaterial.SetFloat("_FogIntensity", currentIntensity);
    }

    private IEnumerator SmoothFogTransition(float targetIntensity)
    {
        while (Mathf.Abs(currentIntensity - targetIntensity) > 0.01f)
        {
            currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * 2f);
            fogMaterial.SetFloat("_FogIntensity", currentIntensity);
            yield return null; // wait one frame
        }

        currentIntensity = targetIntensity; // snap to final value
        fogMaterial.SetFloat("_FogIntensity", currentIntensity);
    }

    void UpdateWindmills(float airQuality)
    {
        float visibility = MapThreshold(airQuality, 6f); // Only starts appearing after value > 6
        int activeCount = Mathf.RoundToInt(windRotators.Length * visibility);

        for (int i = 0; i < windRotators.Length; i++)
        {
            WindmillRotator rotatorScript = windRotators[i].GetComponent<WindmillRotator>();
            if (rotatorScript != null)
                rotatorScript.enabled = (i < activeCount);
        }
    }

    private float MapThreshold(float value, float threshold, bool inverse = false)
    {
        if (inverse)
        {
            return value < threshold ? 1f - Mathf.Clamp01((value - 0f) / (threshold - 0f)) : 0f;
        }
        else
        {
            return value > threshold ? Mathf.Clamp01((value - threshold) / (10f - threshold)) : 0f;
        }
    }
}
