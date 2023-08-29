using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //API client to make requests
    [SerializeField]
    APIClient apiClient;

    //prefabs
    [SerializeField]
    GameObject robotPrefab;

    [SerializeField]
    GameObject paquetePrefab;

    //robot height used to instantiate robot
    [SerializeField]
    float robotHeight = 14.0f;

    //is the request and postprocessing finished?
    bool finished = false;
    
    //maps id to robot script
    Dictionary<int, Robot> robots;

    // Start is called before the first frame update
    void Start()
    {
        robots = new Dictionary<int, Robot>();
        //start the model on Start
        StartCoroutine(StartModel());
    }

    // Update is called once per frame
    void Update()
    {
        //if the request is finished
        if (finished)
        {
            //make the next step
            StartCoroutine(StepModel());
        }
    }

    //start the model
    IEnumerator StartModel()
    {
        finished = false;
        //waits before calling
        yield return new WaitForSeconds(.1f);
        //makes the request to start and get all data
        yield return apiClient.Init();
        yield return apiClient.GetRobots();
        yield return apiClient.GetPaquetes();
        yield return apiClient.GetData();

        //instantiate robots at initial positions
        for(int i = 0; i < apiClient.robotData.Count; i++)
        {
            RobotModel rm = apiClient.robotData[i];
            Vector3 position = CalculateRobotPosition(rm.x, rm.y);
            GameObject go = Instantiate(robotPrefab, position, Quaternion.identity);
            Robot robot = go.GetComponent<Robot>();
            robots.Add(rm.id, robot);
        }

        finished = true;
    }

    //makes a step
    IEnumerator StepModel()
    {
        finished = false;
        //wait before requesting
        yield return new WaitForSeconds(.1f);
        //make a step and get all data
        yield return apiClient.Step();
        yield return apiClient.GetRobots();
        yield return apiClient.GetPaquetes();
        yield return apiClient.GetData();

        //update robots positions
        for (int i = 0; i < apiClient.robotData.Count; i++){
            RobotModel rm = apiClient.robotData[i];
            Vector3 newPosition = CalculateRobotPosition(rm.x, rm.y);
            robots[rm.id].Move(newPosition);
        }

        finished = true;
    }

    //map MESA position to Unity position
    Vector3 CalculateRobotPosition(float x, float y)
    {
        int cellSize = 64;
        int halfCellSize = cellSize / 2;
        return new Vector3(cellSize * x + halfCellSize, robotHeight / 2, cellSize * y + halfCellSize);
    }

}
