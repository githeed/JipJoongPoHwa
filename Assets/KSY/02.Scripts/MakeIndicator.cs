using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeIndicator : MonoBehaviour
{
    public Canvas indicatorFac;
    Canvas indicator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            indicator = Instantiate(indicatorFac);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo))
            {
                indicator.transform.position = hitInfo.point;
            }
        }
    }
}
