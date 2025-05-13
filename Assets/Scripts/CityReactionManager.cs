using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;


public class CityReactionManager : MonoBehaviour
{
    public static CityReactionManager Instance;

    [Header("UI Refs")]
    //public GameObject reactionPopup;
    public TextMeshProUGUI narrativeTextUI;
    public GameObject reactionImage;
    public GameObject narrativePanel;

    [Header("Audio")]
    public AudioSource reactionAudioSource;
    public AudioSource ambientAudioSource;
    public AudioClip ambientCityLoop;

    [Header("Billboards")]
    [SerializeField] private TextMeshProUGUI statusBillboard;
    [SerializeField] private TextMeshProUGUI humorBillboard;
    [SerializeField] private TextMeshProUGUI eventBillboard;

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
    private Dictionary<GameObject, Vector3> highRiseTargetPositions = new Dictionary<GameObject, Vector3>();

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

        if (ambientAudioSource != null && ambientCityLoop != null)
        {
            ambientAudioSource.clip = ambientCityLoop;
            ambientAudioSource.Play();
        }

        foreach (GameObject highRise in highRises)
        {
            highRiseTargetPositions[highRise] = highRise.transform.position;
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
        //UpdateEventBillboardMessage(challenge);
    }

    private void ShowReaction(CityReaction reaction)
    {
        narrativePanel.SetActive(true);
        narrativeTextUI.text = reaction.NarrativeText;
        reactionImage.GetComponent<Image>().sprite = reaction.VisualCue;
        //reactionImage.sprite = reaction.visualCue;

        if (reaction.ReactionSound)
        {
            //bool playTwice = false;
            //if ((reaction.Challenge.Name == "Housing Shortage" || reaction.Challenge.Name == "Water Supply Crisis") && reaction.ReactionLevel == ChallengeReactionLevel.Failure)

            if (reaction.ReactionSound.name != "distant-ambulance-siren-long")
            {
                StartCoroutine(PlayReactionSoundTwice(reaction.ReactionSound));
            }
            else
            {
                reactionAudioSource.PlayOneShot(reaction.ReactionSound);
            }
        }

        //reactionImage.SetActive(true);
    }

