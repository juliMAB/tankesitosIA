using UnityEngine;
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

    public bool load = false;

    //public FileDataHandler dataHandler;

    GeneticAlgorithm genAlg;

    List<Tank> populationGOs = new List<Tank>();
    List<Genome> population = new List<Genome>();
    List<Genome> savePopulation = new List<Genome>();
    List<NeuralNetwork> brains = new List<NeuralNetwork>();

    public void LoadData()
    {
        string TeamData = "TeamData" + TeamID.ToString();
        GameData data = new GameData();
        data = Main.Instance.dataHandler.Load(TeamID.ToString());
        generation         = data.generationNumber    ;
        PopulationCount    = data.PopulationCount     ;
        EliteCount         = data.EliteCount          ;
        MutationChance     = data.MutationChance      ;
        MutationRate       = data.MutationRate        ;
        InputsCount        = data.InputsCount         ;
        HiddenLayers       = data.HiddenLayers        ;
        OutputsCount       = data.OutputsCount        ;
        NeuronsCountPerHL  = data.NeuronsCountPerHL   ;
        Bias               = data.Bias                ;
        P                  = data.P                   ;

        savePopulation     = data.genomes             ;

        load = true;
    }

    public void SaveData()
    {
        string TeamData = "TeamData" + TeamID.ToString();
        GameData data = new GameData();
        data.generationNumber   = generation            ;
        data.PopulationCount    = PopulationCount       ;
        data.EliteCount         = EliteCount            ;
        data.MutationChance     = MutationChance        ;
        data.MutationRate       = MutationRate          ;
        data.InputsCount        = InputsCount           ;
        data.HiddenLayers       = HiddenLayers          ;
        data.OutputsCount       = OutputsCount          ;
        data.NeuronsCountPerHL  = NeuronsCountPerHL     ;
        data.Bias               = Bias                  ;
        data.P                  = P                     ;

        data.genomes = population                       ;
        data.brains =  brains                           ;
        data.populationGOs = populationGOs              ;

        Main.Instance.dataHandler.Save(data,TeamID.ToString());
    }

    public int generation = 0;

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

    public void StartSimulation()
    {
        // Create and confiugre the Genetic Algorithm
        genAlg = new GeneticAlgorithm(EliteCount, MutationChance, MutationRate);


        
        GenerateInitialPopulation();
    }

    // Generate the random initial population
    void GenerateInitialPopulation()
    {
        // Destroy previous tanks (if there are any)

        DestroyTanks();

        if (load)
        {
            for (int i = 0; i < PopulationCount; i++)
            {

                NeuralNetwork brain = CreateBrain();

                Genome genome = savePopulation[i];
                //Genome genome = new Genome(brain.GetTotalWeightsCount());

                brain.SetWeights(genome.genome);
                brains.Add(brain);

                population.Add(genome);

                populationGOs.Add(CreateTank(genome, brain, TeamID));
            }
        
            //load = false;
            return;
        }

        
        generation = 0;

        
        
        for (int i = 0; i < PopulationCount; i++)
        {
            NeuralNetwork brain = CreateBrain();
            
            Genome genome = new Genome(brain.GetTotalWeightsCount());

            brain.SetWeights(genome.genome);
            brains.Add(brain);

            population.Add(genome);

            populationGOs.Add(CreateTank(genome, brain,TeamID));
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

    void DestroyTanks()
    {
        foreach (Tank go in populationGOs)
            Destroy(go.gameObject);

        populationGOs.Clear();
        population.Clear();
        brains.Clear();
    }

    Vector3 GetRandomPos()
    {
        Vector3 SceneHalfExtents = Main.Instance.SceneHalfExtents;
        return new Vector3(UnityEngine.Random.value * SceneHalfExtents.x * 2.0f - SceneHalfExtents.x, 0.0f, UnityEngine.Random.value * SceneHalfExtents.z * 2.0f - SceneHalfExtents.z); 
    }

    Quaternion GetRandomRot()
    {
        return Quaternion.AngleAxis(UnityEngine.Random.value * 360.0f, Vector3.up);
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

    //public void LoadData(GameData data, string id)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void SaveData(GameData data, string id)
    //{
    //    var fields = typeof(GameData).GetFields();
    //    foreach (var field in fields)
    //    {
    //        if (field.GetType() == typeof(SerializableDictionary<string, int>))
    //        {
    //            SerializableDictionary<string, int> a = (SerializableDictionary<string, int>)field.GetValue(null);
    //            if (a.ContainsKey(id))
    //                a.Remove(id);
    //        }
    //        else if (field.GetType() == typeof(SerializableDictionary<string, float>))
    //        {
    //            SerializableDictionary<string, int> a = (SerializableDictionary<string, int>)field.GetValue(null);
    //            if (a.ContainsKey(id))
    //                a.Remove(id);
    //        }
    //        else if (field.GetType() == typeof(SerializableDictionary<string, List<Genome>>))
    //        {
    //            SerializableDictionary<string, List<Genome>> a = (SerializableDictionary<string, List<Genome>>)field.GetValue(null);
    //            if (a.ContainsKey(id))
    //                a.Remove(id);
    //        }
    //        else if (field.GetType() == typeof(SerializableDictionary<string, List<NeuralNetwork>>))
    //        {
    //            SerializableDictionary<string, List<NeuralNetwork>> a = (SerializableDictionary<string, List<NeuralNetwork>>)field.GetValue(null);
    //            if (a.ContainsKey(id))
    //                a.Remove(id);
    //        }
    //    }
    //    data.generationNumber.Add(id,generation);
    //    data.PopulationCount.Add(id, PopulationCount);
    //    data.EliteCount.Add(id, EliteCount);
    //    data.MutationChance.Add(id, MutationChance);
    //    data.MutationRate.Add(id, MutationRate);
    //    data.InputsCount.Add(id, InputsCount);
    //    data.HiddenLayers.Add(id, HiddenLayers);
    //    data.OutputsCount.Add(id, OutputsCount);
    //    data.NeuronsCountPerHL.Add(id, NeuronsCountPerHL);
    //    data.Bias.Add(id, Bias);
    //    data.P.Add(id, P);
    //
    //    data.genomes.Add(id, population);
    //    data.brains.Add(id, brains);
    //
    //    //SaveSystem.Save(data, "Team" + TeamID);
    //    //throw new System.NotImplementedException();
    //}



    #endregion

}
