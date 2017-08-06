﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    // Use this for initialization
    void OnEnable()
    {
        StartCoroutine(FadeOutObject(gameObject.GetComponent<Renderer>()));
    }

    IEnumerator FadeOutObject(Renderer g)
    {
        g.enabled = true;
        if (g.material.HasProperty("_Color"))
        {
            Color c = g.material.color;
            c.a = 1.0f;
            float startTime = Time.time;
            float t = 0.0f;
            while (t < 1)
            {
                t = Time.time - startTime;
                c.a = 1.0f - t;
                g.material.color = c;
                yield return 0;
            }
            c.a = 0.0f;
            g.material.color = c;
        }
        g.enabled = false;
        enabled = false;
    }
}
