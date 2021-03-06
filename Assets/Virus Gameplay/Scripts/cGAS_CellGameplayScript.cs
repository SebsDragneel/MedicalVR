﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class cGAS_CellGameplayScript : MonoBehaviour
{
    public SubstitlesScript subtitles;
    public Transform dna;
    private float moveSpeed = .1f;
    private float vComparer = .01f;

    IEnumerator MoveTo(Transform t)
    {
        Vector3 start = t.position;
        Vector3 pos = t.position + (dna.position - t.position) * .95f;

        while (!V3Equal(t.position, pos))
        {
            t.position = Vector3.MoveTowards(t.position, pos, moveSpeed/4);
            yield return new WaitForFixedUpdate();
        }
    }

    public void DoAction(Transform t)
    {
        if (((int)subtitles.GetComponent<SubstitlesScript>().theTimer == 44))
        {
            subtitles.GetComponent<SubstitlesScript>().theTimer += 1;
            subtitles.GetComponent<SubstitlesScript>().Continue();
            StartCoroutine(MoveTo(t));
        }
    }

    private bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < vComparer;
    }
}
