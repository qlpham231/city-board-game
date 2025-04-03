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
    public Dictionary<Player, int> PlayerScores { get; private set; } = new Dictionary<Player, int>();

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

    public void CalculateScores(Player solutionProvider, Solution solution, List<KeyValuePair<Player, Resource>> resources)
    {
        Debug.Log("Calculating score...");

        // Step 1: Allocate solution contribution points
        int solutionPoints = (int)Math.Ceiling(solution.Points * 0.4); // 40% to solution provider
        AddPointsToPlayer(solutionProvider, solutionPoints);

        // Step 2: Distribute resource contribution points
        int remainingPoints = (int)Math.Ceiling(solution.Points * 0.6); // 60% split among resource providers
        int pointsPerResource = resources.Count > 0 ? remainingPoints / resources.Count : 0;

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

    private void AddPointsToPlayer(Player player, int points)
    {
        if (!PlayerScores.ContainsKey(player))
        {
            PlayerScores[player] = 0;
        }
        PlayerScores[player] += points;
    }
}
