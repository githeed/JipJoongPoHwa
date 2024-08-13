using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_CameraMove : MonoBehaviour
{
    Vector3 camPos = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.gameTime < 8)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, camPos, Time.deltaTime);
            if((transform.localPosition - camPos).magnitude < 0.4f)
            {
                transform.localPosition = camPos;
            }
        }
    }
}
