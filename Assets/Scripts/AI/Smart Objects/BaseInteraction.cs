using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <Summary>
/// The base class for interactions. All interactions must inherit from this class.
/// </Summary>
public abstract class BaseInteraction : MonoBehaviour
{
    private GameVariableConnector _gameVariableConnectorScript; // The GameVariableConnector script
    public InteractionType interactionType = InteractionType.Need; // The type of interaction
    public float interactionDuration = 1f; // The duration of the interaction
    public List<InteractionNeedsChange> needsChanges = new List<InteractionNeedsChange>(); // The list of needs that will be changed by the interaction

    public abstract bool CanPerformInteraction(); // Returns true if the interaction can be performed
    public abstract void HeadToInteraction(); // Head to the interaction
    public abstract void PerformInteraction(BaseCharacterIntelligence performer, UnityAction<BaseInteraction> onInteractionComplete); // Performs the interaction
    public abstract void CancelInteraction(); // Cancels the interaction
    public abstract void CompleteInteraction(); // Completes the interaction

    private void Start()
    {
        _gameVariableConnectorScript = GameVariableConnector.instance; // Get the GameVariableConnector script
    }

    public void AssignWorkFromType(PerformerInformation performer)
    {
        switch (interactionType)
        {
            case InteractionType.Need:
                NeedInteraction(performer);
                break;

            case InteractionType.Work:
                WorkInteraction(performer);
                break;

            default:
                break;
        }
    }

    public void NeedInteraction(PerformerInformation performer)
    {
        float previousElapsedTime = performer.elapsedTime; // Get the previous interaction time
        performer.elapsedTime = Mathf.Min(performer.elapsedTime + Time.deltaTime, interactionDuration); // Update the interaction time
        float needChangePercentage = (performer.elapsedTime - previousElapsedTime) / interactionDuration; // Get the percentage of the interaction that has been completed

        if (needsChanges.Count > 0)
        {
            ApplyNeedsChanges(performer.performingAIIntelligence, needChangePercentage); // Apply the needs changes (if any)
        }
    }

    public void ApplyNeedsChanges(BaseCharacterIntelligence performerIntelligence, float percentage)
    {
        foreach (var needsChange in needsChanges)
        {
            // Update performer need through BaseCharacterIntelligence.cs 
            performerIntelligence.UpdateIndividualNeed(needsChange.targetNeedType, needsChange.changeAmount * percentage);
        }
    }

    private void WorkInteraction(PerformerInformation performer)
    {
        float previousElapsedTime = performer.elapsedTime; // Get the previous interaction time
        performer.elapsedTime = Mathf.Min(performer.elapsedTime + Time.deltaTime, interactionDuration); // Update the interaction time

        float percentageIncrease = performer.performingAIIntelligence.characterSkillsScript.skillLevel / 100f; // Get the percentage increase of the work
        float percentageSpeedIncrease = performer.performingAIIntelligence.characterSkillsScript.skillLevel / 100f; // Get the percentage increase of the work speed

        _gameVariableConnectorScript.IncreaseProgress(percentageIncrease, percentageSpeedIncrease); // Increase the progress of the work
    }

    /// <summary>
    /// Information about a performer.
    /// </summary>
    public class PerformerInformation
    {
        public BaseCharacterIntelligence performingAIIntelligence;
        public float elapsedTime;
        public UnityAction<BaseInteraction> onInteractionComplete;
    }
}

/// <summary>
/// The type of interaction.
/// </summary>
public enum InteractionType
{
    Need,
    Work
}

/// <summary>
/// Get the changing needs of the interaction.
/// </summary>
[System.Serializable]
public class InteractionNeedsChange
{
    public NeedType targetNeedType; // The type of need that will be changed
    public float changeAmount; // The amount that the need will be changed by
}
