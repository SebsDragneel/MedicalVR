﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimonSays : MonoBehaviour {

    // Use this for initialization
    public GameObject yellow;
    public GameObject red;
    public GameObject blue;
    public GameObject green;
    public GameObject sign;

    Color cY, cR, cB, cG;

    int round = 1, inputed = 0, shownSign = 0;

    float timer = 3;

    bool makeInput = false, buttonPressed = false, showStuff = false, waitAsec = false;
    enum theColors
    {
        YELLOW,
        RED,
        BLUE,
        GREEN
    }

    theColors selectedColor;

    List<theColors> theList = new List<theColors>();
    List<theColors> inputedColors = new List<theColors>();
    void Start ()
    {
       
        cY = yellow.GetComponent<Renderer>().material.color;
        cR = red.GetComponent<Renderer>().material.color;
        cB = blue.GetComponent<Renderer>().material.color;
        cG = green.GetComponent<Renderer>().material.color;
        selectedColor = (theColors)5;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        timer += Time.deltaTime;
        WaitForTime();
       
        if(waitAsec == false)
        {
            if (showStuff == false && makeInput == false)
            {
                ResetStuff();
                GeneratePattern();
                showStuff = true;
            }
            if (makeInput)
            {
                sign.GetComponent<MeshRenderer>().enabled = false;
                TakeInput();
            }
            if (showStuff)
            {
                ShowPattern();
                timer = 0;
                waitAsec = true;
            }
        }
        
        if (shownSign == theList.Count && showStuff)
        {
            makeInput = true;
            showStuff = false;
        }
        else
            return;
    }
   void WaitForTime()
    {
        if(waitAsec)
        {
            if (timer > 1.0)
            {
                sign.GetComponent<MeshRenderer>().enabled = false;
                TurnOffLights();
            }
            if (timer > 2.0)
                waitAsec = false;
        }
    }
    void GeneratePattern()
    {
        for (int i = 0; i < round; i++)
        {
            theColors tmpColor = new theColors();
            tmpColor = (theColors)Random.Range(0, 3);
            theList.Add(tmpColor);
        }
    }
    void ShowPattern()
    {
       
        switch (theList[shownSign])
        {
            case theColors.YELLOW:
                sign.GetComponent<Renderer>().material.color = Color.yellow;
                break;
            case theColors.RED:
                sign.GetComponent<Renderer>().material.color = Color.red;
                break;
            case theColors.BLUE:
                sign.GetComponent<Renderer>().material.color = Color.blue;
                break;
            case theColors.GREEN:
                sign.GetComponent<Renderer>().material.color = Color.green;
                break;
            default:
                break;
        }
        sign.GetComponent<MeshRenderer>().enabled = true;
        shownSign++;
    }
    void TakeInput()
    {
        if (buttonPressed)
        {
           
            switch (selectedColor)
            {
                case theColors.YELLOW:
                    yellow.GetComponent<Renderer>().material.color = Color.yellow;
                    break;
                case theColors.RED:
                    red.GetComponent<Renderer>().material.color = Color.red;
                    break;
                case theColors.BLUE:
                    blue.GetComponent<Renderer>().material.color = Color.blue;
                    break;
                case theColors.GREEN:
                    green.GetComponent<Renderer>().material.color = Color.green;
                    break;
                default:
                    break;
            }

            inputedColors.Add(selectedColor);
            buttonPressed = false;
            inputed++;
            CheckInput();
        }          
    }
    void CheckInput()
    {
        bool correct = true;
        for (int i = 0; i < inputed; i++)
        {
            if (inputedColors[i] != theList[i])
            {
                correct = false;
                break;
            }
        }
        if (correct)
        {
            if (inputed == round)
            {
                TurnOnLights();
                round++;
                waitAsec = true;
                timer = 0;
                //ResetStuff();
                showStuff = false;
                makeInput = false;
            }

        }
        else
        {
            SignalIncorrect();
            // ResetStuff();
            waitAsec = true;
            timer = 0;
            showStuff = false;
            makeInput = false;
        }
    }

    void ResetStuff()
    {
        theList.Clear();
        inputedColors.Clear();
        makeInput = false;
        inputed = 0;
        shownSign = 0;
        TurnOffLights();
    }
    void SignalIncorrect()
    {
        yellow.GetComponent<Renderer>().material.color = Color.black;
        red.GetComponent<Renderer>().material.color = Color.black;
        blue.GetComponent<Renderer>().material.color = Color.black;
        green.GetComponent<Renderer>().material.color = Color.black;
    }
    void TurnOnLights()
    {
        yellow.GetComponent<Renderer>().material.color = Color.yellow;
        red.GetComponent<Renderer>().material.color = Color.red;
        blue.GetComponent<Renderer>().material.color = Color.blue;
        green.GetComponent<Renderer>().material.color = Color.green;
    }
    void TurnOffLights()
    {
        yellow.GetComponent<Renderer>().material.color = cY;
        red.GetComponent<Renderer>().material.color = cR;
        blue.GetComponent<Renderer>().material.color = cB;
        green.GetComponent<Renderer>().material.color = cG;
    }
   public void selectYellow()
    {   if(makeInput)
        {
            selectedColor = theColors.YELLOW;
            buttonPressed = true;
            TurnOffLights();
        }
       
    }
    public void selectRed()
    {
        if (makeInput)
        {
            selectedColor = theColors.RED;
            buttonPressed = true;
            TurnOffLights();
        }
    }
    public void selectBlue()
    {
        if (makeInput)
        {
            selectedColor = theColors.BLUE;
            buttonPressed = true;
            TurnOffLights();
        }
    }
    public void selectGreen()
    {
        if (makeInput)
        {
            selectedColor = theColors.GREEN;
            buttonPressed = true;
            TurnOffLights();
        }
    }
}
