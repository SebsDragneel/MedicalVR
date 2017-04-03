﻿using UnityEngine;
using System.Collections;

public class BannerScript : MonoBehaviour {

    public GameObject target, hidden;
    public float speed;

    bool show = false;
    bool hide = false;
    float theTimer = 0;
    //Vector3 orgPos;
	// Use this for initialization
	void Start ()
    {
        //orgPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(show == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime*speed);
            if(transform.position == target.transform.position)
            {
                show = false;
                hide = true;
            }

        }
        if (hide == true)
        {
            theTimer += Time.deltaTime;
            if(theTimer > 2.0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, hidden.transform.position, Time.deltaTime*speed);
                if (transform.position == hidden.transform.position)
                {
                    hide = false;
                    theTimer = 0;
                }
            }
        }
	}
    public void ShowUp()
    {
        show = true;
    }
}
