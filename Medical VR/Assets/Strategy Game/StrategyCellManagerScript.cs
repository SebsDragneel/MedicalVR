﻿using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class StrategyCellManagerScript : MonoBehaviour
{
    public Dictionary<Vector2, StrategyCellScript> tiles = new Dictionary<Vector2, StrategyCellScript>(new Vector2Comparer());
    [HideInInspector]
    public List<StrategyCellScript> cells = new List<StrategyCellScript>();
    [HideInInspector]
    public List<StrategyVirusScript> viruses = new List<StrategyVirusScript>();
    public GameObject cellPrefab;
    public GameObject virusPrefab1;
    public GameObject virusPrefab2;
    public GameObject virusPrefab3;
    public GameObject transporter;
    public StrategyBox mysteryBox;
    public SimulateSun sun;
    public TextMesh screenUI;
    public int turnNumber = 0;
    public int cellNum = 1;
    public int virNum = 0;
    public Vector2 selected = new Vector2(0.0f, 0.0f);
    public bool viewingStats = false;
    public float randomRange = .5f;

    [System.NonSerialized]
    public List<StrategyItem> inventory;

    private GameObject virusPrefab
    {
        get
        {
            float p3 = cells.Count * .5f;
            float p2 = cells.Count;
            float p1 = 100.0f - p3 - p2;
            float r = Random.Range(0.0f, 100.0f);
            if (r <= p1)
            {
                return virusPrefab1;
            }
            else if (r > p1 && r <= p1 + p2)
            {
                return virusPrefab2;
            }
            else
            {
                return virusPrefab3;
            }
        }
    }
    private Vector4 spawnCellStats;
    //private float xOffset = 1.0f;
    //private float yOffset = 1.0f;
    public float xOffset = 2.0f;
    public float yOffset = 2.0f;

    // Use this for initialization
    void Start()
    {
        GameObject t = Instantiate(cellPrefab, new Vector3(xOffset * .5f, 0, 0), cellPrefab.transform.rotation, transform) as GameObject;
        t.GetComponent<StrategyCellScript>().key = new Vector2(0, 0);
        AddToDictionary(t.GetComponent<StrategyCellScript>());
        t.name = "Cell0_0";
        t.GetComponent<StrategyCellScript>().parent = this;
        t.GetComponent<StrategyCellScript>().reproduction = 1;
        t.GetComponent<StrategyCellScript>().defense = 1;
        t.GetComponent<StrategyCellScript>().immunity = 1;
        t.GetComponent<StrategyCellScript>().enabled = true;
        t.GetComponent<Collider>().enabled = true;

        inventory = mysteryBox.items;
    }

    public void SetSelected(Vector2 k)
    {
        if (tiles.ContainsKey(selected))
            tiles[selected].ToggleUI(false);
        else if (selected == new Vector2(-100, -100))
            mysteryBox.ToggleUI();
        else if (!tiles.ContainsKey(k))
            Camera.main.GetComponent<MoveCamera>().SetDestination(new Vector3(k.y % 2 == 0 ? k.x * xOffset + xOffset * .5f : k.x * xOffset, 5, k.y * yOffset));
        selected = k;
    }

    public void Unselect()
    {
        if (tiles.ContainsKey(selected))
            tiles[selected].ToggleUI(false);
        selected = new Vector2(-100, -100);
        viewingStats = false;
    }

    public void ActionPreformed()
    {
        StartCoroutine(TurnUpdate());
        if (screenUI)
            screenUI.text = "Turn Number: " + turnNumber + "\nCells Alive: " + cellNum + "\nViruses Alive: " + virNum;
    }

    IEnumerator TurnUpdate()
    {
        Debug.Log("Turn Updating");
        turnNumber++;

        foreach (StrategyCellScript child in cells.ToList())
        {
            child.TurnUpdate();
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Cells Updated");

        foreach (StrategyVirusScript child in viruses.ToList())
        {
            child.TurnUpdate();
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Viruses Updated");

        foreach (StrategyCellScript child in cells.ToList())
        {
            child.DelayedTurnUpdate();
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Cells Late Updated");

        if (turnNumber % 15 == 0)
        {
            SpawnVirus();
        }

        cellNum = cells.Count;
        virNum = viruses.Count;

        sun.TurnUpdate();

        screenUI.text = "Turn Number: " + turnNumber + "\nCells Alive: " + cellNum + "\nViruses Alive: " + virNum;
        Debug.Log("Turn Updated");
    }

    public void AddToDictionary(StrategyCellScript cell)
    {
        tiles.Add(cell.key, cell);
    }

    void SpawnCell(Vector2 k, Vector2 p)
    {
        Vector3 spawnLocation = tiles[p].transform.position;
        Vector3 desination = new Vector3(k.y % 2 == 0 ? k.x * xOffset + xOffset * .5f : k.x * xOffset, CalculateY(k), k.y * yOffset);
        GameObject t = Instantiate(transporter, spawnLocation, Quaternion.identity, transform) as GameObject;
        t.GetComponent<StrategyTransporter>().destination = desination;
        GameObject c = Instantiate(cellPrefab, spawnLocation, cellPrefab.transform.rotation, t.transform) as GameObject;
        c.GetComponent<StrategyCellScript>().key = k;
        AddToDictionary(c.GetComponent<StrategyCellScript>());
        c.name = "Cell" + k.x + "_" + k.y;
        c.GetComponent<StrategyCellScript>().parent = this;
        if (spawnCellStats != Vector4.zero)
        {
            c.GetComponent<StrategyCellScript>().reproduction = (int)spawnCellStats.x;
            c.GetComponent<StrategyCellScript>().defense = (int)spawnCellStats.y;
            c.GetComponent<StrategyCellScript>().immunity = (int)spawnCellStats.z;
            c.GetComponent<StrategyCellScript>().protein = (StrategyCellScript.Proteins)((int)spawnCellStats.w);
            spawnCellStats = Vector4.zero;
        }
        t.GetComponent<StrategyTransporter>().enabled = true;
        c.GetComponent<StrategyCellScript>().enabled = true;
    }

    public void SelectCellSpawn(Vector2 starting)
    {
        Queue<Vector2> que = new Queue<Vector2>();
        HashSet<Vector2> visited = new HashSet<Vector2>(new Vector2Comparer());
        que.Enqueue(starting);
        visited.Add(starting);
        while (true)
        {
            //Top Right (0, +1)
            Vector2 check = que.Peek();
            check.y += 1;
            if (!tiles.ContainsKey(check))
            {
                SpawnCell(check, starting);
                return;
            }
            if (!visited.Contains(check))
                que.Enqueue(check);

            //Right (+1, 0)
            check = que.Peek();
            check.x += 1;
            if (!tiles.ContainsKey(check))
            {
                SpawnCell(check, starting);
                return;
            }
            if (!visited.Contains(check))
                que.Enqueue(check);

            //Bottom Right (0, -1)
            check = que.Peek();
            check.y -= 1;
            if (!tiles.ContainsKey(check))
            {
                SpawnCell(check, starting);
                return;
            }
            if (!visited.Contains(check))
                que.Enqueue(check);

            //Bottom Left (-1, -1)
            check = que.Peek();
            check.x -= 1;
            check.y -= 1;
            if (!tiles.ContainsKey(check))
            {
                SpawnCell(check, starting);
                return;
            }
            if (!visited.Contains(check))
                que.Enqueue(check);

            //Left (-1, 0)
            check = que.Peek();
            check.x -= 1;
            if (!tiles.ContainsKey(check))
            {
                SpawnCell(check, starting);
                return;
            }
            if (!visited.Contains(check))
                que.Enqueue(check);

            //Top Left (-1, +1)
            check = que.Peek();
            check.x -= 1;
            check.y += 1;
            if (!tiles.ContainsKey(check))
            {
                SpawnCell(check, starting);
                return;
            }
            if (!visited.Contains(check))
                que.Enqueue(check);

            que.Dequeue();
        }
    }

    public void DuplicateCell(Vector2 k, Vector4 stats)
    {
        spawnCellStats = stats;
        SelectCellSpawn(k);
    }

    float CalculateY(Vector2 k)
    {
        float avg = 0.0f;
        int total = 0;

        //Top Right (0, +1)
        Vector2 check = k;
        check.y += 1;
        if (tiles.ContainsKey(check))
        {
            avg += tiles[check].transform.position.y;
            total++;
        }

        //Right (+1, 0)
        check = k;
        check.x += 1;
        if (tiles.ContainsKey(check))
        {
            avg += tiles[check].transform.position.y;
            total++;
        }

        //Bottom Right (0, -1)
        check = k;
        check.y -= 1;
        if (tiles.ContainsKey(check))
        {
            avg += tiles[check].transform.position.y;
            total++;
        }

        //Bottom Left (-1, -1)
        check = k;
        check.x -= 1;
        check.y -= 1;
        if (tiles.ContainsKey(check))
        {
            avg += tiles[check].transform.position.y;
            total++;
        }

        //Left (-1, 0)
        check = k;
        check.x -= 1;
        if (tiles.ContainsKey(check))
        {
            avg += tiles[check].transform.position.y;
            total++;
        }

        //Top Left (-1, +1)
        check = k;
        check.x -= 1;
        check.y += 1;
        if (tiles.ContainsKey(check))
        {
            avg += tiles[check].transform.position.y;
            total++;
        }

        avg /= total;
        if (avg == float.NaN)
        {
            avg = 0.0f;
        }

        return Mathf.Clamp(Random.Range(-randomRange, randomRange) + avg, -4.0f, 4.0f);
    }

    public void KillCell(Vector2 k)
    {
        StrategyCellScript instance = tiles[k];
        tiles.Remove(k);
        Destroy(instance.gameObject);
    }

    public float SpreadImmunity(Vector2 starting, float imm)
    {
        float immunitySpread = 0;

        //Top Right (0, +1)
        Vector2 check = starting;
        check.y += 1;
        if (tiles.ContainsKey(check))
        {
            tiles[check].AddImmunity(imm);
            immunitySpread += imm;
        }

        //Right (+1, 0)
        check = starting;
        check.x += 1;
        if (tiles.ContainsKey(check))
        {
            tiles[check].AddImmunity(imm);
            immunitySpread += imm;
        }

        //Bottom Right (0, -1)
        check = starting;
        check.y -= 1;
        if (tiles.ContainsKey(check))
        {
            tiles[check].AddImmunity(imm);
            immunitySpread += imm;
        }

        //Bottom Left (-1, -1)
        check = starting;
        check.x -= 1;
        check.y -= 1;
        if (tiles.ContainsKey(check))
        {
            tiles[check].AddImmunity(imm);
            immunitySpread += imm;
        }

        //Left (-1, 0)
        check = starting;
        check.x -= 1;
        if (tiles.ContainsKey(check))
        {
            tiles[check].AddImmunity(imm);
            immunitySpread += imm;
        }

        //Top Left (-1, +1)
        check = starting;
        check.x -= 1;
        check.y += 1;
        if (tiles.ContainsKey(check))
        {
            tiles[check].AddImmunity(imm);
            immunitySpread += imm;
        }

        return immunitySpread;
    }

    public void SpawnVirus()
    {
        Vector3 direction = Random.onUnitSphere;
        direction.y = Mathf.Clamp(direction.y, 0.65f, 1f);
        float distance = 100.0f;
        Vector3 position = direction * distance;
        GameObject v = Instantiate(virusPrefab, position, Quaternion.identity, transform) as GameObject;
        v.GetComponent<StrategyVirusScript>().target = FindVirusNewTarget(v);
        v.GetComponent<StrategyVirusScript>().parent = this;
        v.GetComponent<Collider>().enabled = true;
        v.GetComponent<StrategyVirusScript>().enabled = true;
    }

    //Attempts to spawn a virus on an adjacent cell
    //If one of them isn't open it targets a random cell or goes on standby
    public void SpawnVirusSingleAdjacent(Vector2 k, Vector3 p)
    {
        //Top Right
        Vector2 check = k;
        check.y += 1;
        if (tiles.ContainsKey(check) && !tiles[check].targeted)
        {
            GameObject t = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
            t.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset + xOffset * .5f : k.x * xOffset, 1, k.y * yOffset + yOffset * .5f);
            tiles[check].targeted = true;
            GameObject v = Instantiate(virusPrefab, p, Quaternion.identity, t.transform) as GameObject;
            v.GetComponent<StrategyVirusScript>().target = tiles[check];
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            v.GetComponent<StrategyVirusScript>().parent = this;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
            return;
        }

        //Right (+1, 0)
        check = k;
        check.x += 1;
        if (tiles.ContainsKey(check) && !tiles[check].targeted)
        {
            GameObject t = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
            t.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset + xOffset : k.x * xOffset + xOffset * .5f, 1, k.y * yOffset);
            tiles[check].targeted = true;
            GameObject v = Instantiate(virusPrefab, p, Quaternion.identity, t.transform) as GameObject;
            v.GetComponent<StrategyVirusScript>().target = tiles[check];
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            v.GetComponent<StrategyVirusScript>().parent = this;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
            return;
        }

        //Bottom Right (0, -1)
        check = k;
        check.y -= 1;
        if (tiles.ContainsKey(check) && !tiles[check].targeted)
        {
            GameObject t = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
            t.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset + xOffset * .5f : k.x * xOffset, 1, k.y * yOffset - yOffset * .5f);
            tiles[check].targeted = true;
            GameObject v = Instantiate(virusPrefab, p, Quaternion.identity, t.transform) as GameObject;
            v.GetComponent<StrategyVirusScript>().target = tiles[check];
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            v.GetComponent<StrategyVirusScript>().parent = this;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
            return;
        }

        //Bottom Left (-1, -1)
        check = k;
        check.x -= 1;
        check.y -= 1;
        if (tiles.ContainsKey(check) && !tiles[check].targeted)
        {
            GameObject t = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
            t.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset - xOffset : k.x * xOffset - xOffset * .5f, 1, k.y * yOffset - yOffset * .5f);
            tiles[check].targeted = true;
            GameObject v = Instantiate(virusPrefab, p, Quaternion.identity, t.transform) as GameObject;
            v.GetComponent<StrategyVirusScript>().target = tiles[check];
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            v.GetComponent<StrategyVirusScript>().parent = this;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
            return;
        }

        //Left (-1, 0)
        check = k;
        check.x -= 1;
        if (tiles.ContainsKey(check) && !tiles[check].targeted)
        {
            GameObject t = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
            t.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset - xOffset : k.x * xOffset - xOffset * .5f, 1, k.y * yOffset);
            tiles[check].targeted = true;
            GameObject v = Instantiate(virusPrefab, p, Quaternion.identity, t.transform) as GameObject;
            v.GetComponent<StrategyVirusScript>().target = tiles[check];
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            v.GetComponent<StrategyVirusScript>().parent = this;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
            return;
        }

        //Top Left (-1, +1)
        check = k;
        check.x -= 1;
        check.y += 1;
        if (tiles.ContainsKey(check) && !tiles[check].targeted)
        {
            GameObject t = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
            t.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset - xOffset : k.x * xOffset - xOffset * .5f, 1, k.y * yOffset + yOffset * .5f);
            tiles[check].targeted = true;
            GameObject v = Instantiate(virusPrefab, p, Quaternion.identity, t.transform) as GameObject;
            v.GetComponent<StrategyVirusScript>().target = tiles[check];
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            v.GetComponent<StrategyVirusScript>().parent = this;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
            return;
        }

        GameObject tra = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
        tra.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset + xOffset * .5f : k.x * xOffset, 2, k.y * yOffset);
        GameObject vir = Instantiate(virusPrefab, p, Quaternion.identity, tra.transform) as GameObject;
        vir.GetComponent<StrategyVirusScript>().target = FindVirusNewTarget(vir);
        vir.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
        vir.GetComponent<StrategyVirusScript>().parent = this;
        tra.GetComponent<StrategyTransporter>().enabled = true;
        vir.GetComponent<StrategyVirusScript>().enabled = true;
    }

    //Attempts to spawn viruses on all adjacent cells
    //If one of them isn't open it targets a random cell or goes on standby
    public void SpawnVirusAllAdjacent(Vector2 k, Vector3 p)
    {
        //Top Right
        Vector2 check = k;
        check.y += 1;
        GameObject t = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
        t.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset + xOffset * .5f : k.x * xOffset, 1, k.y * yOffset + yOffset * .5f);
        GameObject v = Instantiate(virusPrefab, p, Quaternion.identity, t.transform) as GameObject;
        v.GetComponent<StrategyVirusScript>().parent = this;
        if (tiles.ContainsKey(check) && !tiles[check].targeted)
        {
            tiles[check].targeted = true;
            v.GetComponent<StrategyVirusScript>().target = tiles[check];
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
        }
        else
        {
            v.GetComponent<StrategyVirusScript>().target = FindVirusNewTarget(v);
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
        }

        //Right (+1, 0)
        check = k;
        check.x += 1;
        t = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
        t.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset + xOffset : k.x * xOffset + xOffset * .5f, 1, k.y * yOffset);
        v = Instantiate(virusPrefab, p, Quaternion.identity, t.transform) as GameObject;
        v.GetComponent<StrategyVirusScript>().parent = this;
        if (tiles.ContainsKey(check) && !tiles[check].targeted)
        {
            tiles[check].targeted = true;
            v.GetComponent<StrategyVirusScript>().target = tiles[check];
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
        }
        else
        {
            v.GetComponent<StrategyVirusScript>().target = FindVirusNewTarget(v);
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
        }

        //Bottom Right (0, -1)
        check = k;
        check.y -= 1;
        t = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
        t.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset + xOffset * .5f : k.x * xOffset, 1, k.y * yOffset - yOffset * .5f);
        v = Instantiate(virusPrefab, p, Quaternion.identity, t.transform) as GameObject;
        v.GetComponent<StrategyVirusScript>().parent = this;
        if (tiles.ContainsKey(check) && !tiles[check].targeted)
        {
            tiles[check].targeted = true;
            v.GetComponent<StrategyVirusScript>().target = tiles[check];
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
        }
        else
        {
            v.GetComponent<StrategyVirusScript>().target = FindVirusNewTarget(v);
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
        }

        //Bottom Left (-1, -1)
        check = k;
        check.x -= 1;
        check.y -= 1;
        t = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
        t.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset - xOffset : k.x * xOffset - xOffset * .5f, 1, k.y * yOffset - yOffset * .5f);
        v = Instantiate(virusPrefab, p, Quaternion.identity, t.transform) as GameObject;
        v.GetComponent<StrategyVirusScript>().parent = this;
        if (tiles.ContainsKey(check) && !tiles[check].targeted)
        {
            tiles[check].targeted = true;
            v.GetComponent<StrategyVirusScript>().target = tiles[check];
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
        }
        else
        {
            v.GetComponent<StrategyVirusScript>().target = FindVirusNewTarget(v);
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
        }

        //Left (-1, 0)
        check = k;
        check.x -= 1;
        t = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
        t.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset - xOffset : k.x * xOffset - xOffset * .5f, 1, k.y * yOffset);
        v = Instantiate(virusPrefab, p, Quaternion.identity, t.transform) as GameObject;
        v.GetComponent<StrategyVirusScript>().parent = this;
        if (tiles.ContainsKey(check) && !tiles[check].targeted)
        {
            tiles[check].targeted = true;
            v.GetComponent<StrategyVirusScript>().target = tiles[check];
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
        }
        else
        {
            v.GetComponent<StrategyVirusScript>().target = FindVirusNewTarget(v);
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
        }

        //Top Left (-1, +1)
        check = k;
        check.x -= 1;
        check.y += 1;
        t = Instantiate(transporter, p, Quaternion.identity, transform) as GameObject;
        t.GetComponent<StrategyTransporter>().destination = new Vector3(k.y % 2 == 0 ? k.x * xOffset - xOffset : k.x * xOffset - xOffset * .5f, 1, k.y * yOffset + yOffset * .5f);
        v = Instantiate(virusPrefab, p, Quaternion.identity, t.transform) as GameObject;
        v.GetComponent<StrategyVirusScript>().parent = this;
        if (tiles.ContainsKey(check) && !tiles[check].targeted)
        {
            tiles[check].targeted = true;
            v.GetComponent<StrategyVirusScript>().target = tiles[check];
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
        }
        else
        {
            v.GetComponent<StrategyVirusScript>().target = FindVirusNewTarget(v);
            v.GetComponent<StrategyVirusScript>().percentTraveled = .75f;
            t.GetComponent<StrategyTransporter>().enabled = true;
            v.GetComponent<StrategyVirusScript>().enabled = true;
        }
    }

    public StrategyCellScript FindVirusNewTarget(GameObject vir)
    {
        for (int i = 0; i < 10; i++)
        {
            StrategyCellScript temp = tiles.Values.ElementAt(Random.Range(0, tiles.Values.Count)).GetComponent<StrategyCellScript>();

            if (!temp.targeted)
            {
                vir.GetComponent<StrategyVirusScript>().standby = false;
                temp.targeted = true;
                return temp;
            }
        }

        vir.GetComponent<StrategyVirusScript>().standby = true;
        return null;
    }

    public Vector3 RandomPositionAboveHex()
    {
        return new Vector3(Random.Range(tiles.Count * .2f * -1.0f, tiles.Count * .17f), 10, Random.Range(tiles.Count * .2f * -1.0f, tiles.Count * .17f));
    }
}

class Vector2Comparer : IEqualityComparer<Vector2>
{
    public bool Equals(Vector2 x, Vector2 y)
    {
        if (x.x == y.x && x.y == y.y)
            return true;
        return false;
    }

    public int GetHashCode(Vector2 x)
    {
        return (int)x.x + (int)x.y * 1000000;
    }
}
