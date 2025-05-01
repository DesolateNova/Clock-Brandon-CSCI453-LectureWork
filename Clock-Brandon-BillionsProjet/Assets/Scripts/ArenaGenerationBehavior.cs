using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;


public class ArenaGenerationBehavior : MonoBehaviour
{

    [SerializeField, Tooltip("Size of Arena")] Vector2Int arenaSize;
    [SerializeField, Tooltip("Percentage of Arena as floor")] float arenaRatio;
    [SerializeField, Tooltip("Included Bases")] GameObject[] bases;

    public Grid[,] tileGrid;
    public List<ArenaWalker> walkers;
    public Tilemap tileMapA;
    public Tilemap tileMapW;
    public Tile arena;
    public Tile wall;
    public Tile baseSpawner;
    

    public readonly int MAX_WALKERS = 6;
    public int tileCount = default;
    private readonly float waitTimer = 0.0f;


    private readonly float MAKE_BASE_SPAWNER_RATIO = 0.20f;
    private readonly int baseSpawnMinDist = 4;


    private static List<GameObject> activeBases = new List<GameObject>();

    private int MAX_BASES;
    private int numBases;
    private int baseIndex = 0;

    private  new GameObject camera;
    private GameObject arenaCenter;

    public static bool isLoaded = false;

    void Awake()
    {
        MAX_BASES = bases.Length;

        CreateGrid();
        camera = GameObject.Find("Main Camera");

        if (camera != null)
        {
            camera.transform.position = new Vector3(arenaSize.x / 2, arenaSize.y / 2, camera.transform.position.z);
        }
    }

    IEnumerator Start()
    {
        arenaCenter = Instantiate(Resources.Load<GameObject>("Prefabs/Arena Center"), transform.position, Quaternion.identity);
        arenaCenter.GetComponent<GameManager>().enabled = true;
        arenaCenter.GetComponent<ProxyManager>().enabled = true;
        arenaCenter.GetComponent<MovementBehavior>().enabled = true;
        arenaCenter.transform.position = new Vector3(arenaSize.x / 2, arenaSize.y / 2, 0);
        arenaCenter.name = "Arena Center";

        foreach (GameObject b in activeBases)
        {
            b.SetActive(true);
        }

        isLoaded = true;

        yield return null;
    }

    private void CreateGrid()
    {
        tileGrid = new Grid[arenaSize.x, arenaSize.y];
        int gridLen = arenaSize.x;
        int gridHeight = arenaSize.y;

        for (int x = 0; x < gridLen; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                tileGrid[x, y] = Grid.EMPTY;
            }
        }

        walkers = new List<ArenaWalker>();

        Vector3Int arenaCenter = new Vector3Int(gridLen / 2, gridHeight / 2, 0);
        ArenaWalker currentWalker = new ArenaWalker(new Vector2(arenaCenter.x, arenaCenter.y), GetDirection(), 0.5f);
        tileGrid[arenaCenter.x, arenaCenter.y] = Grid.ARENA;
        tileMapA.SetTile(arenaCenter, arena);
        walkers.Add(currentWalker);

