using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{

    public GameObject gameTargetToFollow; // var to follow the GameTarget - set to the player's position as to be relative to the target's position
    public Vector2 scalling; // Game Camera Balancer x/y axis - var to help with errors in scalling the Game Camera i.e., counterbalance to the right or left from the Player

    private Rigidbody2D cameraPhysicsController; // ref to the game's camera PhysicsController - 0 by default

    // Start is called before the first frame update
    void Start()
    {
        cameraPhysicsController = GetComponent<Rigidbody2D>(); // fetching the ref by accessing MainCamera - Game Object and getting the component of type Rigidbody2D

        // vector3 used in 3D Space - "XYZ" = using 'transform' as position is stored in transform component as opposed to the main game object itself
        // set value on x - target's the horizontal position + the x val of the scalling var
        // set value on y -  target's the vertical position + the y val of the scalling var
        // set value on z - whatever val is stored in MainCamera Inspector ( done so for an easier update if required at a later stage )
        transform.position = new Vector3(
            gameTargetToFollow.transform.position.x + scalling.x,
            gameTargetToFollow.transform.position.y + scalling.y,
            transform.position.z);
    }
    // Update is called once per frame - using FixedUpdate instead of just Update since working with Rigidbody2D i.e.,
    // cameraPhysicsController therefore millisecond precision is required as updates must happen in fixed moments
    void FixedUpdate()
    {
        // if the GameTarget is different than null
        if (gameTargetToFollow != null)
        {
            cameraPhysicsController.velocity = new Vector2(gameTargetToFollow.GetComponent<Rigidbody2D>().velocity.x, 0); // y = 0 to stop it from moving vertically
        }
        // otherwise - the target was destroyed
        else
        {
            cameraPhysicsController.velocity = Vector2.zero; // stop the camera from moving
        }
    }
}
