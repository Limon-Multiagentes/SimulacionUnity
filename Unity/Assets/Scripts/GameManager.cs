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

    [SerializeField]
    float packageHeight = 8.0f;

    //is the request and postprocessing finished?
    bool finished = false;
    
    //have all robots finished moving?
    bool doneMoving = false;
    
    //maps id to robot script
    Dictionary<int, Robot> robots;
    public Dictionary<int, Paquete> paquetes;

    //data about the grid
    int cellSize = 64;
    int width = 16;
    int height = 16;
    int halfCellSize = 32;
    float cintaHeight = 27.5f;
    float estanteHeight = 37.5f;


    void Start()
    {
        robots = new Dictionary<int, Robot>();
        paquetes = new Dictionary<int, Paquete>();
        //start the model on Start
        finished = false;
        StartCoroutine(StartModel());
    }

    void Update()
    {
        //if request is not finished return
        if (!finished)
        {
            return;
        }

        //if objects have finished moving
        if (doneMoving)
        {
            //set flags to false and make the next step
            finished = false;
            doneMoving = false;
            StartCoroutine(StepModel());
        } else
        {
            //if at least one robot is still moving return
            foreach(var item in robots)
            {
                if (item.Value.moving)
                {
                    return;
                }
            }
            foreach(var item in paquetes)
            {
                if (item.Value.moving)
                {
                    return;
                }
            }
            //all robots have finished moving
            doneMoving = true;
        }
    }

    //start the model
    IEnumerator StartModel()
    {
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

        //set the request as finished and robots to have finished moving
        finished = true;
        doneMoving = true;
    }

    //makes a step
    IEnumerator StepModel()
    {
        //make a step and get all data
        yield return apiClient.Step();
        yield return apiClient.GetRobots();
        yield return apiClient.GetPaquetes();
        yield return apiClient.GetData();

        //update robots positions
        for (int i = 0; i < apiClient.robotData.Count; i++){
            RobotModel rm = apiClient.robotData[i];
            Vector3 newPosition = CalculateRobotPosition(rm.x, rm.y);
            robots[rm.id].SetTarget(newPosition);
        }

        //update package positions
        for(int i = 0; i < apiClient.paqueteData.Count; i++)
        {
            PaqueteModel pm = apiClient.paqueteData[i];
            //if the package is already in the dictionary update its position
            if (paquetes.ContainsKey(pm.id))
            {
                Vector3 position = CalculatePackagePosition(pm.x, pm.y, pm.surface.ToLower());
                paquetes[pm.id].SetTarget(position);
                paquetes[pm.id].surface = pm.surface.ToLower();
                paquetes[pm.id].updated = true;
            }
            else //else create the package instantiate it behind the start of the conveyor belt and move it to the first position
            {
                Vector3 position = CalculatePackagePosition(pm.x+1, pm.y, pm.surface.ToLower());
                GameObject go = Instantiate(paquetePrefab, position, Quaternion.identity);
                Paquete paquete = go.GetComponent<Paquete>();
                Vector3 newPosition = CalculatePackagePosition(pm.x, pm.y, pm.surface.ToLower());
                paquete.SetTarget(newPosition);
                paquete.surface = pm.surface.ToLower();
                paquete.updated = true;
                paquetes.Add(pm.id, paquete);
            }

        }

        List<int> idsEliminate = new List<int>();

        //delete packages that were not updated
        foreach(var item in paquetes)
        {
            if (item.Value.updated) //reset updated status if updated
            {
                item.Value.updated = false;
            } else //if not updated it was deleted
            {
                idsEliminate.Add(item.Key);
                item.Value.GoOut();
            }
        }

        foreach(int id in idsEliminate){
            paquetes.Remove(id);
        }


        //set request as finished
        finished = true;

    }

    //map robot MESA position to Unity position
    Vector3 CalculateRobotPosition(float x, float y)
    {
        return new Vector3(cellSize * x + halfCellSize, robotHeight / 2, cellSize * y + halfCellSize);
    }
    //map package MESA position to Unity position
    Vector3 CalculatePackagePosition(float x, float y, string superficie)
    {
        float x1 = cellSize * x + halfCellSize;
        float z1 = cellSize * y + halfCellSize;
        float y1 = 0;

        //height depends on the surface
        if (superficie == "cinta")
        {
            y1 = cintaHeight + packageHeight / 2;
        } else if(superficie == "robot")
        {
            y1 = robotHeight + packageHeight / 2;
        } else
        {
            y1 = estanteHeight + packageHeight / 2;
        }
        return new Vector3(x1, y1, z1);
    }

}
