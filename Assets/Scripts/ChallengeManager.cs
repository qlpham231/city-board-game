using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeManager : MonoBehaviour
{
    public Transform challengeContainer; // Grid container for challenge cards
    public GameObject challengeCardPrefab; // Prefab for displaying challenge cards
    public TextMeshProUGUI roundText; // UI Text to display current round
    public TextMeshProUGUI timerText; // UI Text to show round timer
    public Sprite[] longTermChallengeSprites; // Array of sprites for long-term challenges
    public Sprite[] suddenCrisisSprites; // Array of sprites for sudden crises

    private int currentRound = 1;
    private bool isRoundActive = false;
    private int consecutiveCrises = 0;
    private float crisisChance = 0.2f; // Starts at 20% and increases
    private Sprite currentLongTermChallenge;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(StartNewRound());
    }

    public IEnumerator StartNewRound()
    {
        isRoundActive = true;
        roundText.text = "Round " + currentRound + " of 4";
        yield return new WaitForSeconds(1);

        // Assign a long-term challenge in Round 1 and Round 3
        if (currentRound == 1 || currentRound == 3)
        {
            currentLongTermChallenge = longTermChallengeSprites[Random.Range(0, longTermChallengeSprites.Length)];
            DisplayChallengeCard(currentLongTermChallenge);
        }

        // Crisis chance increases if no crisis has happened in previous rounds
        if (currentRound > 1 && Random.value < crisisChance && consecutiveCrises < 2)
        {
            Sprite suddenCrisis = suddenCrisisSprites[Random.Range(0, suddenCrisisSprites.Length)];
            DisplayChallengeCard(suddenCrisis);
            consecutiveCrises++;
            crisisChance = 0.2f; // Reset crisis chance
        }
        else
        {
            crisisChance += 0.2f; // Increase crisis probability for next round
            consecutiveCrises = 0; // Reset consecutive crisis counter if no crisis occurs
        }

        // Start countdown timer
        StartCoroutine(StartTimer());
    }

    void DisplayChallengeCard(Sprite challengeSprite)
    {
        GameObject newCard = Instantiate(challengeCardPrefab, challengeContainer);
        newCard.GetComponent<Image>().sprite = challengeSprite;
    }

    IEnumerator StartTimer()
    {
        float timeRemaining = 10f; // 15 minutes in seconds
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timeRemaining = Mathf.Max(timeRemaining, 0); // Ensure it never goes below 0
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = minutes + ":" + seconds.ToString("00");
            yield return null;
        }

        EndRound();
    }

    void EndRound()
    {
        isRoundActive = false;
        Debug.Log("Round " + currentRound + " ended. Proceed to voting phase.");
        // TODO: Trigger voting system here

        currentRound++;
        if (currentRound <= 4)
        {
            StartCoroutine(StartNewRound());
        }
        else
        {
            Debug.Log("Game Over. Calculate scores.");
            // TODO: Call final scoring and results display here
        }
    }
}
