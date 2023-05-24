using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTargetCube : MonoBehaviour
{
    public Transform TargetPrefab;
    private Transform m_Target;

    void Awake()
    {
        Instantiate(TargetPrefab, transform.position, Quaternion.identity, transform);
    }

    void Update()
    {
        
    }
}
