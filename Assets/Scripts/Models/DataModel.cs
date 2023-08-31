using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataModel
{
    public List<int> ciclosCarga;
    public List<int> movimientos;
    public List<int> paquetesEnviados;
    public List<int> paquetesRecibidos;

    public DataModel() {
        ciclosCarga= new List<int>();
        movimientos= new List<int>();
        paquetesEnviados= new List<int>();
        paquetesRecibidos = new List<int>();
    }
}