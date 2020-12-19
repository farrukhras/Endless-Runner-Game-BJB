using UnityEngine;
using System.Collections;

public class TrackPlayerDuringRun : MonoBehaviour {

    // the camera can only move forward(with the player) and to left or right (when the player moves left or right)
    // public bool[] initializers;
    public bool isStartPos;
    public bool xPos;
    public bool zPos;

    // the player transformation object, to get access to the position of the player and update the camera pos with player's position

    public Transform playerGameObjectTransf;

    // store the initial position of the player
    Vector3 startingDisplacement;

    void Awake() {
        // set the variables to true when game instance begins
        isStartPos = true;
        // xPos = true;
        zPos = true;
    }

	// at the start of the game, compute the initial position of the player
	void Start () {
        startingDisplacement = transform.position - playerGameObjectTransf.position;    
	}

	// Update is called once per frame
	void Update () {
        if (PlayerController.playerConInst.playerLifeStatus)
        {
            // we know that our player will always move forwad, so we will track him forward (in z-axis) always
            transform.position = updateAxisValues(zPos, "z");

            // we also have to check if the player moves left or right, if so then update the camera position accordingly
            if (xPos) {
                transform.position = updateAxisValues(xPos, "x");
            }   
        }
    }

    // update the axis values, if the player is statring from the START, meanign starting the game
    Vector3 updateAxisValues(bool axis, string axisLabel) {
        float new_x = 0;
        float new_y = 0;
        float new_z = 0;

        // compute the updated axis values, this is achieved by finding the change in the position of the player along any axis
        if (axisLabel == "z") {
            new_x = transform.position.x;
            new_y = transform.position.y;
            // compute the new position of the player in z-axis (forward direction)
            new_z = playerGameObjectTransf.position.z + startingDisplacement.z; 
        }
        else if (axisLabel == "x") {
            // compute the new position of the player in x-axis (left/right direction)
            new_x = playerGameObjectTransf.position.x + startingDisplacement.x; 
            new_y = transform.position.y;
            new_z = transform.position.z;
        }

        Vector3 updatedPos = new Vector3(new_x, new_y, new_z);
        return updatedPos;
    }
}
