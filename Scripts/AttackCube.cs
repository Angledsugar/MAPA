using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCube : MonoBehaviour
{
    AttackAgent Attack;
    void Start()
    {
        Attack = GetComponent<AttackAgent>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        // Debug.Log($"Detect!!!!!!!!!!!!!!");
        try
        {
            if (collision.gameObject.CompareTag("agent")) 
            {
                // Debug.Log($"Agent Attack");
                Attack.AddReward(1f);
                Attack.crawleragent.AddReward(100f);
                Attack.EndEpisode();
                // Attack.OnCollisionEnter(collision);
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.Log(ex);
        }

        
    }
}
