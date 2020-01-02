using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour //Contient toutes les informations sur les tiles
{
    public bool walkable = true;
    public bool current = false;
    public bool visited = false;
    public bool target = false;
    public bool selectable = false;
    public bool inPath = false;
    public bool inPathFromStart = false;
    public bool inPathFromGoal = false;
    public bool explode;

    public List<Tile> adjacentList = new List<Tile>();
    public Tile parent = null;

    public float explosionForce = 500f;
    public float explosionRadius = 4f;
    public float explosionUpward = 0.4f;
    public float cubeSize = 0.15f;
    public int cubeInRows = 5;

    public int distance = 0; 
    public int cost = 1;

    // Start is called before the first frame update
    void Start()
    {
        cubeInRows = 4;
    }

    // Update is called once per frame
    void Update()
    {
        if (current)
        {
            GetComponent<Renderer>().material.color = Color.magenta;
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if (inPath)
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (inPathFromStart)
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (inPathFromGoal)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
        if (explode)
        {
            explosion();
        }
    }

    public void explosion()
    {
        this.gameObject.SetActive(false);

        //loop 3 times to create 5x5x5 piece in x, y, z axis
        for (int x = 0; x < cubeInRows; x++)
        {
            for (int y = 0; y < cubeInRows; y++)
            {
                for (int z = 0; z < cubeInRows; z++)
                {
                    createPiece(x,y,z);
                }
            }
        }

        //get explosiont position
        Vector3 explosionPos = this.transform.position;
        
        //get Collider in that position and radius
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);

        //Add explosionForce foreach Collider
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, this.transform.position, explosionRadius, explosionUpward);
            }
            
        }
    }

    void createPiece(int x, int y, int z)
    {
        float cubesPivotDistance = cubeSize * cubeInRows / 2;
        Vector3 CubePivot = new Vector3(cubesPivotDistance,cubesPivotDistance,cubesPivotDistance);

        //Create piece
        GameObject piece;
        piece = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //Set piece position and scale
        piece.transform.position = this.transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - CubePivot;
        piece.transform.localScale = new Vector3(cubeSize,cubeSize,cubeSize);

        //add rigidbody and mass
        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = 0.2f;
    }

    public void Reset()
    {
        adjacentList.Clear();
        current = false;
        target = false;
        selectable = false;

        //BFS (breath first search)
        visited = false;
        parent = null;
        distance = 0;

    }

    public void FindNeighbors(float jumpHeight)
    {
        Reset();
        CheckTile(Vector3.forward, jumpHeight);
        CheckTile(-Vector3.forward, jumpHeight);
        CheckTile(Vector3.right, jumpHeight);
        CheckTile(-Vector3.right, jumpHeight);
        CheckTile(Vector3.right + Vector3.forward, jumpHeight);
        CheckTile(-Vector3.right + Vector3.forward, jumpHeight);
        CheckTile(Vector3.right - Vector3.forward, jumpHeight);
        CheckTile(-Vector3.right - Vector3.forward, jumpHeight);
    }

    public void CheckTile(Vector3 direction, float jumpHeight)
    {
        Vector3 halfExtends = new Vector3(0.25f, (1 + jumpHeight) / 2.0f, 0.25f); //attention le milieu pour le jump
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtends);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null && tile.walkable)
            {
                RaycastHit hit;

                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                {
                    adjacentList.Add(tile);

                }
            }
        }
    }
}
