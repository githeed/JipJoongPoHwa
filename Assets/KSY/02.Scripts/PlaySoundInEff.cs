using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundInEff : MonoBehaviour
{
    AudioSource myAudio;
    private void OnEnable()
    {
        if (myAudio == null) GetComponent<AudioSource>();
        if(myAudio != null) myAudio.Play();
    }
    // Start is called before the first frame update
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
