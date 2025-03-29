using System.Collections.Generic;
using UnityEngine;

public enum ChallengeType { LongTerm, Sudden }

[System.Serializable]
public class Challenge
{
    public string Name;
    public ChallengeType Type;
    public int PenaltyPoints;
    public List<Solution> AcceptedSolutions;
    public Sprite ChallengeImage;

    public Challenge(string name, ChallengeType type, int penaltyPoints, Sprite image)
    {
        Name = name;
        Type = type;
        PenaltyPoints = penaltyPoints;
        AcceptedSolutions = new List<Solution>();
        ChallengeImage = image;
    }
}

[System.Serializable]
public class Solution
{
    public string Name;
    public int Points;
    public List<Resource> RequiredResources;

    public Solution(string name, int points)
    {
        Name = name;
        Points = points;
        RequiredResources = new List<Resource>();
        // Bonus collaboration points
    }
}

[System.Serializable]
public class Resource
{
    public string Name;
    public int Points;

    public Resource(string name, int points)
    {
        Name = name;
        Points = points; //bonus collaboration
    }
}
