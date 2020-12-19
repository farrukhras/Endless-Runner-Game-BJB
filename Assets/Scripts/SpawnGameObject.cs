using UnityEngine;
using System.Collections;

public class SpawnGameObject : MonoBehaviour {
    public GameObject[] tempobstaclesList; // a list of obstacles that are to be generated randomly
    // public GameObject[] tempobstaclesList; // a list of obstacles that are to be generated randomly

    void Start()
    {
        GenerateObstacles();
    }

    void GenerateObstacles()
    {
        int len = tempobstaclesList.Length; // len of the list of obstacles
        int obsId = Random.Range(0, len); // generate a random number between 0 and len and choose the obstacle with that numbered index as the obstacle to be displayed on the screen

        if (tempobstaclesList[obsId] != null)
        {
            // if these is an obstacle then clone it and return the clone
            Instantiate(tempobstaclesList[obsId], transform.position, Quaternion.identity);
        }

        // if the player is still alive, then keep on generating obstacles after certain duration of time
        if (PlayerController.playerConInst.playerLifeStatus)
        {
            float lowerB = 1; // lower bound of the time after which obstacles will be generated
            float upperB = 3; // upper bound of the time after which obstacles will be generated
            
            float generateAfter = Random.Range(lowerB, upperB);
            
            Invoke("GenerateObstacles", generateAfter);
        }
    }
}
