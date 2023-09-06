using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class ChartController : MonoBehaviour
{
    //pause/resume button and its sprites

    [SerializeField]
    private Image prImage;

    [SerializeField]
    private Sprite pauseSprite;

    [SerializeField]
    private Sprite resumeSprite;

    //chart objects
    [SerializeField]
    private LineChart numMovimientos;

    [SerializeField]
    private LineChart ciclosCarga;

    [SerializeField]
    private LineChart paquetesRecibidos;

    [SerializeField]
    private LineChart paquetesEnviados;

    [SerializeField]
    private GameManager gm;
    [SerializeField]
    private APIClient apiClient;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //adjust button icon
        if (gm.paused)
        {
            prImage.sprite = resumeSprite;
        }
        else
        {
            prImage.sprite = pauseSprite;
        }
    }

    //create all charts
    public void CreateCharts()
    {
        MakeChart(numMovimientos, "Movimientos de los robots", apiClient.data.movimientos);
        MakeChart(ciclosCarga, "Ciclos de carga de los robots", apiClient.data.ciclosCarga);
        MakeChart(paquetesRecibidos, "Número de paquetes recibidos", apiClient.data.paquetesRecibidos);
        MakeChart(paquetesEnviados, "Número de paquetes enviados", apiClient.data.paquetesEnviados);
    }

    //make a chart
    void MakeChart(LineChart chart, string text, List<int> data)
    {
        //set size
        chart.SetSize(400, 220);
        //add title
        var title = chart.EnsureChartComponent<Title>();
        title.text = text;
        //add tooltip
        var tooltip = chart.EnsureChartComponent<Tooltip>();
        tooltip.show = true;
        //set axes
        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.type = Axis.AxisType.Value;
        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;

        //detele preset data
        chart.RemoveData();
        chart.AddSerie<Line>("line");
        //Add data
        int top = gm.count;
        for (int i = 0; i < top; i++)
        {
            chart.AddData(0, i, data[i]);
        }
    }
}
