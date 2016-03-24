using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Grid : MonoBehaviour {
	//public static Grid inst;

	//Hex Settings
	private float hexRadius = 0.8f;  //1 initial
	public Material hexMaterial;

	//Generation Options
	public bool addColliders = true;
	public bool drawOutlines = true;
	public Material lineMaterial;

	private Dictionary<string, Tile> grid = new Dictionary<string, Tile>();

	//Internal variables
	private Mesh hexMesh = null;
	private CubeIndex[] directions = 
		new CubeIndex[] {
			new CubeIndex(1, -1, 0), 
			new CubeIndex(1, 0, -1), 
			new CubeIndex(0, 1, -1), 
			new CubeIndex(-1, 1, 0), 
			new CubeIndex(-1, 0, 1), 
			new CubeIndex(0, -1, 1)
		}; 

	#region Getters and Setters
	public Dictionary<string, Tile> Tiles {
		get {return grid;}
	}
	#endregion

	#region Public Methods
	public void GenerateGrid() {

		GetMesh();
		GenHexShape();
	}

	public Tile TileAt(CubeIndex index){
		if(grid.ContainsKey(index.ToString()))
		   return grid[index.ToString()];
		return null;
	}

	public Tile TileAt(int x, int y, int z){
		return TileAt(new CubeIndex(x,y,z));
	}

	public Tile TileAt(int x, int z){
		return TileAt(new CubeIndex(x,z));
	}

	public List<Tile> Neighbours(Tile tile) {
		List<Tile> ret = new List<Tile>();
		CubeIndex o;

		for(int i = 0; i < 6; i++) {
			o = tile.index + directions[i];
			if(grid.ContainsKey(o.ToString()))
				ret.Add(grid[o.ToString()]);
		}
		return ret;
	}

	public List<Tile> Neighbours(CubeIndex index){
		return Neighbours(TileAt(index));
	}

	public List<Tile> Neighbours(int x, int y, int z){
		return Neighbours(TileAt(x,y,z));
	}

	public List<Tile> Neighbours(int x, int z){
		return Neighbours(TileAt(x,z));
	}

	public List<Tile> TilesInRange(Tile center, int range){
		//Return tiles rnage steps from center, http://www.redblobgames.com/grids/hexagons/#range
		List<Tile> ret = new List<Tile>();
		CubeIndex o;

		for(int dx = -range; dx <= range; dx++){
			for(int dy = Mathf.Max(-range, -dx-range); dy <= Mathf.Min(range, -dx+range); dy++){
				o = new CubeIndex(dx, dy, -dx-dy) + center.index;
				if(grid.ContainsKey(o.ToString()))
					ret.Add(grid[o.ToString()]);
			}
		}
		return ret;
	}

	public List<Tile> TilesInRange(CubeIndex index, int range){
		return TilesInRange(TileAt(index), range);
	}

	public List<Tile> TilesInRange(int x, int y, int z, int range){
		return TilesInRange(TileAt(x,y,z), range);
	}

	public List<Tile> TilesInRange(int x, int z, int range){
		return TilesInRange(TileAt(x,z), range);
	}

	public int Distance(CubeIndex a, CubeIndex b){
		return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
	}

	public int Distance(Tile a, Tile b){
		return Distance(a.index, b.index);
	}
	#endregion

	#region Private Methods
	private void Awake() {
		GenerateGrid();
	}

	private void GetMesh() {
		hexMesh = null;
		Tile.GetHexMesh(hexRadius, ref hexMesh);
	}

	private void GenHexShape() {
		Tile tile;
		Vector3 pos = Vector3.zero;
		int mapSize = 3; // how many rings of hexes
		
		for (int q = -mapSize; q <= mapSize; q++){
			int r1 = Mathf.Max(-mapSize, -q-mapSize);
			int r2 = Mathf.Min(mapSize, -q+mapSize);
			for(int r = r1; r <= r2; r++){
				pos.x = hexRadius * Mathf.Sqrt(3.0f) * (q + r/2.0f);
				pos.z = hexRadius * 3.0f/2.0f * r;
				
				tile = CreateHexGO(pos,("Hex[" + q + "," + r + "," + (-q-r).ToString() + "]"));
				tile.index = new CubeIndex(q,r,-q-r);
				grid.Add(tile.index.ToString(), tile);
			}
		}
	}


	private Tile CreateHexGO(Vector3 position, string name) {
		GameObject go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(Tile));

		if(addColliders)
			go.AddComponent<MeshCollider>();

		if(drawOutlines)
			go.AddComponent<LineRenderer>();

		go.transform.position = position;
		go.transform.parent = this.transform;
		go.layer = 8;

		Tile tile = go.GetComponent<Tile>();
		MeshFilter fil = go.GetComponent<MeshFilter>();

		fil.sharedMesh = hexMesh;

		//GenerateTerrain(go,name);

		if(addColliders){
			MeshCollider col = go.GetComponent<MeshCollider>();
			col.sharedMesh = hexMesh;
		}

		if(drawOutlines) {
			LineRenderer lines = go.GetComponent<LineRenderer>();
			lines.useLightProbes = false;
			lines.receiveShadows = false;

			lines.SetWidth(0.03f,.03f);
			lines.SetColors(Color.black, Color.black);
			lines.material = lineMaterial;

			lines.SetVertexCount(7);

			for(int vert = 0; vert <= 6; vert++)
				lines.SetPosition(vert, Tile.Corner(tile.transform.position, hexRadius, vert));
		}

		return tile;
	}
	#endregion
}