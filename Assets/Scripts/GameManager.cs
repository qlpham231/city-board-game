using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton for easy access

    public TextMeshProUGUI roundText;  // UI Text to display current round
    public TextMeshProUGUI timerText; // UI Text to show round timer
    public int roundDuration = 10;    // Set your round duration
    public int currentRound = 1;

    private bool isSolutionSubmitted = false;

    public List<Player> playerList = new List<Player>();
    public List<Challenge> challenges = new List<Challenge>();
    public List<Solution> solutions = new List<Solution>();
    public List<Resource> resources = new List<Resource>();

    public UnityEvent<List<Player>> onPlayerListUpdated = new();

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
        InitializeData();
    }

    private void InitializeData()
    {
        Resource govGrant = new Resource("Government Grant", ResourceType.Funding, 10, null, 2, new List<Role> { Role.EnvAdvocate, Role.Citizen });
        resources = new List<Resource> { govGrant };

        Solution campaignAir = new Solution("Public Awareness Campaign on Air Quality", 6, 3, new List<Role> { Role.PrivateRep, Role.CityOfficial }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 3 }, { ResourceType.Community, 3 } }, 1, 2, 0, 0, 3, 1);
        solutions = new List<Solution> { campaignAir };


        challenges.Add(new Challenge(
            "Housing Shortage",
            ChallengeType.LongTerm,
            -5,
            new List<Solution> { campaignAir },
            Resources.Load<Sprite>("Textures/HousingShortage")));

        challenges.Add(new Challenge(
            "Air Quality",
            ChallengeType.Sudden,
            -10,
            new List<Solution> { campaignAir },
            Resources.Load<Sprite>("Textures/AirQuality")));
    }


    public void SetPlayers(List<Player> players)
    {
        playerList = players;
        Debug.Log($"GameManager received {playerList.Count} players.");
        onPlayerListUpdated.Invoke(players);
    }

    public void StartGame()
    {
        if (playerList.Count == 0)
        {
            Debug.LogError("No players set! Cannot start game.");
            return;
        }

        Debug.Log("Game is starting...");
        StartNextRound();
    }

    private void StartNextRound()
    {
        roundText.text = "Round " + currentRound + " of 4";
        ChallengeManager.Instance.StartNewRound();
        StartCoroutine(StartRoundTimer());
    }

    private IEnumerator StartRoundTimer()
    {
        float timeRemaining = roundDuration; // seconds
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timeRemaining = Mathf.Max(timeRemaining, 0); // Ensure it doesn't go below 0
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = minutes + ":" + seconds.ToString("00");
            yield return null;
        }

        EndRound();
    }

    private void EndRound()
    {
        Debug.Log("Round " + currentRound + " ended.");

        isSolutionSubmitted = false;  // Reset flag for the next round

        // Trigger solution selection UI
        SubmissionManager.Instance.solutionSelectionCanvas.SetActive(true);

        // Wait until the solution is submitted before proceeding
        StartCoroutine(WaitForSolutionSubmission());
    }

    public void RegisterSolution()
    {
        isSolutionSubmitted = true;
    }

    private IEnumerator WaitForSolutionSubmission()
    {
        while (!isSolutionSubmitted)
        {
            yield return null; // Keep waiting
        }

        currentRound++;

        if (currentRound <= 4)
        {
            StartNextRound();
        }
        else
        {
            Debug.Log("Game Over! Final scores calculated.");
            // TODO: Show final score UI
        }
    }

    

    public Challenge GetChallengeByName(string name)
    {
        return challenges.Find(ch => ch.Name == name);
    }

    public Solution GetSolutionByName(string name)
    {
        return solutions.Find(sol => sol.Name == name);
    }

    public Resource GetResourceByName(string name)
    {
        return resources.Find(res => res.Name == name);
    }

    public Player GetPlayerByName(string name)
    {
        return playerList.Find(p => p.PlayerNr == Int32.Parse(name.Substring(7)));
    }

}
