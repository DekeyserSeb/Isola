using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove //ATTENTION ICI C'EST pas MONOBEHAVIOUR
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
        FindSelectablesTilesDFS();


    }

    // Update is called once per frame
    void Update()
    {
        if (!moving)
        {
            //FindSelectablesTilesDFS();
            CheckMouse();
        }
        else
        {
            //Move();
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
                        OptimalSearchBBAMove(goal);
                        //MoveToTile(t);
                    }
                }
            }
        }
    }
}
