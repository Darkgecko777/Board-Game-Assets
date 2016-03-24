using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CombatController : MonoBehaviour {

	public GameController game;
	public UIController myUI;

	private List<GameComponents> friendlyWarriors, hostileWarriors, friendlyUniques, hostileUniques;
	private int friendlies, hostiles, friendlyWounds, hostileWounds;

	public void ResolveCombat(Tile t){
		friendlyWarriors = t.gameComponents.Where(g => g.GetType() == typeof(Warrior) 
			&& g.playerController == game.activePlayer).ToList();
		friendlyUniques = t.gameComponents.Where(g => g.GetType() == typeof(Unique) 
			&& g.playerController == game.activePlayer).ToList();
		hostileWarriors = t.gameComponents.Where(g => g.GetType() == typeof(Warrior) 
			&& g.playerController != game.activePlayer).ToList();
		hostileUniques = t.gameComponents.Where(g => g.GetType() == typeof(Unique)
			&& g.playerController != game.activePlayer).ToList();
		hostiles = hostileWarriors.Count() + hostileUniques.Count();
		friendlies = friendlyWarriors.Count() + friendlyUniques.Count();
		while(hostiles > 0 && friendlies > 0){
			// check for combat success from friendly warriors
			foreach(Warrior fw in friendlyWarriors){
				if(fw.CombatRoll()){
					hostileWounds++;
					myUI.combatText.text = myUI.combatText.text + "\n" + "Your warrior hit";
				}
			}
			// check for combat success from friendly uniques
			foreach(Unique fu in friendlyUniques){
				if(fu.CombatRoll()){
					hostileWounds++;
					myUI.combatText.text = myUI.combatText.text + "\n" + "Your unique hit";
				}
			}
			// check for combat success from hostile warriors
			foreach(Warrior hw in hostileWarriors){
				if(hw.CombatRoll()){
					friendlyWounds++;
					myUI.combatText.text = myUI.combatText.text + "\n" + "Enemy warrior hit";
				}
			}
			// check for combat success from hostile uniques
			foreach(Unique hu in hostileUniques){
				if(hu.CombatRoll()){
					friendlyWounds++;
					myUI.combatText.text = myUI.combatText.text + "\n" + "Enemy unique hit";
				}
			}
			// allocate wounds to hostile units
			for(int hWounds = hostileWounds; hWounds > 0; hWounds--){
				if(hostileWarriors.Any()){
					GameComponents casualty = hostileWarriors.First();
					t.gameComponents.Remove(casualty);
					hostileWarriors.RemoveAt(0);
				} else if(hostileUniques.Any()) {
					GameComponents casualty = hostileUniques.First();
					t.gameComponents.Remove(casualty);
					hostileUniques.RemoveAt(0);
				}
				hostileWounds = hWounds;
			}
			// allocate wounds to friendly units
			for(int fWounds = friendlyWounds; fWounds > 0; fWounds--){
				if(friendlyWarriors.Any()){
					GameComponents casualty = friendlyWarriors.First();
					t.gameComponents.Remove(casualty);
					friendlyWarriors.RemoveAt(0);
				} else if(friendlyUniques.Any()){
					GameComponents casualty = friendlyUniques.First();
					t.gameComponents.Remove(casualty);
					friendlyUniques.RemoveAt(0);
				}
				friendlyWounds = fWounds;
			}
		// check to see if combat continues
		friendlies = friendlyWarriors.Count() + friendlyUniques.Count();
		hostiles = hostileWarriors.Count() + hostileUniques.Count();
		}
	}


}
