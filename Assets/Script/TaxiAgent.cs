using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class TaxiAgent : Agent
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;
    private Vector3 agent_position;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    PassengerSetting m_PassengerSettings;
    public GameObject area;
    public GameObject[] PassengerSet;
    PassengerArea m_MyArea;
    bool m_Frozen;
    bool m_Poisoned;
    bool m_Satiated;
    bool m_Shoot;
    float m_FrozenTime;
    float m_EffectTime;
    Rigidbody m_AgentRb;
    float m_LaserLength;
    // Speed of agent rotation.
    public float turnSpeed = 300;

    // Speed of agent movement.
    public float moveSpeed = 2;
    public Material normalMaterial;
    public Material badMaterial;
    public Material goodMaterial;
    public Material frozenMaterial;
    public GameObject myLaser;
    public bool contribute;
    public bool useVectorObs;
    [Tooltip("Use only the frozen flag in vector observations. If \"Use Vector Obs\" " +
             "is checked, this option has no effect. This option is necessary for the " +
             "VisualFoodCollector scene.")]
    public bool useVectorFrozenFlag;

    EnvironmentParameters m_ResetParams;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<PassengerArea>();
        m_PassengerSettings = FindObjectOfType<PassengerSetting>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
    }

    private void FixedUpdate()
    {
        // PassengerSet = GameObject.FindGameObjectsWithTag("passenger");
        // for(int i = 0; i < PassengerSet.Length; i++ )
        // {
        //     if(Vector3.Distance(PassengerSet[i].transform.position, gameObject.transform.position) < 5f)
        //     {
        //         PassengerSet[i].GetComponent<PassengerLogic>().OnEaten();
        //         AddReward(1f);
        //     }
        // }

        if(gameObject.transform.rotation.x > 10f || gameObject.transform.rotation.z > 10f || gameObject.transform.rotation.z < -10f || gameObject.transform.rotation.x < -10f)
        {
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        UpdateWheels();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(gameObject.transform.position.x);
        sensor.AddObservation(gameObject.transform.position.z);
        sensor.AddObservation(gameObject.transform.rotation.y);

        // PassengerSet = GameObject.FindGameObjectsWithTag("passenger");
        // for(int i = 0; i < PassengerSet.Length; i++ )
        // {
        //     sensor.AddObservation(PassengerSet[i]);
        // }
        
        // var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
        // sensor.AddObservation(localVelocity.x);
        // sensor.AddObservation(localVelocity.z);
    }

    public Color32 ToColor(int hexVal)
    {
        var r = (byte)((hexVal >> 16) & 0xFF);
        var g = (byte)((hexVal >> 8) & 0xFF);
        var b = (byte)(hexVal & 0xFF);
        return new Color32(r, g, b, 255);
    }

    public void MoveAgent(ActionBuffers actionBuffers)
    {           
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var continuousActions = actionBuffers.ContinuousActions;
        var discreteActions = actionBuffers.DiscreteActions;

        var forward = Mathf.Clamp(continuousActions[0], -1f, 1f);
        var rotate = Mathf.Clamp(continuousActions[1], -1f, 1f);
        int brake = discreteActions[0];

        if(brake == 0)
        {
            frontLeftWheelCollider.brakeTorque = 0f;
            frontRightWheelCollider.brakeTorque = 0f;
            rearLeftWheelCollider.brakeTorque = 0f;
            rearRightWheelCollider.brakeTorque = 0f;

            // Taxi is front wheel drive.
            frontLeftWheelCollider.motorTorque = forward * motorForce;
            frontRightWheelCollider.motorTorque = forward * motorForce;

            frontLeftWheelCollider.steerAngle = rotate * 30;
            frontRightWheelCollider.steerAngle = rotate * 30;
        }
        else if(brake == 1)
        {
            frontLeftWheelCollider.brakeTorque = breakForce;
            frontRightWheelCollider.brakeTorque = breakForce;
            rearLeftWheelCollider.brakeTorque = breakForce;
            rearRightWheelCollider.brakeTorque = breakForce;
        }
        
        // AddReward(0.01f);

        // Debug.Log(Vector3.Distance(agent_position, gameObject.transform.position));
        // Debug.Log(this.gameObject.name + " : " + Vector3.Distance(agent_position, gameObject.transform.position));
        // if (Vector3.Distance(agent_position, gameObject.transform.position) < 5f)
        // {
        //     agent_position = gameObject.transform.position;
        //     AddReward(-0.01f);
        // }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
         MoveAgent(actionBuffers);
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
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    public override void OnEpisodeBegin()
    {
        m_AgentRb.velocity = Vector3.zero;
        myLaser.transform.localScale = new Vector3(0f, 0f, 0f);
        transform.position = new Vector3(Random.Range(-m_MyArea.range, m_MyArea.range),
            2f, Random.Range(-m_MyArea.range, m_MyArea.range))
            + area.transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        SetResetParameters();

        // agent_position = gameObject.transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log(Vector3.Distance(agent_position, gameObject.transform.position));
        if (collision.gameObject.CompareTag("wall"))
        {
            AddReward(-10f);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("agent"))
        {
            AddReward(-10f);
            EndEpisode();
        }
        // if (collision.gameObject.CompareTag("passenger"))
        // {
        //     collision.gameObject.GetComponent<PassengerLogic>().OnEaten();
        //     AddReward(1f);
        //     if (contribute)
        //     {
        //         m_PassengerSettings.totalScore += 1;
        //     }
        // }
        // if (collision.gameObject.CompareTag("attacker"))
        // {
        //     collision.gameObject.GetComponent<AttackerLogic>().OnEaten();

        //     AddReward(100f);
        //     if (contribute)
        //     {
        //         m_PassengerSettings.totalScore -= 1;
        //     }
        // }
    }

    public void SetLaserLengths()
    {
        m_LaserLength = m_ResetParams.GetWithDefault("laser_length", 1.0f);
    }

    public void SetAgentScale()
    {
        float agentScale = m_ResetParams.GetWithDefault("agent_scale", 1.0f);
        gameObject.transform.localScale = new Vector3(agentScale, agentScale, agentScale);
    }

    public void SetResetParameters()
    {
        SetLaserLengths();
        SetAgentScale();
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();       
    }
    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }
    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }
    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }
    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

}
