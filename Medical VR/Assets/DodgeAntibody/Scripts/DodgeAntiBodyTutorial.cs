﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DodgeAntiBodyTutorial : MonoBehaviour
{
    public GameObject Subtitles;
    public GameObject Player;
    public GameObject RedCell;
    public GameObject WhiteCell;

    public Transform PlayerTutorialLocation;
    public Transform CellTutorialLocation;

    public bool WhiteCellHitsPlayerFirstTime = false;
    Vector3 SavedRedCellPosition;
    public AntibodySpawnerScript spawner;
    public bool prevState;

    //For text
    public int MoveText = 0;
    public bool CanIRead = true;
    public TextMeshPro Text;
    private string[] TextList = new string[10];
    private bool last = false, text = false, finish = false;

    //For events
    bool WaitForRedCell = false;
    bool WaitForWhiteCell = false;
    bool WaitForPlayerMovement = false;
    public bool WaitForPlayerCollide = false;
    public bool StopInput = false;

    void Start()
    {
        if (GlobalVariables.tutorial)
        {
            prevState = GlobalVariables.arcadeMode;
            GlobalVariables.arcadeMode = false;

            //Set the player in the other tunnel
            Player.transform.position = PlayerTutorialLocation.position;
            Player.GetComponent<MovingCamera>().stopMoving = true;

            //Save red cell orginal position
            SavedRedCellPosition = RedCell.transform.position;
            RedCell.transform.position = CellTutorialLocation.position;

            TextList[0] = "Alright Human, this is what you have to do.";
            TextList[1] = "Your objective is to lead the virion to the end of the blood vessel where this red cell is normally located.";
            TextList[2] = "You will want to watch out for these white blood cells or they will reset you back to the beginning.";
            TextList[3] = "You will be controlling where the virion goes by facing the direction you want to head towards to.";
            TextList[4] = "Trying to go to the opposite direction will pause your movemet.";
            TextList[5] = "Reach the red cell to proceed to the next steps. Good luck.";
            TextForTutorial();
        }

        else
            enabled = false;
    }

    void Update()
    {
        if (StopInput == false)
        {
            bool held = Input.GetButton("Fire1");
            if (held && !last)
            {
                if (text)
                    finish = true;

                else
                    TextForTutorial();
            }

            last = held;
        }
    }

    void TextForTutorial()
    {
        switch (MoveText)
        {
            case 0:
                StartCoroutine(TurnTextOn(0));
                break;

            case 1:
                StartCoroutine(TurnTextOn(1));
                break;

            case 2:
                WaitForRedCell = true;
                StopInput = true;
                break;

            case 3:
                StartCoroutine(TurnTextOn(2));
                break;

            case 4:
                WaitForWhiteCell = true;
                StopInput = true;
                break;

            case 5:
                StartCoroutine(TurnTextOn(3));
                break;

            case 6:
                WaitForPlayerMovement = true;
                StopInput = true;
                break;

            case 7:
                StartCoroutine(TurnTextOn(4));
                break;

            case 8:
                StartCoroutine(TurnTextOn(5));
                break;

            case 9:
                Player.GetComponent<MovingCamera>().speed = 5.0f;
                Player.GetComponent<MovingCamera>().stopMoving = false;
                Debug.Log("in Case 9");

                break;

            default:
                break;
        }
    }

    void FixedUpdate()
    {
        if (WaitForRedCell == true)
        {
            RedCell.transform.position = Vector3.MoveTowards(RedCell.transform.position, SavedRedCellPosition, 10.0f * Time.fixedDeltaTime);

            if (RedCell.transform.position == SavedRedCellPosition)
            {
                MoveText += 1;
                WaitForRedCell = false;
                StopInput = false;
            }
        }

        if (WaitForWhiteCell == true)
        {
            WhiteCell.transform.position = Vector3.MoveTowards(WhiteCell.transform.position, CellTutorialLocation.position, 10.0f * Time.fixedDeltaTime);
            if (WhiteCell.transform.position == CellTutorialLocation.position)
            {
                WhiteCell.GetComponent<SphereCollider>().enabled = true;
                MoveText += 1;
                WaitForWhiteCell = false;
                StopInput = false;
            }
        }

        if (WaitForPlayerMovement == true)
        {
            Player.GetComponent<MovingCamera>().speed = 5.0f;
            Player.GetComponent<MovingCamera>().stopMoving = false;
            Debug.Log("in WaitForPlayer");

            if (WhiteCell)
                WhiteCell.transform.position = Vector3.MoveTowards(WhiteCell.transform.position, Player.transform.position, 1.0f * Time.fixedDeltaTime);

            WaitForPlayerMovement = false;
        }

        //By this point the white cell is already moving
        //Looking for collision 
    }

    public void MoveStoy()
    {
        Destroy(WhiteCell);
        spawner.enabled = true;
    }

    IEnumerator TurnTextOn(int index)
    {
        while (text)
            yield return 0;

        text = true;
        Text.text = " ";

        while (Text.text != TextList[index] && !finish)
        {
            yield return new WaitForSeconds(GlobalVariables.textDelay);

            if (Text.text.Length == TextList[index].Length)
                Text.text = TextList[index];

            else
                Text.text = Text.text.Insert(Text.text.Length - 1, TextList[index][Text.text.Length - 1].ToString());
        }

        Text.text = TextList[index];
        finish = false;
        text = false;
        MoveText += 1;
    }
}
