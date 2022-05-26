using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangerDash : MonoBehaviour
{
    public int myColorNum;

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
            collision.gameObject.GetComponent<Player>().colorNumDash = myColorNum;
    }
}
