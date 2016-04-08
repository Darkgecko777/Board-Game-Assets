using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameComponents {

	//public GameComponents componentType;
	public int playerController;
	public int amount;
	public List<Commodity> cost;
	public bool isMartial;

	public GameComponents(){
		this.amount = 0;
		this.cost = new List<Commodity>();
	}
	public override string ToString ()
	{
		return string.Format (this.GetType().ToString());
	}
}

public class Peasant : GameComponents {

	//private string terrain;
	public Tile tile;
	public bool hasMoved;

	public Peasant(int pc, Tile t){
		this.playerController = pc;
		this.tile = t;
		setCost();
	}

	public Peasant(int pc, Tile t, bool hm){
		this.hasMoved = hm;
		this.playerController = pc;
		this.tile = t;
		setCost();
	}

	private void setCost(){
		this.cost.Add(new Commodity(Commodity.CommodityType.Wheat));
		this.cost.Add(new Commodity(Commodity.CommodityType.Wheat));
		this.cost.Add(new Commodity(Commodity.CommodityType.WildGame));
		this.cost.Add(new Commodity(Commodity.CommodityType.WildGame));
	}

	public void GenerateResource(Player p){
		if(tile.resource != null){
			p.IncreaseCommodity(tile.resource);
		}
	}

	public void GenerateAdvancedResource(Player p){
		if(tile.advResource != null){
			p.IncreaseCommodity(tile.advResource);
		}
	}

}

public class Burgher : GameComponents {

	public Burgher(int pc){
		this.playerController = pc;
		setCost();
	}

	private void setCost(){
		this.cost.Add(new Commodity(Commodity.CommodityType.Wheat));
		this.cost.Add(new Commodity(Commodity.CommodityType.Spidersilk));
		this.cost.Add(new Commodity(Commodity.CommodityType.WildGame));
	}
}

public class Warrior : GameComponents {

	public int hitValue;

	public Warrior(int pc){
		this.playerController = pc;
		this.isMartial = true;
		setCost();
		this.hitValue = 5;
	}

	private void setCost(){
		this.cost.Add(new Commodity(Commodity.CommodityType.Wheat));
		this.cost.Add(new Commodity(Commodity.CommodityType.Wheat));
		this.cost.Add(new Commodity(Commodity.CommodityType.WildGame));
		this.cost.Add(new Commodity(Commodity.CommodityType.WildGame));
		this.cost.Add(new Commodity(Commodity.CommodityType.Ore));
		this.cost.Add(new Commodity(Commodity.CommodityType.Dragonbone));
	}

	public bool CombatRoll(){
		if(UnityEngine.Random.Range(1,10) >= hitValue){
			return true;
		} else {
			return false;
		}
	}
}

public class Unique : GameComponents {

	public int hitValue;

	public Unique(int pc){
		this.playerController = pc;
		this.isMartial = true;
		setCost();
		this.hitValue = 3;
	}

	private void setCost(){
		this.cost.Add(new Commodity(Commodity.CommodityType.Wheat));
		this.cost.Add(new Commodity(Commodity.CommodityType.Wheat));
		this.cost.Add(new Commodity(Commodity.CommodityType.WildGame));
		this.cost.Add(new Commodity(Commodity.CommodityType.WildGame));
	}

	public bool CombatRoll(){
		if(UnityEngine.Random.Range(1,10) >= hitValue){
			return true;
		} else {
			return false;
		}
	}
}