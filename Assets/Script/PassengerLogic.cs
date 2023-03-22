using UnityEngine;
// using Unity.MLAgents;

public class PassengerLogic : MonoBehaviour
{
    public bool respawn;
    public PassengerArea myArea;
    private GameObject[] AgentSet;
    // private TaxiAgent Taxiagent;

    void Update()
    {
        AgentSet = GameObject.FindGameObjectsWithTag("agent");
        for(int i = 0; i < AgentSet.Length; i++ )
        {
            // Taxiagent = AgentSet[i].GetComponent<TaxiAgent>();
            if(Vector3.Distance(AgentSet[i].transform.position, gameObject.transform.position) < 3f)
            {
                // PassengerSet[i].GetComponent<PassengerLogic>().OnEaten();
                AgentSet[i].GetComponent<TaxiAgent>().AddReward(1f);
                OnEaten();
            }
        }
    }

    public void OnEaten()
    {
        if (respawn)
        {
            transform.position = new Vector3(Random.Range(-myArea.range, myArea.range),
                0.6f,
                Random.Range(-myArea.range, myArea.range)) + myArea.transform.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
