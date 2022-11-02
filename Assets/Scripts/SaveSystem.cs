using System.Collections.Generic;
using UnityEngine;

public class SaveSystem
{
    public static void Save(GameData data,string id)
    {

        PlayerPrefs.SetString(id,JsonUtility.ToJson(data));
    }
    public static GameData Load(string id)
    {
        string json = PlayerPrefs.GetString(id);
        GameData data = new GameData();
        data = JsonUtility.FromJson<GameData>(json);
        return data;
    }
}

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
}
