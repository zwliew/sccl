﻿using System;
using UnityEngine;

/**
 * Controls the logic and flow of the current game
 * This delegates some heavy-work to separate sub-controllers.
 * 
 * Subcontrollers:
 * Handling the data: DataController
 * Displaying the game: DisplayController
 */
public class GameController : MonoBehaviour {

	private DataController dataController;
	private DisplayController displayController;

	private Word curWord;
	private string[] curSelections;

	void Start () {
		//dataController = new DataController ();
		dataController = GetComponent<DataController>();

		//displayController = new DisplayController ();

		displayController = GetComponent<DisplayController> ();

		RestartGame ();
	}

	private void RestartGame () {
		dataController.Initialize ();
		curWord = dataController.GetRandomWord ();
		displayController.Initialize (curWord);
		curSelections = new string[2];
	}

	void Update () {

		/*if (HasPlayerWon ()) {
			displayController.DisplayWin ();
			return;
		}
		*/

		if (AreSelectionsFilled ()) {
			// Reset selections
			displayController.UnselectAllSides ();
			curSelections = new string[2];
			return;
		}

		if (Input.GetMouseButtonDown (0)) {
			PinZiPP selectedSide = GetSelectedSide (Input.mousePosition);
			if (selectedSide != null) {
				//displayController.SelectSide (selectedSide);
				SelectSide (selectedSide);
			}
		}
		if (Input.GetMouseButtonDown (1)) {// Just for clearing selected effects. Merge with Display controller in the future
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;

			if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("PinZiSide")))
			{
				hitInfo.collider.gameObject.GetComponent<PinZiPP> ().SetUnselected ();
			}
		}
	}

	/**
	* Gets the side being selected based on position
	* Returns null if no side is being selected
	*/
	private PinZiPP GetSelectedSide (Vector3 position) {
		Ray ray = Camera.main.ScreenPointToRay(position);
		RaycastHit hitInfo;
		PinZiPP side = null;
		
		if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("PinZiSide")))
		{
			side = hitInfo.collider.gameObject.GetComponent<PinZiPP>();
		}
		return side;
	}

	private void SelectSide(PinZiPP side) {
		if (curSelections [0] == null) {
			displayController.SelectSide (side);
			curSelections [0] = side.name;
			Debug.Log ("curSelection[0] is " + side.name);
		} else {
			displayController.SelectSide (side);
			curSelections [1] = side.name;
			Debug.Log ("curSelection[1] is " + side.name);

			if (HasPlayerWon ()) {
				displayController.DisplayWin ();
				displayController.UnselectAllSides ();
				return;
			} else {
				Debug.Log ("Wrong Selection!");
				displayController.UnselectAllSides ();
				curSelections = new string[2];
			}
		}
	}

	private bool AreSelectionsFilled() {
		return curSelections [0] != null && curSelections [1] != null;
	}

	private bool HasPlayerWon () { // got bug. Go back debug!
		if (!AreSelectionsFilled()) {
			return false;
		}
		string[] correctSelections = curWord.sides;
		return Array.Exists (correctSelections, element => string.Equals (element, curSelections [0]))
			&& Array.Exists (correctSelections, element => string.Equals (element, curSelections [1]));
	}


}
