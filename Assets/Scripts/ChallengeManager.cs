using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeManager : MonoBehaviour
{
    public static ChallengeManager Instance;
    public Transform challengeContainer; // Grid container for challenge cards
    public GameObject challengeCardPrefab; // Prefab for displaying challenge cards
    public List<Challenge> longTermChallenges; // List of long-term challenges
    public List<Challenge> suddenCrisis; // List of sudden crises challenges

    private int consecutiveCrises = 0;
    private float crisisChance = 0.2f; // Starts at 20% and increases

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        longTermChallenges = GameManager.Instance.challenges
            .Where(c => c.Type == ChallengeType.LongTerm)
            .ToList();
        suddenCrisis = GameManager.Instance.challenges
            .Where(c => c.Type == ChallengeType.Sudden)
            .ToList();
    }

    // Start a new challenge round (called by GameManager)
    public void StartNewRound()
    {
        Debug.Log("Starting a new challenge round...");
        DisplayChallengeCard();
    }

    private void DisplayChallengeCard()
    {
        // Assign a long-term challenge in Round 1 and Round 3
        if (GameManager.Instance.currentRound == 1 || GameManager.Instance.currentRound == 3)
        {
            Sprite longTermSprite = longTermChallenges[Random.Range(0, longTermChallenges.Count)].ChallengeImage;
            GameObject newCard = Instantiate(challengeCardPrefab, challengeContainer);
            newCard.GetComponent<Image>().sprite = longTermSprite;
            Debug.Log("Challenge card added");
        }


        if (GameManager.Instance.currentRound > 1 && Random.value < crisisChance && consecutiveCrises < 2)
        {
            Sprite suddenCrisisSprite = suddenCrisis[Random.Range(0, suddenCrisis.Count)].ChallengeImage;
            GameObject newCard = Instantiate(challengeCardPrefab, challengeContainer);
            newCard.GetComponent<Image>().sprite = suddenCrisisSprite;

            consecutiveCrises++;
            crisisChance = 0.2f; // Reset crisis chance
        }
        else
        {
            crisisChance += 0.2f; // Increase crisis probability for next round
            consecutiveCrises = 0; // Reset consecutive crisis counter if no crisis occurs
        }
    }
}
