using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveStuff
{
    [Header("Settings")]
    public bool antiAlias = true;
    public int resolution = 1;
    public bool windowMode = true;
    public int quality = 3;
    [Header("Game Rules")]
    public bool nightcore = false;
    public bool godMode = false;
    public bool oneHitDieMode = false;
    public bool infiniteWillpower = false;
    public bool infiniteSkillpoints = false;

}
