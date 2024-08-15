using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class H_StartUIClick : MonoBehaviour
{
    public Image StartUIClick;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    public void OnClickButton()
    {
        StartUIClick.enabled = true;
        SoundManager.instance.PlayCardSound(0);
        StartCoroutine(SceneLoader());
    }

    IEnumerator SceneLoader()
    {
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("Alpha_Scene_Final_ByJSK_0811_Test 1");
    }
}
