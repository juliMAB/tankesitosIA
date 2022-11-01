using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    void Save(GameData data,string id)
    {

        PlayerPrefs.SetString(id,JsonUtility.ToJson(data));
    }
    void Load(string id)
    {
        string json = PlayerPrefs.GetString(id);
        GameData data = new GameData();
        data = JsonUtility.FromJson<GameData>(json);
    }
}

public class GameData
{
    List<Genome> population;
    List<NeuralNetwork> brains;
}
