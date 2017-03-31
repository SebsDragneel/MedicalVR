﻿using UnityEngine;
using System.Collections;

public class MovingCellsScript : MonoBehaviour {

    public GameObject Cam;
    public float speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Vector3.Distance(transform.position, Cam.transform.position) < 3000)
        {
            if (GetComponent<Renderer>() != null)
            GetComponent<Renderer>().enabled = true;
            else
               GetComponentInChildren<Renderer>().enabled = true;
        }
        else
        {
            if (GetComponent<Renderer>() != null)
                GetComponent<Renderer>().enabled = false;
            else
                GetComponentInChildren<Renderer>().enabled = false;
        }
    }
    void FixedUpdate()
    {
        transform.position += transform.forward * speed;
    }
}
