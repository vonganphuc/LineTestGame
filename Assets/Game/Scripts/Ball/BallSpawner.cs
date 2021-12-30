using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour {
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tileParent;
    [SerializeField] private Color[] possibleColors;
    private Ball[] tiles;
    private Arena arena;
    private Game game;

	void Awake () {
        tiles = GetComponentsInChildren<Ball>();
        arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<Arena>();
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) 
            RandNewTiles();
        if (Input.GetKeyDown(KeyCode.S)) 
            Spawn();
    }

    public void RandNewTiles() {
        foreach(var til in tiles) 
        {
            int rand = Random.Range(0, possibleColors.Length);
            til.color = possibleColors[rand];
        }
    }
	
	public void Spawn() 
    {
        List<ArenaTile> possibleTiles = arena.GetEmptyTiles();

        if(possibleTiles.Count < 6) game.GameLost();
        else {
            foreach (var ti in tiles) {
                ArenaTile pos = possibleTiles[Random.Range(0, possibleTiles.Count)];
                possibleTiles.Remove(pos);
                Ball obj = Instantiate(tilePrefab, tileParent).GetComponent<Ball>();
                obj.Initialize(pos, ti.color);
                Debug.Log("Spawned new tile.", pos);
            }

            RandNewTiles();
            arena.CheckPoints(true);
        }
    }

    //WIP
    //public void SpawnGhost()
    //{
    //    List<ArenaTile> possibleTiles = arena.GetEmptyTiles();

    //    if (possibleTiles.Count < 6) game.GameLost();
    //    else
    //    {
    //        ArenaTile pos = possibleTiles[Random.Range(0, possibleTiles.Count)];
    //        possibleTiles.Remove(pos);
    //        Ball obj = Instantiate(tilePrefab, tileParent).GetComponent<Ball>();
    //        obj.InitializeGhost(pos);
    //        Debug.Log("Spawned new ghost tile.", pos);

    //        RandNewTiles();
    //        arena.CheckPoints(true);
    //    }
    //}
}
