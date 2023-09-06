using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //API client to make requests
    [SerializeField]
    APIClient apiClient;

    //UIController to control parameters
    [SerializeField]
    UiController uiController;

    //prefabs
    [SerializeField]
    GameObject robotPrefab;

    [SerializeField]
    GameObject paquetePrefab;

    //robot height used to instantiate robot
    [SerializeField]
    float robotHeight = 18.0f;

    [SerializeField]
    float packageHeight = 8.0f;

    //is the request and postprocessing finished?
    bool finished = false;
    
    //have all robots finished moving?
    bool doneMoving = true;
    
    //maps id to robot script
    Dictionary<int, Robot> robots;
    public Dictionary<int, Paquete> paquetes;

    //data about the grid
    int cellSize = 64;
    int halfCellSize = 32;
    float cintaHeight = 48f;
    float estanteHeight = 47f;

    //isPaused?
    public bool paused = false;

    public int count = -1;
    Queue<List<RobotModel>> robotModels;
    Queue<List<PaqueteModel>> paqueteModels; 

    void Start()
    {
        InitializeModel();
    }

    void Update()
    {
        if (paused) return;

        //if request is not finished 
        if (finished)
        {
            //set flags to false and make the next step
            finished = false;
            StartCoroutine(StepModel());
        }

        if(count >= apiClient.count)
        {
            return;
        }

        //if objects have finished moving
        if (doneMoving && robotModels.Count > 0)
        {
            doneMoving = false;
            updateRobots(robotModels.Dequeue());
            updatePackages(paqueteModels.Dequeue());
            eliminatePackages();
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
            count++;
        }
    }

    //start the model for the first time
    void InitializeModel()
    {
        //initialize the model
        robots = new Dictionary<int, Robot>();
        paquetes = new Dictionary<int, Paquete>();
        robotModels = new Queue<List<RobotModel>>();
        paqueteModels = new Queue<List<PaqueteModel>>();
        finished = false;
        StartCoroutine(StartModel());
    }
    
    //alternate between pause and resume
    public void togglePause()
    {
        if (!paused)
        {
            PauseModel();
        } else
        {
            ResumeModel();
        }
    }

    //pause the mdodel
    void PauseModel()
    {
        paused = true;
        StartCoroutine(StopModel());
    }

    //resume the model
    void ResumeModel()
    {
        paused = false;
        StartCoroutine(ContinueModel());
    }


    //restart the model and reset parameters
    public void RestartModel()
    {
        if (!paused)
        {
            return;
        }

        finished = false;
        count = -1;
        DestroyAll();
        robots = new Dictionary<int, Robot>();
        paquetes = new Dictionary<int, Paquete>();
        robotModels = new Queue<List<RobotModel>>();
        paqueteModels = new Queue<List<PaqueteModel>>();
        StartCoroutine(SendParams());
    }

    //make requests to start model and get data
    IEnumerator StartModel()
    {
        //makes the request to start and get all data
        yield return apiClient.Init();
        yield return apiClient.GetRobots();
        yield return apiClient.GetPaquetes();

        instantiateRobots();

        //set the request as finished and robots to have finished moving
        finished = true;
        doneMoving = true;
    }

   
    //make requests for a step and get all data
    IEnumerator StepModel()
    {
        //make a step and get all data
        yield return apiClient.Step();
        yield return apiClient.GetRobots();
        yield return apiClient.GetPaquetes();
        yield return apiClient.GetData();

        //add data to queues
        robotModels.Enqueue(apiClient.robotData);
        paqueteModels.Enqueue(apiClient.paqueteData);

        //set request as finished
        finished = true;

    }

    //make request to stop model
    IEnumerator StopModel()
    {
        yield return apiClient.Stop();
    }

    //make the request to continue the model
    IEnumerator ContinueModel()
    {
        yield return apiClient.Continue();
    }

    //send the parameters to instantia
    IEnumerator SendParams()
    {
        //create new parameter model with slider values
        ParameterModel pm = new ParameterModel();
        pm.numRobots =(int) uiController.robotSlider.value;
        pm.tasaEntrada = (int) uiController.sliderEntrada.value;
        pm.tasaSalida = (int) uiController.sliderSalida.value;

        //post parameters
        yield return apiClient.PostParams(pm);


        //makes the request to start and get all data
        yield return apiClient.GetRobots();
        yield return apiClient.GetPaquetes();

        instantiateRobots();

        //set the request as finished and robots to have finished moving
        finished = true;
        doneMoving = true;
        paused = false;
    }


    //instantiate robots at initial positions
    void instantiateRobots()
    {
        for (int i = 0; i < apiClient.robotData.Count; i++)
        {
            RobotModel rm = apiClient.robotData[i];
            Vector3 position = CalculateRobotPosition(rm.x, rm.y);
            GameObject go = Instantiate(robotPrefab, position, Quaternion.identity);
            Robot robot = go.GetComponent<Robot>();
            robots.Add(rm.id, robot);
        }
    }

    //update robot positions
    void updateRobots(List<RobotModel> robotData)
    {
        //update robots positions
        for (int i = 0; i < robotData.Count; i++)
        {
            RobotModel rm = robotData[i];
            Vector3 newPosition = CalculateRobotPosition(rm.x, rm.y);
            robots[rm.id].SetTarget(newPosition);
            robots[rm.id].isFast = rm.isFast;
        }
    }

    //update package positions and instantiate new packages
    void updatePackages(List<PaqueteModel> paqueteData)
    {
        //update package positions
        for (int i = 0; i < paqueteData.Count; i++)
        {
            PaqueteModel pm = paqueteData[i];
            //if the package is already in the dictionary update its position
            if (paquetes.ContainsKey(pm.id))
            {
                string surface = pm.surface.ToLower();
                Vector3 position = CalculatePackagePosition(pm.x, pm.y, surface);
                //if changing surface move by steps
                if(surface != paquetes[pm.id].surface)
                {
                    paquetes[pm.id].SetTargetBySteps(position, surface);
                } else
                {
                    paquetes[pm.id].SetTarget(position);
                }
                paquetes[pm.id].surface = surface;

                //if on robot check if it should move faster
                int robotId = pm.robotId;
                if (robotId > 0)
                {
                    if (robots[robotId].isFast) {
                        paquetes[pm.id].isFast = true;
                    } else
                    {
                        paquetes[pm.id].isFast = false;
                    }
                } else
                {
                    paquetes[pm.id].isFast = false;
                }

                paquetes[pm.id].updated = true;
            }
            else //else create the package instantiate it behind the start of the conveyor belt and move it to the first position
            {
                Vector3 position = CalculatePackagePosition(pm.x + 1, pm.y, pm.surface.ToLower());
                GameObject go = Instantiate(paquetePrefab, position, Quaternion.identity);
                Paquete paquete = go.GetComponent<Paquete>();
                Vector3 newPosition = CalculatePackagePosition(pm.x, pm.y, pm.surface.ToLower());
                paquete.SetTarget(newPosition);
                paquete.surface = pm.surface.ToLower();
                paquete.updated = true;
                paquetes.Add(pm.id, paquete);
            }

        }
    }

    //eliminate packages whose data is no longer received
    void eliminatePackages()
    {

        List<int> idsEliminate = new List<int>();

        //delete packages that were not updated
        foreach (var item in paquetes)
        {
            if (item.Value.updated) //reset updated status if updated
            {
                item.Value.updated = false;
            }
            else //if not updated it was deleted
            {
                idsEliminate.Add(item.Key);
                item.Value.GoOut();
            }
        }

        foreach (int id in idsEliminate)
        {
            paquetes.Remove(id);
        }

    }

    //destroys all robots and packages 
    void DestroyAll()
    {

        GameObject[] robs = GameObject.FindGameObjectsWithTag("Robot");
        foreach (var robot in robs)
        {
            GameObject.Destroy(robot);
        }

        GameObject[] packs = GameObject.FindGameObjectsWithTag("Paquete");
        foreach (var p in packs)
        {
            GameObject.Destroy(p);
        }
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
