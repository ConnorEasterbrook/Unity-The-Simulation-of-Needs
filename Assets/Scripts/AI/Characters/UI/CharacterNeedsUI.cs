using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This class stores the character's UI elements and handles the logic for updating them.
/// </summary>
[System.Serializable]
public class CharacterNeedsUI
{
    private GameObject _performerDetailPanelPrefab;
    private static Transform _performerDetailPanelParent;
    private static List<AutonomousIntelligence> performers = new List<AutonomousIntelligence>();
    private static List<GameObject> performerDetailPanels = new List<GameObject>();

    public CharacterNeedsUI(GameObject prefab, Transform prefabParent)
    {
        _performerDetailPanelPrefab = prefab;
        _performerDetailPanelParent = prefabParent;
    }

    public void AddPerformer(AutonomousIntelligence performer)
    {
        performers.Add(performer);

        GameObject performerDetailPanel = MonoBehaviour.Instantiate(_performerDetailPanelPrefab, _performerDetailPanelParent);
        performerDetailPanels.Add(performerDetailPanel);
    }

    public void PopulatePerformerDetails()
    {
        foreach (AutonomousIntelligence performer in performers)
        {
            if (performerDetailPanels[performers.IndexOf(performer)] == null)
            {
                return;
            }

            GetPerformerDetails(performer);
        }
    }

    private void GetPerformerDetails(AutonomousIntelligence performer)
    {
        TextMeshProUGUI performerName = performerDetailPanels[performers.IndexOf(performer)].transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        Slider performerHunger = performerDetailPanels[performers.IndexOf(performer)].transform.GetChild(1).GetComponentInChildren<Slider>();
        Slider performerHygiene = performerDetailPanels[performers.IndexOf(performer)].transform.GetChild(2).GetComponentInChildren<Slider>();
        Slider performerEnergy = performerDetailPanels[performers.IndexOf(performer)].transform.GetChild(3).GetComponentInChildren<Slider>();
        Slider performerHappiness = performerDetailPanels[performers.IndexOf(performer)].transform.GetChild(4).GetComponentInChildren<Slider>();

        performerName.text = performer.name;

        performerHunger.maxValue = performer.characterNeedsScript.hungerCap;
        performerHunger.value = performer.characterNeedsScript.hunger;

        performerHygiene.maxValue = performer.characterNeedsScript.hygieneCap;
        performerHygiene.value = performer.characterNeedsScript.hygiene;

        performerEnergy.maxValue = performer.characterNeedsScript.energyCap;
        performerEnergy.value = performer.characterNeedsScript.energy;

        // performerHappiness.maxValue = performer.characterNeedsScript.happinessCap;
        // performerHappiness.value = performer.characterNeedsScript.happiness;
    }
}