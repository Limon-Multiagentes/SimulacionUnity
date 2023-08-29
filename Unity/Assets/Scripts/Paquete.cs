using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Paquete : MonoBehaviour
{

    //variables for movement
    float distTreshold = 1f;
    float speed = 50.0f;
    float rotSpeed = 2.0f;
    public bool moving = false;
    Vector3 target;

    //the surface which the package is on top of
    public string surface;

    //whether the package was updated in the step
    public bool updated;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //destroy the object if it is out of bounds
        if(transform.position.x < 0)
        {

            Destroy(gameObject);
        }

        //if moving flag active
        if (moving)
        {
            //calculate distance to the target
            float dist = Vector3.Distance(target, transform.position);
            //if the distance is less than a treshold stop moving
            if (dist < distTreshold)
            {
                moving = false;
            }
            else
            {
                //set and normalize direction
                Vector3 direction = target - transform.position;
                direction = Vector3.Normalize(direction);
                
                //update position and rotation
                transform.position += direction * Time.deltaTime * speed;

                if(surface == "robot") //rotate if on the robot
                {
                    //set rotation
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
                }

            }
        }
    }

    //Sets target and activates flag for moving
    public void SetTarget(Vector3 position)
    {
        moving = true;
        target = position;
    }

    //Goes out of the grid
    public void GoOut()
    {
        moving = true;
        target = new Vector3(transform.position.x - 64, transform.position.y, transform.position.z);
    }

}
