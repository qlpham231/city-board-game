using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public Transform challengeContainer; // Grid container for challenge cards
    public GameObject challengeCardPrefab; // Prefab for displaying challenge cards
    public List<Challenge> challenges = new List<Challenge>();

    // Start is called before the first frame update
    void Start()
    {
        challenges.Add(new Challenge(
            "Housing Shortage",
            ChallengeType.LongTerm,
            -5,
            null,
            Resources.Load<Sprite>("Textures/HousingShortage"),
            new int[] { -1, -1, -1, 0, 0, -2 }));

        Sprite longTermSprite = challenges[Random.Range(0, challenges.Count)].ChallengeImage;
        GameObject newCard = Instantiate(challengeCardPrefab, challengeContainer);
        newCard.GetComponent<Image>().sprite = longTermSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
