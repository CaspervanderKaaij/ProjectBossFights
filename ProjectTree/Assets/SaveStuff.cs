using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveStuff
{
    [Header("Settings")]
    public bool antiAlias;
    public int resolution;
    public bool windowMode;
    public int quality;
    [Header("Game Rules")]
    public bool nightcore;
    public bool godMode;
    public bool oneHitDieMode;
    public bool infiniteWillpower;
    public bool infiniteSkillpoints;

    public SaveStuff(SaveManager manager){
        //settings
        antiAlias = manager.antiAlias;
        resolution = manager.resolution;
        resolution = manager.resolution;
        windowMode = manager.windowMode;
        quality = manager.quality;
        //rules
        nightcore = manager.nightcore;
        godMode = manager.godMode;
        oneHitDieMode = manager.oneHitDieMode;
        infiniteSkillpoints = manager.infiniteSkillpoints;
    }
}
