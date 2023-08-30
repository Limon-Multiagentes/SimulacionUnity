using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{

    //game manager

    [SerializeField]
    private GameManager gm;

    //pause/resume button and its sprites

    [SerializeField]
    private Image prButton;

    [SerializeField]
    private Sprite pauseSprite;

    [SerializeField]
    private Sprite resumeSprite;

    //sliders and texts

    [SerializeField]
    private TMP_Text numRobots;
    [SerializeField]
    public Slider robotSlider;

    [SerializeField]
    private TMP_Text tasaEntrada;
    [SerializeField]
    public Slider sliderEntrada;

    [SerializeField]
    private TMP_Text tasaSalida;
    [SerializeField]
    public Slider sliderSalida;

    //prefab and instance of chart canvas

    [SerializeField]
    private GameObject chartCanvas;

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
            prButton.sprite = resumeSprite;
        } else
        {
            prButton.sprite = pauseSprite;
        }

        //update labels with slider values
        numRobots.text = "Cantidad de robots: " + robotSlider.value;
        tasaEntrada.text = "Tasa de entrada: " + sliderEntrada.value;
        tasaSalida.text = "Tasa de salida: " + sliderSalida.value;
    }

    public void ToggleChartCanvas()
    {

        if (!chartCanvas.activeInHierarchy) //if it is about to activate build the charts
        {
            chartCanvas.GetComponent<ChartController>().CreateCharts();
        }
        chartCanvas.SetActive(!chartCanvas.activeInHierarchy);
    }
}
