using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class H_EndUI : MonoBehaviour
{
    public Image winImage;
    public Image loseImage;
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.bISWin)
        {
            winImage.enabled = true;
        }
        else
        {
            loseImage.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickEndUI()
    {
        SoundManager.instance.PlayCardSound(0);
        SceneManager.LoadScene("StartUIScene");
    }
}
