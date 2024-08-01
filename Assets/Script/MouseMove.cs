using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviour
{
    public float sesitivity = 550f;
    public float rotationX;
    public float rotationY;
    void Start()
    {
        
    }

    void Update()
    {
        float mouseMoveX = Input.GetAxis("Mouse X");
        float mouseMoveY = Input.GetAxis("Mouse Y");

        rotationY += mouseMoveX * sesitivity * Time.deltaTime;
        
        rotationX += mouseMoveY * sesitivity * Time.deltaTime;

        

        /*
         * 아래의 if문은 마우스로 이동하는 시야가 사람의 시야처럼 보이기 하기 위함 
         */
        if (rotationX > 35f)
        {
            rotationX = 35f;
        }

        if(rotationX < -30f)
        {
            rotationX = -30f;
        }

        transform.eulerAngles = new Vector3(-rotationX, rotationY, 0);
    }
}