using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Utility class to allow target placement and collision detection with an agent
/// Add this script to the target you want the agent to touch.
/// Callbacks will be triggered any time the target is touched with a collider tagged as 'tagToDetect'
/// </summary>
public class TemptingAttacks : MonoBehaviour
{
    float timer;
    float waitingTime;

    public float randomX = 0;
    public float randomZ = 0;
    public int entercount = 0;
    public GameObject TemptingCubeGO;
    
    Vector3 TargetCubePos;
    Vector3 CrawlerAgentPos;
    Vector3 restPos;

    // Transform transformP = null;
    public int targetCount = 1;
    public int goalCount = 1;
    // List<TemptingCube> TemptingCubeList = new List<TemptingCube>();
    [Header("Target Cube")]
    public GameObject TargetCube;

    [Header("Ground")]
    public GameObject Ground;

    [Header("Body")]
    public GameObject Body;
    
    [SerializeField]//HideInInspector SerializeField
    public Agent agent0;

    [Header("Collider Tag To Detect")]
    public string tagToDetect = "agent"; //collider tag to detect 

    [Header("Target Placement")]
    public float spawnRadius; //The radius in which a target can be randomly spawned.
    public bool respawnIfTouched; //Should the target respawn to a different position when touched

    [Header("Target Fell Protection")]
    public bool respawnIfFallsOffPlatform = true; //If the target falls off the platform, reset the position.
    public float fallDistance = 100; //distance below the starting height that will trigger a respawn 


    private Vector3 m_startingPos; //the starting position of the target
    private Agent m_agentTouching; //the agent currently touching the target

    [System.Serializable]
    public class TriggerEvent : UnityEvent<Collider>
    {
    }

    [Header("Trigger Callbacks")]
    public TriggerEvent onTriggerEnterEvent = new TriggerEvent();
    public TriggerEvent onTriggerStayEvent = new TriggerEvent();
    public TriggerEvent onTriggerExitEvent = new TriggerEvent();

    [System.Serializable]
    public class CollisionEvent : UnityEvent<Collision>
    {
    }

    [Header("Collision Callbacks")]
    public CollisionEvent onCollisionEnterEvent = new CollisionEvent();
    public CollisionEvent onCollisionStayEvent = new CollisionEvent();
    public CollisionEvent onCollisionExitEvent = new CollisionEvent();

    // Start is called before the first frame update
    void OnEnable()
    {
        restPos = Ground.transform.position;
        m_startingPos = transform.position;
        transform.position = new Vector3(m_startingPos.x, m_startingPos.y+20, m_startingPos.z);
        timer = 0.0f;
        waitingTime = 5;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer == waitingTime) MoveTargetToPosition();

        int cubeactive = Random.Range(-10000,10000);
        // Debug.Log(cubeactive);

        if (cubeactive == 0)
        {       
            this.gameObject.SetActive(true);
            MoveTargetToPosition();
        }

        if (respawnIfFallsOffPlatform)
        {
            // if (transform.position.y < m_startingPos.y - fallDistance)
            // {
            //     Debug.Log($"{transform.name} Fell Off Platform");
            //     MoveTargetToPosition();
            // }
        }
    }

    /// <summary>
    /// Moves target to a random position within specified radius.
    /// </summary>
    public void MoveTargetToPosition()
    {
        TargetCubePos = TargetCube.transform.position;
        CrawlerAgentPos = Body.transform.position;
        transform.position = new Vector3((TargetCubePos.x + CrawlerAgentPos.x)/2, m_startingPos.y, (TargetCubePos.z + CrawlerAgentPos.z)/2);
    }

    private void OnCollisionEnter(Collision col)
    {
        // this.gameObject.SetActive(true);
        if (col.transform.CompareTag(tagToDetect))
        {
            onCollisionEnterEvent.Invoke(col);
            Debug.Log("Tempting Attack");
            agent0.AddReward(100.0f);
            transform.position = new Vector3(restPos.x + 50 , restPos.y+5, restPos.z + 50);

            if (respawnIfTouched)
            {
                MoveTargetToPosition();
            }
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if (col.transform.CompareTag(tagToDetect))
        {
            onCollisionStayEvent.Invoke(col);
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.transform.CompareTag(tagToDetect))
        {
            onCollisionExitEvent.Invoke(col); 
            // CopyCube();
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag(tagToDetect))
        {
            onTriggerEnterEvent.Invoke(col);
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag(tagToDetect))
        {
            onTriggerStayEvent.Invoke(col);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag(tagToDetect))
        {
            onTriggerExitEvent.Invoke(col);
        }
    }
}

