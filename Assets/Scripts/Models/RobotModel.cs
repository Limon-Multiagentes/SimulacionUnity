using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RobotModel
{
    public string action;
    public float carga;
    public int id;
    public float x;
    public float y;
    public bool isFast;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}