using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMinMaxMove : TacticsMove
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
        if (endExplosion)// ICI on choisit l'endroit à exploser
        {
            endExplosion = false;
            chooseExplosion().explode = true; // Je choisis ici la case à exploser
            return;
        }
        if (!moving) // ICI on recherche le chemin
        {
            FindSelectablesTilesDFS();      // Je cherche toutes les cases adjacents a mon joueurs
            FindBestTargetTile();           // Je vais calculé le nombres de voisins par itération et chercher le meilleur chemin;
            CalculatePath();                // Je calcule le chemin pour y parvenir
            FindSelectablesTilesDFS();      // Je remets en évidence les chemin possible pour le robot (esthétique)
            actualTargetTile.target = true; // Je rejoins la case cible
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

    void FindBestTargetTile()//Trouve par ou passer pour se rapprocher le plus possible du joueur le plus proche
    {
        // Je créé une liste constitué de toutes les cases ou l'ordi peut se déplacer
        List<Tile> targetList = new List<Tile>();
        GameObject[] AllTiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject tile in AllTiles) 
        {
            if (tile.GetComponent<Tile>().selectable == true)
            {
                targetList.Add(tile.GetComponent<Tile>());
            }
        }

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

        target = nearest; //A CHANGER

    }
}
