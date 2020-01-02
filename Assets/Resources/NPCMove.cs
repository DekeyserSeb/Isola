using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : TacticsMove
{
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
            return;
        }

        if (!moving && endExplosion == false)
        {

        }
        else
        {
            //Move();

        }
    }
}
