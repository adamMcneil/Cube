using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    private Rigidbody myRigidbody;
    private Rigidbody parentRigidbody;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        Player myParent = transform.parent.GetComponent<Player>();
        parentRigidbody = myParent.GetComponent<Rigidbody>();
        myRigidbody = GetComponent<Rigidbody>();
        startPosition = myRigidbody.position;
    }

    // Update is called once per frame
    void Update()
    {
        myRigidbody.position = parentRigidbody.position + startPosition;
    }
}
