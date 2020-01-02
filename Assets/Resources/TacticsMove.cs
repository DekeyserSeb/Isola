using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    public bool turn = false;
    public bool endExplosion = false;
    List<Tile> selectableTiles = new List<Tile>(); //Liste des tuiles
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>(); //Chemin
    List<Tile[]> processBBA = new List<Tile[]>();
    Tile currentTile;
    Tile previousTile;
    Tile targetTile;

    public bool moving = false;
    public int move = 5; //Nombre de pas possible pour le character
    public float jumpHeight = 2;
    public float movespeed = 2; 

    Vector3 velocity = new Vector3();
    Vector3 direction = new Vector3(); //Direction = heading

    float halfHeight = 0;


    protected void Init()
 
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        halfHeight = GetComponent<Collider>().bounds.extents.y;

        TurnManager.AddUnit(this);

    }

    public void GetCurrentTile()

    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>(); 
        }
        return tile;
    }

    public void GetListCaseAdjacent()
    {
        //tiles = GameObject.FindGameObjectsWithTag("tiles"); 
        //Si on changes la taille de la carte


        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(jumpHeight);

        }
    }

    public void FindSelectablesTilesDFS() //Breath First Search
    {
        GetListCaseAdjacent(); 
        GetCurrentTile();

        //DEBUT BFS

        Queue<Tile> process = new Queue<Tile>();

        // 1 ajout de la case de départ et la mettre en tant que case visité

        process.Enqueue(currentTile);
        currentTile.visited = true;
        previousTile = currentTile;
        //currentTile.Parent = ?? leave as null

        // 2 le while n'est pas vide

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();

            selectableTiles.Add(t);
            t.selectable = true;


            if (t.distance < move)
            {
                foreach (Tile tile in t.adjacentList) // Pour chaque case dans la liste de cases
                {
                    if (!tile.visited) // Si la case n'est pas visité.
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }
        }
        currentTile.selectable = false;
    }

    public void DFSMove(Tile goal)
    {
        bool pathIsDone = false;
        GetListCaseAdjacent();
        GetCurrentTile();
        Queue<Tile[]> process = new Queue<Tile[]>();
        Tile[] FirstPath = new Tile[1] {currentTile};
        process.Enqueue(FirstPath);

        while (pathIsDone == false) //process.Count > 0 && 
        {
            Tile[] t = process.Dequeue();

            if (t[t.Length - 1] == goal)
            {
                pathIsDone = true;
                foreach (Tile tile in t)
                {
                    tile.inPath = true;
                }
            }
            else
            { 
                if (t[t.Length - 1].distance < move)
                {
                    foreach (Tile tile in t[t.Length - 1].adjacentList) // Pour chaque case dans la liste de cases
                    {
                        if (!ContainTile(t, tile)) // Si le t ne contient pas la tuile
                        {
                            tile.distance = 1 + t[t.Length - 1].distance;
                            Tile[] newPath = new Tile[t.Length + 1];
                            for (int i = 0; i < t.Length; i++)
                            {
                                newPath[i] = t[i];
                            }
                            newPath[newPath.Length - 1] = tile;
                            process.Enqueue(newPath);

                        }
                    }
                }
            }
        }
    }

    public void BiDirectionnalMove(Tile goal)
    {
        bool pathIsDone = false;
        GetListCaseAdjacent();
        GetCurrentTile();
        List<Tile[]> processStart = new List<Tile[]>();
        List<Tile[]> processGoal = new List<Tile[]>();
        Tile[] FirstPath = new Tile[1] { currentTile };
        Tile[] SecondPath = new Tile[1] { goal };

        processStart.Add(FirstPath);
        processGoal.Add(SecondPath);


        while (pathIsDone == false && processStart.Count > 0 && processGoal.Count > 0) //process.Count > 0 && 
        {

            Tile[] tStart = processStart[0]; 
            processStart.RemoveAt(0);
            Tile[] tGoal = processGoal[0];
            processGoal.RemoveAt(0);


            if (CommunTile(tStart, tGoal))
            {
                pathIsDone = true;
                foreach (Tile tile in tStart)
                {
                    tile.inPathFromStart = true;
                }
                foreach (Tile tile in tGoal)
                {
                    tile.inPathFromGoal = true;
                }
            }
            else
            {
                foreach (Tile tile in tStart[tStart.Length - 1].adjacentList) // Pour chaque case dans la liste de cases
                {
                    if (!ContainTile(tStart, tile)) // Si le t ne contient pas la tuile
                    {
                        tile.distance = 1 + tStart[tStart.Length - 1].distance;
                        Tile[] newPath = new Tile[tStart.Length + 1];
                        for (int i = 0; i < tStart.Length; i++)
                        {
                            newPath[i] = tStart[i];
                        }
                        newPath[newPath.Length - 1] = tile;
                        processStart.Insert(0,newPath);

                    }
                }

                foreach (Tile tiles in tGoal[tGoal.Length - 1].adjacentList) // Pour chaque case dans la liste de cases
                {
                    if (!ContainTile(tGoal, tiles)) // Si le t ne contient pas la tuile
                    {
                        tiles.distance = 1 + tGoal[tGoal.Length - 1].distance;
                        Tile[] newPath = new Tile[tGoal.Length + 1];
                        for (int i = 0; i < tGoal.Length; i++)
                        {
                            newPath[i] = tGoal[i];
                        }
                        newPath[newPath.Length - 1] = tiles;
                        processGoal.Insert(0,newPath);

                    }
                }            
            }
        }
    }

    public void GreedySearchMove(Tile goal)
    {
        bool pathIsDone = false;
        GetListCaseAdjacent();
        GetCurrentTile();
        List<Tile[]> process = new List<Tile[]>();
        Tile[] FirstPath = new Tile[1] { currentTile };
        targetTile = goal;
        process.Add(FirstPath);

        while (pathIsDone == false && process.Count > 0) //process.Count > 0 && 
        {
            Tile[] t = process[0];
            process.RemoveAt(0);

            if (t[t.Length - 1] == goal)
            {
                pathIsDone = true;
                foreach (Tile tile in t)
                {
                    tile.inPath = true;
                }
            }
            else
            {

                foreach (Tile tile in t[t.Length - 1].adjacentList) // Pour chaque case dans la liste de cases
                {
                    if (!ContainTile(t, tile)) // Si le t ne contient pas la tuile
                    {
                        tile.distance = 1 + t[t.Length - 1].distance;
                        Tile[] newPath = new Tile[t.Length + 1];
                        for (int i = 0; i < t.Length; i++)
                        {
                            newPath[i] = t[i];
                        }
                        newPath[newPath.Length - 1] = tile;
                        process.Insert(0, newPath);
                        process.Sort(SortListHeurestic);

                    }
                }
            }
        }
    }

    public void BeamSearchMove(Tile goal)
    {
        bool pathIsDone = false;
        GetListCaseAdjacent();
        GetCurrentTile();
        List<Tile[]> process = new List<Tile[]>();
        Tile[] FirstPath = new Tile[1] { currentTile };
        targetTile = goal;
        process.Add(FirstPath);
        float bestDistFirstNode = Vector3.Distance(currentTile.transform.position, targetTile.transform.position);
        float bestDistSecondNode = Vector3.Distance(currentTile.transform.position, targetTile.transform.position);
        float bestDistThirdNode = Vector3.Distance(currentTile.transform.position, targetTile.transform.position);
        bool thirdDistActive = false;

        while (pathIsDone == false && process.Count > 0) //process.Count > 0 && 
        {
            Tile[] t = process[0];
            process.RemoveAt(0);

            if (t[t.Length - 1] == goal)
            {
                pathIsDone = true;
                foreach (Tile tile in t)
                {
                    tile.inPath = true;
                }
            }
            else
            {
                foreach (Tile tile in t[t.Length - 1].adjacentList) // Pour chaque case dans la liste de cases
                {
                    float distBetweenTG = Vector3.Distance(tile.transform.position, targetTile.transform.position);
                    if (!ContainTile(t, tile)) // Si le t ne contient pas la tuile
                    {
                        if (bestDistFirstNode > distBetweenTG) { 
                            tile.distance = 1 + t[t.Length - 1].distance;
                            Tile[] newPath = new Tile[t.Length + 1];
                            for (int i = 0; i < t.Length; i++)
                        {
                            newPath[i] = t[i];
                        }
                            newPath[newPath.Length - 1] = tile;
                            process.Insert(0, newPath);
                            bestDistFirstNode = distBetweenTG;

                        }

                        else if (bestDistSecondNode > distBetweenTG)
                        {
                            tile.distance = 1 + t[t.Length - 1].distance;
                            Tile[] newPath = new Tile[t.Length + 1];
                            for (int i = 0; i < t.Length; i++)
                            {
                                newPath[i] = t[i];
                            }
                            newPath[newPath.Length - 1] = tile;
                            process.Insert(0, newPath);
                            bestDistSecondNode = distBetweenTG;
                        }

                        else if (bestDistThirdNode > distBetweenTG && thirdDistActive == true)
                        {
                            tile.distance = 1 + t[t.Length - 1].distance;
                            Tile[] newPath = new Tile[t.Length + 1];
                            for (int i = 0; i < t.Length; i++)
                            {
                                newPath[i] = t[i];
                            }
                            newPath[newPath.Length - 1] = tile;
                            process.Insert(0, newPath);
                            bestDistThirdNode = distBetweenTG;
                        }
                    }
                }
            }
        }
    }

    public void OptimalSearchMove(Tile goal)
    {
        bool pathIsDone = false;
        GetListCaseAdjacent();
        GetCurrentTile();
        List<Tile[]> process = new List<Tile[]>();
        Tile[] FirstPath = new Tile[1] { currentTile };
        targetTile = goal;
        process.Add(FirstPath);

        while (pathIsDone == false && process.Count > 0) //process.Count > 0 && 
        {
            Tile[] t = process[0];
            process.RemoveAt(0);

            if (t[t.Length - 1] == goal)
            {
                pathIsDone = true;
                foreach (Tile tile in t)
                {
                    tile.inPath = true;
                }
            }
            else
            {

                foreach (Tile tile in t[t.Length - 1].adjacentList) // Pour chaque case dans la liste de cases
                {
                    if (!ContainTile(t, tile)) // Si le t ne contient pas la tuile
                    {
                        tile.distance = 1 + t[t.Length - 1].distance;
                        Tile[] newPath = new Tile[t.Length + 1];
                        for (int i = 0; i < t.Length; i++)
                        {
                            newPath[i] = t[i];
                        }
                        newPath[newPath.Length - 1] = tile;
                        process.Insert(0, newPath);
                        process.Sort(SortListCost);

                    }
                }
            }
        }
    }

    public void OptimalSearchBBMove(Tile goal)
    {
        int currentPathCost = 0;
        int lastPathCost = 999999;
        bool firstPathIsDone = false;
        bool secondPathIsDone = false;
        GetListCaseAdjacent();
        GetCurrentTile();
        List<Tile[]> process = new List<Tile[]>();
        Tile[] startPath = new Tile[1] { currentTile };
        Tile[] bestPath = null;
        targetTile = goal;
        process.Add(startPath);

        

        while (process.Count > 0 || secondPathIsDone == true ) //process.Count > 0 && 
        {
            Tile[] t = process[0];
            process.RemoveAt(0);

            if (t[t.Length - 1] == goal && lastPathCost < currentPathCost && firstPathIsDone == true)
            {
                bestPath = t;
                currentPathCost = lastPathCost;
                secondPathIsDone = true;
            }

            if (t[t.Length - 1] == goal && firstPathIsDone == false)
            {
                firstPathIsDone = true;
                bestPath = t;
                foreach (Tile tile in t)
                {
                    currentPathCost = currentPathCost + tile.cost;    
                }
            }
            else
            {
                if (firstPathIsDone == false)
                { 

                    foreach (Tile tile in t[t.Length - 1].adjacentList) // Pour chaque case dans la liste de cases
                    {
                        if (!ContainTile(t, tile))
                        {
                            tile.distance = 1 + t[t.Length - 1].distance;
                            Tile[] newPath = new Tile[t.Length + 1];
                            for (int i = 0; i < t.Length; i++)
                            {
                                newPath[i] = t[i];
                            }
                            newPath[newPath.Length - 1] = tile;
                            process.Insert(0, newPath);
                            process.Sort(SortListCost);
                        }
                    } 
                }
            }
            if (firstPathIsDone == true)
            {
                lastPathCost = 0;
                foreach (Tile tile in t)
                {
                    lastPathCost = lastPathCost + tile.cost;
                }

                    foreach (Tile tile in t[t.Length - 1].adjacentList) // Pour chaque case dans la liste de cases
                    {
                        lastPathCost = lastPathCost + tile.cost;
                        if (!ContainTile(t, tile) && lastPathCost < currentPathCost)
                        {
                            tile.distance = 1 + t[t.Length - 1].distance;
                            Tile[] newPath = new Tile[t.Length + 1];
                            for (int i = 0; i < t.Length; i++)
                            {
                                newPath[i] = t[i];
                            }
                            newPath[newPath.Length - 1] = tile;
                            process.Insert(0, newPath);
                            process.Sort(SortListCost);
                        }
                        lastPathCost = lastPathCost - tile.cost;
                }
                
            }
            
        }
        foreach (Tile tile in bestPath)
        {
            tile.inPath = true;
        }
    }

    public void OptimalSearchBBAMove(Tile goal)
    {
        int currentPathCost = 0;
        int lastPathCost = 999999;
        bool firstPathIsDone = false;
        bool secondPathIsDone = false;
        GetListCaseAdjacent();
        GetCurrentTile();
        Tile[] startPath = new Tile[1] { currentTile };
        Tile[] bestPath = null;
        targetTile = goal;
        processBBA.Add(startPath);



        while (processBBA.Count > 0 && secondPathIsDone == false) //process.Count > 0 && 
        {
            Tile[] t = processBBA[0];
            processBBA.RemoveAt(0);

            if (t[t.Length - 1] == goal && lastPathCost < currentPathCost && firstPathIsDone == true)
            {
                bestPath = t;
                currentPathCost = lastPathCost;
                secondPathIsDone = true;
            }

            if (t[t.Length - 1] == goal && firstPathIsDone == false)
            {
                firstPathIsDone = true;
                bestPath = t;
                foreach (Tile tile in t)
                {
                    currentPathCost = currentPathCost + tile.cost;
                }
            }
            else
            {
                if (firstPathIsDone == false)
                {

                    foreach (Tile tile in t[t.Length - 1].adjacentList) // Pour chaque case dans la liste de cases
                    {
                        if (!ContainTile(t, tile))
                        {
                            tile.distance = 1 + t[t.Length - 1].distance;
                            Tile[] newPath = new Tile[t.Length + 1];
                            for (int i = 0; i < t.Length; i++)
                            {
                                newPath[i] = t[i];
                            }
                            newPath[newPath.Length - 1] = tile;
                            processBBA.Insert(0, newPath);
                            processBBA.Sort(SortListCost);
                        }
                    }
                }
            }
            if (firstPathIsDone == true)
            {
                lastPathCost = 0;
                foreach (Tile tile in t)
                {
                    lastPathCost = lastPathCost + tile.cost;
                }

                foreach (Tile tile in t[t.Length - 1].adjacentList) // Pour chaque case dans la liste de cases
                {
                    lastPathCost = lastPathCost + tile.cost;
                    if (!ContainTile(t, tile) && lastPathCost < currentPathCost)
                    {
                        if (!Redondant(tile, processBBA, lastPathCost))
                        { 
                            tile.distance = 1 + t[t.Length - 1].distance;
                            Tile[] newPath = new Tile[t.Length + 1];
                            for (int i = 0; i < t.Length; i++)
                            {
                                newPath[i] = t[i];
                            }
                            newPath[newPath.Length - 1] = tile;
                            processBBA.Insert(0, newPath);
                            processBBA.Sort(SortListCost);
                        }
                    lastPathCost = lastPathCost - tile.cost;
                    }
                }

            }
        }
        foreach (Tile tile in bestPath)
        {
            tile.inPath = true;
        }
    }

    private bool Redondant(Tile tile, List<Tile[]> process, int lastPathCost)
    {
        int tileIntoListCost = 0;
        foreach (Tile[] tileList in process)
        {
            foreach (Tile tileIntoList in tileList)
            {
                if (tileIntoList == tile)
                {
                    foreach (Tile tileCost in tileList)
                    {
                        tileIntoListCost = tileIntoListCost + tileCost.cost;
                    }
                    if (lastPathCost > tileIntoListCost || lastPathCost == tileIntoListCost)
                    {
                        return true;
                    } else
                    {
                        processBBA.Remove(tileList);
                        return false;
                    }
                }
            }
        }
        return false;
    }

    private int SortListCost(Tile[] x, Tile[] y)
    {
        if (x == null) //Return 1 implique une place plus haute sur la liste [10] tandis que -1 [0]
        {
            if (y == null)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            if (y == null)
            {
                return 1;
            }
            else //Si rien n'est null
            {
                int totalCostX = 0;
                int totalCostY = 0;
                foreach (Tile tileX in x)
                {
                    totalCostX = totalCostX + tileX.cost;
                }

                foreach (Tile tileY in y)
                {
                    totalCostY = totalCostY + tileY.cost;
                }

                if (totalCostX > totalCostY)
                {
                    return 1;
                }
                else if (totalCostX < totalCostY)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    private int SortListHeurestic(Tile[] x, Tile[] y) //Je regarde la distance entre le dernier de la liste et le goal
    {
        if (x == null) //Return 1 implique une place plus haute sur la liste [10] tandis que -1 [0]
        {
            if (y == null)
            {
                // If x is null and y is null, they're
                // equal. 
                return 0;
            }
            else
            {
                // If x is null and y is not null, y
                // is greater. Le x sera plus BAS sur la liste
                return -1;
            }
        }
        else
        {
            // If x is not null...
            //
            if (y == null)
            // ...and y is null, x is greater.
            {
                return 1;
            }
            else //Si rien n'est null
            {
                // ...and y is not null, compare the 
                // lengths of the two strings.
                //
                float distBetweenXG = Vector3.Distance(x[x.Length - 1].transform.position, targetTile.transform.position);
                float distBetweenYG = Vector3.Distance(y[y.Length - 1].transform.position, targetTile.transform.position);

                if (distBetweenXG > distBetweenYG)
                {
                    // If the strings are not of equal length,
                    // the longer string is greater.
                    //
                    return 1;
                }
                else if (distBetweenXG < distBetweenYG)
                {
                    // If the strings are of equal length,
                    // sort them with ordinary string comparison.
                    //
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    private bool CommunTile(Tile[] Start, Tile[] Goal)
    {
        foreach (Tile startingTile in Start)
        {
            foreach (Tile goalTile in Goal)
            {
                if (startingTile == goalTile)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool ContainTile(Tile[] t, Tile tile)
    {
        foreach (Tile tileToCheck in t)
        {
            if (tileToCheck == tile)
            {
                return true;
            }
        }
        return false;
    }

    public void MoveToTile(Tile tile)
    {
        path.Clear();
        tile.target = true;
        moving = true;

        Tile next = tile;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    public void Move()
    {
        if (path.Count > 0)
        {
            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                Calculateddirection(target);
                SetHorizontalVelocity();

                transform.forward = direction;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                transform.position = target;
                path.Pop();

            }
        }
        else
        {
            RemoveSelectableTiles();
            moving = false;
            //previousTile.explode = true;
            //TurnManager.EndTurn();
            GameObject[] tileToDestroy = GameObject.FindGameObjectsWithTag("Tile");
            foreach (GameObject tile in tileToDestroy)
            {
                tile.GetComponent<Tile>().toDestroy = true;
            }
            endExplosion = true;
        }

    }

    private void SetHorizontalVelocity()
    {
        velocity = direction * movespeed;
    }

    private void Calculateddirection(Vector3 target)
    {
        direction = target - transform.position;
        direction.Normalize();
    }

    public void RemoveSelectableTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }
        foreach (Tile tile in selectableTiles)
        {
            tile.Reset();
        }

        selectableTiles.Clear();
    }

    public void BeginTurn()
    {
        turn = true;
    }

    public void EndTurn()
    {
        turn = false;
    }
}


