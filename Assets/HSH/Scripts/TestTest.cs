using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TestTest : MonoBehaviour
{
    public float intencity = 0;
    
    Volume postVolume;

    Vignette myVignette;

    // Start is called before the first frame update
    void Start()
    {
        postVolume = GetComponent<Volume>();

        Vignette vv;

        if (postVolume.profile.TryGet<Vignette>(out vv))
        {
            myVignette = vv;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        ClampedFloatParameter test = new ClampedFloatParameter(intencity, 0, 1, true);
        myVignette.intensity = test;
    }
}
