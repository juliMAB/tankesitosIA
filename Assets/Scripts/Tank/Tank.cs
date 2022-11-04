using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Tank : TankBase
{
    [SerializeField] float fitness = 1;

    public Tank(Tank copy) : base(copy)
    {
        fitness = copy.fitness;
    }
    //public void Copy(Tank copy)
    //{
    //    base.Copy(copy);
    //    fitness = copy.fitness;
    //}
    protected override void OnReset()
    {
        fitness = 1;
    }

    protected override void OnThink(float dt) 
	{
        Vector3 dirToMine = GetDirToMine(nearMine);
        Vector3 dir = this.transform.forward;

        inputs[0] = dirToMine.x;
        inputs[1] = dirToMine.z;
        inputs[2] = dir.x;
        inputs[3] = dir.z;

        float[] output = brain.Synapsis(inputs);

        SetForces(output[0], output[1], dt);
	}
    
    protected override void OnTakeMine(Mine mine,int teamID)
    {
        int addScore = 0;
        float multyply = 0;
        if (mine.teamId==teamID)
        {
            addScore = 1;
            multyply = 1;
        }
        ScoreManager.Instance.score[teamID] += addScore;
        fitness += multyply;
        genome.fitness = fitness;
    }
}
