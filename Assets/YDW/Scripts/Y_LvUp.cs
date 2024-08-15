using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Y_LvUp : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    int lvInt;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lvInt = H_PlayerManager.instance.indexLev;
        textMeshPro.text = lvInt.ToString();
    }
}
