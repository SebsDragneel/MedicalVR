﻿using UnityEngine;
using System.Collections;
using TMPro;

public class ManageScores : MonoBehaviour {

    public GameObject MemoryScoreText;
    public GameObject SimonDNAScoreText;
    public GameObject cGAMPScoreText;
    public GameObject DodgeScoreText;
    public GameObject VirusScoreText;
    public GameObject CellScoreText;
    public GameObject StrategyScoreText;
    public GameObject ATPScoreText;


    // Use this for initialization
    void Start () {
	
	}

    
	
	// Update is called once per frame
	void Update () {

        MemoryScoreText.GetComponent<TextMeshPro>().text = "Memory Game: " + MemoryUI.finalScore.ToString();
        SimonDNAScoreText.GetComponent<TextMeshPro>().text = "Simon DNA: " + SimonSays.finalScore.ToString();
        cGAMPScoreText.GetComponent<TextMeshPro>().text = "cGAMP Snatcher: " + Storebullets.finalScore.ToString();
        DodgeScoreText.GetComponent<TextMeshPro>().text = "Dodge Antibodies: " + MovingCamera.finalScore.ToString();  _TGameController.finalATPScore.ToString();
        VirusScoreText.GetComponent<TextMeshPro>().text = "Fight Virus: " + Player.BestScoreForFightVirus.ToString();
        CellScoreText.GetComponent<TextMeshPro>().text = "Destroy Cell: " + VirusPlayer.FinalScore.ToString();
        StrategyScoreText.GetComponent<TextMeshPro>().text = "";
        ATPScoreText.GetComponent<TextMeshPro>().text = "ATP/GTP Shooter: " + _TGameController.finalATPScore.ToString();

        FBscript.GlobalScore = ((int)MemoryUI.finalScore + (int)Storebullets.finalScore + (int)_TGameController.finalATPScore) % 1000;

    }
}
