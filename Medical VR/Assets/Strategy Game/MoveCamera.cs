﻿using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour
{
    private static MoveCamera localInstance;
    public static MoveCamera instance { get { return localInstance; } }

    private float speed = 10;
    private float startTime;
    private float distance;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Awake()
    {
        if (localInstance != null && localInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            localInstance = this;
        }
    }

    // Use this for initialization
    void Start()
    {
        startPosition = endPosition = transform.position;
        distance = .001f;
        startTime = Time.time;
        GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
    }

    IEnumerator Move()
    {
        while (transform.position != endPosition)
        {
            float disCovered = (Time.time - startTime) * speed;
            float fracJourney = disCovered / distance;
            if (fracJourney <= 1.0f)
            {
                GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(startPosition, endPosition, fracJourney));
            }
            else
            {
                GetComponent<Rigidbody>().MovePosition(endPosition);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void SetDestination(Vector3 endpoint)
    {
        endPosition = endpoint;
        startPosition = transform.position;
        distance = Mathf.Max(Vector3.Distance(startPosition, endPosition), .001f);
        startTime = Time.time;
        StopCoroutine(Move());
        StartCoroutine(Move());
    }
}