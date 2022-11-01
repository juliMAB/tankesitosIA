using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationScreen : MonoBehaviour
{
    public PopulationManager populationManager;
    public Text generationsCountTxt;
    public Text bestFitnessTxt;
    public Text avgFitnessTxt;
    public Text worstFitnessTxt;
    public Text timerTxt;
    public Slider timerSlider;
    public Button pauseBtn;
    public Button stopBtn;
    public GameObject startConfigurationScreen;

    string generationsCountText;
    string bestFitnessText;
    string avgFitnessText;
    string worstFitnessText;
    string timerText;
    int lastGeneration = 0;

    // Start is called before the first frame update
    void Start()
    {
        timerSlider.onValueChanged.AddListener(OnTimerChange);
        timerText = timerTxt.text;

        timerTxt.text = string.Format(timerText, Main.Instance.IterationCount);

        if (string.IsNullOrEmpty(generationsCountText))
            generationsCountText = generationsCountTxt.text;   
        if (string.IsNullOrEmpty(bestFitnessText))
            bestFitnessText = bestFitnessTxt.text;   
        if (string.IsNullOrEmpty(avgFitnessText))
            avgFitnessText = avgFitnessTxt.text;   
        if (string.IsNullOrEmpty(worstFitnessText))
            worstFitnessText = worstFitnessTxt.text;   

        pauseBtn.onClick.AddListener(OnPauseButtonClick);
        stopBtn.onClick.AddListener(OnStopButtonClick);
    }

    void OnEnable()
    {
        if (string.IsNullOrEmpty(generationsCountText))
            generationsCountText = generationsCountTxt.text;   
        if (string.IsNullOrEmpty(bestFitnessText))
            bestFitnessText = bestFitnessTxt.text;   
        if (string.IsNullOrEmpty(avgFitnessText))
            avgFitnessText = avgFitnessTxt.text;   
        if (string.IsNullOrEmpty(worstFitnessText))
            worstFitnessText = worstFitnessTxt.text;   

        generationsCountTxt.text = string.Format(generationsCountText, 0);
        bestFitnessTxt.text = string.Format(bestFitnessText, 0);
        avgFitnessTxt.text = string.Format(avgFitnessText, 0);
        worstFitnessTxt.text = string.Format(worstFitnessText, 0);
    }

    void OnTimerChange(float value)
    {
        Main.Instance.IterationCount = (int)value;
        timerTxt.text = string.Format(timerText, Main.Instance.IterationCount);
    }

    void OnPauseButtonClick()
    {
        Main.Instance.PauseSimulation();
    }

    void OnStopButtonClick()
    {
        Main.Instance.StopSimulation();
        this.gameObject.SetActive(false);
        startConfigurationScreen.SetActive(true);
        lastGeneration = 0;
    }

    void LateUpdate()
    {
        if (lastGeneration != populationManager.generation)
        {
            lastGeneration = populationManager.generation;
            generationsCountTxt.text = string.Format(generationsCountText, populationManager.generation);
            bestFitnessTxt.text = string.Format(bestFitnessText, populationManager.bestFitness);
            avgFitnessTxt.text = string.Format(avgFitnessText, populationManager.avgFitness);
            worstFitnessTxt.text = string.Format(worstFitnessText, populationManager.worstFitness);
        }
    }
}
