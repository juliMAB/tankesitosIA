using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private string extention;
    [SerializeField] private bool useEncryption;
    //private GameData gameData;

    public SimulationScreenMain simulationScreenMain = null;
    public GameObject MinePrefab = null;
    public PopulationManager[] populationManagers;
    public Color[] colors;
    float accumTime = 0;
    bool isRunning = false;
    public float GenerationDuration = 20.0f;
    public Vector3 SceneHalfExtents = new Vector3(20.0f, 0.0f, 20.0f);
    public int IterationCount = 1;

    public Action onStartSimulation;

    public FileDataHandler dataHandler;

    public int MinesCount = 50;

    

    public int TeamsCount { get { return populationManagers.Length; } }
    public float AccumTime { get { return accumTime; } }

    List<Mine> mines = new List<Mine>();
    #region SINGLETON
    static Main instance = null;

    public static Main Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Main>();

            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }
    #endregion


    #region UNITY_CALLS

    private void Start()
    {
        int c = populationManagers.Length;
        for (int i = 0; i < c; i++)
        {
            populationManagers[i].TeamID = i;
        }
        dataHandler = new FileDataHandler(Application.dataPath, fileName, extention, false);
    }

    private void FixedUpdate()
    {
        if (!isRunning)
            return;
        float dt = Time.fixedDeltaTime;
        int c = populationManagers.Length;
        for (int i = 0; i < c; i++)
        {
            for (int w = 0; w < Mathf.Clamp((float)(Main.Instance.IterationCount / 100.0f) * 50, 1, 50); w++)
            {
                populationManagers[i].LocalFixelUpdate(dt, SceneHalfExtents, mines, i);

                accumTime += dt;

                if (ScoreManager.Instance.getLoseTeam() == i && accumTime >= GenerationDuration)
                {
                    Debug.Log("end match");
                    accumTime -= GenerationDuration;
                    ScoreManager.Instance.lastLoser = i;
                    populationManagers[i].Epoc();
                    ScoreManager.Instance.lastLoser = i;
                    for (int z = 0; z < ScoreManager.Instance.score.Length; z++)
                        ScoreManager.Instance.score[z] = 0;
                    break;
                }
            }
        }
    }
    #endregion
    #region PRIVATE_METHODS
    Vector3 GetRandomPos()
    {
        return new Vector3(UnityEngine.Random.value * SceneHalfExtents.x * 2.0f - SceneHalfExtents.x, 0.0f, UnityEngine.Random.value * SceneHalfExtents.z * 2.0f - SceneHalfExtents.z);
    }
    //Quaternion GetRandomRot()
    //{
        //return Quaternion.AngleAxis(Random.value * 360.0f, Vector3.up);
    //}

    void SetMineColor(int id, Mine go)
    {
        go.GetComponent<Renderer>().material.color = colors[id];
    }
    void CreateMines()
    {
        // Destroy previous created mines
        DestroyMines();
        int midCount = MinesCount / populationManagers.Length;
        int deltaCount = 0;
        int teamID = 0;
        for (int i = 0; i < MinesCount; i++)
        {
            deltaCount++;
            if (deltaCount > midCount)
            {
                deltaCount = 0;
                teamID++;
            }
            Vector3 position = GetRandomPos();
            GameObject go = Instantiate<GameObject>(MinePrefab, position, Quaternion.identity);
            Mine mine = go.AddComponent<Mine>();
            mine.teamId = teamID;

            SetMineColor(teamID, mine);

            mines.Add(mine);
        }
    }
    void DestroyMines()
    {
        foreach (Mine go in mines)
            Destroy(go.gameObject);

        mines.Clear();
    }
    #endregion
    #region PUBLIC_METHODS
    public void RelocateMine(Mine mine)
    {
        mine.transform.position = GetRandomPos();
    }
    public void StartSimulation()
    {
        onStartSimulation?.Invoke();
        int c = populationManagers.Length;
        for (int i = 0; i < c; i++)
            populationManagers[i].StartSimulation();

        CreateMines();

        isRunning = true;
        accumTime = 0.0f;
        simulationScreenMain.gameObject.SetActive(true);
    }
    public void PauseSimulation()
    {
        isRunning = !isRunning;
    }
    public void StopSimulation()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //isRunning = false;
        //int c = populationManagers.Length;
        //for (int i = 0; i < c; i++)
        //    populationManagers[i].StopSimulation();

        // Destroy previous tanks (if there are any)
        //DestroyTanks();

        // Destroy all mines
        //DestroyMines();
    }
    #endregion
}
