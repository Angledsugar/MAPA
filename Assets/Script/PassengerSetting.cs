using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;

public class PassengerSetting : MonoBehaviour
{
    [HideInInspector]
    public GameObject[] agents;
    [HideInInspector]
    public PassengerArea[] listArea;

    public int totalScore;
    public Text scoreText;

    StatsRecorder m_Recorder;

    public void Awake()
    {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
        m_Recorder = Academy.Instance.StatsRecorder;
    }

    void EnvironmentReset()
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("passenger"));
        ClearObjects(GameObject.FindGameObjectsWithTag("attacker"));

        agents = GameObject.FindGameObjectsWithTag("agent");
        listArea = FindObjectsOfType<PassengerArea>();
        foreach (var fa in listArea)
        {
            if(fa) fa.ResetPassengerArea(agents);
        }

        totalScore = 0;
    }

    void ClearObjects(GameObject[] objects)
    {
        foreach (var passenger in objects)
        {
            Destroy(passenger);
        }
    }

    public void Update()
    {
        scoreText.text = $"Score: {totalScore}";

        // Send stats via SideChannel so that they'll appear in TensorBoard.
        // These values get averaged every summary_frequency steps, so we don't
        // need to send every Update() call.
        if ((Time.frameCount % 100) == 0)
        {
            m_Recorder.Add("TotalScore", totalScore);
        }
    }
}
