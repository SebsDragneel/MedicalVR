﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Virus_VirusGameplay : MonoBehaviour {

    public List<GameObject> places;
    public GameObject subtitltes, manager;

    delegate void Func();
    Func doAction;
    public float moveSpeed;
    float fadeSpeed;
    int I = 0;
    void Start () {
        doAction = NullFunction;
        switch (VirusGameplayScript.loadCase)
        {
            case (1):
                I = 2;
                transform.position = places[I].transform.position;
                break;
            default:
                break;
        }
    }
    void NullFunction()
    {

    }
    // Update is called once per frame
    void Update ()
    {
        CheckCaases();
        doAction();
        MoveTo();
    }
    void MoveTo()
    {
        if (I != places.Count)
            transform.position = Vector3.MoveTowards(transform.position, places[I].transform.position, moveSpeed * Time.deltaTime);
    }
    void CheckCaases()
    {
        switch ((int)subtitltes.GetComponent<SubstitlesScript>().theTimer)
        {
            case (105):
                I = 1;
                moveSpeed = 450;
                break;
            case (130):
                I = 3;
                moveSpeed = 50;
                break;
            case (145):
                I = 4;
                moveSpeed = 50;
                break;
            default:
                break;
        }
    }
}
