using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

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

    public Agent crawleragent;

    public Boolean DestroyCube = true;

    public override void Initialize()
    {
        agents = GameObject.FindGameObjectsWithTag("Crawler");
        agentsLength = agents.Length;
    }
    public void Update()
    {
        DestroyChilds();
    }

    public override void OnEpisodeBegin()
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        for(int i = 0; i < agentsLength; i++)
        {
            sensor.AddObservation(agents[i].transform.position);
            // sensor.AddObservation(agents[i].GetComponent<CrawlerAgent>().m_Target.transform.position);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var attackcubeDestroy = actionBuffers.DiscreteActions;
        var attackcubePosition = actionBuffers.ContinuousActions;
        
        var attack_x = Mathf.Clamp(attackcubePosition[0] * 100, -45f, 45f);
        var attack_z = Mathf.Clamp(attackcubePosition[1] * 100, -45f, 45f);

        // Debug.Log($"attack_x[0] : {attack_x} | attack_z[1] : {attack_z}");
        // Debug.Log($"discreteActionsOut[0] : {attackcubeDestroy[0]} |  [1] : {attackcubeDestroy[1]}");
        if(attackcubeDestroy[0] == 1)
        {
            // Debug.Log($"Destory Cube!");
            // AddReward(-1f);
            DestroyChilds();  
        }
        if(attackcubeDestroy[1] == 1)
        {
            // Debug.Log($"Instantiate Cube!");
            GameObject a_cube = Instantiate(attackcube, 
                new Vector3(attack_x, 1f, attack_z) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f))) as GameObject;
            a_cube.transform.SetParent(this.transform, false);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetKey(KeyCode.W) ?  1 : 0;
        continuousActionsOut[0] = Input.GetKey(KeyCode.S) ? -1 : 0;
        continuousActionsOut[1] = Input.GetKey(KeyCode.D) ?  1 : 0;
        continuousActionsOut[1] = Input.GetKey(KeyCode.A) ?  -1 : 0;

        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Q) ? 1 : 0;
        discreteActionsOut[1] = Input.GetKey(KeyCode.Space) ? 1 : 0;

        // Debug.Log($"continuousActionsOut[0] : {continuousActionsOut[0]} | [1] : {continuousActionsOut[1]}");
        // Debug.Log($"discreteActionsOut[0] : {discreteActionsOut[0]} |  [1] : {discreteActionsOut[1]}");
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        // AddReward(1f);
        // crawleragent.AddReward(100f);
        // Debug.Log("Success Attack");
        // EndEpisode();
    }

    public void DestroyChilds()
    {
        var child = this.GetComponentsInChildren<Transform>();
        if(child.Length >= 6)
        {
            AddReward(-1f);
            foreach (var iter in child)
            {
                if(iter != this.transform)
                {
                    // Debug.Log("Find Error");
                    Destroy(iter.gameObject);
                }
            }
        }
        else if(child.Length >= 2 && DestroyCube)
        {
            Destroy(child[1].gameObject);
        }

    }

            
}
