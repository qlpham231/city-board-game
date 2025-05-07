using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public TextMeshProUGUI cityTransformationPointsText;
    public int CityTransformationScore { get; private set; } = 0;
    public Dictionary<Player, float> PlayerScores { get; private set; } = new Dictionary<Player, float>();

    public GameObject gameSummaryPanel;
    public TextMeshProUGUI finalCityScoreText;
    public Transform playerScoreListContainer;
    public GameObject playerScoreRowPrefab;

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

    private void Start()
    {
        gameSummaryPanel.SetActive(false);
    }

    public void CalculateScores(Player solutionProvider, Solution solution, List<KeyValuePair<Player, Resource>> resources)
    {
        Debug.Log("Calculating score...");

        // Step 1: Allocate solution contribution points
        //int solutionPoints = (int)Math.Ceiling(solution.Points * 0.4); // 40% to solution provider
        float solutionPoints = solution.Points * 0.4f;
        AddPointsToPlayer(solutionProvider, solutionPoints);

        // Step 2: Distribute resource contribution points
        //int remainingPoints = (int)Math.Ceiling(solution.Points * 0.6); // 60% split among resource providers
        //int pointsPerResource = resources.Count > 0 ? remainingPoints / resources.Count : 0;
        float remainingPoints = solution.Points * 0.6f; // Keep as float
        float pointsPerResource = resources.Count > 0 ? remainingPoints / resources.Count : 0f;

        foreach (var kvp in resources)
        {
            AddPointsToPlayer(kvp.Key, pointsPerResource);
        }

        // Step 3: Calculate collaboration bonuses
        int collabBonus = 0;
        if (solution.CollabRoles != null && solution.CollabRoles.Any())
        {
            foreach (var kvp in resources)
            {
                Player p = kvp.Key;
                if (solution.CollabRoles.Contains(p.Role))
                {
                    collabBonus += solution.CollabPoints;
                    AddPointsToPlayer(solutionProvider, solution.CollabPoints);
                    Debug.Log("collab points sol " + collabBonus + "points " + solution.CollabPoints);
                }
            }

            //if (GameManager.Instance.playerList.Any(p => solution.CollabRoles.Contains(p.Role) && resources.ContainsKey(p)))
            //{
            //    collabBonus += solution.CollabPoints;
            //}
        }
        List<Player> providingPlayers = resources.Select(kvp => kvp.Key).Distinct().ToList();
        providingPlayers.Add(solutionProvider);
        Debug.Log("Selected Resources start:");

        foreach (var kvp in resources)
        {
            Debug.Log($"Player: {kvp.Key.PlayerNr}, Resource: {kvp.Value.Name}, Type: {kvp.Value.CollabPoints}, CRoles: {kvp.Value.CollabRoles}");
        }
        foreach (var kvp in resources)
        {
            Resource resource = kvp.Value;
            if (resource.CollabRoles != null && resource.CollabRoles.Any())
            {
                foreach (Player p in providingPlayers)
                {
                    if (p.PlayerNr == kvp.Key.PlayerNr) continue;

                    if (resource.CollabRoles.Contains(p.Role))
                    {
                        collabBonus += resource.CollabPoints;
                        AddPointsToPlayer(kvp.Key, resource.CollabPoints);
                    }
                }

                //if (GameManager.Instance.playerList.Any(p => resource.CollabRoles.Contains(p.Role) && (p != kvp.Key) && (p == solutionProvider || resources.ContainsKey(p))))
                //{
                //    collabBonus += resource.CollabPoints;
                //    AddPointsToPlayer(kvp.Key, resource.CollabPoints);
                //}
            }
        }

        // Step 4: Update City Transformation Score
        CityTransformationScore += solution.Points + collabBonus;
        cityTransformationPointsText.text = "City transformation points: " + CityTransformationScore.ToString();
    }

    private void AddPointsToPlayer(Player player, float points)
    {
        if (!PlayerScores.ContainsKey(player))
        {
            PlayerScores[player] = 0;
        }
        PlayerScores[player] += points;
        PlayerScores[player] = Math.Max(0, PlayerScores[player]);
        Debug.Log("Player " + player.PlayerNr + " " + PlayerScores[player]);
        //foreach (KeyValuePair<Player, float> entry in PlayerScores)
        //{
        //    Debug.Log($"Player: {entry.Key.PlayerNr}, Score: {entry.Value}");
        //}
    }

    public int CalculateRoleScore(Player player)
    {
        int score = 0;

        // Get the goal that matches this player's role
        RoleGoal goal = GameManager.Instance.roleGoals.Find(g => g.Role == player.Role);
        List<Solution> implementedSolutions = GameManager.Instance.implementedSolutions;

        if (goal == null) return 0;

        int completed = 0;
        int completed2 = 0;

        if (player.Role == Role.CityOfficial || player.Role == Role.PrivateRep || player.Role == Role.EnvAdvocate)
        {
            foreach (Solution s in implementedSolutions)
            {
                // Is this required solution implemented?
                if (goal.PossibleSolutions.Contains(s))
                {
                    completed++;
                }
            }
        }
        else if (player.Role == Role.Citizen)
        {
            foreach (Solution s in implementedSolutions)
            {
                // Is this required solution implemented?
                if (goal.PossibleSolutions.Contains(s))
                {
                    // Check if player contributed to it (owner or helper)
                    if (s.Contributors.Contains(player))
                    {
                        completed++;
                    }
                }
            }
        }
        else if (player.Role == Role.ExternalCollaborator)
        {
            foreach (Solution s in implementedSolutions)
            {
                // Is this required solution implemented?
                if (goal.PossibleSolutions.Contains(s))
                {
                    completed++;
                }

                if (s.Contributors.Contains(player) && s.Owner != player)
                {
                    completed2++;
                }
            }
        }

        if (player.Role == Role.ExternalCollaborator)
        {
            if (completed >= goal.RequiredCount && completed2 >= 1)
            {
                score += goal.FullPoints;
            } 
            else if (completed >= goal.RequiredCount && completed2 < 1)
            {
                float ratio = (float)goal.RequiredCount / (goal.RequiredCount + 1);
                score += Mathf.RoundToInt(goal.FullPoints * ratio);
            }
            else if (completed < goal.RequiredCount && completed2 >= 1)
            {
                float ratio = (float)(completed + 1) / (goal.RequiredCount + 1);
                score += Mathf.RoundToInt(goal.FullPoints * ratio);
            } else
            {
                float ratio = (float)completed / (goal.RequiredCount + 1);
                score += Mathf.RoundToInt(goal.FullPoints * ratio);
            }
        }
        else
        {
            if (completed >= goal.RequiredCount)
            {
                score += goal.FullPoints;
            }
            else
            {
                float ratio = (float)completed / goal.RequiredCount;
                score += Mathf.RoundToInt(goal.FullPoints * ratio);
            }
        }

        return score;
    }

    public void ApplyPenalty(int penaltyPoints)
    {
        Debug.Log($"Applying penalty: -{penaltyPoints} points");

        CityTransformationScore -= penaltyPoints;
        if (CityTransformationScore < 0) CityTransformationScore = 0;

        float penaltyPerPlayer = GameManager.Instance.playerList.Count > 0 ? (float)penaltyPoints / GameManager.Instance.playerList.Count : 0;
        GameManager.Instance.playerList.ForEach(player => AddPointsToPlayer(player, -penaltyPerPlayer));

        cityTransformationPointsText.text = "City transformation score: " + CityTransformationScore;
    }

    public void ApplyFinalRoleScore()
    {
        foreach(Player p in GameManager.Instance.playerList)
        {
            int score = CalculateRoleScore(p);
            AddPointsToPlayer(p, score);
        }
    }

    public void ShowPostGameSummary()
    {
        gameSummaryPanel.SetActive(true);
        SpiderDiagram.Instance.spiderDiagramCanvas.sortingOrder = 6;

        // Show final city score
        finalCityScoreText.text = "City transformation score: " + CityTransformationScore;

        // Sort players by score descending
        var rankedPlayers = PlayerScores.OrderByDescending(pair => pair.Value).ToList();

        int rank = 1;
        foreach (var pair in rankedPlayers)
        {
            GameObject row = Instantiate(playerScoreRowPrefab, playerScoreListContainer);
            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = $"#{rank}";
            texts[1].text = $"Player {pair.Key.PlayerNr}: " + pair.Key.Role.GetDescription(); 
            //texts[2].text = pair.Value.ToString() + " TPs";
            texts[2].text = Mathf.CeilToInt(pair.Value).ToString() + " TPs";

            rank++;
        }
    }


}
