using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rigidbodyComponent;
    private Vector3 startPosition;

    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        startPosition = rigidbodyComponent.position;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
           Restart();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Goal")
        {
            Restart();
        }
    }

    private void Restart()
    {
        rigidbodyComponent.position = startPosition;
        rigidbodyComponent.velocity = Vector3.zero;
    }
}
