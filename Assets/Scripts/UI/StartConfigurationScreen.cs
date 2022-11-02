using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartConfigurationScreen : MonoBehaviour
{
    public PopulationManager populationManager;

    public Text populationCountTxt;
    public Slider populationCountSlider;
    public Text minesCountTxt;
    public Slider minesCountSlider;
    public Text generationDurationTxt;
    public Slider generationDurationSlider;
    public Text eliteCountTxt;
    public Slider eliteCountSlider;
    public Text mutationChanceTxt;
    public Slider mutationChanceSlider;
    public Text mutationRateTxt;
    public Slider mutationRateSlider;
    public Text hiddenLayersCountTxt;
    public Slider hiddenLayersCountSlider;
    public Text neuronsPerHLCountTxt;
    public Slider neuronsPerHLSlider;
    public Text biasTxt;
    public Slider biasSlider;
    public Text sigmoidSlopeTxt;
    public Slider sigmoidSlopeSlider;
    public Button startButton;
    public Button LoadButton;
    public GameObject simulationScreen;
    
    string populationText;
    string minesText;
    string generationDurationText;
    string elitesText;
    string mutationChanceText;
    string mutationRateText;
    string hiddenLayersCountText;
    string biasText;
    string sigmoidSlopeText;
    string neuronsPerHLCountText;

    void Start()
    {   
        populationCountSlider.onValueChanged.AddListener(OnPopulationCountChange);
        minesCountSlider.onValueChanged.AddListener(OnMinesCountChange);
        generationDurationSlider.onValueChanged.AddListener(OnGenerationDurationChange);
        eliteCountSlider.onValueChanged.AddListener(OnEliteCountChange);
        mutationChanceSlider.onValueChanged.AddListener(OnMutationChanceChange);
        mutationRateSlider.onValueChanged.AddListener(OnMutationRateChange);
        hiddenLayersCountSlider.onValueChanged.AddListener(OnHiddenLayersCountChange);
        neuronsPerHLSlider.onValueChanged.AddListener(OnNeuronsPerHLChange);
        biasSlider.onValueChanged.AddListener(OnBiasChange);
        sigmoidSlopeSlider.onValueChanged.AddListener(OnSigmoidSlopeChange);

        populationText = populationCountTxt.text;
        minesText = minesCountTxt.text;
        generationDurationText = generationDurationTxt.text;
        elitesText = eliteCountTxt.text;
        mutationChanceText = mutationChanceTxt.text;
        mutationRateText = mutationRateTxt.text;
        hiddenLayersCountText = hiddenLayersCountTxt.text;
        neuronsPerHLCountText = neuronsPerHLCountTxt.text;
        biasText = biasTxt.text;
        sigmoidSlopeText = sigmoidSlopeTxt.text;

        populationCountSlider.value =       populationManager.PopulationCount;
        minesCountSlider.value =            Main.Instance.MinesCount;
        generationDurationSlider.value =    Main.Instance.GenerationDuration;
        eliteCountSlider.value =            populationManager.EliteCount;
        mutationChanceSlider.value =        populationManager.MutationChance * 100.0f;
        mutationRateSlider.value =          populationManager.MutationRate * 100.0f;
        hiddenLayersCountSlider.value =     populationManager.HiddenLayers;
        neuronsPerHLSlider.value =          populationManager.NeuronsCountPerHL;
        biasSlider.value = -                populationManager.Bias;
        sigmoidSlopeSlider.value =          populationManager.P;

        startButton.onClick.AddListener(OnStartButtonClick);

        Main.Instance.onStartSimulation += StartSimulationUI;
        LoadButton.onClick.AddListener(LoadData);
    }
    void LoadData()
    {
        populationManager.LoadData();
    }
    void OnPopulationCountChange(float value)
    {
        populationManager.PopulationCount = (int)value;
        
        populationCountTxt.text = string.Format(populationText, populationManager.PopulationCount);
    }

    void OnMinesCountChange(float value)
    {
        Main.Instance.MinesCount = (int)value;        

        minesCountTxt.text = string.Format(minesText, Main.Instance.MinesCount);
    }

    void OnGenerationDurationChange(float value)
    {
        Main.Instance.GenerationDuration = (int)value;
        
        generationDurationTxt.text = string.Format(generationDurationText, Main.Instance.GenerationDuration);
    }

    void OnEliteCountChange(float value)
    {
        populationManager.EliteCount = (int)value;

        eliteCountTxt.text = string.Format(elitesText, populationManager.EliteCount);
    }

    void OnMutationChanceChange(float value)
    {
        populationManager.MutationChance = value / 100.0f;

        mutationChanceTxt.text = string.Format(mutationChanceText, (int)(populationManager.MutationChance * 100));
    }

    void OnMutationRateChange(float value)
    {
        populationManager.MutationRate = value / 100.0f;

        mutationRateTxt.text = string.Format(mutationRateText, (int)(populationManager.MutationRate * 100));
    }

    void OnHiddenLayersCountChange(float value)
    {
        populationManager.HiddenLayers = (int)value;
        

        hiddenLayersCountTxt.text = string.Format(hiddenLayersCountText, populationManager.HiddenLayers);
    }

    void OnNeuronsPerHLChange(float value)
    {
        populationManager.NeuronsCountPerHL = (int)value;

        neuronsPerHLCountTxt.text = string.Format(neuronsPerHLCountText, populationManager.NeuronsCountPerHL);
    }

    void OnBiasChange(float value)
    {
        populationManager.Bias = -value;

        biasTxt.text = string.Format(biasText, populationManager.Bias.ToString("0.00"));
    }

    void OnSigmoidSlopeChange(float value)
    {
        populationManager.P = value;

        sigmoidSlopeTxt.text = string.Format(sigmoidSlopeText, populationManager.P.ToString("0.00"));
    }


    void OnStartButtonClick()
    {
        Main.Instance.StartSimulation();
    }
    void StartSimulationUI()
    {
        this.gameObject.SetActive(false);
        simulationScreen.SetActive(true);
    }
    
}
