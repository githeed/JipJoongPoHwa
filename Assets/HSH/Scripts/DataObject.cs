using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataObject : MonoBehaviour
{
    public Image hpBar;
    public Image expBar;
    public Text hpText;
    public Text lvText;
    public Image eCool;
    public Image rCool;
    public Text eCoolText;
    public Text rCoolText;
    public Text timeText;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.hpBar = hpBar;
        GameManager.instance.expBar = expBar;
        GameManager.instance.hpText = hpText;
        GameManager.instance.lvText = lvText;
        GameManager.instance.eCool = eCool;
        GameManager.instance.rCool = rCool;
        GameManager.instance.eCoolText = eCoolText;
        GameManager.instance.rCoolText = rCoolText;
        GameManager.instance.timeText = timeText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
