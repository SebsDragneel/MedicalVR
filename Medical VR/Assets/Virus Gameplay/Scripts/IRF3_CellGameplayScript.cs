﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IRF3_CellGameplayScript : MonoBehaviour {

    public GameObject subtitles, leftP, rightP;
    public List<GameObject> places;
    int I = 0;
    float moveSpeed = 0;
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = Vector3.MoveTowards(transform.position, places[I].transform.position, Time.deltaTime * moveSpeed);
        switch ((int)subtitles.GetComponent<SubstitlesScript>().theTimer)
        {
            case 86:
                I = 1;
                transform.position = places[I].transform.position;
                break;
            case 90:
                I = 2;
                moveSpeed = 200;
                break;
            default:
                break;
        }

    }
}
