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
    public bool bySteps = false;
    Vector3 target;
    Vector3 firstTarget;

    //the surface which the package is on top of
    public string surface;

    //whether the package was updated in the step
    public bool updated;

    // Start is called before the first frame update
    void Start()
    {
        moving = false;
        bySteps = false;
        surface = "cinta";
    }

    // Update is called once per frame
    void Update()
    {
        //destroy the object if it is out of bounds
        if(transform.position.x < 0)
        {

            Destroy(gameObject);
        }

        //if moving by steps move to the first target
        if (bySteps)
        {
            MoveRotate(firstTarget, false);
            return;
        }

        //if moving flag active move to target
        if (moving)
        {
            MoveRotate(target, true);
        }
    }

    //move and rotate
    private void MoveRotate(Vector3 goal, bool last)
    {
        //calculate distance to the target
        float dist = Vector3.Distance(goal, transform.position);
        //if the distance is less than a treshold stop moving
        if (dist < distTreshold)
        {
            if (last)
            {
                moving = false;
            } else
            {
                bySteps = false;
            }
        }
        else
        {
            //set and normalize direction
            Vector3 direction = goal - transform.position;
            direction = Vector3.Normalize(direction);

            //update position and rotation
            transform.position += direction * Time.deltaTime * speed;

            if (surface == "robot") //rotate if on the robot
            {
                //set rotation
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
            }

        }
    }

    //Sets target and activates flag for moving
    public void SetTarget(Vector3 position)
    {
        moving = true;
        target = position;
    }

    //Sets target by steps, used when changing surface to first move on y and then on xz, or viceversa
    public void SetTargetBySteps(Vector3 position, string newSurface)
    {
        moving = true;
        bySteps = true;
        target = position;
        //move first vertically and then on xz
        if(newSurface == "cinta" || newSurface == "estante"){
            firstTarget = new Vector3(transform.position.x, position.y, transform.position.z);
        } else //if surface is the robot move first on xz and then vertically
        {
            firstTarget = new Vector3(position.x, transform.position.y, position.z);
        }
    }

    //Goes out of the grid
    public void GoOut()
    {
        moving = true;
        target = new Vector3(transform.position.x - 64, transform.position.y, transform.position.z);
    }

    //destroys package
    public void DestroyPackage()
    {
        Destroy(gameObject);
    }


}
