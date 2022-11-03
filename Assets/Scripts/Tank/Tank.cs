using UnityEngine;
using System.Collections;

public class Tank : TankBase
{
    float fitness = 0;
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
        float multyply = 2;
        if (mine.teamId==teamID)
        {
            addScore = 1;
            multyply = 4;
        }
        ScoreManager.Instance.score[teamID] += addScore;
        fitness += multyply;
        genome.fitness = fitness;
    }
}
