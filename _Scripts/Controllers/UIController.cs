using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class UIController : MonoBehaviour {
	public Grid grid; // accesses tile set
	public GameController game; // communicates with gamecontroller
	// Text fields displaying unit numbers
	public Text[] playerstxt, burgherstxt, peasantstxt, warriorstxt, uniquestxt, goldtxt = new Text[6];
	// Text fields to display counters and refineries
	public Text[] actionCounterstxt, refineriestxt, advancedRefineriestxt = new Text[6];
	// Text fields displaying tile information
	public Text terraintxt, basicResourcestxt, advancedResourcestxt, influencetxt, exotictxt;
	// buildings
	public Text[] buildingstxt = new Text[5];
	//player contents
	public Text[] playerResources = new Text[20];
	// movement option displays
	public Text burghersMove, warriorsMove, uniquesMove, peasantsMove;
	// general comment field
	public Text actionText, activePlayerText;
	// terrain selection
	public Text terrainOne, terrainTwo;
	public Slider burghersInput, warriorsInput, uniquesInput; // input field to move pieces
	// panels used for various functions
	public GameObject movementPanel, explorePanel, localExPanel, matrixPanel, producePanel, resourcePanel, refineryPanel, TVPanel,
		TVConfirm, moveOptionPanel; 
	// displays for production and trade
	public Text produceCost, produceQueue, ProducePayment, refineryCost, refineryPayment;
	public Button[] commodoties = new Button[20];

	private GameObject activePanel; // correspond to panels for buildings and resources
	public bool panelActive; // toggles to prevent multiple panels opening on each other

	public GameObject actionButtons; // used to toggle buttons active/inactive

	public bool[] containsPlayerUnits = new bool[6];
	public Text combatText, matrixText; // used for testing

	public Dropdown resources, advResources, goods, advGoods;

	// action buttons and buttons for production and exploration
	public Button scout, explore, influence, caravan, shipping, investigate, hunt, refinery, production, assassinate,
	diplomacy, entertain, localButton, exoticButton, produce, clearPayment, completeProduction, closeMatrix, refineryProduce,
	refineryClear, buildingsB, resourcesB, tvYes, moveYes, moveMore, pass;

	void Start() {
		for(int i = 0; i < 6; i++){
			playerstxt[i].text = (game.players[i].playerNumber + 1).ToString();
			goldtxt[i].text = (game.players[i].gold).ToString();
		}
	}

	void Update () {
		ActionButtonsActive();
		OnMouseHover();
		for(int i = 0; i < 6; i++){
			goldtxt[i].text = (game.players[i].gold).ToString();
		}
		activePlayerText.text = "Player " + (game.activePlayer + 1).ToString();
	}

	public void MoveDisplay(){
		burghersMove.text = "0";
		warriorsMove.text = "0";
		uniquesMove.text = "0";
	}

	public void TVDisplay(){
		peasantsMove.text = "0";
	}

	public void BSliderValues(float input){
		burghersMove.text = input.ToString();
	}

	public void WSliderValues(float input){
		warriorsMove.text = input.ToString();
	}

	public void USliderValues(float input){
		uniquesMove.text = input.ToString();
	}
	public void PSliderValues(float input){
		peasantsMove.text = input.ToString();
	}
	// Function that sets all the tiles to their appropriate player colour based on control
	public void UpdateInfluence(){
		for (int i = 0; i < 6; i++)
			foreach(Tile tile in grid.Tiles.Values){
				if (tile.influence[i]){
					tile.LineColour(game.playerColours[i]);
					tile.LineWidth(.08f);	
			} else if (!tile.influence.Contains(true)){
				tile.LineColour(Color.black);
				tile.LineWidth(.03f,.03f);
			}
		}
	}

	private void OnMouseHover () {
			terraintxt.text = "";
			basicResourcestxt.text = "";
			basicResourcestxt.text = "";
			advancedResourcestxt.text = "";
			advancedResourcestxt.text = ""; 
			matrixText.text = "";
			exotictxt.text = "";
			for (int y = 0; y < 6; y++){
				// int values
				burgherstxt[y].text = "";
				peasantstxt[y].text = "";
				warriorstxt[y].text = "";
				uniquestxt[y].text = "";
				refineriestxt[y].text = "";
				advancedRefineriestxt[y].text = "";
				influencetxt.text = "";
				actionCounterstxt[y].text = "";
			}
			RaycastHit hit;
			Ray ray =  Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)){
				if (hit.collider.gameObject != null ){
					GameObject go = hit.collider.gameObject;
					Tile tile = go.GetComponent<Tile>();
					terraintxt.text = tile.tt.ToString();
					if (tile.resource != null){
						basicResourcestxt.text = tile.resource.ToString();
					} else {
						basicResourcestxt.text = "None";
					}
					if (tile.advResource != null){
						advancedResourcestxt.text = tile.advResource.ToString();
					} else {
						advancedResourcestxt.text = "None";
					}
					if(tile.tt == Tile.TerrainType.City){
						int i = tile.influence.IndexOf(true);
						if(game.players[i].basicResources.Contains(true)){ 
							string resourceDisplay = game.players[i].basicResourcesType.ElementAt
							(game.players[i].basicResources.IndexOf(true)).ToString();
							string advResourceDisplay = game.players[i].advancedResourcesType.ElementAt
							(game.players[i].advancedResources.IndexOf(true)).ToString() ;
							string goodsDisplay = game.players[i].basicGoodsType.ElementAt(game.players[i].basicGoods.
							IndexOf(true)).ToString();
							string advGoodsDisplay = game.players[i].advancedGoodsType.ElementAt
							(game.players[i].advancedGoods.IndexOf(true)).ToString();
							matrixText.text = resourceDisplay + "\n" + advResourceDisplay + "\n" + 
							goodsDisplay + "\n" + advGoodsDisplay;
						} else {
							matrixText.text = "Unselected";
						}
					}
					//combatText.text = tile.index.ToString(); // for testing
					if (tile.exotic){
						exotictxt.text = "Exotic";
					} else {
						exotictxt.text = "Local";	
					}
					// iterates through each player and displays each type of piece present in the tile
					for (int y = 0; y < 6; y++){
						// int values
						burgherstxt[y].text = tile.gameComponents.Where
						(g => g.playerController == y && g.GetType() == typeof(Burgher)).Count().ToString();
						if (tile.gameComponents.Where(g => g.playerController == y && g.GetType() == typeof(Burgher)).Count() == 0){
							burgherstxt[y].text = "";
						}
						peasantstxt[y].text = tile.gameComponents.Where
						(g => g.playerController == y && g.GetType() == typeof(Peasant)).Count().ToString();
						if (tile.gameComponents.Where(g => g.playerController == y && g.GetType() == typeof(Peasant)).Count() == 0){
							peasantstxt[y].text = "";
						}
						warriorstxt[y].text = tile.gameComponents.Where
						(g => g.playerController == y && g.GetType() == typeof(Warrior)).Count().ToString();
						if (tile.gameComponents.Where(g => g.playerController == y && g.GetType() == typeof(Warrior)).Count() == 0){
							warriorstxt[y].text = "";
						}
						uniquestxt[y].text = tile.gameComponents.Where
						(g => g.playerController == y && g.GetType() == typeof(Unique)).Count().ToString();
						if (tile.gameComponents.Where(g => g.playerController == y && g.GetType() == typeof(Unique)).Count() == 0){
							uniquestxt[y].text = "";
						}

						refineriestxt[y].text = tile.gameComponents.Where
						(g => g.playerController == y && g.GetType() == typeof(Refinery)).Count().ToString();
						if (tile.gameComponents.Where(g => g.playerController == y && g.GetType() == typeof(Refinery)).Count() == 0){
							refineriestxt[y].text = "";
						}
						advancedRefineriestxt[y].text = tile.gameComponents.Where
						(g => g.playerController == y && g.GetType() == typeof(AdvancedRefinery)).Count().ToString();
						if (tile.gameComponents.Where(g => g.playerController == y && g.GetType() == 
							typeof(AdvancedRefinery)).Count() == 0){
							advancedRefineriestxt[y].text = "";
						}

						// boolean values 
						if (tile.influence[y]){
							influencetxt.text = "Player " + (y + 1);
							}
						if (tile.actionCounter[y]){
							actionCounterstxt[y].text = "Y";
						} else if (!tile.actionCounter[y]){
							actionCounterstxt[y].text = "";
							}
					}
					if (!tile.influence.Contains(true)){
					influencetxt.text = "None";
					}
				if(Input.GetMouseButtonDown(0) && tile.tt == Tile.TerrainType.City){
					TogglePanel(matrixPanel);
				}
			} 
		}
	}

	public void TogglePanel(GameObject panel){
		if(panel == producePanel || panel == refineryPanel){
			resourcePanel.SetActive(!resourcePanel.activeSelf);
		}
		if(activePanel != panel && panelActive == true){
			activePanel.SetActive(!activePanel.activeSelf);
			panel.SetActive(!panel.activeSelf);
			activePanel = panel;
			return;
		} else {
			panel.SetActive(!panel.activeSelf);
			panelActive = !panelActive;
			activePanel = panel;
		}
	}

	private void ActionButtonsActive(){
		if(game.gamePhase == GameController.GamePhase.Actions && !game.action){
			if(game.CanScout()){
				scout.interactable = true;
			} else {
				scout.interactable = false;
			}
			if(game.CanExplore()){
				explore.interactable = true;
			} else {
				explore.interactable = false;
			}
			if(game.CanInfluence()){
				influence.interactable = true;
			} else {
				influence.interactable = false;
			}
			if(game.CanCaravan()){
				caravan.interactable = true;
			} else {
				caravan.interactable = false;
			}
			if(game.CanShip()){
				shipping.interactable = true;
			} else {
				shipping.interactable = false;
			}
			if(game.CanRefinery()){
				refinery.interactable = true;
			} else {
				refinery.interactable = false;
			}
			if(game.CanProduction()){
				production.interactable = true;
			} else {
				production.interactable = false;
			}
			pass.interactable = true;
			resourcesB.interactable = true;
			buildingsB.interactable = true;
		} else {
			foreach(Button b in actionButtons.GetComponentsInChildren<Button>()){
			b.interactable = false;
			}
		}
		if(game.CanMove()){
			moveYes.interactable = true;
			moveMore.interactable = true;
		} else {
			moveYes.interactable = false;
			moveMore.interactable = false;
		}
		if(game.CanTransplant()){
			tvYes.interactable = true;
		} else {
			tvYes.interactable = false;
		}
	}

	public void BuildingDisplay(){
		foreach(GameComponents g in game.players[game.activePlayer].homeCity.gameComponents){
			string temp = g.ToString();
			switch(temp){
			case "Botanist":
			buildingstxt[0].color = Color.red;
			break;
			case "DruidsCircle":
			buildingstxt[1].color = Color.red;
			break;
			case "Ranch":
			buildingstxt[2].color = Color.red;
			break;
			case "AdventurersGuild":
			buildingstxt[3].color = Color.red;
			break;
			case "MinersGuild":
			buildingstxt[4].color = Color.red;
			break;
			default:
			break;
			}
		}
	}
}