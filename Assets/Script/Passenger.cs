using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    public Transform randomtarget;
    public Vector3 initialPosition;
    
    void Start()
    {
        MoveCube();
    }

    void Update()
    {   
        Debug.Log(nameof(CubeIndicator.TouchTarget));
        Debug.Log(CubeIndicator.TouchTarget);
        if(CubeIndicator.TouchTarget || (Vector3.Distance(randomtarget.position, transform.position) <= 1.5f))
        {
            MoveCube();
        } 
    }

    void MoveCube()
    {
        initialPosition = new Vector3(Random.Range(-45f, 45f), 1f, Random.Range(-45f, 45f));
        transform.position = initialPosition;
    }
}