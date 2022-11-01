using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int[] score;

    public int lastLoser; 
    #region SINGLETON
    static ScoreManager instance = null;

    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ScoreManager>();

            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }
    #endregion

    private void Start()
    {
        score = new int[Main.Instance.TeamsCount];
    }
    public int getTeamMoreScore()
    {
        int indx=-1;
        int maxVal = 0;
        for (int i = 0; i < score.Length; i++)
        {
            if (score[i] > maxVal)
            {
                maxVal = score[i];
                indx = i;
            }
        }
        return indx;
    }
    public int getLoseTeam()
    {
        int indx = -1;
        int maxVal = 9999999;
        for (int i = 0; i < score.Length; i++)
        {
            if (score[i] < maxVal)
            {
                maxVal = score[i];
                indx = i;
            }
        }
        return indx;
    }
}
