using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopulationManager : MonoBehaviour
{
    public int TeamID;

    public GameObject TankPrefab;

    public int PopulationCount = 40;

    public int EliteCount = 4;
    public float MutationChance = 0.10f;
    public float MutationRate = 0.01f;

    public int InputsCount = 4;
    public int HiddenLayers = 1;
    public int OutputsCount = 2;
    public int NeuronsCountPerHL = 7;
    public float Bias = 1f;
    public float P = 0.5f;


    GeneticAlgorithm genAlg;

    List<Tank> populationGOs = new List<Tank>();
    List<Genome> population = new List<Genome>();
    List<NeuralNetwork> brains = new List<NeuralNetwork>();

    public void LoadData()
    {
        GameData data = SaveSystem.Load("Team" + TeamID);
        generation          = data.generationNumber;
        PopulationCount     = data.PopulationCount;
        EliteCount          = data.EliteCount;
        MutationChance      = data.MutationChance;
        MutationRate        = data.MutationRate;
        InputsCount         = data.InputsCount;
        HiddenLayers        = data.HiddenLayers;
        OutputsCount        = data.OutputsCount;
        NeuronsCountPerHL   = data.NeuronsCountPerHL;
        Bias                = data.Bias;
        P                   = data.P;

        population          = data.genomes;
        brains              = data.brains;



    }

    public void SaveData()
    {
        GameData data = new GameData();
        data.generationNumber = generation;
        data.PopulationCount = PopulationCount;
        data.EliteCount = EliteCount;
        data.MutationChance = MutationChance;
        data.MutationRate = MutationRate;
        data.InputsCount = InputsCount;
        data.HiddenLayers = HiddenLayers;
        data.OutputsCount = OutputsCount;
        data.NeuronsCountPerHL = NeuronsCountPerHL;
        data.Bias = Bias;
        data.P = P;

        data.genomes = population;
        data.brains = brains;
        SaveSystem.Save(data, "Team" + TeamID);
    }

    public int generation {
        get; private set;
    }

    public float bestFitness 
    {
        get; private set;
    }

    public float avgFitness 
    {
        get; private set;
    }

    public float worstFitness 
    {
        get; private set;
    }

    private float getBestFitness()
    {
        float fitness = 0;
        foreach(Genome g in population)
        {
            if (fitness < g.fitness)
                fitness = g.fitness;
        }

        return fitness;
    }

    private float getAvgFitness()
    {
        float fitness = 0;
        foreach(Genome g in population)
        {
            fitness += g.fitness;
        }

        return fitness / population.Count;
    }

    private float getWorstFitness()
    {
        float fitness = float.MaxValue;
        foreach(Genome g in population)
        {
            if (fitness > g.fitness)
                fitness = g.fitness;
        }

        return fitness;
    }

    //static PopulationManager instance = null;
    //
    //public static PopulationManager Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //            instance = FindObjectOfType<PopulationManager>();
    //
    //        return instance;
    //    }
    //}
    //
    //void Awake()
    //{
    //    instance = this;
    //}

    void Start()
    {
    }

    public void StartSimulation(int teamID)
    {
        // Create and confiugre the Genetic Algorithm
        genAlg = new GeneticAlgorithm(EliteCount, MutationChance, MutationRate);

        GenerateInitialPopulation(teamID);
        //CreateMines();

        //isRunning = true;
    }

    //public void PauseSimulation()
    //{
    //    isRunning = !isRunning;
    //}

    public void StopSimulation()
    {
        //isRunning = false;

        generation = 0;

        // Destroy previous tanks (if there are any)
        DestroyTanks();

        // Destroy all mines
        //DestroyMines();
    }

    // Generate the random initial population
    void GenerateInitialPopulation(int teamID)
    {
        generation = 0;

        // Destroy previous tanks (if there are any)
        DestroyTanks();
        
        for (int i = 0; i < PopulationCount; i++)
        {
            NeuralNetwork brain = CreateBrain();
            
            Genome genome = new Genome(brain.GetTotalWeightsCount());

            brain.SetWeights(genome.genome);
            brains.Add(brain);

            population.Add(genome);
            populationGOs.Add(CreateTank(genome, brain,teamID));
        }
    }

    // Creates a new NeuralNetwork
    NeuralNetwork CreateBrain()
    {
        NeuralNetwork brain = new NeuralNetwork();

        // Add first neuron layer that has as many neurons as inputs
        brain.AddFirstNeuronLayer(InputsCount, Bias, P);

        for (int i = 0; i < HiddenLayers; i++)
        {
            // Add each hidden layer with custom neurons count
            brain.AddNeuronLayer(NeuronsCountPerHL, Bias, P);
        }

        // Add the output layer with as many neurons as outputs
        brain.AddNeuronLayer(OutputsCount, Bias, P);

        return brain;
    }

    // Evolve!!!
    public void Epoc()
    //void Epoch()
    {
        // Increment generation counter
        generation++;

        // Calculate best, average and worst fitness
        bestFitness = getBestFitness();
        avgFitness = getAvgFitness();
        worstFitness = getWorstFitness();

        // Evolve each genome and create a new array of genomes
        Genome[] newGenomes = genAlg.Epoch(population.ToArray());

        // Clear current population
        population.Clear();

        // Add new population
        population.AddRange(newGenomes);

        // Set the new genomes as each NeuralNetwork weights
        for (int i = 0; i < PopulationCount; i++)
        {
            NeuralNetwork brain = brains[i];

            brain.SetWeights(newGenomes[i].genome);

            populationGOs[i].SetBrain(newGenomes[i], brain);
            populationGOs[i].transform.position = GetRandomPos();
            populationGOs[i].transform.rotation = GetRandomRot();
        }
    }

    // Update is called once per frame
    public void LocalFixelUpdate(float dt,Vector3 SceneHalfExtents,List<Mine> mines,int teamID)
    //void FixedUpdate () 
	{
       // if (!isRunning)
       //     return;
        
        

        //for (int i = 0; i < Mathf.Clamp((float)(Main.Instance.IterationCount / 100.0f) * 50, 1, 50); i++)
        {
            foreach (Tank t in populationGOs)
            {
                // Get the nearest mine
                Mine mine = GetNearestMine(t.transform.position,mines);

                // Set the nearest mine to current tank
                t.SetNearestMine(mine);

                mine = GetNearestGoodMine(t.transform.position,mines, teamID);

                // Set the nearest mine to current tank
                t.SetGoodNearestMine(mine);

                mine = GetNearestBadMine(t.transform.position,mines, teamID);

                // Set the nearest mine to current tank
                t.SetBadNearestMine(mine);

                // Think!! 
                t.Think(dt);

                // Just adjust tank position when reaching world extents
                Vector3 pos = t.transform.position;
                if (pos.x > SceneHalfExtents.x)
                    pos.x -= SceneHalfExtents.x * 2;
                else if (pos.x < -SceneHalfExtents.x)
                    pos.x += SceneHalfExtents.x * 2;

                if (pos.z > SceneHalfExtents.z)
                    pos.z -= SceneHalfExtents.z * 2;
                else if (pos.z < -SceneHalfExtents.z)
                    pos.z += SceneHalfExtents.z * 2;

                // Set tank position
                t.transform.position = pos;
            }

            // Check the time to evolve
            //accumTime += dt;
            //if (accumTime >= GenerationDuration)
            //{
            //    accumTime -= GenerationDuration;
            //    //Epoch();
            //    return true;
            //    //break;
            //}
        }
	}

#region Helpers
    Tank CreateTank(Genome genome, NeuralNetwork brain,int teamID)
    {
        Vector3 position = GetRandomPos();
        GameObject go = Instantiate<GameObject>(TankPrefab, position, GetRandomRot());
        Tank t = go.GetComponent<Tank>();
        t.teamID = teamID;
        t.SetBrain(genome, brain);
        return t;
    }

    //void DestroyMines()
    //{
    //    foreach (GameObject go in mines)
    //        Destroy(go);
    //
    //    mines.Clear();
    //    goodMines.Clear();
    //    badMines.Clear();
    //}

    void DestroyTanks()
    {
        foreach (Tank go in populationGOs)
            Destroy(go.gameObject);

        populationGOs.Clear();
        population.Clear();
        brains.Clear();
    }

    //void CreateMines()
    //{
    //    // Destroy previous created mines
    //    DestroyMines();
    //
    //    for (int i = 0; i < MinesCount; i++)
    //    {
    //        Vector3 position = GetRandomPos();
    //        GameObject go = Instantiate<GameObject>(MinePrefab, position, Quaternion.identity);
    //
    //        bool good = Random.Range(-1.0f, 1.0f) >= 0;
    //
    //        SetMineGood(good, go);
    //
    //        mines.Add(go);
    //    }
    //}

    //void SetMineGood(bool good, GameObject go)
    //{
    //    if (good)
    //    {
    //        go.GetComponent<Renderer>().material.color = Color.green;
    //        goodMines.Add(go);
    //    }
    //    else
    //    {
    //        go.GetComponent<Renderer>().material.color = Color.red;
    //        badMines.Add(go);
    //    }
    //}

    //public void RelocateMine(GameObject mine)
    //{
    //    if (goodMines.Contains(mine))
    //        goodMines.Remove(mine);
    //    else
    //        badMines.Remove(mine);
    //
    //    bool good = Random.Range(-1.0f, 1.0f) >= 0;
    //
    //    SetMineGood(good, mine);
    //
    //    mine.transform.position = GetRandomPos();
    //}

    Vector3 GetRandomPos()
    {
        Vector3 SceneHalfExtents = Main.Instance.SceneHalfExtents;
        return new Vector3(Random.value * SceneHalfExtents.x * 2.0f - SceneHalfExtents.x, 0.0f, Random.value * SceneHalfExtents.z * 2.0f - SceneHalfExtents.z); 
    }

    Quaternion GetRandomRot()
    {
        return Quaternion.AngleAxis(Random.value * 360.0f, Vector3.up);
    }

    Mine GetNearestMine(Vector3 pos, List<Mine> mines)
    {
        Mine nearest = mines[0];
        float distance = (pos - nearest.transform.position).sqrMagnitude;

        foreach (Mine go in mines)
        {
            float newDist = (go.transform.position - pos).sqrMagnitude;
            if (newDist < distance)
            {
                nearest = go;
                distance = newDist;
            }
        }

        return nearest;
    }

    Mine GetNearestGoodMine(Vector3 pos,List<Mine> mines,int teamID)
    {
        Mine nearest = mines[0];
        float distance = (pos - nearest.transform.position).sqrMagnitude;

        foreach (Mine go in mines)
        {
            if(go.teamId!= teamID)
                continue;
            float newDist = (go.transform.position - pos).sqrMagnitude;
            if (newDist < distance)
            {
                nearest = go;
                distance = newDist;
            }
        }

        return nearest;
    }

    Mine GetNearestBadMine(Vector3 pos, List<Mine> mines,int teamID)
    {
        Mine nearest = mines[0];
        float distance = (pos - nearest.transform.position).sqrMagnitude;

        foreach (Mine go in mines)
        {
            if (go.teamId == teamID)
                continue;
            float newDist = (go.transform.position - pos).sqrMagnitude;
            if (newDist < distance)
            {
                nearest = go;
                distance = newDist;
            }
        }

        return nearest;
    }

    #endregion

}
