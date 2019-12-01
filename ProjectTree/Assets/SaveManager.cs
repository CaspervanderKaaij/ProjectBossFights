using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
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

    void OnApplicationQuit() {
        SaveSystem.Save(this);
    }

    void Start(){
        SaveStuff data = SaveSystem.LoadStuff();
        antiAlias = data.antiAlias;
        resolution = data.resolution;
        windowMode = data.windowMode;
        quality = data.quality;
        nightcore = data.nightcore;
        godMode = data.godMode;
        oneHitDieMode = data.oneHitDieMode;
        infiniteWillpower = data.infiniteWillpower;
        infiniteSkillpoints = data.infiniteSkillpoints;
    }

}
