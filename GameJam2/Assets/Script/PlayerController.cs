using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    public float maxSpeed = 5;                                                                                  //Sets max speed
    public float jumpTakeOffSpeed = 5;                                                                          //Sets jump speed
    public float additionalJumps = 0;                                                                           //Sets additional jumps

    private float totalJumps = 0;                                                                               //Total additional jumps taken
    private GameController gc;
   // private HealthBar hb;

    private Transform m_currMovingPlatform;

    public bool moving;
    public bool AllowedMovement = true;
    private GameObject platform;
    private bool invincible;
    private float invincibleTimer;
    public float timeInvincible = 5;

    Vector2 lookDirection = new Vector2(0, 0);

    Animator animator;

    //public int maxhealth = 10;
    //private int currenthealth;

    void Start()
    {
        gc = GameObject.FindGameObjectWithTag("GC").GetComponent<GameController>();
        transform.position = gc.lastCheckPoint;
        animator = GetComponent<Animator>();
        animator.SetBool("Grounded", true);
        invincible = false;
      //  currenthealth = maxhealth;
       // hb = GameObject.FindGameObjectWithTag("HB").GetComponent<HealthBar>();
    }

    protected override void ComputeVelocity()
    {

        if (AllowedMovement)
        {
            Vector2 move = Vector2.zero;                                                                            //zero out move

            if (Input.GetAxis("Horizontal") != 0 && transform.parent != null)
            {
                transform.parent = null;
                move.x = Input.GetAxis("Horizontal");                                                                   //Gets horizontal input from keyboard 
                moving = true;

            }
            else if (Input.GetAxis("Horizontal") == 0)
            {
                moving = false;

            }
            else
            {
                move.x = Input.GetAxis("Horizontal");                                                                   //Gets horizontal input from keyboard 
            }

            if (Input.GetButton("Jump") && grounded)                                                                //If jump is pressed and is on ground...
            {
                velocity.y = jumpTakeOffSpeed;                                                                      //Sets velocity's y to jumptakeoffspeed
            }

            if (Input.GetButtonDown("Jump") && !grounded && totalJumps != additionalJumps)                          //If button is pressed down, is not grounded, and total jumps does not exceed aditional jumps...
            {
                velocity.y = jumpTakeOffSpeed;                                                                      //Sets velocity's y to jumptakeoffspeed
                totalJumps += 1;                                                                                    //total jumps increased by 1                                                                                                                       
            }

            else if (Input.GetButtonUp("Jump"))                                                                     //If jump is let go...
            {
                if (velocity.y > 0)                                                                                 //If y velocity is greater than 0...
                {
                    velocity.y = velocity.y * .5f;                                                                  //y velocity equals velocity times .5
                }
            }

            targetVelocity = move * maxSpeed;                                                                       //target veloicty equals move times max speed

            if (!Mathf.Approximately(targetVelocity.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
            {
                lookDirection.Set(move.x, move.y);
                lookDirection.Normalize();
            }

            if (grounded)                                                                                           //If grounded...
            {
                totalJumps = 0;                                                                                     //total jumps is set back to 0
                //animator.SetBool("Grounded", true);
                //animator.SetFloat("Speed", Mathf.Abs(targetVelocity.x));
            }

            if (!grounded)
            {
                // animator.SetBool("Grounded", false);
                //animator.SetFloat("Speed", 0.0f);
            }

            //animator.SetFloat("Look X", lookDirection.x);

        }

        if (!AllowedMovement)
        {
            //animator.SetFloat("Look X", 0);
            // animator.SetFloat("Speed", 0.0f);
            // animator.SetBool("Grounded", true);
        }
    }

 /*   public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (invincible)
                return;

            invincible = true;
            currenthealth -= amount;
            hb.SetValue(currenthealth);
            invincibleTimer = timeInvincible;
        }
    }
 */
}
