using UnityEngine;

public class CubeIndicator : MonoBehaviour
{
    public static bool TouchTarget = false;
    public Transform targetToLookAt; //target in the scene the indicator will point to
    public float heightOffset;
    private float m_StartingYPos;
    
    public static Vector3 TargetRandomPosition;
    
    void Start()
    {
        TargetRandomPosition = new Vector3(Random.Range(-45f, 45f), 0.5f, Random.Range(-45f, 45f));
        transform.position = TargetRandomPosition;
    }

    void OnEnable()
    {
        m_StartingYPos = transform.position.y;
    }

    void Update()
    {
        TouchTarget = false;
        if(Vector3.Distance(targetToLookAt.position, transform.position) <= 1.6f)
        {
            TouchTarget = true;
            TargetRandomPosition = new Vector3(Random.Range(-45f, 45f), 0.5f, Random.Range(-45f, 45f));
            transform.position = TargetRandomPosition;
        }
        Vector3 walkDir = targetToLookAt.position - transform.position;
        walkDir.y = 0; //flatten dir on the y
        transform.rotation = Quaternion.LookRotation(walkDir);
        
    }

    //Public method to allow an agent to directly update this component
    public void MatchOrientation(Transform t)
    {
        transform.position = new Vector3(t.position.x, m_StartingYPos + heightOffset, t.position.z);
        transform.rotation = t.rotation;
    }
}

