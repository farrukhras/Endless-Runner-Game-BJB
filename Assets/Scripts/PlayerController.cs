using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public static PlayerController playerConInst; // create an instance of the playercontroller to pass the player life status to other scripts

    Animator aniController; // animation controller, controls the animation of the player
    Rigidbody rigidBodyController; // component to detect the collition of the player with an obstacle and to add some speed when the player performs a jump action
    public GameObject DuringGameDataUpdaterView;
    public GameObject PostGameDataUpdaterView;
    // player variables
    public bool playerLifeStatus; // check whether the player is alive or dead
    public float playerInitalSpeed; // initial speed of the player
    public float playerAccIncFactor; // the rate of increase of the player speed, it increases the speed gradually
    float x_position; // for the horizontal movement of the character
    public Text DuringGameScoreUpdaterView;
    public Text DuringGameCoinUpdaterView;
    float playerSpeed; // the speed of the player with which it runs
    int playerScore; // increment for the score of the player, it increases the player score as the player runs, by using the time of the system
    Vector3 playerPostCollisionPosition; // end position of the player once it collides with an obstacle
    bool touchingGround; // operator to check if the player is touching the ground or not
    int playerCoinCounter; // increments the number of coins that a player has wo during the game

    // for mobile, store the position, where the player has touched, like if the player swiped right, then store 
    // the distance from the initial position to the right, this will be used to see which operation 
    // to perform (left, right, jump, slide)
    Vector3 distanceChangeOnTouch; // store the distance change in all 3 axis thus a Vector3

    float initialGameTime; // time at which the player started the game

    public Text PostGameScoreUpdaterView;
    public Text PostGameCoinUpdaterView;

    // called when the script instance is being loaded.
    void Awake()
    {
        // set the player life status to true, when the game starts
        if (playerConInst == null) {
            playerConInst = this;
        }
        playerLifeStatus = true;

        // get the animation and the rigidBody Controllers of the player
        aniController = GetComponent<Animator>(); // get the animation component of the player

        // we will freeze the rotation of the player in all 3 axis
        rigidBodyController = GetComponent<Rigidbody>(); // get the rigid Body component of the player and increment it as player runs along
    }

	// initialize anything at the start of the game
	void Start () {
        playerCoinCounter = 0; // initially there will be no coins for the player
        initialGameTime = Time.time; // initial time will be the time the game is started
        playerSpeed = playerInitalSpeed; // set the initial speed of the player to a certain value
        x_position = 0; // player is at the center initially
	}

    // called every single frame
    void Update()
    {
        // if the player is alive then update its info once per frame
        if (playerLifeStatus)  
        {
            // GIVEN THAT THE PLAYER IS ALIVE, UPDATE ALL ITS RELEVANT VALUES
            // increase the player score, (currentTime - initialGameTime)
            float currTime = Time.time;
            int newScore = (int)(currTime - initialGameTime); // convert the score to an integer va;ue

            playerScore = newScore; // new score of the player

            // compute the displacement on the x-axis (initial x position - transformed x position)
            float changedXPos = playerDisplacement();

            // move the player forward along its z-axis
            Vector3 translation = (playerSpeed * Time.deltaTime * Vector3.forward + changedXPos * Vector3.right);
            transform.Translate(translation);
            
            // update the score on the canvas, by converting the current score to text
            string p_s = playerScore.ToString();
            DuringGameScoreUpdaterView.text = p_s;

            /// set the animation of the player (C)
            aniController.SetFloat("Velocity", playerSpeed);

            // increase the player speed, **this is to increase the difficulty level**, multiply it by acceleration
            playerSpeed += playerAccIncFactor * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.UpArrow)) { // player jumps
                // Debug.Log("Make the player Jump!!");
                // if the player was touching the ground
                if(touchingGround)
                {
                    // public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force);
                    aniController.SetTrigger("Jump");

                    // update the y-position of the rigidbody(our player)
                    // Force is applied continuously along the direction of the force vector. 
                    // we will set the ForceMode to VelocityChange, because it will change the velocity of the player, despite its mass, in the given direction
                    Vector3 force = 5 * Vector3.up; // set the force to add to the rigid body along the y-axis
                    rigidBodyController.AddForce(force, ForceMode.VelocityChange);
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow)) { // right movement (means negative x-axis)
                // Debug.Log("Make the player move Right!!");
                if (transform.position.x <= 0.1) {
                    x_position = Mathf.Round(transform.position.x + 2);
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) { // left movement (means negative x-axis)
                // Debug.Log("Make the player move Left!!");
                if (transform.position.x >= -0.1) {
                    x_position = Mathf.Round(transform.position.x - 2);
                }
            }
        }

        // if the player is not alive, then set the current postion of the player to be equal to the end position
        else {
            // if a player hits an obstacle or collides with another thing, then the end position is set to that x-axis and z-axis position
            transform.position = playerPostCollisionPosition;
        }
    }

    float playerDisplacement() {
        // compute the displacement on the x-axis (initial x position - transformed x position)
        float changedXPos = x_position - transform.position.x;

        if(changedXPos<-0.1) { 
            changedXPos = -playerSpeed * Time.deltaTime;
        }
        else if (changedXPos > 0.1) {
            changedXPos = playerSpeed * Time.deltaTime;
        }

        return changedXPos;
    }

    // OnCollisionEnter is called when this player has begun touching another rigidbody/collider (in our case an obstacle)
    void OnCollisionEnter(Collision collision)
    {
        // check if the player is colliding with an obstacle
        // if the tag of the gameObject is "Obstacle" then set the end position to be the players current position
        if (collision.gameObject.tag == "Obstacle") {
            // if tag of the "with collided" is "Obstacle" then set the playerPostCollisionPosition to the current player position
            if (touchingGround) {
                float new_x = transform.position.x; // currnet player x pos
                float new_z = transform.position.z; // current player z pos
                playerPostCollisionPosition = new Vector3(new_x, 0, new_z);

                // kill the player and update the stats
                Destroy(gameObject);
                PostDeathScreen();
            }
        }
        else if (collision.gameObject.tag == "Running Track") { // called when this player has begun touching the ground or the obstacle
            touchingGround = true;
        }
    }

    // when the player collides with an object, then kill the player and display the score
    void PostDeathScreen() {
        // set playerLifeStatus to false, to tell the player controller that the player is dead and stop the running 
        playerLifeStatus = false;
    
        // wait for 2 seconds to display the post game statistics
        System.Threading.Thread.Sleep(200);

        // hide the visibility of the main canvas, use the SetActive function of the gameobject
        DuringGameDataUpdaterView.SetActive(false);   

        string p_s = playerScore.ToString();
        string p_cc = playerCoinCounter.ToString();
        PostGameScoreUpdaterView.text = p_s;
        PostGameCoinUpdaterView.text = p_cc;
        
        // set the post death screen's status to true 
        PostGameDataUpdaterView.SetActive(true);
    }

    // called when this player has stopped touching the ground
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Running Track")
        {
            touchingGround = false;
        }
    }

    // destroy the collision gameobject (in this case it will be the coin and mask gameobject)
    void DestroyGameObject(Collider collision)
    {
        Destroy(collision.gameObject);
    } 

    // when a GameObject (player) collides with another GameObject (coin and mask), Unity calls OnTriggerEnter.
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Mask" || collision.gameObject.tag == "Coin") {
            DestroyGameObject(collision);
            switch (collision.gameObject.tag)
            {
                case "Mask":
                    playerCoinCounter += 10;
                    string p_cc1 = playerCoinCounter.ToString();
                    DuringGameCoinUpdaterView.text = p_cc1;

                    int newScore1 = (int)(100 + initialGameTime); // convert the score to an integer va;ue
                    playerScore = newScore1;
                    // Debug.Log(initialGameTime);
                    // Debug.Log(newScore1);
                    // Debug.Log(playerScore);
                    string p_s = playerScore.ToString();
                    // Debug.Log(p_s);
                    DuringGameScoreUpdaterView.text = p_s;
                    break;
                case "Coin":
                    playerCoinCounter++;
                    string p_cc = playerCoinCounter.ToString();
                    DuringGameCoinUpdaterView.text = p_cc;
                    break;
            }
        }
    }
}
