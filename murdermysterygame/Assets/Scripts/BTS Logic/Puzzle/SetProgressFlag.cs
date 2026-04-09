using UnityEngine;

public class SetProgressFlag : MonoBehaviour
{
    public string flagToSet;

    public void SetFlag()
    {
        if (GameProgress.Instance != null)
            GameProgress.Instance.SetFlag(flagToSet);
    }
}