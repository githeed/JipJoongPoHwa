using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParticle : MonoBehaviour
{
    ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnParticleTrigger()
    {
        print("트리거됨.");
    }

    

    private void OnParticleCollision(GameObject other)
    {
        print(other.name);
    }
}
