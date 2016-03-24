using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Tile : MonoBehaviour {
	public CubeIndex index;

	//Terrain Inherents
	public enum TerrainType {Unexplored, Desert, City, Coastal, Fields, Forest, Hills, Badlands, Mountains};
	public TerrainType tt;
	public Commodity resource, advResource, good, advGood;
	public bool exotic = true;
	public bool coastal;

	//Player Components
	public List<bool> actionCounter = new List<bool>(){false, false, false, false, false, false};
	public List<bool> influence = new List<bool>(){false, false, false, false, false, false};
	public List<bool> basicRefinery = new List<bool>(){false, false, false, false, false, false};
	public List<bool> advancedRefinery = new List<bool>(){false, false, false, false, false, false};
	public List<GameComponents> gameComponents = new List<GameComponents>();

	// setting up a function to be called to set the resources according to the terrain type
	public void SetResources(){
		if(tt == TerrainType.Fields){
			this.resource = new Commodity(Commodity.CommodityType.Wheat);
			this.advResource = new Commodity(Commodity.CommodityType.Fruit);
			this.good = new Commodity(Commodity.CommodityType.Bread);
			this.advGood = new Commodity(Commodity.CommodityType.Liquor);
		}	else if(tt == TerrainType.Forest){
			this.resource = new Commodity(Commodity.CommodityType.Wood);
			this.advResource = new Commodity(Commodity.CommodityType.Spidersilk);
			this.good = new Commodity(Commodity.CommodityType.Lumber);
			this.advGood = new Commodity(Commodity.CommodityType.Fabric);
		} else if(tt == TerrainType.Hills){
			this.resource = new Commodity(Commodity.CommodityType.WildGame);
			this.advResource = new Commodity(Commodity.CommodityType.Livestock);
			this.good = new Commodity(Commodity.CommodityType.Furs);
			this.advGood = new Commodity(Commodity.CommodityType.ExoticPets);
		} else if(tt == TerrainType.Badlands){
			this.resource = new Commodity(Commodity.CommodityType.Stone);
			this.advResource = new Commodity(Commodity.CommodityType.Dragonbone);
			this.good = new Commodity(Commodity.CommodityType.Marble);
			this.advGood = new Commodity(Commodity.CommodityType.Armour);
		} else if (tt == TerrainType.Mountains){
			this.resource = new Commodity(Commodity.CommodityType.Ore);
			this.advResource = new Commodity(Commodity.CommodityType.Gems);
			this.good = new Commodity(Commodity.CommodityType.Weapons);
			this.advGood = new Commodity(Commodity.CommodityType.Jewelry);
		} 
	}

	public void ExploreTile(Material mat){
		switch (mat.name.ToString()) {
			case "Fields":
				this.tt = TerrainType.Fields;
			break;
			case "Forest":
				this.tt = TerrainType.Forest;
			break;
			case "Hills":
				this.tt = TerrainType.Hills;
			break;
			case "Badlands":
				this.tt = TerrainType.Badlands;
			break;
			case "Mountains":
				this.tt = TerrainType.Mountains;
			break;
			case "Coast":
				this.tt = TerrainType.Coastal;
			break;
		}
	}

	public static Vector3 Corner(Vector3 origin, float radius, int corner){
		float angle = 60 * corner + 30;
		angle *= Mathf.PI / 180;
		return new Vector3(origin.x + radius * Mathf.Cos(angle), 0.0f, origin.z + radius * Mathf.Sin(angle));
	}

	public static void GetHexMesh(float radius, ref Mesh mesh) {
		mesh = new Mesh();

		List<Vector3> verts = new List<Vector3>();
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		for (int i = 0; i < 6; i++)
			verts.Add(Corner(Vector3.zero, radius, i));

		tris.Add(0);
		tris.Add(2);
		tris.Add(1);
		
		tris.Add(0);
		tris.Add(5);
		tris.Add(2);
		
		tris.Add(2);
		tris.Add(5);
		tris.Add(3);
		
		tris.Add(3);
		tris.Add(5);
		tris.Add(4);

		uvs.Add(new Vector2(0.5f, 1f));
		uvs.Add(new Vector2(1, 0.75f));
		uvs.Add(new Vector2(1, 0.25f));
		uvs.Add(new Vector2(0.5f, 0));
		uvs.Add(new Vector2(0, 0.25f));
		uvs.Add(new Vector2(0, 0.75f));

		mesh.vertices = verts.ToArray();
		mesh.triangles = tris.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.name = "Hexagonal Plane";

		mesh.RecalculateNormals();
	}

	#region Coordinate Conversion Functions
	public static OffsetIndex CubeToEvenFlat(CubeIndex c) {
		OffsetIndex o;
		o.row = c.x;
		o.col = c.z + (c.x + (c.x&1)) / 2;
		return o;
	}

	public static CubeIndex EvenFlatToCube(OffsetIndex o){
		CubeIndex c;
		c.x = o.col;
		c.z = o.row - (o.col + (o.col&1)) / 2;
		c.y = -c.x - c.z;
		return c;
	}

	public static OffsetIndex CubeToOddFlat(CubeIndex c) {
		OffsetIndex o;
		o.col = c.x;
		o.row = c.z + (c.x - (c.x&1)) / 2;
		return o;
	}
	
	public static CubeIndex OddFlatToCube(OffsetIndex o){
		CubeIndex c;
		c.x = o.col;
		c.z = o.row - (o.col - (o.col&1)) / 2;
		c.y = -c.x - c.z;
		return c;
	}

	public static OffsetIndex CubeToEvenPointy(CubeIndex c) {
		OffsetIndex o;
		o.row = c.z;
		o.col = c.x + (c.z + (c.z&1)) / 2;
		return o;
	}
	
	public static CubeIndex EvenPointyToCube(OffsetIndex o){
		CubeIndex c;
		c.x = o.col - (o.row + (o.row&1)) / 2;
		c.z = o.row;
		c.y = -c.x - c.z;
		return c;
	}

	public static OffsetIndex CubeToOddPointy(CubeIndex c) {
		OffsetIndex o;
		o.row = c.z;
		o.col = c.x + (c.z - (c.z&1)) / 2;
		return o;
	}
	
	public static CubeIndex OddPointyToCube(OffsetIndex o){
		CubeIndex c;
		c.x = o.col - (o.row - (o.row&1)) / 2;
		c.z = o.row;
		c.y = -c.x - c.z;
		return c;
	}

	public static Tile operator+ (Tile one, Tile two){
		Tile ret = new Tile();
		ret.index = one.index + two.index;
		return ret;
	}

	public void LineColour(Color colour) {
		LineRenderer lines = GetComponent<LineRenderer>();
		if(lines)
			lines.SetColors(colour, colour);
	}

	public void LineColour(Color start, Color end){
		LineRenderer lines = GetComponent<LineRenderer>();
		if(lines)
			lines.SetColors(start, end);
	}

	public void LineWidth(float width){
		LineRenderer lines = GetComponent<LineRenderer>();
		if(lines)
			lines.SetWidth(width, width);
	}

	public void LineWidth(float start, float end){
		LineRenderer lines = GetComponent<LineRenderer>();
		if(lines)
			lines.SetWidth(start, end);
	}
	#endregion

	#region A* Herustic Variables
	public int MoveCost { get; set; }
	public int GCost { get; set; }
	public int HCost { get; set; }
	public int FCost { get { return GCost + HCost; } }
	public Tile Parent { get; set; }
	#endregion
}

