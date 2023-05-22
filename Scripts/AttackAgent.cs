using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class AttackAgent : Agent
{
    public GameObject attackcube;
    public Transform[] childAttakcube;

    public GameObject[] agents;
    public int agentsLength = 0;

    public override void Initialize()
    {
        agents = GameObject.FindGameObjectsWithTag("Crawler");
        agentsLength = agents.Length;
        GameObject a_cube = Instantiate(attackcube, 
                new Vector3(0f, 10f, 0f) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f))) as GameObject;
        a_cube.transform.SetParent(this.transform, false);
    }

    public override void OnEpisodeBegin()
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        for(int i = 0; i < agentsLength; i++)
        {
            sensor.AddObservation(agents[i].transform.position);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var attackcubeDestroy = actionBuffers.DiscreteActions;
        var attackcubePosition = actionBuffers.ContinuousActions;
        
        Debug.Log(attackcubeDestroy);
        
        if(attackcubeDestroy[0] == 1)
        {
            DestroyChilds();    
        }
        else
        {
            GameObject a_cube = Instantiate(attackcube, 
                new Vector3(0f, 1f, 0f) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f))) as GameObject;
            a_cube.transform.SetParent(this.transform, false);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (Input.GetKey(KeyCode.D))
        {
            continuousActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            continuousActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            continuousActionsOut[1] = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            continuousActionsOut[0] = -1;
        }
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 0 : 1;
        Debug.Log(continuousActionsOut);
        Debug.Log(discreteActionsOut);
    }


    public void DestroyChilds()
    {
        var child = this.GetComponentsInChildren<Transform>();
        foreach (var iter in child)
        {
            if(iter != this.transform)
            {
                Destroy(iter.gameObject);
            }
        }
    }

            
}
