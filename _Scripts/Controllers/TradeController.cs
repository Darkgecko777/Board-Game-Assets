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
private int currentGC; // stores the int in the dropdown list for use with refund
private Tile rTile, hTile; // used to build refineries and peasants
private Player sender, receiver;

	void Update(){
		ActivateButtons();
		UpdateCommodities();
	}

	#region Production
	// method for setting the production cost of each item from the ui dropdown list.  Displays the required commodities as 
	// a string.  Sets payment to their cost, allowing specific commodity buttons to be active for use
	public void SetProduceCost(int item){
		currentGC = item;
		switch(item){
			case 0: // peasant
			payment.Clear();
			hTile = game.players[game.activePlayer].homeCity;
			gc = new Peasant(game.activePlayer, hTile);
			myUI.produceCost.text = "2 Wheat or Wild Game";
			payment= gc.cost;
				break;
			case 1: // burgher
			payment.Clear();
			gc = new Burgher(game.activePlayer);
			myUI.produceCost.text = "1 Wheat or Wild Game\r\n1 Spidersilk";
			payment= gc.cost;
				break;
			case 2: // Warrior
			payment.Clear();
			gc = new Warrior(game.activePlayer);
			myUI.produceCost.text = "2 Wheat or Wild Game\r\n1 Dragonbone\r\n1 Ore" ;
			payment= gc.cost;
			break;
			case 3: // Unique
			payment.Clear();
			gc = new Unique(game.activePlayer);
			myUI.produceCost.text = "2 Wheat or Wild Game\r\n2 Racial Commodities";
			payment= gc.cost;
			break;
			case 4: // Botanist
			payment.Clear();
			gc = new Botanist(game.activePlayer);
			myUI.produceCost.text = "1 Wheat\n1 Ore";
			payment= gc.cost;
			break;
			case 5: // Druid's Circle
			payment.Clear();
			gc = new DruidsCircle(game.activePlayer);
			myUI.produceCost.text = "1 Wood\n1 Stone";
			payment= gc.cost;
			break;
			case 6: // Ranch
			payment.Clear();
			gc = new Ranch(game.activePlayer);
			myUI.produceCost.text = "1 Wild Game\n1 Wood ";
			payment= gc.cost;
			break;
			case 7: // Adventurer's Guild
			payment.Clear();
			gc = new AdventurersGuild(game.activePlayer);
			myUI.produceCost.text = "1 Stone\n1 Wood";
			payment= gc.cost;
			break;
			case 8: // Miner's Guild
			payment.Clear();
			gc = new MinersGuild(game.activePlayer);
			myUI.produceCost.text = "1 Ore\r\n1 Wood";
			payment= gc.cost;
			break;
			default:
				break;
		}
	}
	#endregion

	#region Buttons
	// Adds the corresponding commodity to the payment list to produce an item
	public void ButtonClick(string s){
		Commodity.CommodityType ct = Commodity.ConvertString(s); 
		deposit.Add(new Commodity(ct)); // stores commodity if needed for refund
		if(game.gamePhase == GameController.GamePhase.Setup){ // activates buttons for use in setting default commodities
			game.players[game.activePlayer].homeCity.resource = new Commodity(ct);
			payment.Clear();
			myUI.resourcePanel.SetActive(!myUI.resourcePanel.activeSelf);
			game.NextPlayer();
			game.nextPlayer = true;
			if(game.activePlayer == 0){
				game.gamePhase = GameController.GamePhase.TradeMatrix;
				game.nextPlayer = true;
				StartCoroutine(game.CoMatrix());
			}
		}
		if(refineryAction){
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
			game.players[game.activePlayer].DecreaseCommodity(ct,1);
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
			game.players[game.activePlayer].DecreaseCommodity(ct,1);
			List<string> payed = new List<string>();
			foreach(Commodity c in deposit){
				payed.Add(c.ToString());
			}
			myUI.ProducePayment.text = string.Join("\n", payed.ToArray()); 
		} else if (caravanActive){
			Commodity sale = new Commodity(ct);
			sender.gold += sale.value;
			receiver.IncreaseCommodity(sale.ct, 1);
			game.players[game.activePlayer].DecreaseCommodity(sale.ct, 1);
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
			game.players[game.activePlayer].DecreaseCommodity(sale.ct, 1);
			sales ++;
			if (sales == 1){
				shippingActive = false;
				myUI.TogglePanel(myUI.resourcePanel);
				myUI.actionText.text = "Trade Complete";
				game.NextPlayer();
			}
		}
	}
	// closes all panels and completes action
	public void CompleteProduction(){
		Tile hc = game.players[game.activePlayer].homeCity;
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
			game.players[game.activePlayer].IncreaseCommodity(c.ct,1);
		}
		deposit.Clear();
		SetProduceCost(currentGC);
		myUI.ProducePayment.text = "";
	}
	// Alternate refund for refineries
	public void ClearRefinery(){
		foreach(Commodity c in deposit){
			game.players[game.activePlayer].IncreaseCommodity(c.ct,1);
		}
		deposit.Clear();
		RefineryProduction(currentGC);
		myUI.refineryPayment.text = "";
	}
	// Creates the currently selected and payed component on the active players city 
	public void Produce(){
		deposit.Clear();
		queue.Add(gc);
		queueCount ++;
		SetProduceCost(currentGC);
		myUI.produceQueue.text += "\n" + gc.ToString();
	}
	#endregion

	#region Trade Matrix
	// Opens trade matrix panel to set initial trade values
	public void InitializeTradeMatrix(){
		game.nextPlayer = false;
		myUI.actionText.text = "Please initialize your trade matrix.";
		myUI.matrixPanel.SetActive(!myUI.matrixPanel.activeSelf);
		// passes default value in case none are changed
		BasicResourcesMatrixValues(0);
		AdvancedResourcesMatrixValues(0);
		BasicGoodsMatrixValues(0);
		AdvancedGoodsMatrixValues(0);
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
		for (int r = 0; r < 5; r++){
			game.players[game.activePlayer].basicResources[r] = false;
		}
		game.players[game.activePlayer].basicResources[i] = true;
	}

	public void AdvancedResourcesMatrixValues(int i){
		for (int r = 0; r < 5; r++){
			game.players[game.activePlayer].advancedResources[r] = false;
		}
		game.players[game.activePlayer].advancedResources[i] = true;
	}
	public void BasicGoodsMatrixValues(int i){
		for (int r = 0; r < 5; r++){
			game.players[game.activePlayer].basicGoods[r] = false;
		}
		game.players[game.activePlayer].basicGoods[i] = true;
	}
	public void AdvancedGoodsMatrixValues(int i){
		for (int r = 0; r < 5; r++){
			game.players[game.activePlayer].advancedGoods[r] = false;
		}
		game.players[game.activePlayer].advancedGoods[i] = true;
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
		Commodity resourceSale = new Commodity(receiver.basicResourcesType.ElementAt(receiver.basicResources.IndexOf(true)));
		Commodity advResourceSale = new Commodity(receiver.advancedResourcesType.ElementAt(receiver.advancedResources.IndexOf
			(true)));;
		Commodity goodsSale = new Commodity(receiver.basicGoodsType.ElementAt(receiver.basicGoods.IndexOf(true)));;
		Commodity advGoodsSale = new Commodity(receiver.advancedGoodsType.ElementAt(receiver.advancedGoods.IndexOf
			(true)));;
		payment.Add(resourceSale);
		payment.Add(advResourceSale);
		payment.Add(goodsSale);
		payment.Add(advGoodsSale);
		myUI.resourcePanel.SetActive(!myUI.resourcePanel.activeSelf);
		//UpdateCommodities();
		caravanActive = true;
	}

	public void Shipping(Player s, Player r){
		sender = s;
		receiver = r;
		Commodity resourceSale = new Commodity(receiver.basicResourcesType.ElementAt(receiver.basicResources.IndexOf(true)));
		Commodity advResourceSale = new Commodity(receiver.advancedResourcesType.ElementAt(receiver.advancedResources.IndexOf
			(true)));;
		Commodity goodsSale = new Commodity(receiver.basicGoodsType.ElementAt(receiver.basicGoods.IndexOf(true)));;
		Commodity advGoodsSale = new Commodity(receiver.advancedGoodsType.ElementAt(receiver.advancedGoods.IndexOf
			(true)));;
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
		currentGC = r;
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
			info[1].text = game.players[game.activePlayer].commodities[Commodity.ConvertString(c)].amount.ToString();
			if(game.gamePhase == GameController.GamePhase.Setup && 
				commodityNeeded(game.players[game.activePlayer].commodities[Commodity.ConvertString(c)])){
					b.interactable = true;
			} else if(commodityNeeded(game.players[game.activePlayer].commodities[Commodity.ConvertString(c)]) 
				&& game.players[game.activePlayer].commodities[Commodity.ConvertString(c)].amount > 0){
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
			myUI.resources.interactable = false;
			myUI.advResources.interactable = false;
			myUI.goods.interactable = false;
			myUI.advGoods.interactable = false;
		} else {
			myUI.closeMatrix.interactable = true;
			myUI.resources.interactable = true;
			myUI.advResources.interactable = true;
			myUI.goods.interactable = true;
			myUI.advGoods.interactable = true;
		}
		if(!payment.Any() && gc != null && (gc.GetType() == typeof(Refinery) || gc.GetType() == typeof(AdvancedRefinery))){
			myUI.refineryProduce.interactable = true;
		} else {
			myUI.refineryProduce.interactable = false;
		}
	}
}
