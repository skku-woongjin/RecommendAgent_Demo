using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBaske.Sensors.Grid;
using System;

public class TrailEnergyDecrease : MonoBehaviour
{
    public float decreaseRate;
    GridBuffer m_SensorBuffer;
    public int gridsize;

    int normx, normy;
    public float normFactor;
    RecommendAgent agent;

    void Start()
    {
        agent = transform.parent.GetComponent<RecommendAgent>();
        // m_SensorBuffer = agent.trailGrid;
    }
    void FixedUpdate()
    {
    }

    public void trailEvap()
    {
        // m_SensorBuffer.ClearChannel(0);
        foreach (Transform child in transform)
        {
            normx = Convert.ToInt32((child.localPosition.x + 50) / gridsize);
            normy = Convert.ToInt32((child.localPosition.z + 50) / gridsize);

            //grid에 적기
            // if (normx < 100 / gridsize && normy < 100 / gridsize && normx >= 0 / gridsize && normy >= 0 / gridsize)
            //     m_SensorBuffer.Write(0, normx, normy, m_SensorBuffer.Read(0, normx, normy) + child.GetComponent<TrailPoint>().energy / normFactor);
            //에너지 감소 
            child.GetComponent<TrailPoint>().energy -= decreaseRate;
            //다 증발했으면 없애기 
            if (child.GetComponent<TrailPoint>().energy <= 0)
            {
                Destroy(child.gameObject);
            }
            else//색 효과 
                child.GetComponent<Renderer>().material.color = Color.Lerp(new Color(1, 0, 0, 0), Color.red, child.GetComponent<TrailPoint>().energy);
        }
    }
}
