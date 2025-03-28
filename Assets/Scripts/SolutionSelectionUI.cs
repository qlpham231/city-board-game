using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SolutionSelectionUI : MonoBehaviour
{
    public GameObject solutionSelectionCanvas;

    [Header("Dropdowns")]
    public TMP_Dropdown challengeDropdown1;
    public TMP_Dropdown solutionDropdown1;
    public TMP_Dropdown challengeDropdown2;
    public TMP_Dropdown solutionDropdown2;

    [Header("Resource Multi-Select Dropdowns")]
    public SearchableMultiSelectDropdown resourceDropdown1; // First resource multi-select
    public SearchableMultiSelectDropdown resourceDropdown2; // Second resource multi-select

    [Header("UI Elements")]
    public Button confirmButton;
    public TextMeshProUGUI feedbackText;

    private List<string> selectedResources1 = new List<string>();
    private List<string> selectedResources2 = new List<string>();

    private Dictionary<string, List<string>> solutionResources = new Dictionary<string, List<string>>();

    // To populate options for the dropdowns
    void Start()
    {
        confirmButton.onClick.AddListener(SubmitSolution);
    }

    public void PopulateDropdowns(List<string> challenges, List<string> solutions, List<string> resources)
    {
        challengeDropdown1.ClearOptions();
        solutionDropdown1.ClearOptions();
        challengeDropdown2.ClearOptions();
        solutionDropdown2.ClearOptions();

        // Adding default options
        challenges.Insert(0, "Select Challenge");
        solutions.Insert(0, "Select Solution");
        resources.Insert(0, "Select Resource");

        challengeDropdown1.AddOptions(challenges);
        solutionDropdown1.AddOptions(solutions);
        challengeDropdown2.AddOptions(challenges);
        solutionDropdown2.AddOptions(solutions);

        // Populate resources dropdowns (assuming these are searchable multi-selects)
        resourceDropdown1.PopulateDropdown(resources);
        resourceDropdown2.PopulateDropdown(resources);
    }

    private void SubmitSolution()
    {
        // 1st solution & resources
        string selectedChallenge1 = challengeDropdown1.options[challengeDropdown1.value].text;
        string selectedSolution1 = solutionDropdown1.options[solutionDropdown1.value].text;

        // 2nd solution & resources
        string selectedChallenge2 = challengeDropdown2.options[challengeDropdown2.value].text;
        string selectedSolution2 = solutionDropdown2.options[solutionDropdown2.value].text;

        bool solution1Valid = false;
        bool solution2Valid = false;

        // Validate only if a solution has been selected (not the default "Select Solution" option)
        if (selectedSolution1 != "Select Solution" && selectedChallenge1 != "Select Challenge")
        {
            solution1Valid = ValidateSolution(selectedSolution1, selectedResources1);
        }

        if (selectedSolution2 != "Select Solution" && selectedChallenge2 != "Select Challenge")
        {
            solution2Valid = ValidateSolution(selectedSolution2, selectedResources2);
        }

        // Handle feedback based on whether solutions are valid or empty
        if (selectedSolution1 == "Select Solution" && selectedSolution2 == "Select Solution")
        {
            feedbackText.text = "No solution selected!";
            feedbackText.color = Color.white;
        }
        else if (solution1Valid && solution2Valid)
        {
            feedbackText.text = "Both solutions successfully applied!";
            feedbackText.color = Color.green;
        }
        else if (solution1Valid)
        {
            feedbackText.text = "Solution 1 applied successfully!";
            feedbackText.color = Color.green;
        }
        else if (solution2Valid)
        {
            feedbackText.text = "Solution 2 applied successfully!";
            feedbackText.color = Color.green;
        }
        else
        {
            feedbackText.text = "Missing resources or invalid solution!";
            feedbackText.color = Color.red;
        }

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
}
