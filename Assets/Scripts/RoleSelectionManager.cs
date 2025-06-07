using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoleSelectionManager : MonoBehaviour
{
    public GameObject roleSelectionCanvas; // Canvas containing panel for role selection
    public GameObject roleSelectionPanel; // Panel containing role selection buttons
    public Transform playerListContainer; // UI container for displaying selected roles
    public GameObject playerRolePrefab; // Prefab for displaying selected roles
    public Button startGameButton; // Button to start the game
    public ChallengeManager challengeManager; // Reference to the ChallengeManager script

    private int maxPlayers = 7;
    private int minPlayers = 2;
    private int currentPlayer = 1;
    private List<Player> playerList = new List<Player>();

    // Start is called before the first frame update
    void Start()
    {
        //roleSelectionPanel.SetActive(false);
        startGameButton.interactable = false;
        roleSelectionCanvas.SetActive(true);
    }

    public void SelectRole(int roleValue)
    {
        Role role = (Role)roleValue;

        if (currentPlayer <= maxPlayers)
        {
            Player player = new Player(currentPlayer, role, 0);
            playerList.Add(player);

            // Display selected role in UI
            GameObject newPlayerRole = Instantiate(playerRolePrefab, playerListContainer);
            newPlayerRole.GetComponent<TextMeshProUGUI>().text = "Player " + currentPlayer + ": " + role.GetDescription();

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
        foreach (Player p in playerList)
        {
            Debug.Log(p.Role);
        }
        // Load the next game scene or initialize the next phase here
        if (challengeManager != null)
        {
            GameManager.Instance.SetPlayers(playerList);
            GameManager.Instance.StartGame();
            //StartCoroutine(challengeManager.StartNewRound()); // This starts the first round
            roleSelectionCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("ChallengeManager reference is missing.");
        }

    }
}
