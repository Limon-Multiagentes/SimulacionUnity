using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using System;
using System.Threading;



public class APIClient : MonoBehaviour
{
    
    private readonly string baseURL = "http://localhost:4000/";
    public List<RobotModel> robotData;
    public List<PaqueteModel> paqueteData;
    public DataModel data;
    public int count = -1;

    //Get robots data
    public IEnumerator GetRobots()
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
            robot.isFast = info[i]["isFast"];
            robots.Add(robot);
        }

        robotData = robots;
        robotRequest.Dispose();
        yield return null;

    }

    //Get packets data
    public IEnumerator GetPaquetes()
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
            paquete.surface = info[i]["surface"];
            paquete.robotId = info[i]["robotId"];
            paquetes.Add(paquete);
        }

        paqueteData = paquetes;
        request.Dispose();
        yield return null;
    }

    //Get data for the charts
    public IEnumerator GetData()
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

        request.Dispose();
        yield return null;

    }

    //Start the model
    public IEnumerator Init()
    {
        string url = baseURL + "init";

        ParameterModel pm = new ParameterModel();
        pm.numRobots = 4;
        pm.tasaEntrada = 5;
        pm.tasaSalida = 20;

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
        data = new DataModel();
        count = -1;
        req.Dispose();
        yield return null;

    }

    //Make a step for the model
    public IEnumerator Step()
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
        request.Dispose();

        yield return null;

    }

    //Stop the model
    public IEnumerator Stop()
    {
        string url = baseURL + "stop";
        UnityWebRequest request = UnityWebRequest.Post(url, "");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
            yield break;
        }
        request.Dispose();

        yield return null;

    }

    //Resume the model
    public IEnumerator Continue()
    {
        string url = baseURL + "continue";
        UnityWebRequest request = UnityWebRequest.Post(url, "");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(request.error);
            yield break;
        }
        request.Dispose();

        yield return null;

    }

    //Reset the model with parameters
    public IEnumerator PostParams(ParameterModel pm)
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
        data = new DataModel();
        count = -1;
        req.Dispose();

        yield return null;

    }

}
