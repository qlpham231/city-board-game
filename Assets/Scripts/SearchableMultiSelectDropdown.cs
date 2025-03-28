using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchableMultiSelectDropdown : MonoBehaviour
{
    public Button dropdownButton; // The button that opens/closes the dropdown
    public GameObject dropdownPanel; // The container for dropdown items
    public TMP_InputField searchField; // Search bar
    public Transform optionContainer; // Parent object for toggle items
    public GameObject togglePrefab; // Prefab for each option
    public TextMeshProUGUI selectedTextDisplay; // Shows selected items

    private List<string> allOptions = new List<string> { "Apple", "Banana", "Orange", "Grapes", "Mango", "Pineapple", "Watermelon" }; // Example options
    private List<string> selectedOptions = new List<string>(); // Stores selected items
    private List<Toggle> toggleItems = new List<Toggle>(); // List of created toggles

    void Start()
    {
        dropdownPanel.SetActive(false); // Hide dropdown initially
        dropdownButton.onClick.AddListener(ToggleDropdown);
        searchField.onValueChanged.AddListener(FilterOptions);
        //PopulateDropdown();
    }

    void ToggleDropdown()
    {
        dropdownPanel.SetActive(!dropdownPanel.activeSelf);
    }

    public void PopulateDropdown(List<string> resources)
    {
        foreach (var option in resources)
        {
            GameObject toggleObj = Instantiate(togglePrefab, optionContainer);
            Toggle toggle = toggleObj.GetComponent<Toggle>();
            toggle.GetComponentInChildren<TextMeshProUGUI>().text = option;

            toggle.onValueChanged.AddListener((isSelected) => ToggleSelection(option, isSelected));

            toggleItems.Add(toggle);
        }
    }

    void ToggleSelection(string option, bool isSelected)
    {
        if (isSelected)
        {
            selectedOptions.Add(option);
        }
        else
        {
            selectedOptions.Remove(option);
        }
        UpdateSelectedText();
    }

    void UpdateSelectedText()
    {
        selectedTextDisplay.text = selectedOptions.Count > 0 ? string.Join(", ", selectedOptions) : "Select options...";
    }

    void FilterOptions(string searchText)
    {
        foreach (var toggle in toggleItems)
        {
            bool shouldShow = toggle.GetComponentInChildren<TextMeshProUGUI>().text.ToLower().Contains(searchText.ToLower());
            toggle.gameObject.SetActive(shouldShow);
        }
    }
}
