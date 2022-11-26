using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserIconScript : MonoBehaviour
{
    public Transform user;
    // Start is called before the first frame update
    private void FixedUpdate()
    {
        transform.position = user.position;
        transform.position += Vector3.up * 30;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
