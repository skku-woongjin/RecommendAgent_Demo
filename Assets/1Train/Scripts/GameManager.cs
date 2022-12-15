using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System;
public class GameManager : MonoBehaviour
{
    public bool trivariate;
    public int flagCount = 8;

    [HideInInspector]
    public Flag[] flags;
    public Flag[] flagFitness;
    public GameObject flagPrefab;
    public int destQSize;
    public TMP_Text valueUI;


    int destQfilled;
    [HideInInspector]
    public int maxcount;
    int worldSize = 100;
    [HideInInspector]
    public float maxdist;

    [HideInInspector]
    public RecommendAgent triMeanAgent;

    Queue<int> destQ;

    Transform candidates;
    Transform trails;

    [HideInInspector]
    public TrailGenerator trailGenerator;
    private static GameManager gm;
    public static GameManager Gmr
    {
        get
        {
            if (gm == null) Debug.LogError("Game Manager is null!");
            return gm;
        }
    }

    private void Awake()
    {
        gm = this;
        trailGenerator = GetComponentInChildren<TrailGenerator>();
        trails = GetComponentInChildren<TrailEnergyDecrease>().transform;
        candidates = GameObject.Find("Candidates").transform;
        triMeanAgent = GetComponentInChildren<RecommendAgent>();

        if (candidates.childCount == 0)
        {
            for (int i = 0; i < flagCount; i++)
            {
                GameObject tmp = Instantiate(flagPrefab, candidates);
                tmp.GetComponentInChildren<TMP_Text>().text = i + "";
            }
        }

        flags = new Flag[flagCount];
        for (int i = 0; i < flagCount; i++)
        {
            flags[i] = new Flag();
        }

        maxdist = worldSize * Mathf.Sqrt(2);
        maxcount = destQSize;
        destQ = new Queue<int>();

        resetEnv();
        randomFlagPos();
        updateFlagUI();
        triMeanAgent.setUserMean();
    }

    //NOTE Flag: Random Position
    public void randomFlagPos()
    {
        for (int i = 0; i < flagCount; i++)
        {
            flags[i].pos = new Vector3(UnityEngine.Random.Range(-worldSize / 2 + 2, worldSize / 2 - 2), 0, UnityEngine.Random.Range(-worldSize / 2 + 2, worldSize / 2 - 2));
        }

        flags = flags.OrderBy(v => -v.pos.z).ThenBy(v => v.pos.x).ToArray<Flag>();

        int j = 0;
        foreach (Transform child in candidates)
        {
            child.transform.localPosition = flags[j].pos;
            flags[j].id = j;
            j++;
        }
        clearChoice(0);
        clearChoice(1);
        clearChoice(2);
        resetEnv();
    }

    //NOTE Flag: Random Visit
    public void randomFlagVisit()
    {
        trailGenerator.waitTime = 0.5f;
        StartCoroutine(trailGenerator.randomWarp(20));
        updateFlagUI();
    }

    //NOTE Flag: Random Dur
    public void randomFlagDur()
    {
        //평균 방문 시간 생성
        foreach (Flag flag in flags)
        {
            flag.time = UnityEngine.Random.Range(0, maxdist);
        }
        updateFlagUI();
    }


    //NOTE Flag: dest에 방문 
    public void visitFlag(int dest)
    {
        if (dest >= 0)
        {
            if (destQfilled == destQSize)
            {
                flags[destQ.Dequeue()].visited--;
                destQfilled--;
            }

            destQ.Enqueue(dest);
            destQfilled++;
            flags[dest].visited++;
        }
        updateFlagUI();

    }

    //NOTE: Flag: UI text update
    public void updateFlagUI()
    {
        for (int i = 0; i < candidates.childCount; i++)
        {
            candidates.GetChild(i).GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = flags[i].visited + "";

            candidates.GetChild(i).GetChild(2).GetChild(2).gameObject.SetActive(false);

        }

    }

    //NOTE 환경 초기화 
    public void resetEnv()
    {
        //reset Flag color 
        foreach (Transform flag in candidates)
        {
            flag.GetComponent<FlagColor>().yellow();
        }

        //destroy trails
        foreach (Transform child in trails)
        {
            Destroy(child.gameObject);
        }

        //reset visit history
        destQfilled = 0;
        destQ.Clear();
        foreach (Flag flag in flags)
        {
            flag.visited = 0;
            flag.time = 0;
        }
        triMeanAgent.clearDots();
        randomOwnerPos(false);
        updateFlagUI();
    }
    public void showValueUI()
    {
        valueUI.text = "";
        valueUI.text += "---Duration---\n";
        foreach (Flag flag in flags)
        {

            valueUI.text += "Flag";
            valueUI.text += flag.id;
            valueUI.text += " : ";
            valueUI.text += Math.Round(flag.time, 1);
            valueUI.text += "\n";
        }

    }
    public void resetDur()
    {
        foreach (Flag flag in flags)
        {
            flag.time = 0;
        }
        updateFlagUI();
    }

    public void randOwner()
    {
        randomOwnerPos(true);
    }

    public void randomOwnerPos(bool withTrail)
    {
        //Random user position
        if (withTrail)
            trailGenerator.warpTo(new Vector3(UnityEngine.Random.Range(-worldSize / 2, worldSize / 2), 0, UnityEngine.Random.Range(-worldSize / 2, worldSize / 2)));
        else
            trailGenerator.transform.localPosition = new Vector3(UnityEngine.Random.Range(-worldSize / 2, worldSize / 2), 0, UnityEngine.Random.Range(-worldSize / 2, worldSize / 2));
    }

    public void updateFlagDist()
    {
        foreach (Flag flag in flags)
        {
            flag.dist = Vector3.Distance(flag.pos, trailGenerator.transform.localPosition);
        }
    }

    public void recommend(int id, int type, bool rec)
    {
        if (rec)
        {
            clearChoice(type);
        }
        candidates.GetChild(id).GetComponentInChildren<HorizontalLayoutGroup>().transform.GetChild(type).gameObject.SetActive(rec);
    }

    public void clearChoice(int type)
    {
        for (int i = 0; i < flagCount; i++)
        {
            recommend(i, type, false);
        }
    }

    public void skipLog()
    {
        trailGenerator.waitTime = 0;
    }


}
