using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Robot : MonoBehaviour
{

    public bool moving = false;
    public bool isFast = false;

    private Queue<Vector3> targets = new Queue<Vector3>();
    private Vector3 currentTarget = Vector3.zero;

    private float distTreshold = 5f;
    private float speed = 50.0f;
    private float rotSpeed = 2.0f;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
                //set speed
                float robotSpeed = speed;
                if (isFast)
                {
                    robotSpeed *= 2;
                }
                //update position and rotation
                transform.position += direction * Time.deltaTime * robotSpeed;
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
            }
        }
    }

    //Sets target and activates flag for moving
    public void SetTarget(Vector3 position)
    {
        moving = true;
        targets.Enqueue(position);
    }

    //destroys robot
    public void DestroyRobot()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Robot"))
        {
            Debug.Log("2 robots collided");
        }
    }


}
