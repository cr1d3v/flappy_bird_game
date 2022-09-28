using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerSettings : MonoBehaviour
{
    // work with events - like when points are scored to birds are killed 
    public delegate void ScoringManager(); // manager to inform other classes about the Scores
    public delegate void KillsManager(); // manager to inform other classes about the Kills
    public event ScoringManager whenToScorePts; // the var name used
    public event KillsManager whenToKillTheBird; // the var name used

    // GamePlayerSettings - GameObject Settings
    public float verticalFlapSpeed = 4.7f; // The speed of flapping vertically 
    public float horizontalFlapSpeed = 2.3f; // The speed of flapping horizontally
    public float verticalBorder; // GameScene - vertical borders (i.e., so the bird does not fly above or beyond the in game visible frame)
    public float fallingForce; // The force of falling

    private Rigidbody2D playerGravity; // var to refference RigigBody2D in GamePlayer - GameObject (0 by default)
    private AudioSource birdFlapNoise;
    private bool birdOnIdle = true; // set the bird on idle in the menu screen so it does not fly away
    private int skinCounter = 0;

    // Use this for initialization
    void Start()
    {
        playerGravity = gameObject.GetComponent<Rigidbody2D>(); // fetching the ref by accessing GamePlayerSettings - Game Object and getting the component of type Rigidbody2D
        birdFlapNoise = gameObject.GetComponent<AudioSource>(); // fetching the ref by accessing GamePlayerSettings - Game Object and getting the component of type AudioSource

        playerGravity.gravityScale = 0; // set to null

        moveThroughTheSkins(0); // set the simple skin as default ( changing to 1 or 2 ) will change the default skin to birdskin(2) or birdskin(3) accordignly 
    }

    // Update is called once per frame
    void Update()
    {
        // Kepp the bird flying motion in-between the in game visible screen
        if (transform.position.y > verticalBorder) // if the bird is exceeding the top of the in game visible screen
        {
            transform.position = new Vector3(transform.position.x, verticalBorder, transform.position.z); // vector3 as camera is used in a 3D space (xyz) - x and z stay untouched and set y as verticalBorder
            playerGravity.velocity = new Vector2(playerGravity.velocity.x, 0);
        }
        else if (transform.position.y < -verticalBorder)
        {
            killTheGamePlayer();
        }

        /* check if ( SpaceBar or left mouse click is being pressed i.e., then Axis res holds 1 and vice versa if not pressed then Axis res holds 0)
        * P.S: keys are editable in - Unity/Edit/ProjectSettings/InputManager/Axes/Fire1 - Fire1 string */
        if (Input.GetAxis("Fire1") == 1f && birdOnIdle == false)
        {
            flappingAction(); // call the flappingAction() method
        }

        // Re-rotate the gamePlayer to face the target
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 6);
    }

    void OnTriggerEnter2D(Collider2D otherCollider) // determine if there's a collision and if there is establish what we're colliding against!
    {
        if (otherCollider.gameObject.tag == "GreenPipeing") // if the tag of the collider's game object is colliding with is ObstacleTag
        {
            killTheGamePlayer(); // execute the killTheGamePlayer() method
        }
        else if (otherCollider.gameObject.tag == "ScoringPerimeter") 
        {
            Destroy(otherCollider.gameObject);

            if (whenToScorePts != null) // whenToScorePts is not null - i.e., if the event was triggered
            {
                whenToScorePts(); // call the method
            }
        }
    }
    // killTheGamePlayer() method
    void killTheGamePlayer() 
    {
        Destroy(gameObject); // destroy the game object - gameobject that's holding the player script

        if (whenToKillTheBird != null) // execute whenToKillTheBird() method if whenToKillTheBird is not null
        {
            whenToKillTheBird();
        }
    }
    //moveThroughTheSkins() method
    // Code responspoible to cycle through the available skins
    // counter variable used for the value 0,1 or 2
    // when player sets a skin the loop will iterate over all available skins
    // and once the counter passed as a perimeter in the Inspector mode is met choose it
    // BirdSkin(1) - counter 0; BirdSkin(2) - counter 1; BirdSkin(3) - counter 3;
    // once the counter is passed as 1 it will disable the BirdSkin(1) and BirdSkin(3) therefore choosing BirdSkin(2) and vice versa
    void moveThroughTheSkins(int counter)
    {
        for (int j = 0; j < transform.childCount; j++)
        {
            // get child at counter "j" if ( j is = in value with counter ) if not equal the childe will be disabled
            transform.GetChild(j).gameObject.SetActive(j == counter);
        }
    }

    // changeTheSkin() method executed in TheMainGameManager class
    public void changeTheSkin()
    {
        skinCounter++; // increment 

        if (skinCounter >= transform.childCount)
        {
            skinCounter = 0;
        }

        moveThroughTheSkins(skinCounter); // execute moveThroughTheSkins method
    }

    public void beginToMove()
    {
        birdOnIdle = false; // take the bird away from idle and beginToMove


        flappingAction(); // call the flappingAction() method

        // Make the GamePlayer move 'Horizontally'
        /* The velocity is defined by 2x values - 'x' for horizontal and 'y' for vertical 
         * Therefore a Vector2 is used to store:
         * val 'x' as horizontalFlapSpeed
         * val 'y' as as what is in use right now i.e., playerGravity.velocity.y
         * Thus, when chaning the verticalFlapSpeed -> repeat the value in a vertical axis */
        playerGravity.velocity = new Vector2(horizontalFlapSpeed, playerGravity.velocity.y);
        playerGravity.gravityScale = fallingForce;
    }

    void flappingAction()
    {
        // Make the GamePlayer move 'Vertically'
        /* The velocity is defined by 2x values - 'x' for horizontal and 'y' for vertical 
         * Therefore a Vector2 is used to store:
         * val 'x' as what is in use right now i.e., playerGravity.velocity.x
         * val 'y' as verticalFlapSpeed
         * Thus, when editing the verticalFlapSpeed -> repeat the value in a horizontal axis */
        playerGravity.velocity = new Vector2(playerGravity.velocity.x, verticalFlapSpeed);

        transform.rotation = Quaternion.Euler(0, 0, 35);

        birdFlapNoise.Play(); // Play the on_click_flap_noise

    }
}
