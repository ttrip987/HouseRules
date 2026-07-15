using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDecision : MonoBehaviour
{
    public enum DrawDecision
    {
        Keep,
        Draw,
        Bluff,
        RiskDraw
    }

    public class DrawDecision
    {
        public List<int> cardsToReplace;

        public float confidence;

        public string reasoning;
    }
}
