using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu (fileName = "Chapter1Intro", menuName = "Level/DiaLevelInfo", order = 0)]
public class DialogueLevelInfo : LevelInfo {
    public DiaVars[] dialogue;
}