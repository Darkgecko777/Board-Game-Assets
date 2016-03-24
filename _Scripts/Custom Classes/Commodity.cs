using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Commodity {

	//public string type;
	public int value;
	public int amount;
	public enum CommodityType {Wheat, Wood, WildGame, Stone, Ore, Fruit, Spidersilk, Livestock, Dragonbone, Gems, Bread, Lumber,
							Furs, Marble, Weapons, Liquor, Fabric, ExoticPets, Armour, Jewelry, Null};
	public CommodityType ct;

	public Commodity(){
	}

	// Constructor for player inventory
	public Commodity(CommodityType type){
		this.ct = type;
		this.amount = 0;
		switch(ct){
			case CommodityType.Wheat:
				this.value = 1;
				break;
			case CommodityType.Wood:
				this.value = 1;
				break;
			case CommodityType.WildGame:
				this.value = 1;
				break;
			case CommodityType.Stone:
				this.value = 1;
				break;
			case CommodityType.Ore:
				this.value = 2;
				break;
			case CommodityType.Fruit:
				this.value = 2;
				break;
			case CommodityType.Spidersilk:
				this.value = 2;
				break;
			case CommodityType.Livestock:
				this.value = 2;
				break;
			case CommodityType.Dragonbone:
				this.value = 2;
				break;
			case CommodityType.Gems:
				this.value = 3;
				break;
			case CommodityType.Bread:
				this.value = 3;
				break;
			case CommodityType.Lumber:
				this.value = 3;
				break;
			case CommodityType.Furs:
				this.value = 3;
				break;
			case CommodityType.Marble:
				this.value = 3;
				break;
			case CommodityType.Weapons:
				this.value = 4;
				break;
			case CommodityType.Liquor:
				this.value = 4;
				break;
			case CommodityType.Fabric:
				this.value = 4;
				break;
			case CommodityType.ExoticPets:
				this.value = 4;
				break;
			case CommodityType.Armour:
				this.value = 4;
				break;
			case CommodityType.Jewelry:
				this.value = 5;
				break;
//			default:
//				Debug.Log("Non Standard Commodity Referenced");
//				break;
		}
	}

	// Constructor for production
	public Commodity(CommodityType type, int amount){
		this.ct = type;
		this.amount = amount;
	}

	public static Dictionary<CommodityType, Commodity> InitCommodities(){
		Dictionary<CommodityType, Commodity> list = new Dictionary<CommodityType, Commodity>();
		list.Add(CommodityType.Wheat, new Commodity(CommodityType.Wheat));
		list.Add(CommodityType.Wood, new Commodity(CommodityType.Wood));
		list.Add(CommodityType.WildGame, new Commodity(CommodityType.WildGame));
		list.Add(CommodityType.Stone, new Commodity(CommodityType.Stone));
		list.Add(CommodityType.Ore, new Commodity(CommodityType.Ore));
		list.Add(CommodityType.Fruit, new Commodity(CommodityType.Fruit));
		list.Add(CommodityType.Spidersilk, new Commodity(CommodityType.Spidersilk));
		list.Add(CommodityType.Livestock, new Commodity(CommodityType.Livestock));
		list.Add(CommodityType.Dragonbone, new Commodity(CommodityType.Dragonbone));
		list.Add(CommodityType.Gems, new Commodity(CommodityType.Gems));
		list.Add(CommodityType.Bread, new Commodity(CommodityType.Bread));
		list.Add(CommodityType.Lumber, new Commodity(CommodityType.Lumber));
		list.Add(CommodityType.Furs, new Commodity(CommodityType.Furs));
		list.Add(CommodityType.Marble, new Commodity(CommodityType.Marble));
		list.Add(CommodityType.Weapons, new Commodity(CommodityType.Weapons));
		list.Add(CommodityType.Liquor, new Commodity(CommodityType.Liquor));
		list.Add(CommodityType.Fabric, new Commodity(CommodityType.Fabric));
		list.Add(CommodityType.ExoticPets, new Commodity(CommodityType.ExoticPets));
		list.Add(CommodityType.Armour, new Commodity(CommodityType.Armour));
		list.Add(CommodityType.Jewelry, new Commodity(CommodityType.Jewelry));
		return list;
	}

	public static CommodityType ConvertString(string s){
		string convertString = s;
		switch (convertString){
			case "Wheat":
			return CommodityType.Wheat;
			case "Wood":
			return CommodityType.Wood;
			case "Wild Game":
			return CommodityType.WildGame;
			case "Stone":
			return CommodityType.Stone;
			case "Ore":
			return CommodityType.Ore;
			case "Fruit":
			return CommodityType.Fruit;
			case "Spidersilk":
			return CommodityType.Spidersilk;
			case "Livestock":
			return CommodityType.Livestock;
			case "Dragonbone":
			return CommodityType.Dragonbone;
			case "Gems":
			return CommodityType.Gems;
			case "Bread":
			return CommodityType.Bread;
			case "Lumber":
			return CommodityType.Lumber;
			case "Furs":
			return CommodityType.Furs;
			case "Marble":
			return CommodityType.Marble;
			case "Weapons":
			return CommodityType.Weapons;
			case "Liquor":
			return CommodityType.Liquor;
			case "Fabric":
			return CommodityType.Fabric;
			case "Exotic Pets":
			return CommodityType.ExoticPets;
			case "Armour":
			return CommodityType.Armour;
			case "Jewelry":
			return CommodityType.Jewelry;
			default:
			Debug.Log("No Commodity Referenced");
			return CommodityType.Null;
		}
	}

	public override string ToString ()
	{
		return string.Format(ct.ToString());
	}
}