using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemptingAttack : MonoBehaviour
{
    public GameObject TemptingCubePrefab = null;
    Transform transformP = null;

    public int targetCount = 1;
    public int goalCount = 1;

    List<TemptingCube> TemptingCubeList = new List<TemptingCube>();

    //void Awake()
    public void OnEpisodeBegin()
    {
        Debug.Log("Start");
        transformP = this.transform;
        if (targetCount != TemptingCubeList.Count)
        {
            for (int i = 0; i < targetCount; i++)
            {
                TemptingCubeList.Add(GameObject.Instantiate(TemptingCubePrefab, transformP).GetComponent<TemptingCube>());
            }
        }

        foreach (var target in TemptingCubeList)
        {
            float rx = 0;
            float rz = 0;

            rx = Random.value * 50 - 10;
            rz = Random.value * 50 - 10;

            target.transform.localPosition = new Vector3(rx,
                                               0.5f,
                                               rz);

            target.gameObject.SetActive(true);
        }
    }
}