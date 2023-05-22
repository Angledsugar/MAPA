using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCube : MonoBehaviour
{
    AttackAgent Attack;
    void Start()
    {
        Attack = GetComponentInParent<AttackAgent>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        // Debug.Log($"Detect!!!!!!!!!!!!!!");
        if (collision.gameObject.CompareTag("agent")) 
        {
            // Debug.Log($"Agent Attack");
            Attack.OnCollisionEnter(collision);
        }
    }
}
