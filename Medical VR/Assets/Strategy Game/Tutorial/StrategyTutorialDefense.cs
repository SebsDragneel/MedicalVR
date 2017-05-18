﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StrategyTutorialDefense : MonoBehaviour
{
    public Transform cam;
    public GameObject eventSystem;
    public GameObject fade;
    public LookCamera lc;
    public GameObject reticle;
    public GameObject immunityParticles;
    public GameObject objects;
    public GameObject fuzeon;
    public TMPro.TextMeshPro defDes;
    public TMPro.TextMeshPro immDes;
    public TMPro.TextMeshPro subtitles;
    public GameObject[] def = new GameObject[0];
    public GameObject[] imm = new GameObject[0];
    public GameObject[] cells = new GameObject[7];
    public GameObject[] viruses = new GameObject[4];

    private string[] texts =
        {
        "The Defense stat delays viruses from killing your cells.",
        "When a virus spawns, it targets a random cell.",
        "When a cell is targeted, it turns black.",
        "Once the virus reaches the cell it attempts to penetrate the cell's membrane.",
        "You can't upgrade the cell while it is defending itself.",
        "But, other cells can spread immunity to it.",
        "During this time immunity is received at twice the value.",
        "Defense is also the only stat that is copied to the child.",
        "Due to this, it is highly advised that you invest into this stat early on.",
        "Alternatively, you could put points into a cell when it becomes targeted by a virus.",
        "The Fuzeon power-up permanently increases defense by 5.",
        "If you are lucky enough you can get the Defense event.",
        "It raises all your cells defense to the max defense in your colony +5.",
        "Be careful, different viruses have different attack values.",
        "The attack values will also increase as the game continues.",
        };
    private Vector3 prevPos;
    private Quaternion prevRotation;
    private int index = 1;
    private bool last = false, text = false, finish = false, clickable = false;
    private List<Coroutine> stop = new List<Coroutine>();
    private Color c;

    void OnEnable()
    {
        eventSystem.SetActive(false);
        clickable = false;
        if (cam == null)
            cam = Camera.main.transform.parent;

        prevPos = cam.position;
        prevRotation = lc.transform.rotation;
        //Fade In
        fade.GetComponent<FadeIn>().enabled = true;
        index = 1;
        Invoke("Click", 1);
    }

    void Update()
    {
        bool held = Input.GetButton("Fire1");
        if (held && !last)
        {
            if (text)
            {
                finish = true;
            }
            else
            {
                if (clickable)
                    Click();
            }
        }
        last = held;
    }

    void Click()
    {
        switch (index)
        {
            case 1:
                //Fade Out
                objects.SetActive(true);
                reticle.SetActive(false);
                cam.position = transform.position;
                lc.target = cells[0].transform;
                lc.enabled = true;
                defDes.text = "Defense: 0";
                immDes.text = "Immunity: 0";
                fade.GetComponent<FadeOut>().enabled = true;
                Invoke("Click", 1);
                break;
            case 2:
                //The Defense stat delays viruses from killing your cells.
                StartCoroutine(TurnTextOn(0));
                stop.Add(StartCoroutine(GrowInObject(cells[0])));
                foreach (GameObject item in def)
                {
                    stop.Add(StartCoroutine(FadeInObject(item)));
                    stop.Add(StartCoroutine(FadeInText(item.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>())));
                }
                clickable = true;
                break;
            case 3:
                foreach (Coroutine co in stop)
                {
                    StopCoroutine(co);
                }
                stop.Clear();
                cells[0].transform.localScale = Vector3.one;
                foreach (GameObject item in def)
                {
                    c = item.GetComponent<Renderer>().material.color;
                    c.a = 1;
                    item.GetComponent<Renderer>().material.color = c;
                    item.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().color = Color.black;
                }

                //When a virus spawns, it targets a random cell.
                StartCoroutine(TurnTextOn(1));
                viruses[0].transform.position = cells[0].transform.position + new Vector3(0, 2, 0);
                stop.Add(StartCoroutine(GrowInObject(viruses[0])));
                break;
            case 4:
                foreach (Coroutine co in stop)
                {
                    StopCoroutine(co);
                }
                stop.Clear();
                viruses[0].SetActive(true);
                viruses[0].transform.localScale = Vector3.one;

                //When a cell is targeted, it turns black.
                StartCoroutine(TurnTextOn(2));
                stop.Add(StartCoroutine(PaintItBlack(cells[0])));
                break;
            case 5:
                foreach (Coroutine co in stop)
                {
                    StopCoroutine(co);
                }
                stop.Clear();
                cells[0].GetComponent<Renderer>().material.color = Color.black;

                //Once the virus reaches the cell it attempts to penetrate the cell's membrane.
                StartCoroutine(TurnTextOn(3));
                stop.Add(StartCoroutine(MoveVirus()));
                break;
            case 6:
                foreach (Coroutine co in stop)
                {
                    StopCoroutine(co);
                }
                stop.Clear();
                viruses[0].GetComponent<Rotate>().enabled = false;
                cells[0].GetComponent<Rotate>().enabled = false;
                viruses[0].transform.position = cells[0].transform.position + new Vector3(0, .9f, 0);

                //You can't upgrade the cell while it is defending itself.
                StartCoroutine(TurnTextOn(4));
                foreach (GameObject item in def)
                {
                    stop.Add(StartCoroutine(FadeOutObject(item)));
                    stop.Add(StartCoroutine(FadeOutText(item.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>())));
                }
                break;
            case 7:
                foreach (Coroutine co in stop)
                {
                    StopCoroutine(co);
                }
                stop.Clear();
                foreach (GameObject item in def)
                {
                    item.SetActive(false);
                }

                //But, other cells can spread immunity to it.
                StartCoroutine(TurnTextOn(5));
                stop.Add(StartCoroutine(GrowInObject(cells[1])));
                stop.Add(StartCoroutine(GrowInObject(cells[2])));
                stop.Add(StartCoroutine(GrowInObject(cells[3])));
                stop.Add(StartCoroutine(GrowInObject(cells[4])));
                stop.Add(StartCoroutine(GrowInObject(cells[5])));
                stop.Add(StartCoroutine(GrowInObject(cells[6])));
                foreach (GameObject item in imm)
                {
                    StartCoroutine(FadeInObject(item));
                    StartCoroutine(FadeInText(item.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>()));
                }
                break;
            case 8:
                foreach (Coroutine co in stop)
                {
                    StopCoroutine(co);
                }
                stop.Clear();
                for (int i = 1; i < 7; i++)
                {
                    cells[i].transform.localScale = Vector3.one;
                }

                //During this time immunity is received at twice the value.
                StartCoroutine(TurnTextOn(6));
                for (int i = 1; i < 7; i++)
                {
                    GameObject p = Instantiate(immunityParticles, cells[i].transform.position, Quaternion.LookRotation(cells[0].transform.position - cells[i].transform.position), cells[0].transform) as GameObject;
                    p.GetComponent<ImmunityParticles>().target = cells[0].transform;
                    p.GetComponent<ImmunityParticles>().immunity = 1;
                    p.GetComponent<ImmunityParticles>().startSpeed = 15;
                    p.GetComponent<ImmunityParticles>().enabled = true;
                }
                Invoke("Immunity12", 1);
                break;
            case 9:
                immDes.text = "Immunity: 12";

                //Defense is also the only stat that is copied to the child.
                StartCoroutine(TurnTextOn(7));
                foreach (GameObject item in imm)
                {
                    stop.Add(StartCoroutine(FadeOutObject(item)));
                    stop.Add(StartCoroutine(FadeOutText(item.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>())));
                }
                foreach (GameObject item in def)
                {
                    stop.Add(StartCoroutine(FadeInObject(item)));
                    stop.Add(StartCoroutine(FadeInText(item.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>())));
                }
                break;
            case 10:
                foreach (Coroutine co in stop)
                {
                    StopCoroutine(co);
                }
                stop.Clear();

                foreach (GameObject item in imm)
                {
                    item.SetActive(false);
                }
                foreach (GameObject item in def)
                {
                    c = item.GetComponent<Renderer>().material.color;
                    c.a = 1;
                    item.GetComponent<Renderer>().material.color = c;
                    item.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().color = Color.black;
                }

                //Due to this, it is highly advised that you invest into this stat early on. 
                StartCoroutine(TurnTextOn(8));
                break;
            case 11:
                //Alternatively, you could put points into a cell when it becomes targeted by a virus.
                StartCoroutine(TurnTextOn(9));
                break;
            case 12:
                //The Fuzeon power-up permanently increases defense by 5.
                StartCoroutine(TurnTextOn(10));
                stop.Add(StartCoroutine(FadeInObject(fuzeon)));
                stop.Add(StartCoroutine(UnPaintItBlack(cells[0])));
                cells[0].GetComponent<Rotate>().enabled = true;
                stop.Add(StartCoroutine(ShrinkOutObject(viruses[0])));
                defDes.text = "Defense: 5";
                break;
            case 13:
                foreach (Coroutine co in stop)
                {
                    StopCoroutine(co);
                }
                stop.Clear();
                c = fuzeon.GetComponent<Renderer>().material.color;
                c.a = 1.0f;
                fuzeon.GetComponent<Renderer>().material.color = c;
                cells[0].GetComponent<Renderer>().material.color = Color.grey;
                viruses[0].SetActive(false);

                //If you are lucky enough you can get the Defense event.
                StartCoroutine(TurnTextOn(11));
                stop.Add(StartCoroutine(FadeOutObject(fuzeon)));
                break;
            case 14:
                foreach (Coroutine co in stop)
                {
                    StopCoroutine(co);
                }
                stop.Clear();
                fuzeon.SetActive(false);
                fuzeon.GetComponent<Renderer>().material.color = Color.green;
                fuzeon.GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.black);

                //It raises all your cells defense to the max defense in your colony +5.
                StartCoroutine(TurnTextOn(12));
                defDes.text = "Defense: 10";
                break;
            case 15:
                //Be careful, different viruses have different attack values.
                StartCoroutine(TurnTextOn(13));
                foreach (GameObject item in def)
                {
                    stop.Add(StartCoroutine(FadeOutObject(item)));
                    stop.Add(StartCoroutine(FadeOutText(item.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>())));
                }
                foreach (GameObject cell in cells)
                {
                    stop.Add(StartCoroutine(FadeOutObject(cell)));
                }
                stop.Add(StartCoroutine(FadeInObject(viruses[1])));
                stop.Add(StartCoroutine(FadeInObject(viruses[2])));
                stop.Add(StartCoroutine(FadeInObject(viruses[3])));
                break;
            case 16:
                foreach (Coroutine co in stop)
                {
                    StopCoroutine(co);
                }
                stop.Clear();
                foreach (GameObject item in def)
                {
                    item.SetActive(false);
                }
                foreach (GameObject cell in cells)
                {
                    cell.SetActive(false);
                }
                c = viruses[1].GetComponent<Renderer>().material.color;
                c.a = 1;
                viruses[1].GetComponent<Renderer>().material.color = c;
                viruses[1].GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.black);
                c = viruses[2].GetComponent<Renderer>().material.color;
                c.a = 1;
                viruses[2].GetComponent<Renderer>().material.color = c;
                viruses[2].GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.black);
                c = viruses[3].GetComponent<Renderer>().material.color;
                c.a = 1;
                viruses[3].GetComponent<Renderer>().material.color = c;
                viruses[3].GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.black);

                //The attack values will also increase as the game continues.
                StartCoroutine(TurnTextOn(14));
                break;
            case 17:
                //Fade In
                fade.GetComponent<FadeIn>().enabled = true;
                subtitles.text = "";
                clickable = false;
                Invoke("Click", 1);
                break;
            case 18:
                //Fade Out
                reticle.SetActive(true);
                cam.position = prevPos;
                lc.transform.rotation = prevRotation;
                fade.GetComponent<FadeOut>().enabled = true;
                Invoke("Click", 1);
                break;
            case 19:
                foreach (GameObject cell in viruses)
                {
                    cell.SetActive(false);
                }
                objects.SetActive(false);
                eventSystem.SetActive(true);
                enabled = false;
                break;
            default:
                break;
        }
        index++;
    }

    #region Objects
    IEnumerator FadeInObject(GameObject g)
    {
        g.SetActive(true);
        Color c = g.GetComponent<Renderer>().material.color;
        Color outline = Color.black;
        bool o = false;
        if (g.GetComponent<Renderer>().material.HasProperty("_OutlineColor"))
        {
            o = true;
            outline = g.GetComponent<Renderer>().material.GetColor("_OutlineColor");
        }
        float startTime = Time.time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t = Time.time - startTime;
            c.a = t;
            g.GetComponent<Renderer>().material.color = c;
            if (o)
            {
                outline.a = t;
                g.GetComponent<Renderer>().material.SetColor("_OutlineColor", outline);
            }
            yield return 0;
        }
    }

    IEnumerator FadeOutObject(GameObject g)
    {
        Color c = g.GetComponent<Renderer>().material.color;
        Color outline = Color.black;
        bool o = false;
        if (g.GetComponent<Renderer>().material.HasProperty("_OutlineColor"))
        {
            o = true;
            outline = g.GetComponent<Renderer>().material.GetColor("_OutlineColor");
        }
        float startTime = Time.time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t = Time.time - startTime;
            c.a = 1.0f - t;
            g.GetComponent<Renderer>().material.color = c;
            if (o)
            {
                outline.a = 1.0f - t;
                g.GetComponent<Renderer>().material.SetColor("_OutlineColor", outline);
            }
            yield return 0;
        }
        g.SetActive(false);
    }

    IEnumerator GrowInObject(GameObject g)
    {
        g.SetActive(true);
        g.transform.localScale = Vector3.zero;
        float startTime = Time.time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t = Time.time - startTime;
            g.transform.localScale = new Vector3(t, t, t);
            yield return 0;
        }
    }

    IEnumerator ShrinkOutObject(GameObject g)
    {
        float startTime = Time.time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t = Time.time - startTime;
            g.transform.localScale = new Vector3(1.0f - t, 1.0f - t, 1.0f - t);
            yield return 0;
        }
        g.SetActive(false);
    }

    IEnumerator PaintItBlack(GameObject g)
    {
        Color c = g.GetComponent<Renderer>().material.color;
        float startTime = Time.time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t = Time.time - startTime;
            g.GetComponent<Renderer>().material.color = Color.Lerp(c, Color.black, t);
            yield return 0;
        }
    }

    IEnumerator UnPaintItBlack(GameObject g)
    {
        Color c = g.GetComponent<Renderer>().material.color;
        float startTime = Time.time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t = Time.time - startTime;
            g.GetComponent<Renderer>().material.color = Color.Lerp(c, Color.grey, t);
            yield return 0;
        }
    }
    #endregion

    #region Text
    IEnumerator TurnTextOn(int index)
    {
        while (text)
            yield return 0;

        text = true;
        subtitles.text = "_";

        while (subtitles.text != texts[index] && !finish)
        {
            yield return new WaitForSeconds(GlobalVariables.textDelay);

            if (subtitles.text.Length == texts[index].Length)
            {
                subtitles.text = texts[index];
            }
            else
            {
                subtitles.text = subtitles.text.Insert(subtitles.text.Length - 1, texts[index][subtitles.text.Length - 1].ToString());
            }
        }
        subtitles.text = texts[index];
        finish = false;
        text = false;
    }

    IEnumerator FadeInText(TMPro.TextMeshPro text)
    {
        text.gameObject.SetActive(true);
        Color a = text.color;
        a.a = 0.0f;
        text.color = a;
        float startTime = Time.time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t = Time.time - startTime;
            a.a = t;
            text.color = a;
            yield return 0;
        }
    }

    IEnumerator FadeOutText(TMPro.TextMeshPro text)
    {
        Color a = text.color;
        float startTime = Time.time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t = Time.time - startTime;
            a.a = 1.0f - t;
            text.color = a;
            yield return 0;
        }
        text.gameObject.SetActive(false);
    }
    #endregion

    #region Viruses

    IEnumerator MoveVirus()
    {
        Vector3 startPos = viruses[0].transform.position;
        Vector3 endPos = new Vector3(cells[0].transform.position.x, cells[0].transform.position.y + .9f, cells[0].transform.position.z);
        float startTime = Time.time;
        float t = 0;
        while (t < 1.0f)
        {
            t = Time.time - startTime;
            viruses[0].transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return 0;
        }
        viruses[0].GetComponent<Rotate>().enabled = false;
        cells[0].GetComponent<Rotate>().enabled = false;
    }
    #endregion

    void Immunity12()
    {
        immDes.text = "Immunity: 12";
    }
}