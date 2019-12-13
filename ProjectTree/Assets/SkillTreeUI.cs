using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillTreeUI : MonoBehaviour
{
    public int curSelected = 0;
    public SkillTreeSkillUI[] skills;
    public Text nameText;
    public Text descriptionText;
    public Text unlockText;
    void Start()
    {
        
    }

    void Update()
    {
        UpdateText();
    }

    void UpdateText(){
        nameText.text = skills[curSelected].name;
        descriptionText.text = skills[curSelected].desription;

        if(skills[curSelected].unlocked == true){
            unlockText.text = "Unlocked";
        } else {
            unlockText.text = skills[curSelected].unlockDescription;
        }
    }

    public void SetSelected(int id){
        curSelected = id;
    }
}
