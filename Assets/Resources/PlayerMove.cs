using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove //ATTENTION ICI C'EST pas MONOBEHAVIOUR
{

    public GameObject[] tileToDestroy;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        FindSelectablesTilesDFS();
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
            CheckMouse();
            return;
        }

        if (!moving && endExplosion == false)
        {
            FindSelectablesTilesDFS();
            CheckMouse();

        }
        else
        {
            Move();
            
        }

    }

    private void CheckMouse()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tile")
                {
                    Tile goal = hit.collider.GetComponent<Tile>();
                    if (goal.selectable)
                    {
                        //DFSMove(goal);
                        //BiDirectionnalMove(goal);
                        //GreedySearchMove(goal);
                        //BeamSearchMove(goal);
                        //OptimalSearchMove(goal);
                        //OptimalSearchBBMove(goal);
                        //OptimalSearchBBAMove(goal);
                        MoveToTile(goal);
                    } else if (goal.toDestroy && !CurrentTile(goal))
                    {
                        GameObject[] tileToDestroy = GameObject.FindGameObjectsWithTag("Tile");
                        foreach (GameObject tile in tileToDestroy)
                        {
                            tile.GetComponent<Tile>().toDestroy = false;
                        }

                        endExplosion = false;
                        goal.explode = true;

                    }
                }
            }
        }
    }

    private bool CurrentTile(Tile goal)
    {
        List<Tile> player = TurnManager.Instance.getPositionUnitsTilePlayer();
        List<Tile> NPC = TurnManager.Instance.getPositionUnitsTileNPC();
        foreach (Tile currentTilePlayer in player)
        {
            if (goal == currentTilePlayer)
            {
                return true;
            }
        }
        foreach (Tile currentTileNPC in NPC)
        {
            if (goal == currentTileNPC)
            {
                return true;
            }
        }
        return false;
    }
}
