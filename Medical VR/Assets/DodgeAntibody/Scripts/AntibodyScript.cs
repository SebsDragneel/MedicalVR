﻿using UnityEngine;
using System.Collections;

public class AntibodyScript : MonoBehaviour
{

    public GameObject Cam, Effects, banner;
    bool reswpawn = false;
    float saveSpeed;
    
    public GameObject TutorialMode;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, Cam.transform.position) < 3000)
        {
            GetComponent<Renderer>().enabled = true;
        }
        else
        {
            GetComponent<Renderer>().enabled = false;
        }
        if (reswpawn == true)
        {
            if (Effects.GetComponent<ParticleSystem>().isPlaying == false)
            {
                Cam.GetComponent<MovingCamera>().stopMoving = true;
                Cam.GetComponent<MovingCamera>().speed = -100;
                Cam.GetComponent<SphereCollider>().enabled = false;
               
                if (TutorialMode == false)
                {
                    if (Cam.transform.position == Cam.GetComponent<MovingCamera>().originPos)
                    {
                        reswpawn = false;
                        Cam.GetComponent<MovingCamera>().stopMoving = false;
                        Cam.GetComponent<MovingCamera>().speed = saveSpeed;
                        Cam.GetComponent<SphereCollider>().enabled = true;
                    }
                    // Cam.GetComponent<MovingCamera>().LoseresetPos();
                    //Cam.GetComponent<MovingCamera>().speed = saveSpeed;
                }
                else if (TutorialMode.GetComponent<DodgeAntiBodyTutorial>().WhiteCellHitsPlayerFirstTime == true)
                {
                    Cam.GetComponent<MovingCamera>().stopMoving = false;
                    Cam.GetComponent<MovingCamera>().speed =10;
                    Cam.GetComponent<SphereCollider>().enabled = true;
                    reswpawn = false;
                    TutorialMode.GetComponent<DodgeAntiBodyTutorial>().RepawnPlayer();
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "virus")
        {
            other.transform.position -= other.transform.forward * 50;
            saveSpeed = other.GetComponent<MovingCamera>().speed;
            other.GetComponent<MovingCamera>().speed = 0;
            reswpawn = true;
            Effects.GetComponent<ParticleSystem>().Stop();
            Effects.GetComponent<ParticleSystem>().Play();
           
            BannerScript.UnlockTrophy("White Cell");
            Cam.GetComponent<MovingCamera>().LoseresetPos();
            //For tutorial mode 
            if (GlobalVariables.tutorial)
            {
                if (TutorialMode.GetComponent<DodgeAntiBodyTutorial>().WhiteCellHitsPlayerFirstTime == false)
                {
                    TutorialMode.GetComponent<DodgeAntiBodyTutorial>().MoveStoy();
                    TutorialMode.GetComponent<DodgeAntiBodyTutorial>().WhiteCellHitsPlayerFirstTime = true;
                }

                //else if (TutorialMode.GetComponent<DodgeAntiBodyTutorial>().WhiteCellHitsPlayerFirstTime == true)
                //{
                //    TutorialMode.GetComponent<DodgeAntiBodyTutorial>().RepawnPlayer();
                //}
            }
        }
    }
}
