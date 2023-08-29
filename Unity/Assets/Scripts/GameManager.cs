using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    APIClient apiClient;

    [SerializeField]
    GameObject robot;

    [SerializeField]
    GameObject paquete;

    bool finished = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartModel());
    }

    // Update is called once per frame
    void Update()
    {
        if (finished)
        {
            Debug.Log("x0:" + apiClient.robotData[0].x);
            Debug.Log("y0:" + apiClient.robotData[0].y);
            //StartCoroutine(StepModel());
        }
    }

    IEnumerator StartModel()
    {
        finished = false;
        yield return new WaitForSeconds(.1f);
        yield return apiClient.Init();
        yield return apiClient.GetRobots();
        yield return apiClient.GetPaquetes();
        yield return apiClient.GetData();

        for(int i = 0; i < apiClient.robotData.Count; i++)
        {
            RobotModel rm = apiClient.robotData[i];
            Vector3 position = new Vector3(64 * rm.x + 32, 0, 64 * rm.y + 32);
            Instantiate(robot, position, Quaternion.identity);
        }

        finished = true;
    }

    IEnumerator StepModel()
    {
        finished = false;
        yield return new WaitForSeconds(.1f);
        yield return apiClient.Step();
        yield return apiClient.GetRobots();
        yield return apiClient.GetPaquetes();
        yield return apiClient.GetData();
        finished = true;
    }

}
