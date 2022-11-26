using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System;
using System.Linq;

public class JunwonAgent : Agent
{

    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        foreach (Flag flag in GameManager.Gmr.flags)
        {
            sensor.AddObservation(flag.visited);
        }
        foreach (Flag flag in GameManager.Gmr.flags)
        {
            sensor.AddObservation((int)(flag.time / GameManager.Gmr.maxdist) * 15 + 5);
        }
        foreach (Flag flag in GameManager.Gmr.flags)
        {
            sensor.AddObservation(flag.dist);
        }

    }

    public override void Initialize()
    {

    }

    public override void OnEpisodeBegin()
    {

    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var action = actionBuffers.DiscreteActions[0];
        GameManager.Gmr.recommend(action, 1, true);

    }
}
