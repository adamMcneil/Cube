using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaCrusher : MonoBehaviour
{
    public Transform whenToStart;
    public Transform Reset;
    private Rigidbody rigidbody;

    Vector3 startPosition;
    private bool start = false;
    private bool done = false;
    private bool reallyDone = false;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        startPosition = rigidbody.position;

    }
    private void Update()
    {
        restart();
        if (!start)
        {
            shouldStart();
        }
        else
        {
            done = true;

        }
        
    }

    private void FixedUpdate()
    {
        if (done)
        {
            if (!reallyDone)
            {
                rigidbody.AddForce(Vector3.up, ForceMode.VelocityChange);
                reallyDone = true;
            }

        }
    }
    private void shouldStart()
    {
        start = Physics.OverlapSphere(whenToStart.position, 0.5f).Length > 0;
    }

    private void restart()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            start = false;
            done = false;
            reallyDone = false;
            rigidbody.position = startPosition;
            rigidbody.velocity = new Vector3(0, 0, 0);

        }
    }
}
