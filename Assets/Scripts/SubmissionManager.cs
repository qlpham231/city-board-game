using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubmissionManager : MonoBehaviour
{
    public static SubmissionManager Instance;
    public GameObject solutionSelectionCanvas;

    [Header("Dropdowns")]
    public TMP_Dropdown challengeDropdown1, solutionDropdown1, contributorDropdown1;
    public TMP_Dropdown resourceDropdown1_1, contributorDropdown1_1;
    public TMP_Dropdown resourceDropdown1_2, contributorDropdown1_2;
    public TMP_Dropdown resourceDropdown1_3, contributorDropdown1_3;
    public TMP_Dropdown resourceDropdown1_4, contributorDropdown1_4;

    public TMP_Dropdown challengeDropdown2, solutionDropdown2, contributorDropdown2;
    public TMP_Dropdown resourceDropdown2_1, contributorDropdown2_1;
    public TMP_Dropdown resourceDropdown2_2, contributorDropdown2_2;
    public TMP_Dropdown resourceDropdown2_3, contributorDropdown2_3;
    public TMP_Dropdown resourceDropdown2_4, contributorDropdown2_4;

    [Header("UI Elements")]
    public Button submitButton;
    public TextMeshProUGUI feedbackText;

    private List<Solution> acceptedSolutions = new List<Solution>();

    private Color greenColor = new Color(0f, 0.55f, 0f, 1f);
    private Color redColor = new Color(0.85f, 0f, 0f, 1f);

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

    // To populate options for the dropdowns
    void Start()
    {
        solutionSelectionCanvas.SetActive(false);
        submitButton.onClick.AddListener(SubmitSolution);
        GameManager.Instance.onPlayerListUpdated.AddListener(UpdatePlayers);
        ChallengeManager.Instance.onCurrentChallengesUpdated.AddListener(UpdateChallenges);

        List<string> challengeNames = GameManager.Instance.challenges.Select(c => c.Name).ToList();
        List<string> solutionNames = GameManager.Instance.solutions.Select(s => s.Name).ToList();
        List<string> resourceNames = GameManager.Instance.resources.Select(r => r.Name).ToList();
        List<string> playerNames = GameManager.Instance.playerList
            .Select(p => $"Player {p.PlayerNr}")
            .ToList();
        Debug.Log("List Contents: " + string.Join(", ", playerNames));
        PopulateDropdowns(challengeNames, solutionNames, resourceNames, playerNames);
    }

    private void UpdatePlayers(List<Player> players)
    {
        List<string> playerNames = players
            .Select(p => $"Player {p.PlayerNr}")
            .ToList();
        playerNames.Insert(0, "Select Player");

        foreach (TMP_Dropdown dropdown in GetComponentsInChildren<TMP_Dropdown>())
        {
            if (dropdown.name == "PlayerDropdown") 
            {
                dropdown.ClearOptions(); 
                dropdown.AddOptions(playerNames); 
            }
        }
    }

    private void UpdateChallenges(List<Challenge> challenges)
    {
        List<string> challengeNames = challenges.Select(c => c.Name).ToList();
        challengeNames = challengeNames.OrderBy(c => c, StringComparer.OrdinalIgnoreCase).ToList();
        challengeNames.Insert(0, "Select Challenge");

        foreach (TMP_Dropdown dropdown in GetComponentsInChildren<TMP_Dropdown>())
        {
            if (dropdown.name == "ChallengeDropdown")
            {
                dropdown.ClearOptions(); 
                dropdown.AddOptions(challengeNames);
            }
        }
    }

    public void PopulateDropdowns(List<string> challenges, List<string> solutions, List<string> resources, List<string> players)
    {
        // Sort alphabetically (case-insensitive) and assign back
        challenges = challenges.OrderBy(c => c, StringComparer.OrdinalIgnoreCase).ToList();
        solutions = solutions.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList();
        resources = resources.OrderBy(r => r, StringComparer.OrdinalIgnoreCase).ToList();
        players = players.OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList();

        // Adding default options
        challenges.Insert(0, "Select Challenge");
        solutions.Insert(0, "Select Solution");
        resources.Insert(0, "Select Resource");
        players.Insert(0, "Select Player");

        foreach (TMP_Dropdown dropdown in GetComponentsInChildren<TMP_Dropdown>())
        {
            Debug.Log("Found Dropdown: " + dropdown.name);
            dropdown.ClearOptions(); // Example: Clear options from all dropdowns

            if (dropdown.name == "ChallengeDropdown") { dropdown.AddOptions(challenges); }
            else if (dropdown.name == "SolutionDropdown") { dropdown.AddOptions(solutions); }
            else if (dropdown.name == "ResourceDropdown") { dropdown.AddOptions(resources); }
            else if (dropdown.name == "PlayerDropdown") { dropdown.AddOptions(players); }
        }
    }

    private void SubmitSolution()
    {
        acceptedSolutions.Clear();
        // 1st solution & resources
        string selectedChallenge1 = challengeDropdown1.options[challengeDropdown1.value].text;
        string selectedSolution1 = solutionDropdown1.options[solutionDropdown1.value].text;

        // 2nd solution & resources
        string selectedChallenge2 = challengeDropdown2.options[challengeDropdown2.value].text;
        string selectedSolution2 = solutionDropdown2.options[solutionDropdown2.value].text;

        bool success1 = Validate(challengeDropdown1, solutionDropdown1, contributorDropdown1,
            resourceDropdown1_1, contributorDropdown1_1, resourceDropdown1_2, contributorDropdown1_2,
            resourceDropdown1_3, contributorDropdown1_3, resourceDropdown1_4, contributorDropdown1_4,
            out Challenge challenge1, out Solution solution1);

        bool success2 = Validate(challengeDropdown2, solutionDropdown2, contributorDropdown2,
            resourceDropdown2_1, contributorDropdown2_1, resourceDropdown2_2, contributorDropdown2_2,
            resourceDropdown2_3, contributorDropdown2_3, resourceDropdown2_4, contributorDropdown2_4,
            out Challenge challenge2, out Solution solution2);


        // Handle feedback based on whether solutions are valid or empty
        if (selectedSolution1 == "Select Solution" && selectedSolution2 == "Select Solution")
        {
            feedbackText.text = "No solution selected!";
            feedbackText.color = Color.white;
        }
        else if (success1 && success2)
        {
            
            feedbackText.text = "Both solutions successfully applied!";
            feedbackText.color = greenColor;
            ChallengeManager.Instance.ResolveChallenge(challenge1);
            ChallengeManager.Instance.ResolveChallenge(challenge2);

        }
        else if (success1)
        {
            feedbackText.text = "Solution 1 applied successfully!";
            feedbackText.color = greenColor;
            ChallengeManager.Instance.ResolveChallenge(challenge1);
        }
        else if (success2)
        {
            feedbackText.text = "Solution 2 applied successfully!";
            feedbackText.color = greenColor;
            ChallengeManager.Instance.ResolveChallenge(challenge2);
        }
        else
        {
            feedbackText.text = "Missing resources or invalid solution!";
            feedbackText.color = redColor;
        }

        SpiderDiagram.Instance.UpdateSpiderDiagram(acceptedSolutions);
        GameManager.Instance.RegisterSolution();
        solutionSelectionCanvas.SetActive(false);
        ResetDropdowns();
    }

    private bool Validate(TMP_Dropdown challengeDropdown, TMP_Dropdown solutionDropdown, TMP_Dropdown playerDropdown,
                                 TMP_Dropdown resourceDropdown1, TMP_Dropdown contributorDropdown1,
                                 TMP_Dropdown resourceDropdown2, TMP_Dropdown contributorDropdown2,
                                 TMP_Dropdown resourceDropdown3, TMP_Dropdown contributorDropdown3,
                                 TMP_Dropdown resourceDropdown4, TMP_Dropdown contributorDropdown4,
                                 out Challenge selectedChallenge, out Solution selectedSolution)
    {
        selectedChallenge = null;
        selectedSolution = null;

        if (challengeDropdown.value == 0 || solutionDropdown.value == 0 || playerDropdown.value == 0) { return false; }

        selectedChallenge = GameManager.Instance.GetChallengeByName(challengeDropdown.options[challengeDropdown.value].text);
        selectedSolution = GameManager.Instance.GetSolutionByName(solutionDropdown.options[solutionDropdown.value].text);
        Player selectedPlayer = GameManager.Instance.GetPlayerByName(playerDropdown.options[playerDropdown.value].text);

        if (selectedChallenge == null || selectedSolution == null || selectedPlayer == null)
        {
            Debug.Log("Invalid selection!");
            return false; // Stop if something is missing
        }

        // ✅ Step 1: Check if solution is accepted for the challenge
        if (!selectedChallenge.AcceptedSolutions.Contains(selectedSolution))
        {
            Debug.Log("Solution is not valid for this challenge.");
            return false;
        }

        // ✅ Step 2: Collect selected resources (allowing up to 4, but some can be empty)
        //List<Resource> selectedResources = new List<Resource>();
        //Dictionary<Player, Resource> selectedResources = new Dictionary<Player, Resource>();
        List<KeyValuePair<Player, Resource>> selectedResources = new List<KeyValuePair<Player, Resource>>();
        AssignResource(selectedResources, resourceDropdown1, contributorDropdown1);
        AssignResource(selectedResources, resourceDropdown2, contributorDropdown2);
        AssignResource(selectedResources, resourceDropdown3, contributorDropdown3);
        AssignResource(selectedResources, resourceDropdown4, contributorDropdown4);

        // ✅ Step 3: Validate if selected resources fulfill the required resources
        if (!ValidateResources(selectedSolution, selectedResources))
        {
            Debug.Log("Resources do not meet the requirements.");
            return false;
        }

        // ✅ Step 4: Register the solution and update game state
        // selectedPlayer.SubmitSolution(selectedSolution);
        selectedSolution.Owner = selectedPlayer;
        acceptedSolutions.Add(selectedSolution);
        GameManager.Instance.implementedSolutions.Add(selectedSolution);
        ScoreManager.Instance.CalculateScores(selectedPlayer, selectedSolution, selectedResources);
        //ChallengeManager.Instance.ResolveChallenge(selectedChallenge);
        CityReactionManager.Instance.PlayReaction(selectedChallenge, ChallengeReactionLevel.Success);
        return true;
    }

    private void AssignResource(List<KeyValuePair<Player, Resource>> selectedResources, TMP_Dropdown resourceDropdown, TMP_Dropdown contributorDropdown)
    {
        if (resourceDropdown.value > 0 && contributorDropdown.value > 0) // Ensure a valid selection
        {
            Resource selectedResource = GameManager.Instance.GetResourceByName(resourceDropdown.options[resourceDropdown.value].text);
            Player contributingPlayer = GameManager.Instance.GetPlayerByName(contributorDropdown.options[contributorDropdown.value].text);
            
            if (selectedResource != null && contributingPlayer != null)
            {
                selectedResources.Add(new KeyValuePair<Player, Resource>(contributingPlayer, selectedResource));
            }
        }
    }

    private bool ValidateResources(Solution solution, List<KeyValuePair<Player, Resource>> selectedResources)
    {
        Dictionary<ResourceType, int> requiredResources = new Dictionary<ResourceType, int>(solution.RequiredResources);

        List<Player> contributingPlayers = selectedResources
            .Select(kvp => kvp.Key)
            .Distinct()
            .ToList();

        foreach (var kvp in selectedResources)
        {
            Resource resource = kvp.Value;
            //if (resource.ApplicableSolutions != null)
            //{
                // ✅ Check if resource has additional conditions (allowed solutions)
                if (resource.ApplicableSolutions != null && !resource.ApplicableSolutions.Contains(solution))
                {
                    Debug.Log($"Resource {resource.Name} cannot be used for {solution.Name}.");
                    return false;
                }
            //}

            // ✅ If the resource type is required, reduce the needed amount
            if (requiredResources.ContainsKey(resource.ResourceType))
            {
                requiredResources[resource.ResourceType] -= resource.Amount;
            }
        }

        // ✅ Check if all required resources have been fully covered
        foreach (var required in requiredResources)
        {
            if (required.Value > 0)
            {
                Debug.Log($"Missing {required.Value} of resource type {required.Key}.");
                return false;
            } 
        }

        solution.Contributors = contributingPlayers;
        return true;
    }


    void ResetDropdowns()
    {
        foreach (TMP_Dropdown dropdown in GetComponentsInChildren<TMP_Dropdown>())
        {
            dropdown.value = 0;
            dropdown.RefreshShownValue();
        }
        Debug.Log("Refreshed dropwdown");
    }
}
