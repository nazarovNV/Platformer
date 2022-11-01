using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public GameObject LeftBorder;
    public GameObject RightBorder;

    public  new Rigidbody2D rigidbody;

    public bool isRightDirection;

    public float speed;

    public GroundDetection groundDetection;

    public Animator animator;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private CollisionDamage collisionDamage;


    private void Update()
    {
        if(groundDetection.isGrounded)
        {
            if(transform.position.x>RightBorder.transform.position.x|| collisionDamage.Direction<0)
            {
                isRightDirection = false;
            } 
            else if(transform.position.x < LeftBorder.transform.position.x || collisionDamage.Direction > 0)
            {
                isRightDirection = true;
            }
            rigidbody.velocity = isRightDirection ? Vector2.right : Vector2.left;
            rigidbody.velocity *= speed;
        }




        if (rigidbody.velocity.x > 0)
            spriteRenderer.flipX = true;
        if (rigidbody.velocity.x < 0)
            spriteRenderer.flipX = false;
    }
}
