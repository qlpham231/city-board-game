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
    private Dictionary<Challenge, int> challengeStartRounds = new(); // Maps challenge to start round
    private Dictionary<Challenge, GameObject> challengeCards = new(); // Maps challenge to its UI card

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
            challengeStartRounds[selectedChallenge] = GameManager.Instance.currentRound;
            onCurrentChallengesUpdated.Invoke(activeChallenges);
            DisplayChallenge(selectedChallenge);

        }

        // Draws a short-term challenge in Round 2 and Round 4
        if (GameManager.Instance.currentRound == 2 || GameManager.Instance.currentRound == 4)
        {
            Challenge selectedCrisis = DrawUniqueChallenge(ref availableSuddenCrises);
            activeChallenges.Add(selectedCrisis);
            challengeStartRounds[selectedCrisis] = GameManager.Instance.currentRound;
            onCurrentChallengesUpdated.Invoke(activeChallenges);
            DisplayChallenge(selectedCrisis);

        }

        //if (GameManager.Instance.currentRound > 1 && Random.value < crisisChance && consecutiveCrises < 2)
        //{
        //    Challenge selectedCrisis = DrawUniqueChallenge(ref availableSuddenCrises);
        //    activeChallenges.Add(selectedCrisis);
        //    challengeStartRounds[selectedCrisis] = GameManager.Instance.currentRound;
        //    onCurrentChallengesUpdated.Invoke(activeChallenges);
        //    DisplayChallenge(selectedCrisis);

        //    consecutiveCrises++;
        //    crisisChance = 0.2f; // Reset crisis chance
        //}
        //else
        //{
        //    crisisChance += 0.2f; // Increase crisis probability for next round
        //    consecutiveCrises = 0; // Reset consecutive crisis counter if no crisis occurs
        //}

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
        challengeCards[challenge] = newCard;
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

    public void ResolveChallenge(Challenge challenge)
    {
        if (activeChallenges.Contains(challenge))
        {
            Debug.Log($"Challenge '{challenge.Name}' resolved by player.");
            activeChallenges.Remove(challenge);
            challengeStartRounds.Remove(challenge);
            //onCurrentChallengesUpdated.Invoke(activeChallenges);
        }

        if (challengeCards.TryGetValue(challenge, out GameObject card))
        {
            Destroy(card);
            challengeCards.Remove(challenge);
        }
    }

    public void ApplyPenalties()
    {
        List<Challenge> unresolvedChallenges = new(activeChallenges);

        foreach (Challenge challenge in unresolvedChallenges)
        {
            int challengeAge = GameManager.Instance.currentRound - challengeStartRounds[challenge];

            if ((challenge.Type == ChallengeType.Sudden && challengeAge >= 1) ||
                (challenge.Type == ChallengeType.LongTerm && challengeAge >= 3))
            {
                Debug.Log($"Penalty applied for unresolved challenge: {challenge.Name}");
                ScoreManager.Instance.ApplyPenalty(challenge.PenaltyPoints);
                activeChallenges.Remove(challenge);
                challengeStartRounds.Remove(challenge);

                if (challengeCards.TryGetValue(challenge, out GameObject card))
                {
                    Destroy(card);
                    challengeCards.Remove(challenge);
                }
                CityReactionManager.Instance.PlayReaction(challenge, ChallengeReactionLevel.Failure);
            }
        }
    }
}
