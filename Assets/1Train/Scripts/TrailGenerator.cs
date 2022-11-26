using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//주인공 움직이기, Trail관리 

public class TrailGenerator : MonoBehaviour
{
    NavMeshAgent nav;
    RecommendAgent agent;
    void OnEnable()
    {
        nav = GetComponent<NavMeshAgent>();
        agent = transform.parent.GetComponentInChildren<RecommendAgent>();
        trail = traces.GetComponent<TrailEnergyDecrease>();
    }

    public void goTo(int id, bool warp)
    {
        if (id < 0)
        {
            if (!warp)
                nav.SetDestination(new Vector3(0, 0, 0));
        }
        else
        {
            Vector3 pos = GameManager.Gmr.flags[id].pos + agent.transform.position;
            makeTrace(5);
            if (!warp)
                nav.SetDestination(pos);
            else
            {
                warpTo(pos);
            }
        }
        GameManager.Gmr.visitFlag(id);
    }
    private Vector3 makeNoisePoint(Vector3 prevPos, Vector3 nextPos)
    {
        while (true)
        {
            float length = Vector3.Magnitude(nextPos - prevPos);
            float ratio = Random.Range(0.2f, 0.8f);
            Vector2 noisePoint = Random.insideUnitCircle.normalized * ratio * length / 2;
            Vector3 noisePointV3 = new Vector3(noisePoint.x, 0, noisePoint.y);

            Vector3 returnPoint = Vector3.Lerp(prevPos, nextPos, 0.5f) + noisePointV3;
            if (returnPoint.x > agent.worldSize || returnPoint.x < -agent.worldSize
            || returnPoint.z < -agent.worldSize || returnPoint.z > agent.worldSize)
            {
                continue;
            }

            return returnPoint;
        }


    }
    public void warpTo(Vector3 pos)
    {
        NavMeshPath path = new NavMeshPath();
        nav.CalculatePath(pos, path);

        Vector3 prevPos = transform.localPosition;

        List<Vector3> corners = new List<Vector3>(path.corners);
        // for (int i = 1; i < corners.Count; i += 2)
        // {
        //     corners.Insert(i, makeNoisePoint(corners[i - 1], corners[i]));
        // }
        foreach (Vector3 corner in corners)
        {
            traceLine(prevPos, corner);
            prevPos = corner;
        }
        traceLine(prevPos, pos);
        transform.position = pos;
    }

    public float waitTime;
    public Button logBtn;
    public IEnumerator randomWarp(int n)
    {
        GameManager.Gmr.triMeanAgent.clearDots();
        logBtn.interactable = false;
        for (int i = 0; i < n; i++)
        {
            GameManager.Gmr.updateFlagDist();
            int idx = Random.Range(0, GameManager.Gmr.flagCount);
            goTo(idx, true);
            GameManager.Gmr.triMeanAgent.addLog3(GameManager.Gmr.flags[idx].visited, GameManager.Gmr.flags[idx].dist, GameManager.Gmr.flags[idx].time);
            yield return new WaitForSecondsRealtime(waitTime);
        }
        GameManager.Gmr.randomOwnerPos(true);
        logBtn.interactable = true;
    }

    void traceLine(Vector3 src, Vector3 dst)
    {
        makeTrace(src);
        float dist = Vector3.Distance(src, dst);
        if (!(dist < traceSpacing))
        {
            for (int i = 1; i <= dist / traceSpacing; i++)
            {
                makeTrace(Vector3.Lerp(src, dst, traceSpacing * i / dist));
            }
        }
    }

    public GameObject tracePrefab;
    public Transform traces;
    TrailEnergyDecrease trail;
    public float traceSpacing;

    Vector3 lastpos;

    public void makeTrace(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(tracePrefab, new Vector3(transform.position.x, 1f, transform.position.z), Quaternion.identity, traces);
            trail.trailEvap();
        }
    }

    public void makeTrace(Vector3 pos)
    {
        if (agent.warp) trail.trailEvap();
        Instantiate(tracePrefab, pos, Quaternion.identity, traces);

    }

    // private void FixedUpdate()
    // {
    //     if (!agent.warp && (lastpos == null || Vector3.SqrMagnitude(lastpos - transform.position) > traceSpacing))
    //     {
    //         makeTrace(1);
    //         lastpos = transform.position;
    //     }
    // }


}



// if (queueFilled > 0)
//     agent.AddReward(Vector3.SqrMagnitude(waypoints.Peek() - transform.position) / 10000);
// agent.AddReward(agent.energy / 10);
