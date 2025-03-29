/*using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton for easy access

    public List<Challenge> Challenges = new List<Challenge>();
    public List<Solution> Solutions = new List<Solution>();
    public List<Resource> Resources = new List<Resource>();

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
        // Create Resources
        Resource water = new Resource("Land Acquisition", 5);
        Resource food = new Resource("Food", 10);
        Resource energy = new Resource("Energy", 15);

        Resources.Add(water);
        Resources.Add(food);
        Resources.Add(energy);

        // Create Solutions
        Solution buildShelter = new Solution("Build Shelter", 20);
        buildShelter.RequiredResources.Add(wood);
        buildShelter.RequiredResources.Add(energy);

        Solution foodDistribution = new Solution("Food Distribution", 15);
        foodDistribution.RequiredResources.Add(food);

        Solutions.Add(buildShelter);
        Solutions.Add(foodDistribution);

        // Create Challenges
        Challenge earthquake = new Challenge("Earthquake", 10, ChallengeType.Sudden);
        Challenge hungerCrisis = new Challenge("Hunger Crisis", 10, ChallengeType.LongTerm);

        // Assign accepted solutions to challenges
        earthquake.AcceptedSolutions.Add(buildShelter);
        hungerCrisis.AcceptedSolutions.Add(foodDistribution);

        Challenges.Add(earthquake);
        Challenges.Add(hungerCrisis);
    }

    public Challenge GetChallengeByName(string name)
    {
        return Challenges.Find(ch => ch.Name == name);
    }

    public Solution GetSolutionByName(string name)
    {
        return Solutions.Find(sol => sol.Name == name);
    }

    public Resource GetResourceByName(string name)
    {
        return Resources.Find(res => res.Name == name);
    }
}
*/