[System.Serializable]
public struct OffsetIndex {
	public int row;
	public int col;

	public OffsetIndex(int row, int col){
		this.row = row; this.col = col;
	}
}

[System.Serializable]
public struct CubeIndex {
	public int x;
	public int y;
	public int z;

	public CubeIndex(int x, int y, int z){
		this.x = x; this.y = y; this.z = z;
	}

	public CubeIndex(int x, int z) {
		this.x = x; this.z = z; this.y = -x-z;
	}

	public static CubeIndex operator+ (CubeIndex one, CubeIndex two){
		return new CubeIndex(one.x + two.x, one.y + two.y, one.z + two.z);
	}

	public override bool Equals (object obj) {
		if(obj == null)
			return false;
		CubeIndex o = (CubeIndex)obj;
		if((System.Object)o == null)
			return false;
		return((x == o.x) && (y == o.y) && (z == o.z));
	}

	public override int GetHashCode () {
		return(x.GetHashCode() ^ (y.GetHashCode() + (int)(Mathf.Pow(2, 32) / (1 + Mathf.Sqrt(5))/2) + (x.GetHashCode() << 6) + (x.GetHashCode() >> 2)));
	}

	public override string ToString () {
		return string.Format("[" + x + "," + y + "," + z + "]");
	}
}