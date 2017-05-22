﻿using UnityEngine;
using System.Collections;


enum ShotNumber { ATPOne, ATPTwo, ATPThree, GTPOne, GTPTwo, GTPThree, reset };

public class _TPlayerController : MonoBehaviour
{
    private ShotNumber currentRound;

    public GameObject Camera;

    public float fireRate;
    public GameObject[] shot;
    public Transform shotSpawn;
    public int shotInClip;

    private float nextFire;
    private bool isATP;
    //private AudioSource audioSource;
    bool isActive = false;
    
    bool isInit = false;
    
    public void Initialize()
    {
        if (isInit)
            return;
        isInit = true;

        currentRound = ShotNumber.ATPOne;
        isATP = true;
    }

    public void SetActiveShooting(bool setActive)
    {
        isActive = setActive;
    }

    void Update()
    {
        bool bPressed = Input.GetButtonDown("Fire1");
        
        if (bPressed && Time.time > nextFire)
        {
            SetFireMode();
        }
    }
    void SetFireMode()
    {
        if (!isActive)
            return;

        if (isATP)
            shootATP();
        else
            shootGTP();

        if (++currentRound == ShotNumber.reset)
            currentRound = ShotNumber.ATPOne;
        if (currentRound == ShotNumber.GTPOne || currentRound == ShotNumber.ATPOne)
        {
            isATP = !isATP;
            nextFire += fireRate * 3;
        }
    }
    private void FixedUpdate()
    {
        gameObject.transform.rotation = Camera.transform.rotation;
    }
    private void shootATP()
    {
        if (shot[0])
        {
            nextFire = Time.time + fireRate;
            Instantiate(shot[0], shotSpawn.position, shotSpawn.rotation);
        }
    }
    private void shootGTP()
    {
        if (shot[1])
        {
            nextFire = Time.time + fireRate;
            Instantiate(shot[1], shotSpawn.position, shotSpawn.rotation);
        }
    }
    public int GetShotNumber()
    {
        return (int)currentRound;
    }
}