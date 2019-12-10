using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D myRigidBody;

    private Animator myAnimator;

    [SerializeField]
    private float movementSpeed;

    private bool attack;

    private bool slide;

    private bool facingRight;

    [SerializeField]
    private Transform[] gruondPoints;

    [SerializeField]
    private float gruondRadius;
    
    [SerializeField]
    private LayerMask whatIsGruond; //Whatever land the player will fall through

    private bool isGruonded;

    private bool jump;
    
    [SerializeField]
    private float jumpForce;



    // Start is called before the first frame update
    void Start()
    {
        facingRight = true;
        myRigidBody = GetComponent<Rigidbody2D>(); // This object contains a reference to the player's rigid body
        myAnimator = GetComponent<Animator>();
    }
   
    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void FixedUpdate() // FixedUpdate - prevents hardware dependency on the game
    {
        float horizontal = Input.GetAxis("Horizontal"); //Reading of the horizontal axis , horizontal = user input

        isGruonded = IsGruonded();

        HandleMovement(horizontal); //horizontal = left key or right key

        Flip(horizontal);

        HandleAttack();

        HandleLayers();

        ResetValues();
    }

    private void HandleMovement(float horizontal)
    {
        if (myRigidBody.velocity.y < 0)
        {
            myAnimator.SetBool("land", true);

        }

        if (!myAnimator.GetBool("slide") && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) //prevents the player from running and attacking together
        {
            myRigidBody.velocity = new Vector2(horizontal * movementSpeed, myRigidBody.velocity.y);
        }

        if(isGruonded && jump) //If the player is grounded and wants to jump
        {
            isGruonded = false; //No more grounded
            myRigidBody.AddForce(new Vector2(0, jumpForce));
            myAnimator.SetTrigger("jump"); //Turn on jump trigger
        }

        if (slide && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            myAnimator.SetBool("slide", true);
        }

        else if (!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            myAnimator.SetBool("slide", false);
        }

        myAnimator.SetFloat("speed", Mathf.Abs(horizontal)); //Change the value of speed from 0 to movement
    }

    private void HandleAttack()
    {
        if (attack && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            myAnimator.SetTrigger("attack");
            myRigidBody.velocity = Vector2.zero; //reset speed after attack
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            attack = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            slide = true;
        }

    }

    //The function Flip changes the value of the scale so that if the figure moves left the face of the figure will be directed to the left
    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) //horizontal > 0 => Move right
        {
            facingRight = !facingRight; //Changes the value

            Vector3 theScale = transform.localScale;

            theScale.x *= -1;

            transform.localScale = theScale; //theScale = (-1,1,1)


        }
    }

    private void ResetValues()
    {
        attack = false;
        slide = false;
        jump = false;
    }

    private bool IsGruonded()
    {
        if (myRigidBody.velocity.y <= 0)
        {
            foreach (Transform point in gruondPoints) //We will check for each point if they have collided with anything
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, gruondRadius, whatIsGruond);


                for (int i = 0; i < colliders.Length; i++) { 
                    
                    if(colliders[i].gameObject != gameObject)
                    {
                        myAnimator.ResetTrigger("jump"); //Turn off jump trigger
                        myAnimator.SetBool("land", false);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void HandleLayers()
    {
        if (!isGruonded)
        {
            myAnimator.SetLayerWeight(1, 1); //If we are in the air you will activate the air layer in the animator
        }

        else
        {
            myAnimator.SetLayerWeight(1, 0); //Back to the gruond layer

        }
    }
}