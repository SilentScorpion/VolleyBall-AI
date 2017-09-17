using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour {

	//Horizontal move speed
    public float moveSpeed;
    Animator animator;
    Rigidbody2D rigidBody;
	//Variable used to flip the character in the side of input key
    bool facingRight = true;
	//A player can jump only when its on the ground. This variable is used to check this
    bool grounded = false;
	//Radius of impact between the 2 colliders
    float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;
    public Transform groundCheck;
	
	//Probably somewhere around 16f is a good value of jumpSpeed
    public float jumpSpeed = 16f;
	//The force with which the ball is to be hit on impact with the player
    public float ballForce = 18f;
	//Ball's rigidbody
    Rigidbody2D ballBody;
    
    private void Start() {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        ballBody = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //Apply force to the ball so that it moves in the proper direction when hit by the player Controller
        if (collision.collider.CompareTag("Ball") && !BallBehaviour.pointWon) {
            ballBody.GetComponent<AudioSource>().Play();
            ballBody.isKinematic = false;
            Vector2 diff = (ballBody.transform.position - transform.position).normalized;
            ballBody.velocity = diff * ballForce;
        }
    }

    private void Update() {
 
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space) && grounded) {
            animator.SetTrigger("Jump");
           rigidBody.velocity = new Vector2(0, jumpSpeed);
         }

       //   print(GameObject.Find("Ball").GetComponent<Rigidbody2D>().velocity.y);
#endif
   
    }


    private void FixedUpdate() {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

#if UNITY_ANDROID && !UNITY_EDITOR

            bool jumping = CrossPlatformInputManager.GetButton("JumpA");
            float moveX = CrossPlatformInputManager.GetAxisRaw("HorizontalA");
            float moveY = CrossPlatformInputManager.GetAxisRaw("VerticalA");
            if (grounded && jumping) {
                //grounded = false;
                rigidBody.velocity = new Vector2(0, jumpSpeed);
                animator.SetTrigger("Jump");
            }
         	else if (grounded && moveY > 0.5f) {
                rigidBody.velocity = new Vector2(0, jumpSpeed * moveY * 1.25f);
                animator.SetTrigger("Jump");
            }

            animator.SetFloat("Speed", Mathf.Abs(moveX));
            rigidBody.velocity = new Vector2(moveSpeed * moveX, rigidBody.velocity.y);
            if (moveX > 0 && !facingRight) {
                Flip();
            }
            else if (moveX < 0 && facingRight) {
                Flip();
            }
#elif UNITY_EDITOR

        float moveX = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(moveX));
        rigidBody.velocity = new Vector2(moveSpeed * moveX, rigidBody.velocity.y);
        if (moveX > 0 && !facingRight) {
            Flip();
        }
        else if (moveX < 0 && facingRight) {
            Flip();
        }

#endif
    }
	
    void Flip() {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
