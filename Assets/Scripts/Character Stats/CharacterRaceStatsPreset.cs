using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterRaceStatPreset", menuName = "Character Race Stats Preset")]
[System.Serializable]
public class CharacterRaceStatsPreset : ScriptableObject
{
    public Enums.eRaceType RaceType;

    public int Base_HP;
    public int Base_MP;
    public int Base_Att;
    public int Base_Def;
    public int Base_Acc;
    public int Base_Mgc;
    public int Base_Evn;
    public int Base_Spd;
    public int Base_Lck;
}