using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objectives/Objective List")]
public class ObjectiveListAsset : ScriptableObject
{
    public string listTitle = "Objectives";

    [TextArea(2, 4)]
    public string noObjectivesText = "No current objectives";

    public List<ObjectiveData> objectives = new List<ObjectiveData>();
}

[System.Serializable]
public class ObjectiveData
{
    [TextArea(2, 4)]
    public string objectiveText;

    [Tooltip("Objective becomes complete when this flag is set.")]
    public string completionFlag;

    [Tooltip("Optional: show this objective only after this flag is set.")]
    public string showAfterFlag;

    [Tooltip("Optional: hide this objective once this flag is set.")]
    public string hideAfterFlag;

    [Tooltip("If true, completed objectives stay visible with a checkmark.")]
    public bool keepVisibleWhenComplete = true;
}