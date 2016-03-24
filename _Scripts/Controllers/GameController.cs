using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public class GameController : MonoBehaviour {

	public Grid grid; // accesses functions for tiles
	public MapController board; // the current gameobject generating the game board
	public UIController myUI; // allows communication between controllers
	public TradeController trade; // handles trading transactions
	public CombatController combat; // resolves combat
	public Player[] players = new Player[6]; // holds the values for all players
	public int activePlayer = 0; // a value to link players with stored values
	public Slider burghersInput, warriorsInput, uniquesInput, peasantsInput; // input field to move pieces
	public bool moveComplete, local, nextPlayer, action;  //booleans related to explore, movement and player iteration
	private Tile primaryTile, secondaryTile; // placeholder to calculate for multiple functions
	private Material matOne, matTwo, matActive;  // used in explore action
	public Color[] playerColours = new Color[6]; // colour coding for players
	public enum GamePhase{Setup, TradeMatrix, Actions, TransplantVillages, Harvest};
	public GamePhase gamePhase; 

	#region Setup
	void Start(){
		board.StartUp(); //uses the map generator to create the board
		gamePhase = GamePhase.Setup;
		foreach (Tile pc in board.playerStart){ // sets starting units for each city tile and adjusts player pools
			players[activePlayer] =  new Player(activePlayer);
			players[activePlayer].homeCity = pc;
			pc.influence[activePlayer] = true;
			myUI.UpdateInfluence();
			pc.gameComponents.Add(new Peasant(activePlayer, pc)); // testing
			pc.gameComponents.Add(new Peasant(activePlayer, pc)); // testing
			pc.gameComponents.Add(new Burgher(activePlayer)); // testing
			pc.gameComponents.Add(new Burgher(activePlayer)); // testing
			pc.gameComponents.Add(new Burgher(activePlayer)); // testing
			NextPlayer();
		}
		nextPlayer = true;
		StartCoroutine(SetRacialCommodotity());
	}

	public IEnumerator SetRacialCommodotity(){
		while(gamePhase == GamePhase.Setup){
			while(!nextPlayer){
				yield return null;
			}
			if(nextPlayer){
				trade.InitializeRacialCommodity();
			}
		yield return null;
		}
	}

	public IEnumerator CoMatrix(){
		while(gamePhase == GamePhase.TradeMatrix){
			while(!nextPlayer){
				yield return null;
			}
			if(nextPlayer){
				trade.InitializeTradeMatrix();
			}
			yield return null;
		}
		myUI.actionText.text = "";
	}
	#endregion

	#region Movement
	// First step in movement.  Passes control to the coroutine
	public void StartMovement(){
		myUI.actionText.text = "Right click a tile which contains valid units.";
		myUI.TogglePanel(myUI.moveOptionPanel);
		StartCoroutine(CoMovement());
	}
	// Second step of movement.  Checks valid tiles and opens up movement panel
	private IEnumerator CoMovement(){
		while (!Input.GetMouseButtonDown(1)){
			yield return null;
			if (Input.GetMouseButtonDown(1)){
				if (SetActiveTile() != primaryTile 
				&& grid.Neighbours(primaryTile).Contains(SetActiveTile()) 
				&& SetActiveTile().gameComponents.Where(g => g.playerController == activePlayer).Any() 
				&& ContainsMoveableUnits(SetActiveTile())){ 
					myUI.actionText.text = "Please select your units to move.";
					secondaryTile = SetActiveTile();
					myUI.TogglePanel(myUI.movementPanel);
					myUI.MoveDisplay();
					burghersInput.maxValue = secondaryTile.gameComponents.Where
						(g => g.playerController == activePlayer && g.GetType() == typeof(Burgher)).Count();;
					warriorsInput.maxValue = secondaryTile.gameComponents.Where
						(g => g.playerController == activePlayer && g.GetType() == typeof(Warrior)).Count();
					uniquesInput.maxValue = secondaryTile.gameComponents.Where
						(g => g.playerController == activePlayer && g.GetType() == typeof(Unique)).Count();
				} else {
					yield return null;
				}
			}
		}
	}
	// Ends movement and adjusts components if needed
	public void FinishMoveButton(){
		MoveComponents();
		myUI.TogglePanel(myUI.movementPanel);
		myUI.actionText.text = "";
		if(ContainsMartialUnits(primaryTile) != null){
			combat.ResolveCombat(primaryTile);
		}
		myUI.UpdateInfluence();
		NextPlayer();
	}
	// Allows for further selection of units
	 public void MoveMoreButton(){
	 		MoveComponents();
	 		myUI.TogglePanel(myUI.movementPanel);
			myUI.actionText.text = "Please right click another tile which contains valid units";
	 		StartCoroutine(CoMovement());
	 }
	 // no movement and action passed to next player
	 public void PassMovement(){
	 	myUI.TogglePanel(myUI.moveOptionPanel);
	 	myUI.UpdateInfluence();
	 	NextPlayer();
	 } 
	 // Method to move components from one tile to another
	 private void MoveComponents(){
		// move burghers and adjust values in each tile
	 	for (int i = 0; i < burghersInput.value; i++ ){
	 		primaryTile.gameComponents.Add(new Burgher(activePlayer));
			GameComponents b = secondaryTile.gameComponents.Find(g => g.playerController == activePlayer 
				&& g.GetType() == typeof(Burgher));
			secondaryTile.gameComponents.Remove(b);
	 	}
		// move warriors and adjust values in each tile
		for (int i = 0; i < warriorsInput.value; i++ ){
	 		primaryTile.gameComponents.Add(new Warrior(activePlayer));
			GameComponents w = secondaryTile.gameComponents.Find(g => g.playerController == activePlayer 
				&& g.GetType() == typeof(Warrior));
			secondaryTile.gameComponents.Remove(w);
	 	}
		// move uniques and adjust values in each tile
		for (int i = 0; i < uniquesInput.value; i++ ){
	 		primaryTile.gameComponents.Add(new Unique(activePlayer));
			GameComponents u = secondaryTile.gameComponents.Find(g => g.playerController == activePlayer 
				&& g.GetType() == typeof(Unique));
			secondaryTile.gameComponents.Remove(u);
	 	}  
	}
	#endregion
	#region Utility

	private void CheckTurnEnd(){
		if(players.All(p => p.actionCounter == 0) && gamePhase == GamePhase.Actions){
			gamePhase = GamePhase.TransplantVillages;
			myUI.TogglePanel(myUI.TVConfirm);
		}
	}

	public void NextPlayer(){
		action = false;
		myUI.actionText.text = "";
		if(activePlayer < 5){
			activePlayer ++;
		} else {
			activePlayer = 0;
			if(gamePhase == GamePhase.Actions){
				CheckTurnEnd();
			}
		}
	}

	// Method called upon to query tile which is under mouse pointer
	private Tile SetActiveTile(){
		Tile tile;
		RaycastHit hit;
		Ray ray =  Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit)){
			tile = hit.collider.gameObject.GetComponent<Tile>();
			return tile;
		} else {
			Debug.Log("No Tile Selected");
			return null;
			}
	}

	public Tile ContainsMartialUnits(Tile t){
		if(t.gameComponents.Where(c => c.playerController != activePlayer && c.isMartial).Any()){
			return t;
		} else {
				return null;
		}
	}

	public Tile Blockaded(Tile t){
		if(t.gameComponents.Where(c => c.playerController != activePlayer && c.GetType() == typeof(Warrior)).Any()){
			return t;
		} else {
				return null;
		}
	}

	public Tile ContainsMoveableUnits(Tile t){
		if(t.gameComponents.Where(c => c.playerController == activePlayer).Any() && !t.actionCounter[activePlayer]){
			return t;
		} else {
				return null;
		}
	}

	public bool CaravanConnected(Player sender, Player receiver){
		List<Tile> senderInfluenced = grid.Tiles.Values.Where(t => t.influence[sender.playerNumber]).ToList();
		List<Tile> receieverInfluenced = grid.Tiles.Values.Where(t => t.influence[receiver.playerNumber]).ToList();
		bool cc = false;
		foreach(Tile t in senderInfluenced){
			if(grid.Neighbours(t).Intersect(receieverInfluenced).Any()){
				cc = true;
			}
		}
		return cc;
	}

	public bool ShippingConnected(Player sender){
		bool cc = false;
		if(grid.Neighbours(players[activePlayer].homeCity).Where(t => t.influence[activePlayer] &&
			t.tt == Tile.TerrainType.Coastal).Any()){
				cc = true;
			}
		return cc;
	}

	#endregion
	#region Action Booleans
	// list of booleans for uicontroller to check in order to make buttons interactable

	public bool CanMove(){
		bool cm = false;
		foreach(Tile t in grid.Tiles.Values){
			if(primaryTile != null && grid.Neighbours(primaryTile).Contains(t) && ContainsMoveableUnits(t)){
				cm = true;
			}
		}
		return cm;
	}

	public bool CanTransplant(){
		bool ct = false;
			foreach(Tile t in grid.Tiles.Values){
				if(t.influence[activePlayer] && grid.Neighbours(t).Find(c => c.gameComponents.Where(p => p.GetType() == typeof(Peasant)
				&& p.playerController == activePlayer && !(p as Peasant).hasMoved).Any())){
					 ct = true;
				}
			}
		return ct;
	}
	            
	public bool CanScout(){
		bool cs = false;
		foreach(Tile t in grid.Tiles.Values){
			if(t.tt != Tile.TerrainType.Unexplored && t.tt != Tile.TerrainType.City && !t.actionCounter[activePlayer]
			&& grid.Neighbours(t).Find(x => x.gameComponents.Where(gc => gc.playerController == activePlayer).Any() 
			&& !x.actionCounter[activePlayer])){
				cs = true;
			}
		}
		return cs;
	}

	public bool CanExplore(){
		bool ce = false;
		foreach(Tile t in grid.Tiles.Values){
			if (t.influence[activePlayer] && grid.Neighbours(t).Where(e => e.tt == Tile.TerrainType.Unexplored).Any()){
				ce = true;
			}
		}
		return ce;
	}

	public bool CanInfluence(){
		bool ci = false;
		foreach(Tile t in grid.Tiles.Values){
			if (!t.influence[activePlayer] && t.tt != Tile.TerrainType.Unexplored && t.tt != Tile.TerrainType.City
			&& grid.Neighbours(t).Find(i => i.influence[activePlayer])){
			ci = true;
			}
		}
		return ci;
	}

	public bool CanCaravan(){
		bool cc = false;
		foreach(Tile t in grid.Tiles.Values){
			if(t.influence[activePlayer] && grid.Neighbours(t).Find(c => !c.influence[activePlayer] && c.influence.Contains(true) 
			&& !players[c.influence.IndexOf(true)].homeCity.actionCounter[activePlayer])){
				cc = true;
			}
		}
		return cc;
	}

	public bool CanShip(){
		bool cs = false;
		foreach(Tile t in grid.Tiles.Values){
			if(t.influence[activePlayer] && t.tt == Tile.TerrainType.Coastal 
			&& board.playerStart.Find(pc => !pc.actionCounter[activePlayer] && !pc.influence[activePlayer])){ 
				cs = true;
			}
		}
		return cs;
	}

	public bool CanRefinery(){
		bool cr = false;
		foreach(Tile t in grid.Tiles.Values){
			if(t.gameComponents.Find(p => p.playerController == activePlayer && p.GetType() == typeof(Peasant)) != null 
			&& t.tt != Tile.TerrainType.City && !t.actionCounter[activePlayer]){
				if((players[activePlayer].commodities[Commodity.CommodityType.Wood].amount >= 1 
				|| players[activePlayer].commodities[Commodity.CommodityType.Stone].amount >= 1)
				&& players[activePlayer].commodities[Commodity.CommodityType.Ore].amount >= 1){
					cr = true;
				}
				if((players[activePlayer].commodities[Commodity.CommodityType.Wood].amount >= 1 
				|| players[activePlayer].commodities[Commodity.CommodityType.Stone].amount >= 1)
				&& players[activePlayer].commodities[Commodity.CommodityType.Ore].amount >= 1 
				&& (players[activePlayer].commodities[Commodity.CommodityType.Fruit].amount >= 1 
				|| players[activePlayer].commodities[Commodity.CommodityType.Livestock].amount >= 1
				|| players[activePlayer].commodities[Commodity.CommodityType.Spidersilk].amount >= 1
				|| players[activePlayer].commodities[Commodity.CommodityType.Gems].amount >= 1 
				|| players[activePlayer].commodities[Commodity.CommodityType.Dragonbone].amount >= 1)){
					cr = true;
				}
			}
		}  
		return cr;
	}

	public bool CanProduction(){
		bool cp = false;
		if(!players[activePlayer].homeCity.actionCounter[activePlayer]){
			cp = true;
		}
		return cp;
	}


	#endregion

	#region Scout, Explore
	public void Scout(){
		action = true;
		StartCoroutine(CoScout(activePlayer));
	}
	// Main step of scout action
	public IEnumerator CoScout(int player){
		myUI.actionText.text = "Please select a tile to Scout.";
		while (!Input.GetMouseButtonDown(0)){
			yield return null;
			if (Input.GetKeyDown(KeyCode.Escape)){
				action = false;
				myUI.actionText.text = "";
				break;
			}
			if (Input.GetMouseButtonDown (0)){
				if (SetActiveTile() != null  && !SetActiveTile().actionCounter[player] 
				&& SetActiveTile().tt != Tile.TerrainType.Unexplored && SetActiveTile().tt != Tile.TerrainType.City){
					SetActiveTile().LineColour(Color.white);
					SetActiveTile().LineWidth(0.06f,0.06f);
					primaryTile = SetActiveTile();
					primaryTile.actionCounter[player] = true;
					players[player].actionCounter--;
					myUI.TogglePanel(myUI.moveOptionPanel);
//					if (grid.Neighbours(primaryTile).Where(t => ContainsMoveableUnits(t)).Any()){
//						StartMovement(primaryTile);
//					} else {
//						myUI.actionText.text = "No units are within range.";
//						NextPlayer();
//					}
//					myUI.UpdateInfluence();
				} else {
					yield return null;
				}
			}
		}
	}

	// Start to explore action
	public void Explore(){
		action = true;
		StartCoroutine(CoExplore());
	}
	// Main step of explore action
	public IEnumerator CoExplore(){
		myUI.actionText.text = "Please select an influenced tile from which to explore.";
		while (!Input.GetMouseButtonDown(0)){
			yield return null;
			if (Input.GetKeyDown(KeyCode.Escape)){
				action = false;
				myUI.actionText.text = "";
				break;
			}
			if (Input.GetMouseButtonDown (0)){
				if (SetActiveTile() != null  && SetActiveTile().influence[activePlayer] 
				&& grid.Neighbours(SetActiveTile()).Where(t => t.tt == Tile.TerrainType.Unexplored).Any()){
					SetActiveTile().LineColour(Color.white);
					SetActiveTile().LineWidth(0.06f,0.06f);
					myUI.TogglePanel(myUI.localExPanel);
					myUI.actionText.text = "Please select which region to explore.";
					if (grid.Neighbours(SetActiveTile()).Where(t => t.exotic && t.tt == Tile.TerrainType.Unexplored).Any()){
						myUI.exoticButton.interactable = true;
					} else {
						myUI.exoticButton.interactable = false;
					}
					if (grid.Neighbours(SetActiveTile()).Where(t => t.tt == Tile.TerrainType.Unexplored && !t.exotic).Any()){
						myUI.localButton.interactable = true;
					} else 
					{
						myUI.localButton.interactable = false;	
					}
				} else {
					yield return null;
				}
			}
		}
	}
	// Action to resolve if local tiles are selected to explore. Pulls two tiles out of list and allows selection, recycling other
	public void SelectLocal(){
		myUI.TogglePanel(myUI.explorePanel);
		myUI.actionText.text = "Please choose a terrain to place.";
		int firstIndex = UnityEngine.Random.Range(0, board.localTerrain.Count);
		matOne = board.localTerrain.ElementAt(firstIndex);
		board.localTerrain.RemoveAt(firstIndex);
		myUI.terrainOne.text = matOne.ToString();
		int secondIndex = UnityEngine.Random.Range(0, board.localTerrain.Count);
		matTwo = board.localTerrain.ElementAt(secondIndex);
		board.localTerrain.RemoveAt(secondIndex);
		myUI.terrainTwo.text = matTwo.ToString();
		local = true;
		while (matOne == matTwo){
			board.localTerrain.Add(matTwo);
			secondIndex = UnityEngine.Random.Range(0, board.localTerrain.Count);
			matTwo = board.localTerrain.ElementAt(secondIndex);
			board.localTerrain.RemoveAt(secondIndex);
			myUI.terrainTwo.text = matTwo.ToString();
		}
	}
	// Action to resolve if exotic tiles are selected to explore.  Pulls two tiles out of list and allows selection, recycling other
	public void SelectExotic(){
		myUI.TogglePanel(myUI.explorePanel);
		myUI.actionText.text = "Please choose a terrain to place.";
		int firstIndex = UnityEngine.Random.Range(0, board.exoticTerrain.Count);
		matOne = board.localTerrain.ElementAt(firstIndex);
		board.exoticTerrain.RemoveAt(firstIndex);
		myUI.terrainOne.text = matOne.ToString();
		int secondIndex = UnityEngine.Random.Range(0, board.exoticTerrain.Count);
		matTwo = board.exoticTerrain.ElementAt(secondIndex);
		board.exoticTerrain.RemoveAt(secondIndex);
		myUI.terrainTwo.text = matTwo.ToString();
		local = false;
		while (matOne ==  matTwo){
			board.exoticTerrain.Add(matTwo);
			secondIndex = UnityEngine.Random.Range(0, board.exoticTerrain.Count);
			matTwo = board.exoticTerrain.ElementAt(secondIndex);
			board.exoticTerrain.RemoveAt(secondIndex);
			myUI.terrainTwo.text = matTwo.ToString();
		}
	}
	// Confirms tile selection
	public void PlaceTerrainOne(){
		matActive = matOne;
		if (local){
			board.localTerrain.Add(matTwo);
		} else {
			board.exoticTerrain.Add(matTwo);
		}
		StartCoroutine(CoTerrain());
	}
	// Confirms tile selection
	public void PlaceTerrainTwo(){
		matActive = matTwo;
		if (local){
			board.localTerrain.Add(matOne);
		} else {
			board.exoticTerrain.Add(matOne);
		}
		StartCoroutine(CoTerrain());
	}
	// Places tile selected from explore action
	private IEnumerator CoTerrain(){
		myUI.actionText.text = "Please select a tile to explore.";
		while (!Input.GetMouseButtonDown(0)){
			yield return null;
			if (Input.GetMouseButtonDown (0)){
				primaryTile = SetActiveTile();
				if (primaryTile != null  && primaryTile.tt == Tile.TerrainType.Unexplored){
					if(!primaryTile.coastal && matActive.name.ToString() == "Coast"){
				 		myUI.actionText.text = "Coastal terrain can only be placed in a coastal region!  Please select again.";
				 		break;	
					}
					if(primaryTile.exotic && local){
				 		myUI.actionText.text = "Local terrain can only be placed in a local region!  Please select again.";
				 		break;	
					}
					if(!primaryTile.exotic && !local){
				 		myUI.actionText.text = "Exotic terrain can only be placed in an exotic region!  Please select again.";
				 		break;	
					}
					MeshRenderer ren = primaryTile.GetComponents<MeshRenderer>().FirstOrDefault();
				 	ren.material = matActive;
				 	primaryTile.ExploreTile(matActive);
				 	primaryTile.SetResources();
				 	if(primaryTile.resource != null){
				 		players[activePlayer].IncreaseCommodity(primaryTile.resource);
				 	}
				 	players[activePlayer].actionCounter --;
				 	myUI.TogglePanel(myUI.explorePanel);
					myUI.UpdateInfluence();
					NextPlayer();
				} else {
					yield return null;
				}
			}
		}
	}
	#endregion
	#region Influence, Trade
	// Start of influence action
	public void Influence(){
		action = true;
		StartCoroutine(CoInfluence());
	}
	// Main body of influence action
	public IEnumerator CoInfluence(){
		myUI.actionText.text = "Please select a tile to expand influence.";
		while (!Input.GetMouseButtonDown(0)){
			yield return null;
			if (Input.GetKeyDown(KeyCode.Escape)){
				myUI.actionText.text = "";
				action = false;
				break;
			}
			if (Input.GetMouseButtonDown (0)){
				// needs to check if there is an explored tile without influence or enemy martial units which is adjacent to a 
				// tile containing influence  
				if (SetActiveTile() != null   
					&& !SetActiveTile().influence[activePlayer] && SetActiveTile().tt != Tile.TerrainType.Unexplored 
					&& SetActiveTile().tt != Tile.TerrainType.City){
					SetActiveTile().LineColour(Color.white);
					SetActiveTile().LineWidth(0.06f,0.06f);
					primaryTile = SetActiveTile();
					players[activePlayer].actionCounter--;
					primaryTile.actionCounter[activePlayer] = true;
					myUI.TogglePanel(myUI.moveOptionPanel);
//					if (grid.Neighbours(primaryTile).Where(n => ContainsMoveableUnits(n)).Any()){
//						StartMovement(primaryTile);
//						
//					} else {
//						myUI.actionText.text = "No units are within range.";
//					}
					if(Blockaded(primaryTile) == null){
						// check the tile to see if anyone else influences it, if they do remove influence
						if (primaryTile.influence.Contains(true)){
							int i = primaryTile.influence.IndexOf(true);
							primaryTile.influence[i] = false;
						} else {
							primaryTile.influence[activePlayer] = true;
						}
					} else {
						myUI.actionText.text = "Tile is blockaded";
					}
//					myUI.UpdateInfluence();
//					NextPlayer();
				} else {
					yield return null;
				}
			}
		}
	}

	public void Caravan(){
		StartCoroutine(CoCaravan());
		action = true;
	}

	public IEnumerator CoCaravan(){
		myUI.actionText.text = "Please select another player's city without your action counter and with which you have " +
			"connecting influence.";
		while (!Input.GetMouseButtonDown(0)){
			yield return null;
			if (Input.GetKeyDown(KeyCode.Escape)){
				myUI.actionText.text = "";
				action = false;
				break;
			}
			if (Input.GetMouseButtonDown (0)){
				if (SetActiveTile() != null  && SetActiveTile().tt == Tile.TerrainType.City   
					&& !SetActiveTile().actionCounter[activePlayer] && !SetActiveTile().influence[activePlayer] &&
					CaravanConnected(players[activePlayer], players[SetActiveTile().influence.IndexOf(true)])){
					SetActiveTile().LineColour(Color.white);
					SetActiveTile().LineWidth(0.06f,0.06f);
					primaryTile = SetActiveTile();
					players[activePlayer].actionCounter--;
					primaryTile.actionCounter[activePlayer] = true;
					trade.Caravan(players[activePlayer], players[SetActiveTile().influence.IndexOf(true)]);
				} else {
					yield return null;
				}
				myUI.UpdateInfluence();
			}
		}
	}

	public void Shipping(){
		StartCoroutine(CoShipping());
		action = true;
	}

	public IEnumerator CoShipping(){
		myUI.actionText.text = "Please select another player's city without your action counter and with which you have " +
			"a connecting sea route.";
		while (!Input.GetMouseButtonDown(0)){
			yield return null;
			if (Input.GetKeyDown(KeyCode.Escape)){
				myUI.actionText.text = "";
				action = false;
				break;
			}
			if (Input.GetMouseButtonDown (0)){
				if (SetActiveTile() != null  && SetActiveTile().tt == Tile.TerrainType.City   
					&& !SetActiveTile().actionCounter[activePlayer] && !SetActiveTile().influence[activePlayer] 
					&& ShippingConnected(players[activePlayer])){
					SetActiveTile().LineColour(Color.white);
					SetActiveTile().LineWidth(0.06f,0.06f);
					primaryTile = SetActiveTile();
					players[activePlayer].actionCounter--;
					primaryTile.actionCounter[activePlayer] = true;
					trade.Shipping(players[activePlayer], players[SetActiveTile().influence.IndexOf(true)]);
				} else {
					yield return null;
				}
			}
		}
	}
	#endregion
	public void Investigate(){
		
	}

	public void Hunt(){
		
	}
	#region Produce Actions
	public void Refinery(){
		StartCoroutine(CoRefinery());
		action = true;
	}

	public IEnumerator CoRefinery(){
		myUI.actionText.text = "Please select a producing tile without enemy martial units containing one of your peasants.";
		while (!Input.GetMouseButtonDown(0)){
			yield return null;
			if (Input.GetKeyDown(KeyCode.Escape)){
				action = false;
				myUI.actionText.text = "";
				break;
			}
			if (Input.GetMouseButtonDown (0)){
				if ((SetActiveTile() != null  && SetActiveTile().gameComponents.Where(g => g.playerController == activePlayer 
					&& g.GetType() == typeof(Peasant)).Count() > 0) && SetActiveTile().resource != null 
					&& !SetActiveTile().actionCounter[activePlayer]){
					SetActiveTile().LineColour(Color.white);
					SetActiveTile().LineWidth(0.06f,0.06f);
					primaryTile = SetActiveTile();
					players[activePlayer].actionCounter--;
					primaryTile.actionCounter[activePlayer] = true;
					myUI.TogglePanel(myUI.moveOptionPanel);
//					if (grid.Neighbours(primaryTile).Where(n => ContainsMoveableUnits(n)).Any()){
//						StartMovement(primaryTile);
//					} else {
//						myUI.actionText.text = "No units are within range.";
//					}
//					if(Blockaded(primaryTile) == null){
//						myUI.TogglePanel(myUI.refineryPanel);
//						trade.BuildRefinery(SetActiveTile());
//					} else {
//						myUI.actionText.text = "Tile is blockaded";
//					}
//					myUI.UpdateInfluence();
				} else {
					myUI.actionText.text = "No Peasant or Available Resource";
					yield return null;
				}
			}
		}
	}

	public void Produce(){
		if(!players[activePlayer].homeCity.actionCounter[activePlayer]){
			action = true;
			players[activePlayer].homeCity.actionCounter[activePlayer] = true;
			players[activePlayer].actionCounter--;
			myUI.TogglePanel(myUI.producePanel);
			trade.SetProduceCost(0); // sets production list to first item
			trade.queueCount = 0; // used to cap production at 4 components
			trade.produceAction = true;
		}
	}
	#endregion

	public void PassAction(){
		players[activePlayer].actionCounter = 0;
		NextPlayer();
	}

	#region Specialist Actions
	public void Assassinate(){
		
	}

	public void Diplomacy(){
		
	}

	public void Entertain(){
		
	}
	#endregion
	#region Transplant Villages
	// "yes" option in transplant village query
	public void TransplantVillage(){
		myUI.TogglePanel(myUI.TVConfirm);
		myUI.actionText.text = "Please select a tile containing one or more of your peasants.";
		StartCoroutine(CoTransplant());	
	}
	// "no" option in transplant village query
	public void PassTV(){
		NextPlayer();
		if(activePlayer == 0){
			gamePhase = GamePhase.Harvest;
			Harvest();
			myUI.TogglePanel(myUI.TVConfirm);
		} 
	}
	// Second step of transplant village; checks valid tiles and opens up TV panel
	private IEnumerator CoTransplant(){
		while (!Input.GetMouseButtonDown(0)){
			yield return null;
			if (Input.GetMouseButtonDown(0)){
				if (SetActiveTile().gameComponents.Where(g => g.GetType() == typeof(Peasant) && g.playerController == activePlayer)
				.Any()){ 
					secondaryTile = SetActiveTile();
					myUI.actionText.text = "Please right click a destination for your peasants";
					StartCoroutine(PeasantMove());
				} else {
					yield return null;
				}
			}
		}
	}
	// sets destination tile for peasants
	private IEnumerator PeasantMove(){
		while (!Input.GetMouseButtonDown(1)){
			yield return null;
			if (Input.GetMouseButtonDown(1)){
				if (SetActiveTile() != secondaryTile && SetActiveTile().influence[activePlayer] && grid.Neighbours(secondaryTile)
				.Contains(SetActiveTile())){ 
					primaryTile = SetActiveTile();
					myUI.TogglePanel(myUI.TVPanel);
					myUI.TVDisplay();
					List<GameComponents> tempG = new List<GameComponents>();
					List<Peasant> tempP = new List<Peasant>();
					tempG = secondaryTile.gameComponents.Where
						(g => g.playerController == activePlayer && g.GetType() == typeof(Peasant)).ToList();
					tempP = tempG.Cast<Peasant>().ToList();
					peasantsInput.maxValue = tempP.Where
						(p => !p.hasMoved).Count();
				} else {
					yield return null;
				}
			}
		}
	}
	// ends transplant village and adjusts components if needed
	public void FinishTVButton(){
		TVComponents();
		myUI.actionText.text = "";
		NextPlayer();
		if(activePlayer == 0){
			gamePhase = GamePhase.Harvest;
			Harvest();
			myUI.TogglePanel(myUI.TVPanel);
		} else {
			myUI.TogglePanel(myUI.TVConfirm);
		}
	}
	// Allows for further selection of peasants
	 public void TVMoreButton(){
	 		TVComponents();
			myUI.TogglePanel(myUI.TVPanel);
			myUI.actionText.text = "Please select another tile which contains valid units";
			//moveMore = false;
	 		StartCoroutine(CoTransplant());
	 } 
	 // Method to move components from one tile to another
	 private void TVComponents(){
		// move peasants and adjust values in each tile
	 	for (int i = 0; i < peasantsInput.value; i++ ){
	 		primaryTile.gameComponents.Add(new Peasant(activePlayer, primaryTile, true));
			GameComponents b = secondaryTile.gameComponents.Find(g => g.playerController == activePlayer 
				&& g.GetType() == typeof(Peasant));
			secondaryTile.gameComponents.Remove(b);
	 	}  
	}
	#endregion
	// final action in end of turn
	private void Harvest(){
		List<Peasant> peasants = new List<Peasant>();
		List<Refinery> refineries = new List<Refinery>();
		List<AdvancedRefinery> advRefineries = new List<AdvancedRefinery>();
		foreach(Tile t in grid.Tiles.Values){
			for(int i = 0; i < 6; i++){
				t.actionCounter[i] = false;
				if(t.influence[i] == true){
					players[i].gold ++;
				}
			}
			foreach(Peasant p in t.gameComponents.Where(g => g.GetType() == typeof(Peasant))){
				peasants.Add(p);
			}
			foreach(Refinery r in t.gameComponents.Where(g => g.GetType() == typeof(Refinery))){
				refineries.Add(r);
			}
			foreach(AdvancedRefinery ar in t.gameComponents.Where(g => g.GetType() == typeof(AdvancedRefinery))){
				advRefineries.Add(ar);
			}
		}
		foreach(Peasant p in peasants){
			p.GenerateResource(players[p.playerController]);
			p.hasMoved = false;
		}
		foreach(Refinery r in refineries){
			r.GenerateGoods(players[r.playerController]);
		}
		foreach(AdvancedRefinery ar in advRefineries){
			ar.GenerateAdvancedGoods(players[ar.playerController]);
		}
		foreach(Player p in players){
			p.actionCounter = 4;
		}
		gamePhase = GamePhase.TradeMatrix;
		nextPlayer = true;
		StartCoroutine(CoMatrix());
	}




}
