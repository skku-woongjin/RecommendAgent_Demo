using UnityEngine;
using Unity.MLAgents;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class RecommendAgent : Agent
{
    public bool trivariate = false;
    public int logLen = 20;
    public bool warp;
    public int worldSize;

    public Slider slider_dist;
    public Slider slider_visN;
    public Slider slider_dur;

    public TMP_Text text_dist;
    public TMP_Text text_visN;
    public TMP_Text text_dur;

    public TMP_Text text_rank;

    Vector2 userMean;
    Vector3 userMean3;
    Vector2[] userLog;
    Vector3[] userLog3;

    public override void CollectObservations(VectorSensor sensor)
    {
        if (trivariate)
        {
            foreach (Vector3 vec in userLog3)
                sensor.AddObservation(vec);
        }
        else
        {
            foreach (Vector2 vec in userLog)
                sensor.AddObservation(vec);
        }

    }

    float maxGaus = 0;

    public GameObject dotPrefab;
    public GameObject agentDotPrefab;
    public GameObject answerDotPrefab;
    public Transform graph2D;
    public Transform graph1D;

    public override void Initialize()
    {
        if (graph2D.childCount == 0)
        {

            for (int i = 0; i < logLen; i++)
            {
                Instantiate(dotPrefab, graph2D);
                if (trivariate)
                {
                    Instantiate(dotPrefab, graph1D);
                }
            }
            if (trivariate)
            {
                Instantiate(answerDotPrefab, graph1D);
                Instantiate(agentDotPrefab, graph1D);
            }
            Instantiate(answerDotPrefab, graph2D);
            Instantiate(agentDotPrefab, graph2D);
        }

        userLog = new Vector2[logLen];
        userLog3 = new Vector3[logLen];

        maxcount = 24;

    }

    public void randomMean()
    {
        if (trivariate)
        {
            slider_dur.value = UnityEngine.Random.Range(0.0f, slider_dur.maxValue);
        }

        slider_dist.value = UnityEngine.Random.Range(0.0f, slider_dist.maxValue);
        slider_visN.value = UnityEngine.Random.Range(0, slider_visN.maxValue + 1);

        setUserMean();
        generateLog();
    }

    public void setUserMean()
    {
        //NOTE 유저 확률분포 slider로부터 받아오기 
        // Vector2 userMean = new Vector2(0, 0);
        if (trivariate)
        {
            // userMean3 = new Vector3(UnityEngine.Random.Range(0, maxcount), UnityEngine.Random.Range(0, maxdist), UnityEngine.Random.Range(0, maxdist));
            userMean3 = new Vector3(slider_visN.value, slider_dist.value, slider_dur.value);
            ((RectTransform)graph2D.GetChild(logLen)).anchoredPosition = new Vector3(userMean3.x / maxcount, userMean3.y / maxdist, 0) * 100;
            ((RectTransform)graph1D.GetChild(logLen)).anchoredPosition = new Vector3(userMean3.z / maxdist, 0, 0) * 100;
            maxGaus = (float)GetTrivariateGuassian(userMean3.x, 5, userMean3.y, 10 * maxdist / maxcount, userMean3.z, 10 * maxdist / maxcount, userMean3.x, userMean3.y, userMean3.z);
            text_dur.text = "dur: " + Math.Round(slider_dur.value, 1);
        }
        else
        {
            userMean = new Vector2(slider_visN.value, slider_dist.value);
            ((RectTransform)graph2D.GetChild(logLen)).anchoredPosition = new Vector3(userMean.x / maxcount, userMean.y / maxdist, 0) * 100;
            maxGaus = (float)GetBivariateGuassian(userMean.x, 5, userMean.y, 10 * maxdist / maxcount, userMean.x, userMean.y, 0);
        }
        text_dist.text = "dist:\n" + Math.Round(slider_dist.value, 1);
        text_visN.text = "#visit: " + slider_visN.value;
    }

    public void generateLog()
    {
        int count = 0;
        float dist;
        int visited;
        float time;
        while (count < logLen)
        {
            dist = UnityEngine.Random.Range(0, maxdist);
            visited = UnityEngine.Random.Range(0, (int)maxcount + 1);
            if (trivariate)
            {
                time = UnityEngine.Random.Range(0, maxdist);
                if (UnityEngine.Random.Range(0, 1.0f) < (float)GetTrivariateGuassian(userMean3.x, 5, userMean3.y, 10 * maxdist / maxcount, userMean3.z, 10 * maxdist / maxcount, visited, dist, time) / (maxGaus * 2))
                {
                    userLog3[count] = new Vector3(dist / maxdist, (float)visited / maxcount, time / maxdist);
                    ((RectTransform)graph2D.GetChild(count)).anchoredPosition = new Vector3(userLog3[count].y, userLog3[count].x, 0) * 100;
                    ((RectTransform)graph1D.GetChild(count)).anchoredPosition = new Vector3(userLog3[count].z, 0, 0) * 100;
                    count++;
                }
            }
            else
            {

                if (UnityEngine.Random.Range(0, 1.0f) < (float)GetBivariateGuassian(userMean.x, 5, userMean.y, 10 * maxdist / maxcount, visited, dist, 0) / (maxGaus * 2))
                {
                    userLog[count] = new Vector2((float)visited / maxcount, dist / maxdist);
                    ((RectTransform)graph2D.GetChild(count)).anchoredPosition = userLog[count] * 100;
                    count++;
                }
            }
        }
    }

    public Button sampleBtn;

    public void logWarp()
    {
        sampleBtn.interactable = false;
        StartCoroutine(generateLogWarp());
    }

    int logcount = 0;
    public void addLog3(int visit, float dist, float dur)
    {
        userLog3[logcount] = new Vector3(dist / maxdist, visit / maxcount, dur / maxdist);
        ((RectTransform)graph2D.GetChild(logcount)).anchoredPosition = new Vector3(userLog3[logcount].y, userLog3[logcount].x, 0) * 100;
        ((RectTransform)graph1D.GetChild(logcount)).anchoredPosition = new Vector3(userLog3[logcount].z, 0, 0) * 100;
        logcount++;
        if (logcount == logLen)
        {
            logcount = 0;
        }
    }

    public void clearDots()
    {
        for (int i = 0; i < logLen; i++)
        {
            ((RectTransform)graph2D.GetChild(i)).anchoredPosition = new Vector3(-100, -100, 0);
            ((RectTransform)graph1D.GetChild(i)).anchoredPosition = new Vector3(-100, -100, 0);
        }
    }

    IEnumerator generateLogWarp()
    {
        int count = 0;
        int idx = 0;
        float dist;
        int visited;
        float time;
        GameManager.Gmr.trailGenerator.waitTime = 0.0f;
        GameManager.Gmr.randomFlagVisit();
        GameManager.Gmr.updateFlagDist();
        GameManager.Gmr.trailGenerator.waitTime = 0.5f;

        int trial = 0;
        while (count < logLen)
        {
            GameManager.Gmr.randomFlagDur();
            idx = UnityEngine.Random.Range(0, GameManager.Gmr.flagCount);
            dist = GameManager.Gmr.flags[idx].dist;
            visited = GameManager.Gmr.flags[idx].visited;
            if (trivariate)
            {
                time = GameManager.Gmr.flags[idx].time;
                if (UnityEngine.Random.Range(0, 1.0f) < (float)GetTrivariateGuassian(userMean3.x, 5, userMean3.y, 10 * maxdist / maxcount, userMean3.z, 10 * maxdist / maxcount, visited, dist, time) / (maxGaus * 4) || trial > 100)
                {
                    userLog3[count] = new Vector3(dist / maxdist, (float)visited / maxcount, time / maxdist);
                    ((RectTransform)graph2D.GetChild(count)).anchoredPosition = new Vector3(userLog3[count].y, userLog3[count].x, 0) * 100;
                    ((RectTransform)graph1D.GetChild(count)).anchoredPosition = new Vector3(userLog3[count].z, 0, 0) * 100;
                    GameManager.Gmr.trailGenerator.goTo(idx, true);
                    count++;
                    yield return new WaitForSecondsRealtime(GameManager.Gmr.trailGenerator.waitTime);
                    GameManager.Gmr.randomOwnerPos(true);
                    GameManager.Gmr.updateFlagDist();
                    GameManager.Gmr.updateFlagUI();
                    yield return new WaitForSecondsRealtime(GameManager.Gmr.trailGenerator.waitTime);
                    trial = 0;
                }
            }
            else
            {

                if (UnityEngine.Random.Range(0, 1.0f) < (float)GetBivariateGuassian(userMean.x, 5, userMean.y, 10 * maxdist / maxcount, visited, dist, 0) / (maxGaus * 2))
                {
                    userLog[count] = new Vector2((float)visited / maxcount, dist / maxdist);
                    ((RectTransform)graph2D.GetChild(count)).anchoredPosition = userLog[count] * 100;
                    count++;
                }
            }
            trial++;
        }

        sampleBtn.interactable = true;
    }

    float maxdist = 100 * Mathf.Sqrt(2);

    float maxcount;
    public static double GetBivariateGuassian(double muX, double sigmaX, double muY, double sigmaY, double x, double y, double rho = 0)
    {
        var sigmaXSquared = Math.Pow(sigmaX, 2);
        var sigmaYSquared = Math.Pow(sigmaY, 2);

        var dX = x - muX;
        var dY = y - muY;

        var exponent = -0.5;
        var normaliser = 2 * Math.PI * sigmaX * sigmaY;
        if (rho != 0)
        {
            normaliser *= Math.Sqrt(1 - Math.Pow(rho, 2));
            exponent /= 1 - Math.Pow(rho, 2);
        }

        var sum = Math.Pow(dX, 2) / sigmaXSquared;
        sum += Math.Pow(dY, 2) / sigmaYSquared;
        sum -= 2 * rho * dX * dY / (sigmaX * sigmaY);

        exponent *= sum;

        return Math.Exp(exponent) / normaliser;
    }

    public static double GetTrivariateGuassian(double muX, double sigmaX, double muY, double sigmaY, double muZ, double sigmaZ, double x, double y, double z)
    {
        var dX = x - muX;
        var dY = y - muY;
        var dZ = z - muZ;

        var exponent = Math.Pow(dX, 2) * sigmaY * sigmaZ + Math.Pow(dY, 2) * sigmaX * sigmaZ + Math.Pow(dZ, 2) * sigmaX * sigmaY;
        exponent *= (-1 / (2 * sigmaX * sigmaY * sigmaZ));
        var normaliser = Math.Pow(2 * Math.PI, 1.5) * Math.Sqrt(sigmaX * sigmaY * sigmaZ);

        return Math.Exp(exponent) / normaliser;
    }

    public double rew;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var count = (actionBuffers.ContinuousActions[0] + 1) / 2 * maxcount;
        var dist = (actionBuffers.ContinuousActions[1] + 1) / 2 * maxdist;
        var time = 0.0f;

        ((RectTransform)graph2D.GetChild(logLen + 1)).anchoredPosition = new Vector3(count / maxcount, dist / maxdist, 0) * 100;

        if (trivariate)
        {
            time = (actionBuffers.ContinuousActions[2] + 1) / 2 * maxdist;
            ((RectTransform)graph1D.GetChild(logLen + 1)).anchoredPosition = new Vector3(time / maxdist, 0, 0) * 100;
            // rew = GetTrivariateGuassian(userMean3.x, 5, userMean3.y, 10 * maxdist / maxcount, userMean3.z, 10 * maxdist / maxcount, count, dist, time) * 10000 / maxGaus;
            rew = Math.Sqrt(Math.Pow(userMean3.x - count, 2) + Math.Pow(userMean3.y - dist, 2) + Math.Pow(userMean3.z - time, 2)) / Math.Sqrt(Math.Pow(maxcount, 2) + Math.Pow(maxdist, 2) + Math.Pow(maxdist, 2));
            rew = 1 - rew;
            // Debug.Log("rew: " + rew);
        }
        else
        {
            rew = GetBivariateGuassian(userMean.x, 5, userMean.y, 10 * maxdist / maxcount, count, dist, 0) / maxGaus;
            // Debug.Log("rew: " + rew + "\nx: " + (actionBuffers.ContinuousActions[0] + 1) / 2 + " realX: " + (float)userMean.x / maxcount + "\n" +
            // "y: " + (actionBuffers.ContinuousActions[1] + 1) / 2 + " realY: " + userMean.y / maxdist + "\n"
            // );
        }

        GameManager.Gmr.updateFlagDist();

        foreach (Flag flag in GameManager.Gmr.flags)
        {
            if (trivariate)
            {
                flag.fitness = Vector3.Distance(new Vector3(flag.visited, flag.dist, flag.time), new Vector3(count, dist, time));
            }
        }
        GameManager.Gmr.flagFitness = GameManager.Gmr.flags.OrderBy(v => v.fitness).ThenBy(v => v.dist).ToArray<Flag>();
        GameManager.Gmr.recommend(GameManager.Gmr.flagFitness[0].id, 0, true);

        text_rank.text = "";
        foreach (Flag flag in GameManager.Gmr.flagFitness)
        {
            text_rank.text += flag.id + "\n";
        }

        AddReward((float)rew);

        return;

    }




}
