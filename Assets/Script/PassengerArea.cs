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
    Component[] components;

    void CreatePassenger(int num, GameObject type, bool whowho)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject f = Instantiate(type, 
            new Vector3(Random.Range(-range, range), 0.6f,
                Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));

            if(whowho)
            {
                f.GetComponent<PassengerLogic>().respawn = respawnPassenger;
                f.GetComponent<PassengerLogic>().myArea = this;
            }

            if(!whowho)
            {
                f.GetComponent<AttackerLogic>().respawn = respawnPassenger;
                f.GetComponent<AttackerLogic>().myArea = this;
            }
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

        CreatePassenger(numPassenger, Passenger, true);
        CreatePassenger(numAttacker, Attacker, false);
    }

    public override void ResetArea()
    {
    }

}