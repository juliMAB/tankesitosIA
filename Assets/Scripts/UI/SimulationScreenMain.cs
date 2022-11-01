using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class SimulationScreenMain : MonoBehaviour
{
    public Text timerTxt;

    public Text score;

    public Text lastLose;

    //public Text generation;

    public Slider timerSlider;
    public Button pauseBtn;
    public Button stopBtn;

    public Text currentTime;


    string timerText;

    // Start is called before the first frame update
    void Start()
    {
        timerSlider.onValueChanged.AddListener(OnTimerChange);
        timerText = timerTxt.text;

        timerTxt.text = string.Format(timerText, Main.Instance.IterationCount);

        pauseBtn.onClick.AddListener(OnPauseButtonClick);
        stopBtn.onClick.AddListener(OnStopButtonClick);
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
    }

    void LateUpdate()
    {
        currentTime.text = "currentT: " + Main.Instance.AccumTime + "\n" + "MaxT: " + Main.Instance.GenerationDuration;
        score.text = "score: <color=blue> " + ScoreManager.Instance.score[0] + "</color>  -  <color=red>" + ScoreManager.Instance.score[1] + "</color>";
        lastLose.text = ScoreManager.Instance.lastLoser == 0 ? "BLUE" : "RED";
        //generation.text = "score: <color=blue> " + Main.Instance.populationManagers[0].generation + "</color>  -  <color=red>" + Main.Instance.populationManagers[1].generation + "</color>";
    }
}
