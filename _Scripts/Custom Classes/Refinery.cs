using UnityEngine;
using System.Collections;

public class Refinery : GameComponents {

	private string terrain;
	private Tile tile;

	public Refinery(int pc, Tile t){
		this.playerController = pc;
		this.tile = t;
		setCost();
	}

	private void setCost(){
		this.cost.Add(new Commodity(Commodity.CommodityType.Wood));
		this.cost.Add(new Commodity(Commodity.CommodityType.Stone));
		this.cost.Add(new Commodity(Commodity.CommodityType.Ore));
	}

	public void GenerateGoods(Player p){
		if(tile.good != null){
			p.IncreaseCommodity(tile.good);
		}
	}
}
public class AdvancedRefinery : GameComponents {

	private string terrain;
	private Tile tile;

	public AdvancedRefinery(int pc, Tile t){
		this.playerController = pc;
		this.tile = t;
		setCost();
	}

	private void setCost(){
		this.cost.Add(new Commodity(Commodity.CommodityType.Wood));
		this.cost.Add(new Commodity(Commodity.CommodityType.Stone));
		this.cost.Add(new Commodity(Commodity.CommodityType.Ore));
		this.cost.Add(new Commodity(Commodity.CommodityType.Fruit));
		this.cost.Add(new Commodity(Commodity.CommodityType.Spidersilk));
		this.cost.Add(new Commodity(Commodity.CommodityType.Gems));
		this.cost.Add(new Commodity(Commodity.CommodityType.Dragonbone));
		this.cost.Add(new Commodity(Commodity.CommodityType.Livestock));
	}
	public void GenerateAdvancedGoods(Player p){
		if(tile.advGood != null){
			p.IncreaseCommodity(tile.advGood);
		}
	}
}

public class Botanist: GameComponents{
	public Botanist(int pc){
		this.playerController = pc;
		SetCost();
	}

	private void SetCost(){
		this.cost.Add(new Commodity(Commodity.CommodityType.Wheat));
		this.cost.Add(new Commodity(Commodity.CommodityType.Ore));
	}
}

public class DruidsCircle: GameComponents{
	public DruidsCircle(int pc){
		this.playerController = pc;
		SetCost();
	}

	private void SetCost(){
		this.cost.Add(new Commodity(Commodity.CommodityType.Wood));
		this.cost.Add(new Commodity(Commodity.CommodityType.Stone));
	}
}

public class Ranch: GameComponents{
	public Ranch(int pc){
		this.playerController = pc;
		SetCost();
	}

	private void SetCost(){
		this.cost.Add(new Commodity(Commodity.CommodityType.WildGame));
		this.cost.Add(new Commodity(Commodity.CommodityType.Wood));
	}
}

public class AdventurersGuild: GameComponents{
	public AdventurersGuild(int pc){
		this.playerController = pc;
		SetCost();
	}

	private void SetCost(){
		this.cost.Add(new Commodity(Commodity.CommodityType.Stone));
		this.cost.Add(new Commodity(Commodity.CommodityType.Wood));
	}
}

public class MinersGuild: GameComponents{
	public MinersGuild(int pc){
		this.playerController = pc;
		SetCost();
	}

	private void SetCost(){
		this.cost.Add(new Commodity(Commodity.CommodityType.Ore));
		this.cost.Add(new Commodity(Commodity.CommodityType.Wood));
	}
}