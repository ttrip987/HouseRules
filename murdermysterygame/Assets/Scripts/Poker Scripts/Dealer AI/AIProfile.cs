using UnityEngine;


[CreateAssetMenu(
    fileName="New AI Profile",
    menuName="Poker/AI Profile"
)]
public class AIProfile : ScriptableObject
{
    public string characterName;


    [Range(0,100)]
    public int aggression;


    [Range(0,100)]
    public int tightness;


    [Range(0,100)]
    public int bluffChance;


    [Range(0,100)]
    public int risk;


    [Range(0,100)]
    public int mistakeChance;


    [Range(0,100)]
    public int adaptability;
}