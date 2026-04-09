using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ObjectiveTrackerUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI objectiveText;

    [Header("Objective Asset")]
    public ObjectiveListAsset objectiveList;

    [Header("Display Settings")]
    public bool showTitle = true;
    public bool checklistMode = true;
    public bool showOnlyCurrentObjective = false;
    public bool hideCompletedIfNotCurrent = false;

    [Header("Checklist Symbols")]
    public string incompleteSymbol = "[ ]";
    public string completeSymbol = "[x]";

    void Start()
    {
        if (GameProgress.Instance != null)
            GameProgress.Instance.OnProgressUpdated += RefreshUI;

        RefreshUI();
    }

    void OnDestroy()
    {
        if (GameProgress.Instance != null)
            GameProgress.Instance.OnProgressUpdated -= RefreshUI;
    }

    public void RefreshUI()
    {
        if (objectiveText == null)
            return;

        if (objectiveList == null)
        {
            objectiveText.text = "No objective list assigned";
            return;
        }

        if (GameProgress.Instance == null)
        {
            objectiveText.text = objectiveList.noObjectivesText;
            return;
        }

        StringBuilder sb = new StringBuilder();
        List<ObjectiveData> visibleObjectives = GetVisibleObjectives();

        if (showOnlyCurrentObjective)
        {
            ObjectiveData current = GetCurrentObjective(visibleObjectives);

            if (current == null)
            {
                objectiveText.text = objectiveList.noObjectivesText;
                return;
            }

            if (showTitle && !string.IsNullOrEmpty(objectiveList.listTitle))
                sb.AppendLine(objectiveList.listTitle);

            bool complete = IsComplete(current);

            if (checklistMode)
                sb.AppendLine((complete ? completeSymbol : incompleteSymbol) + " " + current.objectiveText);
            else
                sb.AppendLine(current.objectiveText);

            objectiveText.text = sb.ToString().TrimEnd();
            return;
        }

        if (showTitle && !string.IsNullOrEmpty(objectiveList.listTitle))
            sb.AppendLine(objectiveList.listTitle);

        bool addedAny = false;

        foreach (ObjectiveData objective in visibleObjectives)
        {
            bool complete = IsComplete(objective);

            if (hideCompletedIfNotCurrent && complete)
                continue;

            if (!objective.keepVisibleWhenComplete && complete)
                continue;

            if (checklistMode)
                sb.AppendLine((complete ? completeSymbol : incompleteSymbol) + " " + objective.objectiveText);
            else
                sb.AppendLine(objective.objectiveText);

            addedAny = true;
        }

        if (!addedAny)
            sb.AppendLine(objectiveList.noObjectivesText);

        objectiveText.text = sb.ToString().TrimEnd();
    }

    List<ObjectiveData> GetVisibleObjectives()
    {
        List<ObjectiveData> result = new List<ObjectiveData>();

        if (objectiveList == null || objectiveList.objectives == null)
            return result;

        foreach (ObjectiveData objective in objectiveList.objectives)
        {
            if (objective == null)
                continue;

            if (!ShouldShow(objective))
                continue;

            result.Add(objective);
        }

        return result;
    }

    ObjectiveData GetCurrentObjective(List<ObjectiveData> visibleObjectives)
    {
        foreach (ObjectiveData objective in visibleObjectives)
        {
            if (!IsComplete(objective))
                return objective;
        }

        if (visibleObjectives.Count > 0)
            return visibleObjectives[visibleObjectives.Count - 1];

        return null;
    }

    bool ShouldShow(ObjectiveData objective)
    {
        if (!string.IsNullOrEmpty(objective.showAfterFlag) &&
            !GameProgress.Instance.HasFlag(objective.showAfterFlag))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(objective.hideAfterFlag) &&
            GameProgress.Instance.HasFlag(objective.hideAfterFlag))
        {
            return false;
        }

        return true;
    }

    bool IsComplete(ObjectiveData objective)
    {
        return !string.IsNullOrEmpty(objective.completionFlag) &&
               GameProgress.Instance.HasFlag(objective.completionFlag);
    }

    public void SetObjectiveList(ObjectiveListAsset newList)
    {
        objectiveList = newList;
        RefreshUI();
    }
}