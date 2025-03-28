using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public SolutionSelectionUI solutionSelectionUI;
    // Start is called before the first frame update
    void Start()
    {
        // Example call
        List<string> challenges = new List<string> { "Challenge 1", "Challenge 2", "Challenge 3" };
        List<string> solutions = new List<string> { "Solution 1", "Solution 2", "Solution 3" };
        List<string> resources = new List<string> { "Resource 1", "Resource 2", "Resource 3" };

        solutionSelectionUI.PopulateDropdowns(challenges, solutions, resources);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
