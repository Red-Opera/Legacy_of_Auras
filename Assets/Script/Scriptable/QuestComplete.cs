using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Quest System/Quest Data")]
public class QuestComplete : ScriptableObject
{
    public bool visitLib = false;
    public bool readBook = false;
    public bool chatLibWoman = false;
    public bool chatNPC = false;
    public bool visitDesert = false;
    public bool getGun = false;
    public bool visitForest = false;
}