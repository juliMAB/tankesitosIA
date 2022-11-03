using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class TankBase : MonoBehaviour
{
    public int teamID = -1;
    public float Speed = 10.0f;
    public float RotSpeed = 20.0f;

    [SerializeField] protected Genome genome;
	[SerializeField] protected NeuralNetwork brain;
    [SerializeField] protected Mine nearMine;
    [SerializeField] protected Mine goodMine;
    [SerializeField] protected Mine badMine;
    [SerializeField] protected float[] inputs;
    //public void Copy(Tank copy)
    //{
    //    teamID = copy.teamID;
    //    Speed = copy.Speed;
    //    RotSpeed = copy.RotSpeed;
    //
    //    genome = copy.genome;
    //    brain = copy.brain;
    //    nearMine = null;
    //    goodMine = null;
    //    badMine = null;
    //    inputs = copy.inputs;
    //}
    public TankBase(TankBase copy)
    {
        teamID = copy.teamID;
        Speed = copy.Speed;
        RotSpeed = copy.RotSpeed;

        genome = copy.genome;
        brain = copy.brain;
        nearMine = copy.nearMine;
        goodMine = copy.goodMine;
        badMine = copy.badMine;
        inputs = copy.inputs;
    }

    public void SetBrain(Genome genome, NeuralNetwork brain)
    {
        this.genome = genome;
        this.brain = brain;
        inputs = new float[brain.InputsCount];
        OnReset();
    }

    public void SetNearestMine(Mine mine)
    {
        nearMine = mine;
    }

    public void SetGoodNearestMine(Mine mine)
    {
        goodMine = mine;
    }

    public void SetBadNearestMine(Mine mine)
    {
        badMine = mine;
    }

    protected bool IsGoodMine(GameObject mine)
    {
        return goodMine == mine;
    }

    protected Vector3 GetDirToMine(Mine mine)
    {
        return (mine.transform.position - this.transform.position).normalized;
    }
    
    protected bool IsCloseToMine(Mine mine)
    {
        return (this.transform.position - nearMine.transform.position).sqrMagnitude <= 2.0f;
    }

    protected void SetForces(float leftForce, float rightForce, float dt)
    {
        Vector3 pos = this.transform.position;
        float rotFactor = Mathf.Clamp((rightForce - leftForce), -1.0f, 1.0f);
        this.transform.rotation *= Quaternion.AngleAxis(rotFactor * RotSpeed * dt, Vector3.up);
        pos += this.transform.forward * Mathf.Abs(rightForce + leftForce) * 0.5f * Speed * dt;
        this.transform.position = pos;
    }

	public void Think(float dt) 
	{
        OnThink(dt);

        if(IsCloseToMine(nearMine))
        {
            OnTakeMine(nearMine, teamID);
            Main.Instance.RelocateMine(nearMine);
        }
	}

    protected virtual void OnThink(float dt)
    {

    }

    protected virtual void OnTakeMine(Mine mine,int teamID)
    {
    }

    protected virtual void OnReset()
    {

    }
}
