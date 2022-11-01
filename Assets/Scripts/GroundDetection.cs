using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    public bool isGrounded;
    public void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
        }
    }
    public void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
        }
    }
}
