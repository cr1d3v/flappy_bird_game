using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsSettings : MonoBehaviour
{

    public GameObject gameTargetToFollow; // var to follow the GameTarget - set to the Camer Position as to be relative to the target's position
    public float frameSize; // layer by layer
    public float interval; // var to store how far away from the Camera Position has exceleed within the gameplay
    private int counter; // var to count how many frames there are by counting how many children it has


    // Use this for initialization
    void Start()
    {
        // Count how many Childs the transform 
        counter = transform.childCount; // using 'transform' as position is stored in transform component as opposed to the main game object itself
    }

    // Update is called once per frame
    void Update()
    {
        // loop to iterate inside every graphics frame 
        for (int i = 0; i < counter; i++)
        {
            GameObject currentFrame = transform.GetChild(i).gameObject;

            // if statement to check the position relative to the gameTargetToFollow
            // if the currentFrame is too far from the gameTargetToFollow the change Camera Position horizontally
            if (gameTargetToFollow.transform.position.x - currentFrame.transform.position.x > interval)
            {
                // vector3 used in 3D Space - "XYZ" = using 'transform' as position is stored in transform component as opposed to the main game object itself
                // set value on x - current horizontal position of the graphics frame + the total ammount of graphics frame * by the size of the frame
                // once the frame (6) is reached move frame (0) in front and keep going ... 
                // set value on y - whatever val is stored in Inspector ( done so for an easier update if required at a later stage )
                // set value on z - whatever val is stored in Inspector ( done so for an easier update if required at a later stage )
                currentFrame.transform.position = new Vector3(
                    currentFrame.transform.position.x + counter * frameSize,
                    currentFrame.transform.position.y,
                    currentFrame.transform.position.z
                );
            }
        }
    }
}
