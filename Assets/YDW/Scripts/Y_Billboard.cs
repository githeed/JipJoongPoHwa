using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_Billboard : MonoBehaviour
{
    public Camera allyCamera;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.name == "AllyHP")
        {
            transform.forward = allyCamera.transform.forward;
        }
        else
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
