using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgentsExamples;

public class PassengerArea : Area
{
    public GameObject Passenger;
    public GameObject Attacker;
    public int numPassenger;
    public int numAttacker;
    public bool respawnPassenger;
    public float range;

    void CreatePassenger(int num, GameObject type)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject f = Instantiate(type, 
            new Vector3(Random.Range(-range, range), 0.6f,
                Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            
            f.GetComponent<PassengerLogic>().respawn = respawnPassenger;
            f.GetComponent<PassengerLogic>().myArea = this;
        }
    }

    public void ResetPassengerArea(GameObject[] agents)
    {
        foreach (GameObject agent in agents)
        {
            if (agent.transform.parent == gameObject.transform)
            {
                agent.transform.position = new Vector3(Random.Range(-range, range), 0.6f,
                    Random.Range(-range, range))
                    + transform.position;
                agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            }
        }

        CreatePassenger(numPassenger, Passenger);
        CreatePassenger(numAttacker, Attacker);
    }

    public override void ResetArea()
    {
    }

}