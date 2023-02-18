using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemptingCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "agent")
        {
            CrawlerAgent ca = other.GetComponent<CrawlerAgent>();
            if (null != ca)
            {
                // ra.EnteredTarget();
                this.gameObject.SetActive(false);
            }
        }
    }
}