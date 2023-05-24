using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCube : MonoBehaviour
{
    AttackAgent Attack;
    void Start()
    {
        Attack = this.GetComponentInParent<AttackAgent>();
    }

    void Update()
    {
        if(this.transform.position.y < 0) 
            this.transform.position = new Vector3(this.transform.position.x,
                                                  1.1f,
                                                  this.transform.position.z);
    }

    public void OnCollisionEnter(Collision collision)
    {
        // Debug.Log($"Detect!!!!!!!!!!!!!!");
        try
        {
            if (collision.gameObject.CompareTag("agent")) 
            {
                Attack.AddReward(1f);
                Attack.crawleragent.AddReward(100f);
                Attack.EndEpisode();
                GameObject.Destroy(this.gameObject);
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.Log(ex);
        }
    }

}
