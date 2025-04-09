using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton for easy access

    public Button openSubmissionPanelButton; // Button to end round and submit early
    public TextMeshProUGUI roundText;  // UI Text to display current round
    public TextMeshProUGUI timerText; // UI Text to show round timer
    public int roundDuration = 900;    // Set your round duration
    public int currentRound = 1;

    private Coroutine timerCoroutine;
    private bool isSolutionSubmitted = false;

    public List<Solution> implementedSolutions = new();
    public List<RoleGoal> roleGoals = new();
    public List<Player> playerList = new List<Player>();
    public List<Challenge> challenges = new List<Challenge>();
    public List<Solution> solutions = new List<Solution>();
    public List<Resource> resources = new List<Resource>();
    public List<CityReaction> allReactions = new List<CityReaction>();

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

    private void Start()
    {
        openSubmissionPanelButton.onClick.AddListener(() =>
        {
            StopCoroutine(timerCoroutine); // Stop the timer
            openSubmissionPanelButton.interactable = false;
            EndRound(); // Ends the round
        });

        openSubmissionPanelButton.interactable = false;
    }

    private void InitializeData()
    {
        Solution tollTaxFreeEV = new Solution("Make Toll Tax Free on EVs", 6, 2, new List<Role> { Role.EnvAdvocate }, new Dictionary<ResourceType, int> { { ResourceType.Policy, 2 } }, new int[] { 1, 1, 0, -1, 1, 0 });
        Solution elVehicleIncen = new Solution("Electric Vehicle Incentive Program", 8, 3, new List<Role> { Role.PrivateRep, Role.Citizen }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 4 }, { ResourceType.Policy, 4 } }, new int[] { 2, 1, 0, -1, 2, -1 });
        Solution waterMoniPol = new Solution("Water Usage Monitoring Policy", 7, 3, new List<Role> { Role.PrivateRep }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 2 }, { ResourceType.Policy, 3 } }, new int[] { 0, 1, 2, -1, 0, 1 });
        Solution waterConserPol = new Solution("Water Conservation Policy", 8, 2, new List<Role> { Role.PrivateRep }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 4 }, { ResourceType.Policy, 2 } }, new int[] { 0, 1, 3, 0, 0, -1 });
        Solution aparConstr = new Solution("Apartment Construction Initiative", 10, 4, new List<Role> { Role.ExternalCollaborator }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 8 }, { ResourceType.Land, 5 }, { ResourceType.Policy, 4 } }, new int[] { 1, -2, -1, -1, -1, 2 });
        Solution builRow = new Solution("Building Row Houses", 9, 0, null, new Dictionary<ResourceType, int> { { ResourceType.Funding, 6 }, { ResourceType.Land, 7 }, { ResourceType.Policy, 3 } }, new int[] { 0, -1, -1, 0, 0, 2 });
        Solution elVehicleRental = new Solution("Electrical Vehicle Rental Program", 7, 2, new List<Role> { Role.CityOfficial }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 5 } }, new int[] { 2, 1, 0, -1, 2, 1 });
        Solution pubPriPart = new Solution("Public-Private Partnership (PPP)", 6, 2, new List<Role> { Role.PrivateRep }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 3 }, { ResourceType.Policy, 2 } }, new int[] { 1, -1, -1, 0, 0, 2 });
        Solution smaIrrTech = new Solution("Smart Irrigation Technology", 8, 0, null, new Dictionary<ResourceType, int> { { ResourceType.Technology, 5 }, { ResourceType.Funding, 3 } }, new int[] { 0, 2, 2, -1, 0, 0 });
        Solution greenBuil = new Solution("Green Building Standards", 8, 0, null, new Dictionary<ResourceType, int> { { ResourceType.Policy, 3 }, { ResourceType.Funding, 2 } }, new int[] { 0, 2, 1, 2, 1, -1 });
        Solution pubAwareAir = new Solution("Public Awareness Campaign on Air Quality", 6, 3, new List<Role> { Role.PrivateRep, Role.CityOfficial }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 3 }, { ResourceType.Community, 3 } }, new int[] { 1, 1, 0, 0, 2, 0 });
        Solution urbForest = new Solution("Urban Forest Program", 7, 2, new List<Role> { Role.Citizen }, new Dictionary<ResourceType, int> { { ResourceType.Community, 4 }, { ResourceType.Funding, 3 }, { ResourceType.Land, 2 } }, new int[] { 0, 3, 2, 0, 2, -1 });
        Solution waterConserAwareCam = new Solution("Water Conservation Awareness Campaign", 6, 3, new List<Role> { Role.Citizen }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 1 }, { ResourceType.Community, 3 } }, new int[] { 0, 1, 2, 0, 0, 0 });
        Solution freePubTrans = new Solution("Free Public Transport Program", 9, 3, new List<Role> { Role.CityOfficial }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 4 }, { ResourceType.Policy, 3 } }, new int[] { 3, 1, 0, -1, 2, -1 });
        Solution subsEvPur = new Solution("Subsidized EV Purchase Program", 8, 3, new List<Role> { Role.CityOfficial, Role.PrivateRep }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 3 }, { ResourceType.Policy, 2 } }, new int[] { 2, 1, 0, -1, 2, -1 });
        Solution subsAparCon = new Solution("Subsidized Apartment Construction", 10, 0, null, new Dictionary<ResourceType, int> { { ResourceType.Funding, 6 } }, new int[] { 1, -2, -1, -1, -1, 2 });
        Solution cityPlanHou = new Solution("City Plan Sharing for Housing Crisis", 5, 0, null, new Dictionary<ResourceType, int> { }, new int[] { 1, 0, 0, 0, 0, 1 });
        Solution waterResSha = new Solution("Water Resource Sharing Agreement", 5, 2, new List<Role> { Role.CityOfficial }, new Dictionary<ResourceType, int> { { ResourceType.Community, 2 }, { ResourceType.Policy, 2 } }, new int[] { 0, 0, 2, 0, 0, 1 });
        Solution waterRecTech = new Solution("Water Recycling Technology Introduction", 9, 3, new List<Role> { Role.EnvAdvocate }, new Dictionary<ResourceType, int> { { ResourceType.Technology, 3 } }, new int[] { 0, 1, 3, -1, 0, 0 });
        Solution airMoniTech = new Solution("Air Quality Monitoring Technology", 10, 3, new List<Role> { Role.PrivateRep }, new Dictionary<ResourceType, int> { { ResourceType.Technology, 4 } }, new int[] { 1, 0, 0, -1, 3, 1 });

        Solution smartInfraIni = new Solution("Smart Infrastructure Initiative", 8, 3, new List<Role> { Role.CityOfficial }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 3 }, { ResourceType.Technology, 5 } }, new int[] { 2, 1, 1, -1, 1, 1 });
        Solution fastContruCon = new Solution("Fast-Track Construction Contracts", 7, 3, new List<Role> { Role.CityOfficial, Role.ExternalCollaborator }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 3 }, { ResourceType.Policy, 4 } }, new int[] { 2, -1, 0, -1, -1, 2 });
        Solution greenInfraRe = new Solution("Green Infrastructure Retrofit", 8, 2, new List<Role> { Role.PrivateRep }, new Dictionary<ResourceType, int> { { ResourceType.Community, 4 }, { ResourceType.Policy, 3 } }, new int[] { 1, 2, 1, 0, 1, -1 });
        Solution camSustainTra = new Solution("Campaign on Sustainable Transit", 6, 3, new List<Role> { Role.Citizen, Role.CityOfficial }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 3 }, { ResourceType.Community, 3 } }, new int[] { 2, 1, 0, -1, 1, 0 });
        Solution volStreRep = new Solution("Volunteer-Led Street Repair Program", 7, 2, new List<Role> { Role.EnvAdvocate }, new Dictionary<ResourceType, int> { { ResourceType.Funding, 2 }, { ResourceType.Community, 4 } }, new int[] { 2, 1, 1, 0, 1, 0 });
        Solution interInfraEx = new Solution("International Infrastructure Exchange", 5, 3, new List<Role> { Role.CityOfficial }, new Dictionary<ResourceType, int> { }, new int[] { 1, 0, 0, 0, 0, 1 });

        solutions = new List<Solution> { tollTaxFreeEV, elVehicleIncen, waterMoniPol, waterConserPol, aparConstr, builRow, elVehicleRental, pubPriPart,
        smaIrrTech, greenBuil, pubAwareAir, urbForest, waterConserAwareCam, freePubTrans, subsEvPur, subsAparCon, cityPlanHou, waterResSha, waterRecTech,
        airMoniTech, smartInfraIni, fastContruCon, greenInfraRe, camSustainTra, volStreRep, interInfraEx
        };

        Resource govGrant = new Resource("Government Grant", ResourceType.Funding, 10, null, 2, new List<Role> { Role.EnvAdvocate, Role.Citizen });
        Resource techGrant = new Resource("Technology Grant", ResourceType.Funding, 8, new List<Solution> { smaIrrTech, waterRecTech, airMoniTech }, 0, null);
        Resource volNetwork = new Resource("Volunteer Network", ResourceType.Community, 3, null, 0, null);
        Resource lobbying = new Resource("Lobbying Effort", ResourceType.Policy, 3, null, 0, null);
        Resource polSupport = new Resource("Political Support", ResourceType.Policy, 2, null, 2, new List<Role> { Role.EnvAdvocate });
        Resource landAqui = new Resource("Land Acquisition", ResourceType.Land, 10, null, 2, new List<Role> { Role.EnvAdvocate, Role.Citizen });
        Resource corSponsor = new Resource("Corporate Sponsorship", ResourceType.Technology, 5, null, 2, new List<Role> { Role.CityOfficial, Role.PrivateRep, Role.EnvAdvocate });
        Resource hackathon = new Resource("Hackathon Initiative", ResourceType.Technology, 3, null, 2, new List<Role> { Role.CityOfficial, Role.PrivateRep });
        Resource finInvest = new Resource("Financial Investment", ResourceType.Funding, 10, null, 2, new List<Role> { Role.CityOfficial });
        Resource comCrowdFun = new Resource("Community Crowdfunding", ResourceType.Funding, 5, new List<Solution> { pubAwareAir, urbForest, waterConserAwareCam }, 2, new List<Role> { Role.Citizen });

        resources = new List<Resource> { govGrant, techGrant, volNetwork, lobbying, polSupport, landAqui, corSponsor, hackathon, finInvest, comCrowdFun };

        roleGoals.Add(new RoleGoal(Role.CityOfficial, new List<Solution> { tollTaxFreeEV, elVehicleIncen, waterMoniPol, waterConserPol }, 3));
        roleGoals.Add(new RoleGoal(Role.PrivateRep, new List<Solution> { smaIrrTech, waterRecTech, airMoniTech, smartInfraIni }, 2));
        roleGoals.Add(new RoleGoal(Role.EnvAdvocate, new List<Solution> { greenBuil, urbForest, smaIrrTech, airMoniTech, pubAwareAir, waterRecTech, waterConserAwareCam, waterMoniPol, waterConserPol, greenInfraRe, camSustainTra, volStreRep }, 3));
        roleGoals.Add(new RoleGoal(Role.Citizen, new List<Solution> { urbForest, greenBuil, waterConserAwareCam, pubAwareAir, freePubTrans, greenInfraRe, camSustainTra, volStreRep }, 3));
        roleGoals.Add(new RoleGoal(Role.ExternalCollaborator, new List<Solution> { cityPlanHou, waterResSha, waterRecTech, airMoniTech, interInfraEx }, 2));


        Challenge houseShortage = new Challenge(
            "Housing Shortage",
            ChallengeType.LongTerm,
            5,
            new List<Solution> { aparConstr, builRow, pubPriPart, greenBuil, subsAparCon, cityPlanHou, interInfraEx },
            Resources.Load<Sprite>("Textures/Challenge Card 1"));

        Challenge agingInfra = new Challenge(
            "Aging Infrastructure",
            ChallengeType.LongTerm,
            5,
            new List<Solution> { smartInfraIni, fastContruCon, greenInfraRe, camSustainTra, volStreRep, interInfraEx, cityPlanHou },
            Resources.Load<Sprite>("Textures/Challenge Card 4"));

        Challenge airCrisis = new Challenge(
            "Air Quality Crisis",
            ChallengeType.Sudden,
            10,
            new List<Solution> { tollTaxFreeEV, elVehicleIncen, elVehicleRental, pubAwareAir, freePubTrans, subsEvPur, airMoniTech },
            Resources.Load<Sprite>("Textures/Challenge Card 2"));

        Challenge waterCrisis = new Challenge(
            "Water Supply Crisis",
            ChallengeType.Sudden,
            10,
            new List<Solution> { waterMoniPol, waterConserPol, smaIrrTech, urbForest, waterConserAwareCam, waterResSha, waterRecTech },
            Resources.Load<Sprite>("Textures/Challenge Card 3"));

        challenges = new List<Challenge> { houseShortage, agingInfra, airCrisis, waterCrisis };


        allReactions.Add(new CityReaction(
            houseShortage,
            ChallengeReactionLevel.Success,
            "New neighborhoods begin to take shape across the city. Families find hope in affordable homes. Community support rises.",
            Resources.Load<Sprite>("Textures/HousingShortageSuccess"),
            Resources.Load<AudioClip>("Audio/construction")));
        
        allReactions.Add(new CityReaction(
            houseShortage,
            ChallengeReactionLevel.Failure,
            "The housing crisis deepens. Families sleep in shelters while public pressure mounts. Approval ratings drop.",
            Resources.Load<Sprite>("Textures/HousingShortageFailure"),
            Resources.Load<AudioClip>("Audio/crowd-worried")));


        allReactions.Add(new CityReaction(
            agingInfra,
            ChallengeReactionLevel.Success,
            "Major repairs breathe new life into the city. Power outages decrease, and commuters rejoice.",
            Resources.Load<Sprite>("Textures/AgingInfraSuccess"),
            Resources.Load<AudioClip>("Audio/road-hammer-drill-machine-short")));

        allReactions.Add(new CityReaction(
            agingInfra,
            ChallengeReactionLevel.Failure,
            "Infrastructure breakdowns lead to increased accidents and power outages. Public safety is compromised.",
            Resources.Load<Sprite>("Textures/AgingInfraFailure"),
            Resources.Load<AudioClip>("Audio/distant-ambulance-siren")));


        allReactions.Add(new CityReaction(
            airCrisis,
            ChallengeReactionLevel.Success,
            "Air quality improves dramatically. Health clinics report fewer respiratory cases. Citizens breathe easier — literally.",
            Resources.Load<Sprite>("Textures/AirCrisisSuccess"),
            Resources.Load<AudioClip>("Audio/birds-chirping")));

        allReactions.Add(new CityReaction(
            airCrisis,
            ChallengeReactionLevel.Failure,
            "Air quality worsens. Hospitals report spikes in asthma and related illnesses. Public frustration escalates.",
            Resources.Load<Sprite>("Textures/AirCrisisFailure"),
            Resources.Load<AudioClip>("Audio/coughing")));
        

        allReactions.Add(new CityReaction(
            waterCrisis,
            ChallengeReactionLevel.Success,
            "Water conservation and innovation ensure a reliable supply. Drought resilience becomes a city success story.",
            Resources.Load<Sprite>("Textures/WaterCrisisSuccess"),
            Resources.Load<AudioClip>("Audio/flowing-water")));

        allReactions.Add(new CityReaction(
            waterCrisis,
            ChallengeReactionLevel.Failure,
            "The city enters emergency water rationing. Public anger grows as daily life is disrupted.",
            Resources.Load<Sprite>("Textures/WaterCrisisFailure"),
            Resources.Load<AudioClip>("Audio/bushes-medium-heavy-wind-in-dry-vegetation")));
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
        timerCoroutine = StartCoroutine(StartRoundTimer());
        openSubmissionPanelButton.interactable = true;
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

        openSubmissionPanelButton.interactable= false;
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
        ChallengeManager.Instance.ApplyPenalties();

        if (currentRound <= 4)
        {
            StartNextRound();
        }
        else
        {
            Debug.Log("Game Over! Final scores calculated.");
            ScoreManager.Instance.ApplyFinalRoleScore();
            ScoreManager.Instance.ShowPostGameSummary();
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
