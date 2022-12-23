using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagColor : MBaske.Sensors.Grid.DetectableGameObject
{
    public Material yellowMat;
    public Material redMat;

    public void yellow()
    {
        transform.GetChild(1).GetComponent<MeshRenderer>().material = yellowMat;
    }

    public void red()
    {
        transform.GetChild(1).GetComponent<MeshRenderer>().material = redMat;
    }

}
