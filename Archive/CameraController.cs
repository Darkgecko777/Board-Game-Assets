using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Camera playerOne;
	public Camera playerTwo;
	public Camera playerThree;
	public Camera playerFour;
	public Camera playerFive;
	public Camera playerSix;
	 
     void Update () {
     	SetCamera();
         }

	void SetCamera(){
		if (Input.GetKey(KeyCode.F1)){
			playerOne.enabled = true;
			playerTwo.enabled = false;
			playerThree.enabled = false;
			playerFour.enabled = false;
			playerFive.enabled = false;
			playerSix.enabled = false;
		}
		if (Input.GetKey(KeyCode.F2)){
			playerOne.enabled = false;
			playerTwo.enabled = true;
			playerThree.enabled = false;
			playerFour.enabled = false;
			playerFive.enabled = false;
			playerSix.enabled = false;
		}
		if (Input.GetKey(KeyCode.F3)){
			playerOne.enabled = false;
			playerTwo.enabled = false;
			playerThree.enabled = true;
			playerFour.enabled = false;
			playerFive.enabled = false;
			playerSix.enabled = false;
		}
		if (Input.GetKey(KeyCode.F4)){
			playerOne.enabled = false;
			playerTwo.enabled = false;
			playerThree.enabled = false;
			playerFour.enabled = true;
			playerFive.enabled = false;
			playerSix.enabled = false;
		}
		if (Input.GetKey(KeyCode.F5)){
			playerOne.enabled = false;
			playerTwo.enabled = false;
			playerThree.enabled = false;
			playerFour.enabled = false;
			playerFive.enabled = true;
			playerSix.enabled = false;
		}
		if (Input.GetKey(KeyCode.F6)){
			playerOne.enabled = false;
			playerTwo.enabled = false;
			playerThree.enabled = false;
			playerFour.enabled = false;
			playerFive.enabled = false;
			playerSix.enabled = true;
		}
	}
}
