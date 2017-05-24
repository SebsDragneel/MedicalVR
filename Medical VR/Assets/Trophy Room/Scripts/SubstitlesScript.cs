﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class SubstitlesScript : MonoBehaviour
{
    [System.Serializable]
    public class Subtitle
    {
        public string text;
        public float start, end;
    }
    // Use this for initialization
    public static AudioSource voice;
    public List<Subtitle> theSubtitles = new List<Subtitle>();
    public float theTimer = 0;
    public float textSpeed;
    public bool stopTime = false;
    public GameObject pressToContiue;
    float textTimer = 0, voiceTimer = 0;
    string theText = "";
    int textIdx = 0;
    int i = 0;
    bool stop = false, done = false, skip = true;
    void Start()
    {
        textSpeed = GlobalVariables.textDelay;
        if (SceneManager.GetActiveScene().name.Contains("CellGameplay"))
        {
            switch (CellGameplayScript.loadCase)
            {
                case 1:
                    i = 14;
                    break;
                case 2:
                    i = 21;
                    break;
                case 3:
                    i = 35;
                    break;
                case 4:
                    i = 40;
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (VirusGameplayScript.loadCase)
            {
                case 1:
                    i = 26;
                    break;
                case 2:
                    i = 54;
                    break;
                case 3:
                    i = 63;
                    break;
                default:
                    break;
            }
        }
        if (pressToContiue != null)
            pressToContiue.SetActive(false);

        if (theSubtitles.Count == 0)
            stop = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (stop == false)
        {
            if (stopTime == false)
                theTimer += Time.deltaTime;

            if (theTimer >= theSubtitles[i].start)
            {
                if(theTimer <= theSubtitles[i].end)
                {
                    if (Input.GetButton("Fire1") && stopTime == true)
                    {
                        Next();
                    }
                    else
                    {
                        textTimer += Time.deltaTime;
                        if (textTimer > textSpeed)
                        {
                            textTimer = 0;

                            if (textIdx < theSubtitles[i].text.Length)
                            {
                                theText = theText + theSubtitles[i].text[textIdx];
                                textIdx++;
                            }

                            else if (theTimer >= theSubtitles[i].end - 0.1)
                            {
                                stopTime = true;
                                skip = false;
                            }
                            if (stopTime == true)
                            {
                                if (voiceTimer >= 0.01)
                                {
                                    if (voice != null)
                                    {
                                        if (voice.isPlaying == true && pressToContiue.activeSelf == false)
                                            Next();
                                    }
                                    else
                                    {
                                        if (pressToContiue != null)
                                            pressToContiue.SetActive(true);
                                    }
                                }
                                voiceTimer += Time.deltaTime;
                            }
                            GetComponent<TMPro.TextMeshPro>().text = theText;
                        }

                    }
                }
                else
                {
                    if (skip == true)
                        i++;
                }
               
            }
            else
                GetComponent<TMPro.TextMeshPro>().text = "";
        }
        if (done == true)
        {
            theTimer += Time.deltaTime;
        }
    }

    void Next()
    {
        skip = true;
        if (pressToContiue != null)
            pressToContiue.SetActive(false);
        i++;
        stopTime = false;
        //theTimer = theSubtitles[i].start;
        textTimer = 0;
        textIdx = 0;
        theText = "";
        if (i >= theSubtitles.Count)
        {
            theTimer = theSubtitles[i - 1].end;
            stop = true;
            done = true;
        }
        GetComponent<TMPro.TextMeshPro>().text = "";
    }
    public void Replay()
    {
        if (stop == true)
        {
            stop = false;
            done = false;
            i = 0;
            theTimer = 0;
        }
    }
    public void Stop()
    {
        stop = true;
        GetComponent<TMPro.TextMeshPro>().text = "";
    }
    public void Continue()
    {
        stop = false;
    }
    public bool IsDone()
    {
        return done;
    }
}
