using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class TradeController : MonoBehaviour {

public GameController game;
public Grid grid;
public UIController myUI;
public bool caravanActive, shippingActive, refineryAction, produceAction;
public int queueCount, sales; // sets max value for production and trade actions

private List<Commodity> deposit = new List<Commodity>(); // temporarily store commodities for refund
private List<Commodity> payment= new List<Commodity>(); // list of commodities needed to produce each gamecomponent
private List<GameComponents> queue = new List<GameComponents>(); // stores built components till action is complete
private GameComponents gc; // variable to access produce cost and to build when payment is complete
private int currentProduction; // stores the int in the dropdown list for use with refund
private Tile rTile, hTile; // used to build refineries and peasants
private Player sender, receiver, activePlayer;  

	void Update(){
		ActivateButtons();
		activePlayer = game.players[game.activePlayer];
		UpdateCommodities();
	}

	#region Production
	// method for setting the production cost of each item from the ui dropdown list.  Displays the required commodities as 
	// a string.  Sets payment to their cost, allowing specific commodity buttons to be active for use
	public void SetProduceCost(int item){
		currentProduction = item;
		string ProductionName = myUI.productionDD.captionText.text;
		switch(ProductionName){
			case "Peasant": // peasant
			payment.Clear();
			hTile = activePlayer.homeCity;
			gc = new Peasant(game.activePlayer, hTile);
			myUI.produceCost.text = "2 Wheat or Wild Game";
			payment = gc.cost;
			Debug.Log("Peasant");
				break;
			case "Burgher": // burgher
			payment.Clear();
			gc = new Burgher(game.activePlayer);
			myUI.produceCost.text = "1 Wheat or Wild Game\r\n1 Spidersilk";
			payment = gc.cost;
			Debug.Log("Burgher");
				break;
			case "Warrior": // Warrior
			payment.Clear();
			gc = new Warrior(game.activePlayer);
			myUI.produceCost.text = "2 Wheat or Wild Game\r\n1 Dragonbone\r\n1 Ore" ;
			payment = gc.cost;
			break;
			case "Unique": // Unique
			payment.Clear();
			gc = new Unique(game.activePlayer);
			myUI.produceCost.text = "2 Wheat or Wild Game\r\n2 Racial Commodities";
			payment = gc.cost;
			payment.Add(activePlayer.homeCity.resource);
			payment.Add(activePlayer.homeCity.resource);
			break;
//			case 4: // Botanist
//			payment.Clear();
//			gc = new Botanist(game.activePlayer);
//			myUI.produceCost.text = "1 Wheat\n1 Ore";
//			payment= gc.cost;
//			break;
//			case 5: // Druid's Circle
//			payment.Clear();
//			gc = new DruidsCircle(game.activePlayer);
//			myUI.produceCost.text = "1 Wood\n1 Stone";
//			payment= gc.cost;
//			break;
//			case 6: // Ranch
//			payment.Clear();
//			gc = new Ranch(game.activePlayer);
//			myUI.produceCost.text = "1 Wild Game\n1 Wood ";
//			payment= gc.cost;
//			break;
//			case 7: // Adventurer's Guild
//			payment.Clear();
//			gc = new AdventurersGuild(game.activePlayer);
//			myUI.produceCost.text = "1 Stone\n1 Wood";
//			payment= gc.cost;
//			break;
//			case 8: // Miner's Guild
//			payment.Clear();
//			gc = new MinersGuild(game.activePlayer);
//			myUI.produceCost.text = "1 Ore\r\n1 Wood";
//			payment= gc.cost;
//			break;
			default:
				break;
		}
	}
	#endregion


	#region Buttons
	// Adds the corresponding commodity to the payment list to produce an item
	public void ButtonClick(string s){
		Commodity.CommodityType ct = Commodity.ConvertString(s);
		if(game.gamePhase != GameController.GamePhase.Setup){ 
			deposit.Add(new Commodity(ct)); // stores commodity if needed for refund
		}
		if(game.gamePhase == GameController.GamePhase.Setup){ // activates buttons for use in setting default commodities
			activePlayer.homeCity.resource = new Commodity(ct);
			payment.Clear();
			myUI.resourcePanel.SetActive(!myUI.resourcePanel.activeSelf);
			game.NextPlayer();
			game.nextPlayer = true;
			if(game.activePlayer == 0){
				game.gamePhase = GameController.GamePhase.TradeMatrix;
				game.nextPlayer = true;
				StartCoroutine(game.CoMatrix());
			}
		} else if(refineryAction){
			if (ct == Commodity.CommodityType.Wood || ct == Commodity.CommodityType.Stone){
				Commodity a = payment.Find(t => t.ct == Commodity.CommodityType.Wood);
				Commodity b = payment.Find(t => t.ct == Commodity.CommodityType.Stone);
				payment.Remove(a);
				payment.Remove(b);
			}
			if (ct == Commodity.CommodityType.Spidersilk || ct == Commodity.CommodityType.Dragonbone 
				|| ct == Commodity.CommodityType.Fruit || ct == Commodity.CommodityType.Gems || ct == Commodity.CommodityType.Livestock){
				Commodity a = payment.Find(t => t.ct == Commodity.CommodityType.Spidersilk);
				Commodity b = payment.Find(t => t.ct == Commodity.CommodityType.Dragonbone);
				Commodity c = payment.Find(t => t.ct == Commodity.CommodityType.Fruit);
				Commodity d = payment.Find(t => t.ct == Commodity.CommodityType.Gems);
				Commodity e = payment.Find(t => t.ct == Commodity.CommodityType.Livestock);
				payment.Remove(a);
				payment.Remove(b);
				payment.Remove(c);
				payment.Remove(d);
				payment.Remove(e);
			} else {
				Commodity a = payment.Find(t => t.ct == ct);
				payment.Remove(a);
			}
			activePlayer.DecreaseCommodity(ct,1);
			List<string> payed = new List<string>();
			foreach(Commodity c in deposit){
				payed.Add(c.ToString());
			}
			myUI.refineryPayment.text = string.Join("\n", payed.ToArray());
		} else if (produceAction){
			if(ct == Commodity.CommodityType.Wheat || ct == Commodity.CommodityType.WildGame){
				Commodity a = payment.Find(t => t.ct == Commodity.CommodityType.Wheat);
				Commodity b = payment.Find(t => t.ct == Commodity.CommodityType.WildGame);
				payment.Remove(a);
				payment.Remove(b);
			} else {
				Commodity a = payment.Find(t => t.ct == ct);
				payment.Remove(a);
			}
			activePlayer.DecreaseCommodity(ct,1);
			List<string> payed = new List<string>();
			foreach(Commodity c in deposit){
				payed.Add(c.ToString());
			}
			myUI.ProducePayment.text = string.Join("\n", payed.ToArray()); 
		} else if (caravanActive){
			Commodity sale = new Commodity(ct);
			sender.gold += sale.value;
			receiver.IncreaseCommodity(sale.ct, 1);
			activePlayer.DecreaseCommodity(sale.ct, 1);
			sales ++;
			if (sales == 2){
				caravanActive = false;
				myUI.TogglePanel(myUI.resourcePanel);
				myUI.actionText.text = "Trade Complete";
				game.NextPlayer();
			}
		} else if (shippingActive){
			Commodity sale = new Commodity(ct);
			sender.gold += sale.value;
			receiver.IncreaseCommodity(sale.ct, 1);
			activePlayer.DecreaseCommodity(sale.ct, 1);
			sales ++;
			if (sales == 1){
				shippingActive = false;
				myUI.TogglePanel(myUI.resourcePanel);
				myUI.actionText.text = "Trade Complete";
				game.NextPlayer();
			}
		}
	}

	public void SetProductionDropdown(){
		myUI.productionDD.ClearOptions();
		myUI.productionDD.AddOptions(FillProduceList());
	}

	// closes all panels and completes action
	public void CompleteProduction(){
		Tile hc = activePlayer.homeCity;
		foreach(GameComponents g in queue){
			hc.gameComponents.Add(g);
		}
		payment.Clear();
		myUI.TogglePanel(myUI.producePanel);
		produceAction = false;
		game.NextPlayer();
	}
	// Refunds current list of commodites to player
	public void ClearPayment(){
		foreach(Commodity c in deposit){
			activePlayer.IncreaseCommodity(c.ct,1);
		}
		deposit.Clear();
		SetProduceCost(currentProduction);
		myUI.ProducePayment.text = "";
	}
	// Alternate refund for refineries
	public void ClearRefinery(){
		foreach(Commodity c in deposit){
			activePlayer.IncreaseCommodity(c.ct,1);
		}
		deposit.Clear();
		RefineryProduction(currentProduction);
		myUI.refineryPayment.text = "";
	}
	// Creates the currently selected and payed component on the active players city 
	public void Produce(){
		deposit.Clear();
		queue.Add(gc);
		activePlayer.units.Remove(activePlayer.units.
			Find(t => t.GetType() == gc.GetType()));
		queueCount ++;
		SetProduceCost(currentProduction);
		myUI.produceQueue.text += "\n" + gc.ToString();
		Debug.Log(activePlayer.units.Where(t => t.GetType() == typeof(Peasant)).Count().ToString());
		SetProductionDropdown();

	}
	#endregion

	#region Trade Matrix
	// Opens trade matrix panel to set initial trade values
	public void InitializeTradeMatrix(){
		myUI.matrixPanel.SetActive(!myUI.matrixPanel.activeSelf);
		game.nextPlayer = false;
		myUI.actionText.text = "Please initialize your trade matrix.";
		myUI.resourcesDD.captionText.text = activePlayer.bResources.ToString();
		Debug.Log(activePlayer.playerNumber.ToString());
		myUI.advResourcesDD.captionText.text = activePlayer.advResources.ToString();
		myUI.goodsDD.captionText.text = activePlayer.bGoods.ToString();
		myUI.advGoodsDD.captionText.text = activePlayer.advGoods.ToString();
	}
	// Closes trade matrix
	public void CompleteTradeMatrix(){
		myUI.matrixPanel.SetActive(!myUI.matrixPanel.activeSelf);
		game.NextPlayer();
		if(game.activePlayer == 0){
			game.gamePhase = GameController.GamePhase.Actions;
			myUI.matrixPanel.SetActive(!myUI.matrixPanel.activeSelf);
			myUI.actionText.text = "";
		}
		myUI.actionText.text = "";
		game.nextPlayer = true;
	}

	// corresponds to dropdown trade matrix
	public void BasicResourcesMatrixValues(int i){
		activePlayer.bResources = (Player.BasicResources)i;
	}
	public void AdvancedResourcesMatrixValues(int i){
		activePlayer.advResources = (Player.AdvancedResources)i;
	}
	public void BasicGoodsMatrixValues(int i){
		activePlayer.bGoods = (Player.BasicGoods)i;
	}
	public void AdvancedGoodsMatrixValues(int i){
		activePlayer.advGoods = (Player.AdvancedGoods)i;	
	}
	#endregion
	public void InitializeRacialCommodity(){
		game.nextPlayer = false;
		myUI.resourcePanel.SetActive(!myUI.resourcePanel.activeSelf);
		myUI.actionText.text = "Please select your default commodity.";
		payment.Add(new Commodity(Commodity.CommodityType.Wheat));
		payment.Add(new Commodity(Commodity.CommodityType.Wood));
		payment.Add(new Commodity(Commodity.CommodityType.WildGame));
		payment.Add(new Commodity(Commodity.CommodityType.Stone));
		payment.Add(new Commodity(Commodity.CommodityType.Ore));
	}

	#region Trading
	public void Caravan(Player s, Player r){
		sender = s;
		receiver = r;
		Commodity resourceSale = new Commodity(Commodity.ConvertString(receiver.bResources.ToString())); 
		Commodity advResourceSale = new Commodity(Commodity.ConvertString(receiver.advResources.ToString()));
		Commodity goodsSale = new Commodity(Commodity.ConvertString(receiver.bGoods.ToString()));
		Commodity advGoodsSale = new Commodity(Commodity.ConvertString(receiver.advGoods.ToString()));
		payment.Add(resourceSale);
		payment.Add(advResourceSale);
		payment.Add(goodsSale);
		payment.Add(advGoodsSale);
		myUI.resourcePanel.SetActive(!myUI.resourcePanel.activeSelf);
		caravanActive = true;
	}

	public void Shipping(Player s, Player r){
		sender = s;
		receiver = r;
		Commodity resourceSale = new Commodity(Commodity.ConvertString(receiver.bResources.ToString())); 
		Commodity advResourceSale = new Commodity(Commodity.ConvertString(receiver.advResources.ToString()));
		Commodity goodsSale = new Commodity(Commodity.ConvertString(receiver.bGoods.ToString()));
		Commodity advGoodsSale = new Commodity(Commodity.ConvertString(receiver.advGoods.ToString()));
		payment.Add(resourceSale);
		payment.Add(advResourceSale);
		payment.Add(goodsSale);
		payment.Add(advGoodsSale);
		myUI.resourcePanel.SetActive(!myUI.resourcePanel.activeSelf);
		shippingActive = true;
	}

	#endregion 

	#region Refinery
	public void BuildRefinery (Tile t){
		rTile = t;
		refineryAction = true;
		RefineryProduction(0);
	}

	public void RefineryProduction(int r){
		currentProduction = r;
		switch(r){
			case 0:
			payment.Clear();
			gc = new Refinery(game.activePlayer, rTile);
			myUI.refineryCost.text = "1 Wood or Stone and 1 Ore";
			payment = gc.cost;
				break;
			case 1:
			payment.Clear();
			gc = new AdvancedRefinery(game.activePlayer, rTile);
			myUI.refineryCost.text = "1 Wood or Stone, 1 Ore and 1 Advanced Resource";
			payment = gc.cost;
				break;
			default:
				break;
		}
	}

	public void FinishRefinery(){
		deposit.Clear();
		rTile.gameComponents.Add(gc);
		myUI.TogglePanel(myUI.refineryPanel);
		rTile.actionCounter[game.activePlayer] = true;
		payment.Clear();
		refineryAction = false;
	}
	#endregion
	// create a boolean that checks to see if the specified commodity is needed to pay for the selected item
	private bool commodityNeeded(Commodity c){
		if(payment.Find(t => t.ct == c.ct) != null){
			return true;
		} else {
			return false;
		}
	}
	// scans through each button and checks if the active player has any of the matching commodoties
	public void UpdateCommodities(){
		int i = 0;
  		foreach(Button b in myUI.commodoties){
  			Text[] info = myUI.commodoties[i].GetComponentsInChildren<Text>(); // takes each of the text components of the button
  			// and assigns them to an array for easy use 
			string c = info[0].text;
			info[1].text = activePlayer.commodities[Commodity.ConvertString(c)].amount.ToString();
			if(game.gamePhase == GameController.GamePhase.Setup && 
				commodityNeeded(activePlayer.commodities[Commodity.ConvertString(c)])){
					b.interactable = true;
			} else if(commodityNeeded(activePlayer.commodities[Commodity.ConvertString(c)]) 
				&& activePlayer.commodities[Commodity.ConvertString(c)].amount > 0){
  				b.interactable = true;
  			} else {
  				b.interactable = false;
  			}
			i++;
  		}
	}
	// checks to see if each button can legally be used
	private void ActivateButtons(){
		myUI.completeProduction.interactable = true;
		if(!payment.Any() && queueCount < 4){
			myUI.produce.interactable = true;
		} else {
			myUI.produce.interactable = false;
		}
		if(deposit.Any()){
			myUI.clearPayment.interactable = true;
			myUI.refineryClear.interactable = true;
		} else {
			myUI.clearPayment.interactable = false;
			myUI.refineryClear.interactable = false;
		}
		if(game.gamePhase != GameController.GamePhase.TradeMatrix){
			myUI.closeMatrix.interactable = false;
			myUI.resourcesDD.interactable = false;
			myUI.advResourcesDD.interactable = false;
			myUI.goodsDD.interactable = false;
			myUI.advGoodsDD.interactable = false;
		} else {
			myUI.closeMatrix.interactable = true;
			myUI.resourcesDD.interactable = true;
			myUI.advResourcesDD.interactable = true;
			myUI.goodsDD.interactable = true;
			myUI.advGoodsDD.interactable = true;
		}
		if(!payment.Any() && gc != null && (gc.GetType() == typeof(Refinery) || gc.GetType() == typeof(AdvancedRefinery))){
			myUI.refineryProduce.interactable = true;
		} else {
			myUI.refineryProduce.interactable = false;
		}
	}

	#region Production Booleans
// Booleans to check if any given item can be built by the active player.  Corresponds to dropdown list

	private bool BuildPeasant(){
		if (((activePlayer.commodities[Commodity.CommodityType.Wheat].amount > 1 ||
			activePlayer.commodities[Commodity.CommodityType.WildGame].amount > 1) ||
			(activePlayer.commodities[Commodity.CommodityType.Wheat].amount > 0 &&
			activePlayer.commodities[Commodity.CommodityType.WildGame].amount > 0)) 
			&& activePlayer.units.Where(t => t.GetType() == typeof(Peasant)).Any()){
			return true;
		} else {
			return false;
		}
	}

	private bool BuildBurgher(){
		if ((activePlayer.commodities[Commodity.CommodityType.Wheat].amount > 0 ||
			activePlayer.commodities[Commodity.CommodityType.WildGame].amount > 0) &&
			activePlayer.commodities[Commodity.CommodityType.Spidersilk].amount > 0 
			&& activePlayer.units.Where(t => t.GetType() == typeof(Burgher)).Any()){
			return true;
		} else {
			return false;
		}
	}	
		
	private bool BuildWarrior(){
		if ((activePlayer.commodities[Commodity.CommodityType.Wheat].amount > 1 ||
			activePlayer.commodities[Commodity.CommodityType.WildGame].amount > 1 ||
			(activePlayer.commodities[Commodity.CommodityType.Wheat].amount > 0 &&
			activePlayer.commodities[Commodity.CommodityType.WildGame].amount > 0)) && 
			activePlayer.commodities[Commodity.CommodityType.Dragonbone].amount > 0 &&
			activePlayer.commodities[Commodity.CommodityType.Ore].amount > 0
			&& activePlayer.units.Where(t => t.GetType() == typeof(Warrior)).Any()){
			return true;
		} else {
			return false;
		}
	}

	private bool BuildUnique(){
		if ((activePlayer.commodities[Commodity.CommodityType.Wheat].amount > 1 ||
			activePlayer.commodities[Commodity.CommodityType.WildGame].amount > 1 ||
			(activePlayer.commodities[Commodity.CommodityType.Wheat].amount > 0 &&
			activePlayer.commodities[Commodity.CommodityType.WildGame].amount > 0)) &&
			activePlayer.commodities[activePlayer.homeCity.resource.ct].amount > 1
			&& activePlayer.units.Where(t => t.GetType() == typeof(Unique)).Any()){
			return true;
		} else {
			return false;
		}
	}

	private bool BuildBasicRefinery(){
		if ((activePlayer.commodities[Commodity.CommodityType.Wood].amount > 0 ||
			activePlayer.commodities[Commodity.CommodityType.Stone].amount > 0) &&
			activePlayer.commodities[Commodity.CommodityType.Ore].amount > 0){
			return true;
		} else {
			return false;
		}
	}

	private bool BuildAdvancedRefinery(){
		if ((activePlayer.commodities[Commodity.CommodityType.Wood].amount > 0 ||
			activePlayer.commodities[Commodity.CommodityType.Stone].amount > 0) &&
			activePlayer.commodities[Commodity.CommodityType.Ore].amount > 0 &&
			(activePlayer.commodities[Commodity.CommodityType.Fruit].amount > 0 ||
			activePlayer.commodities[Commodity.CommodityType.Livestock].amount > 0 ||
			activePlayer.commodities[Commodity.CommodityType.Spidersilk].amount > 0 ||
			activePlayer.commodities[Commodity.CommodityType.Gems].amount > 0 ||
			activePlayer.commodities[Commodity.CommodityType.Dragonbone].amount > 0)){
			return true;
		} else {
			return false;
		}
	}

	private List<string> FillProduceList(){
		List<string> myList = new List<string>();
		if (BuildPeasant()){
			myList.Add("Peasant");
		}
		if (BuildBurgher()){
			myList.Add("Burgher");
		}
		if (BuildWarrior()){
			myList.Add("Warrior");
		}
		if (BuildUnique()){
			myList.Add("Unique");
		}
		return myList;
	}
	#endregion

	public void TestButton(){
		Debug.Log(activePlayer.bResources.ToString());
		Debug.Log(activePlayer.playerNumber.ToString());
	}
}
