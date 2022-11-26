using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : MonoBehaviour
{
    public Transform owner;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void stop()
    {
        transform.SetParent(null);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void resume()
    {
        transform.SetParent(owner);
    }
}
