using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using System;
using System.Threading;

[System.Serializable]
public class RobotModel
{
    public string action;
    public float carga;
    public int id;
    public float x;
    public float y;
}

[System.Serializable]
public class PaqueteModel
{
    public int id;
    public float peso;
    public float x;
    public float y;
}

[System.Serializable]
public class DataModel
{
    public List<int> ciclosCarga;
    public List<int> movimientos;
    public List<int> paquetesEnviados;
    public List<int> paquetesRecibidos;
}

[System.Serializable]
public class ParameterModel
{
    public int numRobots;
    public int tasaEntrada;
    public int tasaSalida;
}


public class APIClient : MonoBehaviour
{
    private readonly string baseURL = "http://localhost:4000/";
    public List<RobotModel> robotData;
    public List<PaqueteModel> paqueteData;
    public DataModel data;
    public int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Init());
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.S))
        {
            StartCoroutine(Step());
        }

        if (Input.GetKeyUp(KeyCode.G)){
            StartCoroutine(GetRobots());
        }

    }

    //Get robots data
    IEnumerator GetRobots()
    {
        string url = baseURL + "robots";
        UnityWebRequest robotRequest = UnityWebRequest.Get(url);
        yield return robotRequest.SendWebRequest();
        if(robotRequest.result == UnityWebRequest.Result.ProtocolError || robotRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(robotRequest.error);
            yield break;
        }

        JSONNode info = JSON.Parse(robotRequest.downloadHandler.text);

        List<RobotModel> robots = new List<RobotModel>(); 
        for(int i = 0; i < info.Count; i++)
        {
            RobotModel robot = new RobotModel();
            robot.action = info[i]["action"];
            robot.carga = info[i]["carga"];
            robot.id = info[i]["id"];
            robot.x = info[i]["x"];
            robot.y = info[i]["y"];
            robots.Add(robot);
        }

        robotData = robots;
    }

    //Get packets data
    IEnumerator GetPaquetes()
    {
        string url = baseURL + "paquetes";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
            yield break;
        }

        JSONNode info = JSON.Parse(request.downloadHandler.text);

        List<PaqueteModel> paquetes = new List<PaqueteModel>();
        for (int i = 0; i < info.Count; i++)
        {
            PaqueteModel paquete = new PaqueteModel();
            paquete.id = info[i]["id"];
            paquete.x= info[i]["x"];
            paquete.y = info[i]["y"];
            paquetes.Add(paquete);
        }

        paqueteData = paquetes;
    }

    //Get data for the charts
    IEnumerator GetData()
    {
        string url = baseURL + "data";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
            yield break;
        }

        JSONNode info = JSON.Parse(request.downloadHandler.text);

        data.ciclosCarga.Add(info["CiclosCarga"][count]);
        data.movimientos.Add(info["Movimientos"][count]);
        data.paquetesEnviados.Add(info["PaquetesEnviados"][count]);
        data.paquetesRecibidos.Add(info["PaquetesRecibidos"][count]);
    }

    //Start the model
    IEnumerator Init()
    {
        string url = baseURL + "init";

        ParameterModel pm = new ParameterModel();
        pm.numRobots = 4;
        pm.tasaEntrada = 10;
        pm.tasaSalida = 30;

        string json = JsonUtility.ToJson(pm);
        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ProtocolError || req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(req.error);
            yield break;
        }
    }

    //Make a step for the model
    IEnumerator Step()
    {
        string url = baseURL + "step";
        UnityWebRequest request = UnityWebRequest.Post(url, "");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
            yield break;
        }
        count += 1;
    }

    //Stop the model
    IEnumerator Stop()
    {
        string url = baseURL + "stop";
        UnityWebRequest request = UnityWebRequest.Post(url, "");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
            yield break;
        }
    }

    //Resume the model
    IEnumerator Continue()
    {
        string url = baseURL + "continue";
        UnityWebRequest request = UnityWebRequest.Post(url, "");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
            yield break;
        }
    }

    //Reset the model with parameters
    IEnumerator PostParams(ParameterModel pm)
    {
        string url = baseURL + "params";

        string json = JsonUtility.ToJson(pm);
        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ProtocolError || req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(req.error);
            yield break;
        }
    }

}
