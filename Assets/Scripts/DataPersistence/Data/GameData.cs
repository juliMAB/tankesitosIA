using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int generationNumber;
    public int PopulationCount;
    public int EliteCount;
    public float MutationChance;
    public float MutationRate;
    public int InputsCount;
    public int HiddenLayers;
    public int OutputsCount;
    public int NeuronsCountPerHL;
    public float Bias;
    public float P;
    public List<Genome> genomes;
    public List<NeuralNetwork> brains;
    public GameData() 
    {
        genomes = new List<Genome>();
        brains =  new List<NeuralNetwork>();
    }
}
