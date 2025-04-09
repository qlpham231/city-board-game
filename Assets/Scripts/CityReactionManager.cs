using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityReactionManager : MonoBehaviour
{
    public static CityReactionManager Instance;

    [Header("UI Refs")]
    //public GameObject reactionPopup;
    public TextMeshProUGUI narrativeTextUI;
    public GameObject reactionImage;
    public AudioSource audioSource;
    public GameObject narrativePanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        narrativePanel.SetActive(false);
    }

    public void PlayReaction(Challenge challenge, ChallengeReactionLevel level)
    {
        CityReaction reaction = GameManager.Instance.allReactions.Find(r =>
            r.Challenge.Name == challenge.Name && r.ReactionLevel == level);

        if (reaction == null)
        {
            Debug.LogWarning("No matching city reaction found!");
            return;
        }

        ShowReaction(reaction);
    }

    private void ShowReaction(CityReaction reaction)
    {
        narrativePanel.SetActive(true);
        narrativeTextUI.text = reaction.NarrativeText;
        reactionImage.GetComponent<Image>().sprite = reaction.VisualCue;
        //reactionImage.sprite = reaction.visualCue;

        if (reaction.ReactionSound)
        {
            audioSource.PlayOneShot(reaction.ReactionSound);
        }

        reactionImage.SetActive(true);
        // Optional: animate in/out or use Timeline/Animator
    }
}
