using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public int myColorNum;

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
            collision.gameObject.GetComponent<Player>().colorNum = myColorNum;
    }
}
