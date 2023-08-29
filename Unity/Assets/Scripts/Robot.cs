using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Robot : MonoBehaviour
{

    public bool moving = false;
    Vector3 target;
    float distTreshold = 1f;

    float speed = 50.0f;
    float rotSpeed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if moving flag active
        if (moving)
        {
            //calculate distance to the target
            float dist = Vector3.Distance(target, transform.position);
            //if the distance is less than a treshold stop moving
            if(dist < distTreshold)
            {
                moving = false;
            } else
            {
                //set and normalize direction
                Vector3 direction = target - transform.position;
                direction = Vector3.Normalize(direction);
                //set rotation
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                //update position and rotation
                transform.position += direction * Time.deltaTime * speed;
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


}
