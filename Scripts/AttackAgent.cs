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
    // public Transform[] childAttakcube;

    public GameObject[] agents;
    public int agentsLength = 0;

    public Agent crawleragent;

    public Boolean DestroyCube = true;

    public int cubechecker = 0;

    public override void Initialize()
    {
        agents = GameObject.FindGameObjectsWithTag("Crawler");
        agentsLength = agents.Length;
        DestroyChilds(0);
    }
    public void Update()
    {
        DestroyChilds(0);
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
        
        var attack_x = Mathf.Clamp(attackcubePosition[0]*50, -25f, 25f);
        var attack_z = Mathf.Clamp(attackcubePosition[1]*50, -25f, 25f);

        // Debug.Log($"attack_x[0] : {attack_x} | attack_z[1] : {attack_z}");
        // Debug.Log($"discreteActionsOut[0] : {attackcubeDestroy[0]} |  [1] : {attackcubeDestroy[1]}");
        if(attackcubeDestroy[0] == 1)
        {
            // Debug.Log($"Destory Cube!");
            // AddReward(-1f);
            // DestroyChilds(1);  
        }
        if(attackcubeDestroy[1] == 1 && cubechecker <= 10)
        {
            Debug.Log($"Instantiate Cube!");
            GameObject a_cube = Instantiate(attackcube, 
                new Vector3(attack_x, 1f, attack_z) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            a_cube.transform.SetParent(this.transform, false);
            cubechecker ++;
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

    public void DestroyChilds(int actions)
    {
        var childAttakcube = this.GetComponentsInChildren<Transform>();
        if(actions == 1)
        {
            for(int i = 1; i < childAttakcube.Length; i++)
            {
                if(childAttakcube[i] != transform)
                    GameObject.Destroy(childAttakcube[i].gameObject);
            }
        }
        else if(childAttakcube != null && childAttakcube.Length >= agentsLength + 2)
        {
            Debug.Log("Sdasdasd");
            AddReward(-1f);
            for(int i = agentsLength + 1 ; i < childAttakcube.Length; i++)
            {
                if(childAttakcube[i] != transform)
                    GameObject.Destroy(childAttakcube[i].gameObject);
            }
            // foreach (var iter in child)
            // {
            //     if(iter != this.transform)
            //     {
            //         // Debug.Log("Find Error");
            //         Destroy(iter.gameObject);
            //     }
            // }
            cubechecker = 0;
        }
        if(childAttakcube.Length == 1 && cubechecker > 10) cubechecker = 0;
    }
}
