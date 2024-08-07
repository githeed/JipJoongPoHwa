using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Y_ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI levelText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        levelText.text = H_PlayerManager.instance.indexLev.ToString();
    }
}