        tileCount++;
        StartCoroutine(CreateFloor());
    }

    IEnumerator CreateFloor()
    {
        while ((float)tileCount / (float)tileGrid.Length < arenaRatio || numBases < MAX_BASES)
        {
            bool hasCreatedFloor = false;
            foreach (ArenaWalker curWalker in walkers)
            {
                Vector3Int curPos = new Vector3Int((int)curWalker.pos.x, (int)curWalker.pos.y, 0);
                Vector3 tileCenter = new Vector3(curPos.x + 0.5f, curPos.y + 0.5f, 0f);

                if (tileGrid[curPos.x, curPos.y] != Grid.ARENA)
                {

                    if (UnityEngine.Random.value < MAKE_BASE_SPAWNER_RATIO)
                    {
                        if (baseIndex == 0 || (baseIndex < MAX_BASES && ValidDistance(tileCenter)))
                        {
                            //
                            GameObject b = Instantiate(bases[baseIndex], tileCenter, Quaternion.identity);
                            activeBases.Add(b);
                            baseIndex++;
                            numBases++;
                            //
                        }
                    }

                    tileMapA.SetTile(curPos, arena);
                    tileCount++;
                    tileGrid[curPos.x, curPos.y] = Grid.ARENA;
                    hasCreatedFloor = true;

                }
            }

            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePos();

            if (hasCreatedFloor)
            {
                yield return new WaitForSeconds(waitTimer);
            }
        }
        StartCoroutine(CreateWalls());
        StartCoroutine(Start());
    }


    IEnumerator CreateWalls()
    {
        for (int x = 0; x < tileGrid.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < tileGrid.GetLength(1) - 1; y++)
            {
                if (tileGrid[x, y] == Grid.ARENA)
                {
                    bool hasCreatedWalll = false;

                    if (tileGrid[x + 1, y] == Grid.EMPTY)
                    {
                        tileMapW.SetTile(new Vector3Int(x + 1, y, 0), wall);
                        tileGrid[x + 1, y] = Grid.WALL;
                        hasCreatedWalll = true;
                    }
                    if (tileGrid[x - 1, y] == Grid.EMPTY)
                    {
                        tileMapW.SetTile(new Vector3Int(x - 1, y, 0), wall);
                        tileGrid[x - 1, y] = Grid.WALL;
                        hasCreatedWalll = true;
                    }
                    if (tileGrid[x, y + 1] == Grid.EMPTY)
                    {
                        tileMapW.SetTile(new Vector3Int(x, y + 1, 0), wall);
                        tileGrid[x, y + 1] = Grid.WALL;
                        hasCreatedWalll = true;
                    }
                    if (tileGrid[x, y - 1]  == Grid.EMPTY)
                    {
                        tileMapW.SetTile(new Vector3Int(x, y - 1, 0), wall);
                        tileGrid[x, y - 1] = Grid.WALL;
                        hasCreatedWalll = true;
                    }

                    if (hasCreatedWalll)
                    {
                        yield return new WaitForSeconds(waitTimer);
                    }
                    Debug.Log($"{x}, {y}");
                }
            }
        }
    }

    void ChanceToRemove()
    {
        int updatedCount = walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (UnityEngine.Random.value < walkers[i].changeChance && walkers.Count > 1)
            {
                walkers.RemoveAt(i);
                break;
            }
        }
    }

    void ChanceToRedirect()
    {
        for (int i = 0; i < walkers.Count; i++)
        {
            if (UnityEngine.Random.value < walkers[i].changeChance)
            {
                ArenaWalker curWalker = walkers[i];
                curWalker.dir = GetDirection();
                walkers[i] = curWalker;
            }
        }

    }

    void ChanceToCreate()
    {
        int updatedCount = walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (UnityEngine.Random.value < walkers[i].changeChance && walkers.Count < MAX_WALKERS)
            {
                Vector3 newDir = GetDirection();
                Vector2 newPos = walkers[i].pos;

                ArenaWalker newWalker = new ArenaWalker(newPos, newDir, 0.5f);
                walkers.Add(newWalker);
            }
        }
    }

    Vector2 GetDirection()
    {

        int choice = Mathf.FloorToInt(UnityEngine.Random.value * 3.99f);

        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            case 3:
                return Vector2.right;
            default:
                return Vector2.zero;

        }
    }

    void UpdatePos()
    {

        for (int i = 0; i < walkers.Count; i++)
        {
            ArenaWalker foundWalker = walkers[i];
            foundWalker.pos += foundWalker.dir;
            foundWalker.pos.x = Mathf.Clamp(foundWalker.pos.x, 1, tileGrid.GetLength(0) - 2);
            foundWalker.pos.y = Mathf.Clamp(foundWalker.pos.y, 1, tileGrid.GetLength(1) - 2);
            walkers[i] = foundWalker;
        }
    }

    bool ValidDistance(Vector3 posToCheck)
    {
        foreach (GameObject b in activeBases)
        {
            if (Vector3.Distance(b.transform.position, posToCheck) <= baseSpawnMinDist)
                return false;
        }
        return true;
    }
}
