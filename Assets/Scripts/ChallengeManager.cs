using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChallengeManager : MonoBehaviour
{
    public static ChallengeManager Instance;
    public Transform challengeContainer; // Grid container for challenge cards
    public GameObject challengeCardPrefab; // Prefab for displaying challenge cards
    public List<Challenge> availableLongTermChallenges; // List of long-term challenges
    public List<Challenge> availableSuddenCrises; // List of sudden crises challenges
    private List<Challenge> activeChallenges = new();

    private int consecutiveCrises = 0;
    private float crisisChance = 0.2f; // Starts at 20% and increases

    public UnityEvent<List<Challenge>> onCurrentChallengesUpdated = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        availableLongTermChallenges = GameManager.Instance.challenges
            .Where(c => c.Type == ChallengeType.LongTerm)
            .ToList();
        availableSuddenCrises = GameManager.Instance.challenges
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
        // Draws a long-term challenge in Round 1 and Round 3
        if (GameManager.Instance.currentRound == 1 || GameManager.Instance.currentRound == 3)
        {
            Challenge selectedChallenge = DrawUniqueChallenge(ref availableLongTermChallenges);
            activeChallenges.Add(selectedChallenge);
            onCurrentChallengesUpdated.Invoke(activeChallenges);
            DisplayChallenge(selectedChallenge);
        }

        if (GameManager.Instance.currentRound > 1 && Random.value < crisisChance && consecutiveCrises < 2)
        {
            Challenge selectedCrisis = DrawUniqueChallenge(ref availableSuddenCrises);
            activeChallenges.Add(selectedCrisis);
            onCurrentChallengesUpdated.Invoke(activeChallenges);
            DisplayChallenge(selectedCrisis);

            consecutiveCrises++;
            crisisChance = 0.2f; // Reset crisis chance
        }
        else
        {
            crisisChance += 0.2f; // Increase crisis probability for next round
            consecutiveCrises = 0; // Reset consecutive crisis counter if no crisis occurs
        }
    }

    private Challenge DrawUniqueChallenge(ref List<Challenge> challengeList)
    {
        if (challengeList.Count == 0)
        {
            Debug.LogWarning("All challenges have been used! Resetting challenge pool.");
            challengeList = ResetChallenges(challengeList);
        }

        int index = Random.Range(0, challengeList.Count);
        Challenge selectedChallenge = challengeList[index];
        challengeList.RemoveAt(index); // Remove so it won’t be repeated

        return selectedChallenge;
    }

    private void DisplayChallenge(Challenge challenge)
    {
        GameObject newCard = Instantiate(challengeCardPrefab, challengeContainer);
        newCard.GetComponent<Image>().sprite = challenge.ChallengeImage;
        Debug.Log($"Displayed challenge: {challenge.Name}");
    }

    private List<Challenge> ResetChallenges(List<Challenge> challengeList)
    {
        if (challengeList == availableLongTermChallenges)
        {
            return new List<Challenge>(GameManager.Instance.challenges
                .Where(c => c.Type == ChallengeType.LongTerm)
                .ToList());
        }
        else
        {
            return new List<Challenge>(GameManager.Instance.challenges
                .Where(c => c.Type == ChallengeType.Sudden)
                .ToList());
        }
    }
}