    public void ShowCityReaction()
    {
        int[] parameters = SpiderDiagram.Instance.parameters;

        // Update billboards
        UpdateStatusBillboard(parameters);
        UpdateHumorBillboard(parameters);

        // Transport
        int transport = SpiderDiagram.Instance.parameters[0];

        // Road material transition
        Color targetRoadColor = Color.Lerp(badRoadColor, goodRoadColor, transport / 10f);
        StartCoroutine(SmoothColor(roadMaterial, "_Color", roadMaterial.color, targetRoadColor, colorChangeSpeed));

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
        //Color targetGrassColor = Color.Lerp(badGrassColor, goodGrassColor, ecological / 10f);
        //StartCoroutine(SmoothColor(grassMaterial, "_Color", grassMaterial.color, targetGrassColor, colorChangeSpeed));

        // Tree density (e.g., enabling/disabling tree prefabs)
        int activeTrees = Mathf.RoundToInt(treeObjects.Length * (ecological / 10f));
        for (int i = 0; i < treeObjects.Length; i++)
            treeObjects[i].SetActive(i < activeTrees);

        //trash

        // Water resources
        int water = SpiderDiagram.Instance.parameters[2];

        // Grass color
        int grassHealth = Mathf.Min(ecological, water); // Use the worse one
        Color targetGrassColor = Color.Lerp(badGrassColor, goodGrassColor, grassHealth / 10f);
        StartCoroutine(SmoothColor(grassMaterial, "_Color", grassMaterial.color, targetGrassColor, colorChangeSpeed));

        // Sky color
        //Color targetSkyColor = Color.Lerp(drySkyColor, goodSkyColor, water / 10f);
        ////skySpriteRenderer.color = Color.Lerp(skySpriteRenderer.color, targetSkyColor, Time.deltaTime * colorChangeSpeed);
        //StartCoroutine(SmoothSpriteColor(skySpriteRenderer, skySpriteRenderer.color, targetSkyColor, colorChangeSpeed));

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
            if (airQuality < 4)
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
        float midPoint = 5f;

        // Calculate absolute distances from center (the more extreme value wins)
        float waterDeviation = Mathf.Abs(water - midPoint);
        float airDeviation = Mathf.Abs(airQuality - midPoint);

        bool waterIsMoreExtreme = waterDeviation > airDeviation;

        // Pick the "bad" color based on the more extreme parameter
        Color badColor = waterIsMoreExtreme ? drySkyColor : pollutedSkyColor;
        int drivingValue = waterIsMoreExtreme ? water : airQuality;

        Debug.Log("air: " + airQuality);
        Color targetSkyColor = Color.Lerp(badColor, goodSkyColor, drivingValue / 10f);
        StartCoroutine(SmoothSpriteColor(skySpriteRenderer, skySpriteRenderer.color, targetSkyColor, colorChangeSpeed));

        // Fog
        UpdateFogSmooth(airQuality);

        // Windmills
        UpdateWindmills(airQuality);


        // Economy
        int economy = SpiderDiagram.Instance.parameters[5];

        // Tents
        float visibilityPercentage = MapThreshold(economy, 4f, inverse: true);
        EnableAmount(tents, Mathf.RoundToInt(tents.Length * visibilityPercentage));
        
        // Highrise buildings
        Debug.Log("economy " +  economy);
        visibilityPercentage = MapThreshold(economy, 6f);
        Debug.Log("visibility "+visibilityPercentage);
        //EnableAmount(highRises, Mathf.RoundToInt(highRises.Length * visibilityPercentage));
        int amountToEnable = Mathf.RoundToInt(highRises.Length * visibilityPercentage);
        Debug.Log("amount to enable  " + amountToEnable);

        for (int i = 0; i < highRises.Length; i++)
        {
            GameObject building = highRises[i];
            bool shouldBeActive = i < amountToEnable;

            if (shouldBeActive && !building.activeSelf)
            {
                building.SetActive(true);

                if (highRiseTargetPositions.TryGetValue(building, out Vector3 targetPos))
                {
                    StartCoroutine(RaiseBuildingToTarget(building, targetPos));
                }
            }
            else if (!shouldBeActive && building.activeSelf)
            {
                building.SetActive(false);
            }
        }

        //EnableAmount(pedestrians, Mathf.RoundToInt(maxPeople * (economy / 10f)));

    }

