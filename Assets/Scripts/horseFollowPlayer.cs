using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class horseFollowPlayer : MonoBehaviour
{
    public Transform player;

    public float horseFollowSpeed = 3f;
  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // transform.position = new Vector3(player.x, player.y, player.z) * horseFollowSpeed * Time.deltaTime; 

        var step = horseFollowSpeed * Time.deltaTime;

        transform.position = new Vector3(player.position.x, player.position.y, player.position.z); 
        // this works but then the horse is directonly ON the player. how can we distance the horse from the player a bit? 
        // transform.position = Vector3.MoveTowards(transform.position, player.position, step); 
       /*
        if(Vector3.Distance(transform.position, player.position) < 0.001f)
        {
            player.position *= -4.0f; 
        }
       */
    }
}
