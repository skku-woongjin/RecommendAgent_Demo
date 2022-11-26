using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Parsing : MonoBehaviour
{
    public string filename;
    private void Start()
    {
        List<Dictionary<string, object>> data_Dialog = CSVReader.Read(filename);

        for (int i = 0; i < data_Dialog.Count; i++)
        {
            Debug.Log(Int32.Parse(data_Dialog[i]["user"].ToString()));
            Debug.Log(data_Dialog[i]["line"].ToString());
        }
    }
}
