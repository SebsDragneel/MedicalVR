﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour, TimedInputHandler
{
    public int index = 0;

    public void LoadScene(string scene)
    {
        SoundManager.PlaySFX("MenuEnter");
        SceneManager.LoadScene(scene);
    }

    public void Exit()
    {
        SoundManager.PlaySFX("MenuEnter");
        Application.Quit();
    }

    public void EnterEvent()
    {
        SoundManager.PlaySFX("MenuEnter");
        switch (index)
        {
            case 0:
                LoadScene("Strategy");
                break;
            case 1:
                LoadScene("MemoryGame");
                MemoryUI.arcadeMode = true;
                break;
            case 2:
                Player.ArcadeMode = true;
                Player.TutorialMode = false;
                LoadScene("FightVirus");
                break;
            case 3:
                LoadScene("DodgeAnitbodies");
                MovingCamera.arcadeMode = true;
                break;
            case 4:
                SimonSays.arcadeMode = true;
                LoadScene("SimonDNA");
                break;
            case 5:
                LoadScene("CGampSnatcher");
                break;
            case 6:
                LoadScene("ATPGTPShooter");
                break;
            case 7:
                LoadScene("MainMenu");
                break;
            case 8:
                Exit();
                break;
            case 9:
                LoadScene("MinigameMenu");
                break;
            case 10:
                LoadScene("TrophyRoom");
                break;
            case 11:
                LoadScene("Credits");
                break;
            case 12:
                LoadScene("OptionsMenu");
                break;
            case 13:
                VirusPlayer.ArcadeMode = true;
                VirusPlayer.TutorialMode = false;
                LoadScene("DestroyTheCell");
                break;
            case 14:
                LoadScene("Virus Gameplay Scene");
                break;
        }
    }

    public void Highlight(int anything)
    {
        index = anything;
    }

    public void HandleTimeInput()
    {
        Highlight(index);
        EnterEvent();
    }
}



