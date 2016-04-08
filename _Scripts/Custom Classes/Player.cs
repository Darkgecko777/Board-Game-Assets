using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {

	public int playerNumber;
	public bool activePlayer;
	public int gold;
	public int actionCounter, peasants, basicRefinery, advancedRefinery, burgher, warrior, unique;
	public Dictionary<Commodity.CommodityType, Commodity> commodities;
	public Tile homeCity = null; 
	public List<GameComponents> units, buildings;
	// trade matrix
	public enum BasicResources {Wheat, Wood, WildGame, Stone, Ore}
	public enum AdvancedResources {Fruit, Spidersilk, Livestock, Dragonbone, Gems}
	public enum BasicGoods {Bread, Lumber, Furs, Marble, Weapons}
	public enum AdvancedGoods {Liquor, Fabric, ExoticPets, Armour, Jewelry}
	public BasicResources bResources;
	public AdvancedResources advResources;
	public BasicGoods bGoods;
	public AdvancedGoods advGoods;
//	public List<string> basicRM, advRM, basicGM, advGM = new List<string>();
//	public List<bool> basicResources, advancedResources, basicGoods, advancedGoods; // trade matrix
//	public List<Commodity.CommodityType> basicResourcesType, advancedResourcesType, basicGoodsType, advancedGoodsType; 

	public Player(int p){
		this.playerNumber = p;
		this.activePlayer = false;
		this.gold = 0;
		this.commodities = Commodity.InitCommodities();
		this.buildings = new List<GameComponents>();
		this.units = new List<GameComponents>();
		this.actionCounter = 4;

		for(int pe = 0; pe < 3; pe++){
			units.Add(new Peasant(p, null));
		}
		for(int b = 0; b < 5; b++){
			units.Add(new Burgher(p));
		}
		for(int w = 0; w < 2; w++){
			units.Add(new Warrior(p));
		}
		units.Add(new Unique(p));

		// trade matrix
		this.bResources = BasicResources.Wheat;
		this.advResources = AdvancedResources.Fruit;
		this.bGoods = BasicGoods.Bread;
		this.advGoods = AdvancedGoods.Liquor;
//		this.basicGoods = new List<bool>{false, false, false, false, false};
//		this.advancedGoods = new List<bool>{false, false, false, false, false};
//		this.basicResources = new List<bool>{false, false, false, false, false};
//		this.advancedResources = new List<bool>{false, false, false, false, false};
//		this.basicResourcesType = new List<Commodity.CommodityType>{Commodity.CommodityType.Wheat, Commodity.CommodityType.Wood, Commodity.CommodityType.WildGame, 
//			Commodity.CommodityType.Stone, Commodity.CommodityType.Ore};
//		this.advancedResourcesType = new List<Commodity.CommodityType>{Commodity.CommodityType.Fruit, Commodity.CommodityType.Spidersilk, Commodity.CommodityType.Livestock, 
//			Commodity.CommodityType.Dragonbone, Commodity.CommodityType.Gems};
//		this.basicGoodsType = new List<Commodity.CommodityType>{Commodity.CommodityType.Bread, Commodity.CommodityType.Lumber, Commodity.CommodityType.Furs, 
//			Commodity.CommodityType.Marble, Commodity.CommodityType.Weapons};
//		this.advancedGoodsType =new List<Commodity.CommodityType>{Commodity.CommodityType.Liquor, Commodity.CommodityType.Fabric, Commodity.CommodityType.ExoticPets, 
//			Commodity.CommodityType.Armour, Commodity.CommodityType.Jewelry};
	}

	public void IncreaseCommodity(Commodity.CommodityType ct, int amount){
		this.commodities[ct].amount += amount;
	}

	public void IncreaseCommodity(Commodity c){
		this.commodities[c.ct].amount ++;
	}

	public void DecreaseCommodity(Commodity.CommodityType ct, int amount){
		this.commodities[ct].amount -= amount;
	}
}
