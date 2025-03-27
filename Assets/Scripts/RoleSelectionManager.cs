using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoleSelectionManager : MonoBehaviour
{
    //public InputField playerCountInput; // Input field to set number of players
    public GameObject roleSelectionCanvas; // Canvas containing panel for role selection
    public GameObject roleSelectionPanel; // Panel containing role selection buttons
    public Transform playerListContainer; // UI container for displaying selected roles
    public GameObject playerRolePrefab; // Prefab for displaying selected roles
    public Button startGameButton; // Button to start the game
    public ChallengeManager challengeManager; // Reference to the ChallengeManager script

    private int maxPlayers = 7;
    private int minPlayers = 2;
    private int currentPlayer = 1;
    private List<string> selectedRoles = new List<string>();
    private string[] roles = { "City Government Official", "Private Sector Representative", "Environmental Advocate", "Citizen", "External Collaborator" };

    // Start is called before the first frame update
    void Start()
    {
        //roleSelectionPanel.SetActive(false);
        startGameButton.interactable = false;
    }

    public void SelectRole(int roleIndex)
    {
        if (currentPlayer <= maxPlayers)
        {
            string selectedRole = roles[roleIndex];
            selectedRoles.Add(selectedRole);

            // Display selected role in UI
            GameObject newPlayerRole = Instantiate(playerRolePrefab, playerListContainer);
            newPlayerRole.GetComponent<TextMeshProUGUI>().text = "Player " + currentPlayer + ": " + selectedRole;

            currentPlayer++;

            // Enable Start Game button when all players have chosen
            if (currentPlayer >= minPlayers)
            {
                startGameButton.interactable = true;
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("Starting game with roles:");
        foreach (string role in selectedRoles)
        {
            Debug.Log(role);
        }
        // Load the next game scene or initialize the next phase here
        if (challengeManager != null)
        {
            StartCoroutine(challengeManager.StartNewRound()); // This starts the first round
            roleSelectionCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("ChallengeManager reference is missing.");
        }

    }
}
