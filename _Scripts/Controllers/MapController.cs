using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MapController : MonoBehaviour {

	// Terrain Visuals
	public Material forest;
	public Material badlands;
	public Material fields;
	public Material mountains;
	public Material hills;
	public Material city;
	public Material desert;
	public Material coast;
	public Material unexplored;

	public Grid grid;
	public List<Tile> playerStart = new List<Tile>();

	private Dictionary<string, Tile> tiles;

	// why not just have a list with all the tile terrains?
	public List<Material> localTerrain = new List<Material>();
	public List<Material> exoticTerrain = new List<Material>();

	// generates preset numbers of each terrain to be allocated to tiles based on local or exotic
	public void GeneratePools(){
		for (int i = 0; i < 4; i++){
			localTerrain.Add(fields);
		}
		for (int i = 0; i < 3; i++){
			localTerrain.Add(forest);
		}
		for (int i = 0; i < 4; i++){
			localTerrain.Add(hills);
		}
		for (int i = 0; i < 2; i++){
			localTerrain.Add(badlands);
		}
		for (int i = 0; i < 2; i++){
			localTerrain.Add(mountains);
		}
		for (int i = 0; i < 9; i++){
			localTerrain.Add(coast);
		}
		for (int i = 0; i < 3; i++){
			exoticTerrain.Add(fields);
		}
		for (int i = 0; i < 4; i++){
			exoticTerrain.Add(forest);
		}
		for (int i = 0; i < 3; i++){
			exoticTerrain.Add(hills);
		}
		for (int i = 0; i < 4; i++){
			exoticTerrain.Add(badlands);
		}
		for (int i = 0; i < 4; i++){
			exoticTerrain.Add(mountains);
		}
	}


	// currently there are a fixed number of tiles in the pool and each one is allocated at startup.  There needs to be an excess of 
	// tiles and the pool needs to be divided into local and exotic, with terrain using a higher move cost in the exotic
	public void StartUp () {
		GeneratePools();
		tiles = grid.Tiles;
		//int tileCount = tiles.Count; // counts the total tiles in the map

		// sets all 6 starting locations
		playerStart.Add(grid.TileAt(0,3,-3));
		playerStart.Add(grid.TileAt(3,0,-3));
		playerStart.Add(grid.TileAt(3,-3,0));
		playerStart.Add(grid.TileAt(0,-3,3));
		playerStart.Add(grid.TileAt(-3,0,3));
		playerStart.Add(grid.TileAt(-3,3,0));

		MakeMap();
		SetLocalTiles();
		SetCoastalTiles();
	}

	private void MakeMap(){
		foreach(Tile tile in tiles.Values){
			MeshRenderer ren = tile.GetComponent<MeshRenderer>();
			if(tile.transform.position == Vector3.zero){ // Sets center tile to desert
				ren.material = desert;
				tile.tt = Tile.TerrainType.Desert;
			} else if(playerStart.Contains(tile)){ // sets terrain for cities
				ren.material = city;
				tile.exotic = false;
				tile.tt = Tile.TerrainType.City;
			} else {
				ren.material = unexplored;
				tile.tt = Tile.TerrainType.Unexplored;
			}
		}
	}

	private void SetLocalTiles(){
		foreach(Tile tile in playerStart){
			tile.exotic = false;
			foreach(Tile tn in grid.Neighbours(tile)){
				tn.exotic = false;
				}
			}
	}

	private void SetCoastalTiles(){
		foreach(Tile tile in tiles.Values){
			if (!grid.TilesInRange(grid.TileAt(0,0,0),2).Contains(tile) && !playerStart.Contains(tile)){
				tile.coastal = true;
			}
		}
	}
}