using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : TacticsMove
{
    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!turn)
        {
            return;
        }
        if (endExplosion)// A CHANGER
        {
            endExplosion = false;
            chooseExplosion().explode = true;
            return;
        }
        if (!moving) //Ajouter explosion ici
        {
            FindBestTargetTile();
            CalculatePath();
            FindSelectablesTilesDFS();
            actualTargetTile.target = true;
        }
        else
        {
            Move();

        }
    }

    private Tile chooseExplosion()
    {
        List<Tile> listToDestroy = new List<Tile>();
        Tile toDestroy = null;
        GameObject[] tileToDestroy = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject tile in tileToDestroy)
        {
           if (tile.GetComponent<Tile>().toDestroy == true)
            {
                listToDestroy.Add(tile.GetComponent<Tile>());
            }
        }

        toDestroy = listToDestroy[0];


        foreach (GameObject tile in tileToDestroy)
        {
            tile.GetComponent<Tile>().toDestroy = false;
        }


        return toDestroy;
    }

    void CalculatePath()
    {
        //Tile targetTile = GetTargetTile(target); //Get target Tile prends le tile sur lequel il est
        List<Tile> player = TurnManager.Instance.getPositionUnitsTilePlayer();
        Tile targetTile = player[0];
        FindPath(targetTile);

    }

    void FindBestTargetTile()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

    }

    void FindNearestTargetTile()//Trouve par ou passer pour se rapprocher le plus possible du joueur le plus proche
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        GameObject nearest = null;
        float distance = Mathf.Infinity;

        foreach (GameObject obj in targets)
        {
            float d = Vector3.Distance(transform.position, obj.transform.position);

            if (d < distance)
            {
                distance = d;
                nearest = obj;
            }
        }

        target = nearest;

    }
}
