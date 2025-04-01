using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SolutionSelectionUI : MonoBehaviour
{
    public GameObject solutionSelectionCanvas;

    [Header("Dropdowns")]
    //public TMP_Dropdown challengeDropdown1;
    //public TMP_Dropdown solutionDropdown1;
    //public TMP_Dropdown challengeDropdown2;
    //public TMP_Dropdown solutionDropdown2;

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

    //[Header("Resource Multi-Select Dropdowns")]
    //public SearchableMultiSelectDropdown resourceDropdown1; // First resource multi-select
    //public SearchableMultiSelectDropdown resourceDropdown2; // Second resource multi-select

    [Header("UI Elements")]
    public Button submitButton;
    public TextMeshProUGUI feedbackText;

    private List<string> selectedResources1 = new List<string>();
    private List<string> selectedResources2 = new List<string>();

    private Dictionary<string, List<string>> solutionResources = new Dictionary<string, List<string>>();

    // To populate options for the dropdowns
    void Start()
    {
        submitButton.onClick.AddListener(SubmitSolution);
        List<string> challengeNames = GameManager.Instance.challenges.Select(c => c.Name).ToList();
        List<string> solutionNames = GameManager.Instance.solutions.Select(s => s.Name).ToList();
        List<string> resourceNames = GameManager.Instance.resources.Select(r => r.Name).ToList();
        List<string> playerNames = GameManager.Instance.playerList
            .Select(p => $"Player {p.PlayerNr}")
            .ToList();
        PopulateDropdowns(challengeNames, solutionNames, resourceNames, playerNames);
    }

    public void PopulateDropdowns(List<string> challenges, List<string> solutions, List<string> resources, List<string> players)
    {
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
        //// 1st solution & resources
        //string selectedChallenge1 = challengeDropdown1.options[challengeDropdown1.value].text;
        //string selectedSolution1 = solutionDropdown1.options[solutionDropdown1.value].text;

        //// 2nd solution & resources
        //string selectedChallenge2 = challengeDropdown2.options[challengeDropdown2.value].text;
        //string selectedSolution2 = solutionDropdown2.options[solutionDropdown2.value].text;

        //bool solution1Valid = false;
        //bool solution2Valid = false;

        //bool success1 = Validate(challengeDropdown1, solutionDropdown1, contributorDropdown1, resourceDropdown1_1, contributorDropdown1_1, resourceDropdown1_2, contributorDropdown1_2);
        //bool success2 = Validate(challengeDropdown2, solutionDropdown2, contributorDropdown2, resourceDropdown2_1, contributorDropdown2_1, resourceDropdown2_2, contributorDropdown2_2);


        //// Validate only if a solution has been selected (not the default "Select Solution" option)
        //if (selectedSolution1 != "Select Solution" && selectedChallenge1 != "Select Challenge")
        //{
        //    solution1Valid = ValidateSolution(selectedSolution1, selectedResources1);
        //}

        //if (selectedSolution2 != "Select Solution" && selectedChallenge2 != "Select Challenge")
        //{
        //    solution2Valid = ValidateSolution(selectedSolution2, selectedResources2);
        //}

        //// Handle feedback based on whether solutions are valid or empty
        //if (selectedSolution1 == "Select Solution" && selectedSolution2 == "Select Solution")
        //{
        //    feedbackText.text = "No solution selected!";
        //    feedbackText.color = Color.white;
        //}
        //else if (solution1Valid && solution2Valid)
        //{
        //    feedbackText.text = "Both solutions successfully applied!";
        //    feedbackText.color = Color.green;
        //}
        //else if (solution1Valid)
        //{
        //    feedbackText.text = "Solution 1 applied successfully!";
        //    feedbackText.color = Color.green;
        //}
        //else if (solution2Valid)
        //{
        //    feedbackText.text = "Solution 2 applied successfully!";
        //    feedbackText.color = Color.green;
        //}
        //else
        //{
        //    feedbackText.text = "Missing resources or invalid solution!";
        //    feedbackText.color = Color.red;
        //}

        solutionSelectionCanvas.SetActive(false);
    }


    private bool ValidateSolution(string solution, List<string> selectedResources)
    {
        if (solutionResources.ContainsKey(solution))
        {
            List<string> requiredResources = solutionResources[solution];
            return selectedResources.Count >= requiredResources.Count && requiredResources.TrueForAll(r => selectedResources.Contains(r));
        }
        return false;
    }

    private bool ValidateAndRegister(TMP_Dropdown challengeDropdown, TMP_Dropdown solutionDropdown, TMP_Dropdown playerDropdown,
                                 TMP_Dropdown resourceDropdown1, TMP_Dropdown contributorDropdown1,
                                 TMP_Dropdown resourceDropdown2, TMP_Dropdown contributorDropdown2,
                                 TMP_Dropdown resourceDropdown3, TMP_Dropdown contributorDropdown3,
                                 TMP_Dropdown resourceDropdown4, TMP_Dropdown contributorDropdown4)
    {
        Challenge selectedChallenge = GameManager.Instance.GetChallengeByName(challengeDropdown.options[challengeDropdown.value].text);
        Solution selectedSolution = GameManager.Instance.GetSolutionByName(solutionDropdown.options[solutionDropdown.value].text);
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
        List<Resource> selectedResources = new List<Resource>();
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
        return true;
    }

    private void AssignResource(List<Resource> selectedResources, TMP_Dropdown resourceDropdown, TMP_Dropdown contributorDropdown)
    {
        if (resourceDropdown.value > 0 && contributorDropdown.value > 0) // Ensure a valid selection
        {
            Resource selectedResource = GameManager.Instance.GetResourceByName(resourceDropdown.options[resourceDropdown.value].text);
            Player contributingPlayer = GameManager.Instance.GetPlayerByName(contributorDropdown.options[contributorDropdown.value].text);

            if (selectedResource != null && contributingPlayer != null)
            {
                selectedResources.Add(selectedResource);
            }
        }
    }

    private bool ValidateResources(Solution solution, List<Resource> selectedResources)
    {
        Dictionary<ResourceType, int> requiredResources = new Dictionary<ResourceType, int>(solution.RequiredResources);

        foreach (Resource resource in selectedResources)
        {
            // ✅ Check if resource has additional conditions (allowed solutions)
            if (resource.ApplicableSolutions.Count > 0 && !resource.ApplicableSolutions.Contains(solution))
            {
                Debug.Log($"Resource {resource.Name} cannot be used for {solution.Name}.");
                return false;
            }

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
