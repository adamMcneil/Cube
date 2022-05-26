using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Rigidbody rigidbodyComponent;
    private ParticleSystem checkPointEffect;


    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();

        checkPointEffect = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        rigidbodyComponent.AddTorque(Vector3.up * 1000);
    }
}
