using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum ChallengeType { LongTerm, Sudden }
public enum Role {

    [Description("City Government Official")]
    CityOfficial,

    [Description("Private Sector Representative")]
    PrivateRep,

    [Description("Environmental Advocate")]
    EnvAdvocate,

    [Description("Citizen")]
    Citizen,

    [Description("External Collaborator")]
    ExternalCollaborator,

}

public enum ResourceType { Funding, Policy, Technology, Community, Land }

[System.Serializable]
public class Challenge
{
    public string Name { get; }
    public ChallengeType Type { get; }
    public int PenaltyPoints { get; }
    public List<Solution> AcceptedSolutions { get; }
    public Sprite ChallengeImage { get; }

    public Challenge(string name, ChallengeType type, int penaltyPoints, List<Solution> solutions, Sprite image)
    {
        Name = name;
        Type = type;
        PenaltyPoints = penaltyPoints;
        AcceptedSolutions = solutions;
        ChallengeImage = image;
    }
}

[System.Serializable]
public class Solution
{
    public string Name { get; }
    public int Points { get; }
    public int CollabPoints { get; }
    public List<Role> CollabRoles { get; }
    public Dictionary<ResourceType, int> RequiredResources { get; }
    public int[] ParameterChanges { get; }

    // Fixed parameter fields
    public int Transport { get; }
    public int Ecological { get; }
    public int WaterResources { get; }
    public int Energy { get; }
    public int AirQuality { get; }
    public int Economy { get; }

    /// <summary>
    /// Creates a solution.
    /// </summary>
    /// <param name="parameterChanges">Parameter changes to the spider diagram: Transport, Ecological, Water Resources, Energy, Air Quality, Economy.</param>
    public Solution(string name, int points, int collabPoints, List<Role> collabRoles, Dictionary<ResourceType, int> cost, int[] parameterChanges)
    {
        Name = name;
        Points = points;
        CollabPoints = collabPoints;
        CollabRoles = collabRoles;
        RequiredResources = cost;
        ParameterChanges = parameterChanges;
    }
}

[System.Serializable]
public class Resource
{
    public string Name { get; }
    public ResourceType ResourceType { get; } 
    public int Amount { get; }
    public List<Solution> ApplicableSolutions { get; }
    public int CollabPoints { get; }
    public List<Role> CollabRoles { get; }

    public Resource(string name, ResourceType type, int amount, List<Solution> solutions, int points, List<Role> collabRoles)
    {
        Name = name;
        ResourceType = type;
        Amount = amount;
        ApplicableSolutions = solutions;
        CollabPoints = points;
        CollabRoles = collabRoles;
    }
}

[System.Serializable]
public class Player
{
    public int PlayerNr { get; }
    public Role Role { get; }
    public int Points { get; }

    public Player(int number, Role role, int points)
    {
        PlayerNr = number;
        Role = role;
        Points = points; 
    }
}

