using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    static Dictionary<string, List<TacticsMove>> units = new Dictionary<string, List<TacticsMove>>();
    static Queue<string> turnKey = new Queue<string>();
    static Queue<TacticsMove> TurnTeam = new Queue<TacticsMove>();
    List<Tile> currentUnitPlayerTiles = new List<Tile>();
    List<Tile> currentUnitNPCTiles = new List<Tile>();
    List<Vector3> currentUnitPlayerVectorTiles = new List<Vector3>();
    List<Vector3> currentUnitNPCVectorTiles = new List<Vector3>();
    public static TurnManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnTeam.Count == 0)
        {
           InitTeamTurnQueue();
        }

    }

    static void InitTeamTurnQueue()
    {
        List<TacticsMove> teamList = units[turnKey.Peek()];
        foreach (TacticsMove unit in teamList)
        {
            TurnTeam.Enqueue(unit);
        }

        StartTurn();
    }

    public static void StartTurn()
    {
        if (TurnTeam.Count > 0)
        {
            TurnTeam.Peek().BeginTurn();
        }
    }

    public static void EndTurn()
    {
        TacticsMove unit = TurnTeam.Dequeue();
        unit.EndTurn();

        if (TurnTeam.Count > 0)
        {
            StartTurn();
        }
        else
        {
            string team = turnKey.Dequeue();
            turnKey.Enqueue(team);
            InitTeamTurnQueue();
        }
    }

    public static void AddUnit(TacticsMove unit)
    {
        List<TacticsMove> list;

        if (!units.ContainsKey(unit.tag))
        {
            list = new List<TacticsMove>();
            units[unit.tag] = list;

            if (!turnKey.Contains(unit.tag))
            {
                turnKey.Enqueue(unit.tag);
            }
        }
        else
        {
            list = units[unit.tag];
        }
        list.Add(unit);
    }

    public List<Tile> getPositionUnitsTilePlayer()
    {
        currentUnitPlayerTiles.Clear();
        GameObject[] Player = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject currentTile in Player)
        {
            currentUnitPlayerTiles.Add(currentTile.GetComponent<TacticsMove>().GetCurrentTileWithReturn());
        }
        return currentUnitPlayerTiles;
    }

    public List<Tile> getPositionUnitsTileNPC()
    {
        currentUnitNPCTiles.Clear();
        GameObject[] Player = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject currentTile in Player)
        {
            currentUnitNPCTiles.Add(currentTile.GetComponent<TacticsMove>().GetCurrentTileWithReturn());
        }
        return currentUnitNPCTiles;
    }

    public List<Vector3> getPositionUnitsVectorPlayer()
    {
        getPositionUnitsTilePlayer();
        currentUnitPlayerVectorTiles.Clear();
        foreach (Tile tilePosition in currentUnitPlayerTiles)
        {
            currentUnitPlayerVectorTiles.Add(tilePosition.transform.position);
        }
        return currentUnitPlayerVectorTiles;
    }

    public List<Vector3> getPositionUnitsVectorNPC()
    {
        getPositionUnitsTileNPC();
        currentUnitNPCVectorTiles.Clear();
        foreach (Tile tilePosition in currentUnitNPCTiles)
        {
            currentUnitNPCVectorTiles.Add(tilePosition.transform.position);
        }
        return currentUnitNPCVectorTiles;
    }
}
