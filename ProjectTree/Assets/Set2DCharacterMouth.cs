using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Set2DCharacterMouth : MonoBehaviour {
    [SerializeField] SpriteRenderer mouth;
    [SerializeField] Sprite[] sprites;
    public void UpdateMouth (int newMouth) {
        mouth.sprite = sprites[newMouth];
    }
}