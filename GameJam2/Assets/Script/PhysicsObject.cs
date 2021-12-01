using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float minGroundNormalY = .65f;                                                                   //Min ground normal allowed
    public float gravityModifier = 1f;                                                                      //Allows the scaling of gravity

    protected Vector2 targetVelocity;                                                                       //Determines movement direction
    protected bool grounded;                                                                                //Checks if on ground
    protected Vector2 groundNormal;                                                                         //Hold ground's normal
    protected Rigidbody2D rb2d;                                                                             //Rigidbody of the gameobject
    public Vector2 velocity;                                                                             //Downward pull of gravity
    protected ContactFilter2D contactFilter;                                                                //Determines what an object can't and can go through and how it reacts to specific settings
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];                                              //Creates a raycast array with a set of 16
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);                                //Creates list (similar to a vector in c++) of raycasts. 


    protected const float minMoveDistance = 0.001f;                                                         //This is the min amount of distance a object will be able to move
    protected const float shellradius = 0.01f;                                                             //Ensures collision is never exact for fear of overlapping                                 

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();                                                                 //Retrieves gameobjects rigidbody and stores it in 2b2d
    }

    void Start()
    {
        contactFilter.useTriggers = false;                                                                  //Does not check collision against triggers

        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));                      //We are setting our filter to follow the rules that the game objects layer has already set 

        contactFilter.useLayerMask = true;                                                                  //Ensures it uses the layer mask
    }

    void Update()
    {
        targetVelocity = Vector2.zero;                                                                      //Zeros out targetVelocity every frame
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {

    }

    void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;                                   //Determines velocity gravityModifer, the default physics engine, and time.deltatime

        velocity.x = targetVelocity.x;                                                                      //Tells velocity the horizontal direction determined by target velocity

        grounded = false;                                                                                   //Sets grounded to false

        Vector2 deltaPosition = velocity * Time.deltaTime;                                                  //Determines next position of object

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);                             //Helps x direction for slopes as it uses the inverse ground to determine the upward/downward direction

        Vector2 move = moveAlongGround * deltaPosition.x;                                                   //Determines x value for move

        Movment(move, false);                                                                               //Sends move variable to movement and sets y movement to false

        move = Vector2.up * deltaPosition.y;                                                                //The actual movement of the character with is determined by vector2 (0,1) and the y position of deltaposition.

        Movment(move, true);                                                                                //Sends move variable to movement and sets y movement to true
    }

    void Movment(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;                                                                    //Stores the magnitude of the move into distance 

        if (distance > minMoveDistance)                                                                     //If the distance that the object wants to move is greater than the minmovedistance.... 
        {                                                                                                   //We check this so we don't contstantly have objects move when they are sitting still on solid ground

            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellradius);                  //Move is the direction to move the object, contact filter checks to see if it can move through an object,
                                                                                                            //Hitbuffer holds the results of any possible collisions, distance + shellradius is the amount moved, and
                                                                                                            //Int count is the number of results in the array

            hitBufferList.Clear();                                                                          //Clears list

            for (int i = 0; i < count; i++)                                                                 //Takes hits from hitbuffer and adds it to the list
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)                                                   //Determines the angle of the object that is being hit
            {
                Vector2 currentNormal = hitBufferList[i].normal;                                            //Sets current normal to hitbufferlist[i] normal

                if (currentNormal.y > minGroundNormalY)                                                     //If current normal's y is larger than the min...
                {                                                                                           //Determines if ground is standable or if falling
                    grounded = true;

                    if (yMovement)                                                                          //If there is yMovement
                    {
                        groundNormal = currentNormal;                                                       //Sets the ground normal to current normal
                        currentNormal.x = 0;                                                                //Sets current normals x to 0
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);                                    //Takes the dot product and finds the magnitude and put it in projection

                if (projection < 0)                                                                         //If projection is less than 0....
                {
                    velocity = velocity - projection * currentNormal;                                       //Cancels out velocity that was stopped by collision
                }

                float modifiedDistance = hitBufferList[i].distance - shellradius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;                       //Checks to see if our distance is small than our shell size. If so uses shell size
            }
        }

        rb2d.position = rb2d.position + move.normalized * distance;                                         //Adds position of normalized move to rb2d position times the distance
    }

}