    private IEnumerator PlayReactionSoundTwice(AudioClip clip)
    {
        reactionAudioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length + 0.1f);
        reactionAudioSource.PlayOneShot(clip);
    }


    private void EnableAmount(GameObject[] objects, int amountToEnable)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(i < amountToEnable);
        }
    }

    private IEnumerator SmoothColor(Material mat, string colorProperty, Color start, Color end, float speed)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            mat.SetColor(colorProperty, Color.Lerp(start, end, t));
            yield return null;
        }
        mat.SetColor(colorProperty, end); // Ensure final color is exact
    }

    private IEnumerator SmoothSpriteColor(SpriteRenderer sprite, Color start, Color end, float speed)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            sprite.color = Color.Lerp(start, end, t);
            yield return null;
        }
        sprite.color = end;
    }

    float currentIntensity;
    private void UpdateFogSmooth(float airQuality)
    {
        float targetIntensity = MapThreshold(airQuality, 4f, inverse: true);
        float clampedFogIntensity = Mathf.Lerp(0f, 0.55f, targetIntensity);
        StartCoroutine(SmoothFogTransition(clampedFogIntensity));
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

    private IEnumerator RaiseBuildingToTarget(GameObject building, Vector3 targetPosition, float startOffset = -15f, float duration = 4f)
    {
        Vector3 startPos = new Vector3(targetPosition.x, targetPosition.y + startOffset, targetPosition.z);
        building.transform.position = startPos;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            building.transform.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        building.transform.position = targetPosition; // Snap to final
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

    void UpdateStatusBillboard(int[] parameters)
    {
        string[] categories = { "Transport", "Ecology", "Water Resource", "Energy", "Air Quality", "Economy" };

        //int maxDeviation = 0;
        //int extremeIndex = 0;

        //for (int i = 0; i < parameters.Length; i++)
        //{
        //    int deviation = Mathf.Abs(parameters[i] - 5); // Midpoint
        //    if (deviation > maxDeviation)
        //    {
        //        maxDeviation = deviation;
        //        extremeIndex = i;
        //    }
        //}

        //int value = parameters[extremeIndex];
        //string category = categories[extremeIndex];


        // First, find the max deviation value
        int maxDeviation = parameters
            .Select(p => Mathf.Abs(p - 5))
            .Max();

        // Collect all indices that match the max deviation
        List<int> extremeIndices = new List<int>();
        for (int i = 0; i < parameters.Length; i++)
        {
            if (Mathf.Abs(parameters[i] - 5) == maxDeviation)
            {
                extremeIndices.Add(i);
            }
        }

        // Randomly choose one of the equally extreme indices
        int chosenIndex = extremeIndices[Random.Range(0, extremeIndices.Count)];
        int value = parameters[chosenIndex];
        string category = categories[chosenIndex];

        if (value <= 3)
        {
            string[] badPhrases = {
            $"{category} Crisis!",
            $"{category} Falling Fast",
            $"Fix {category} Now!",
            $"Low {category} Alert",
            $"City {category} Struggles"
            };
            statusBillboard.text = badPhrases[Random.Range(0, badPhrases.Length)];
        }
        else if (value >= 7)
        {
            string[] goodPhrases = {
            $"{category} Looking Great!",
            $"{category} Is Thriving!",
            $"{category} Success!",
            $"{category} Wins Big",
            $"Top Marks in {category}"
            };
            statusBillboard.text = goodPhrases[Random.Range(0, goodPhrases.Length)];
        }
        else
        {
            string[] neutralPhrases = {
            $"{category} Holding Steady",
            $"{category} Looks Fine",
            $"{category} Okay For Now",
            $"Mild {category} Status",
            $"Stable {category} Levels"
            };
            statusBillboard.text = neutralPhrases[Random.Range(0, neutralPhrases.Length)];
        }
    }

    private void UpdateHumorBillboard(int[] parameters)
    {
        if (parameters.All(p => p >= 4 && p <= 6))
        {
            string[] jokes = {
            "Smog? That's just city seasoning.",
            "Our pigeons have jobs now!",
            "Still no flying cars... yet.",
            "Your taxes built this billboard.",
            "Free Wi-Fi: Just kidding."
            };

            humorBillboard.text = jokes[Random.Range(0, jokes.Length)];
        }
        else
        {
            string[] messages = {
            "Every Decision Matters",
            "Small Choices, Big Impact",
            "Invest in the Future, Now.",
            "The Future Is in Your Hands.",
            "Don’t Let Your City Go Dark."
            };
            humorBillboard.text = messages[Random.Range(0, messages.Length)];
        }
    }

    public void UpdateEventBillboardMessage(Challenge challenge)
    {
        if (challenge.Name == "Housing Shortage")
        {
            eventBillboard.text = "Housing crisis averted!";
        } 
        else if (challenge.Name == "Aging Infrastructure")
        {
            eventBillboard.text = "🔧 City upgrades underway!";
        }
        else if (challenge.Name == "Air Quality Crisis")
        {
            eventBillboard.text = "Masks off, breath easy.";
        }
        else if (challenge.Name == "Water Supply Crisis")
        {
            eventBillboard.text = "💧 Water flows again!";
        }
            
    }

}
