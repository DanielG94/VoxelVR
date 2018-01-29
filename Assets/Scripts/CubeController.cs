using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Manages placed cubes
public class CubeController {
	private static List<GameObject> placedCubes;
	static CubeController() {
		Debug.Log("CubeController awake");
		placedCubes = new List<GameObject>();

		Blackboard.OnGridsizeChanged += DoGridsizeChanged;
	}

	private static void DoGridsizeChanged() {
		/*
		foreach (GameObject cube in placedCubes) {
			cube.transform.localScale = new Vector3(GridController.GridSize, GridController.GridSize, GridController.GridSize);
		}*/
		
	}

	public static void CubePlaced(GameObject cube) {
		//Debug.Log(cube + " placed");
		placedCubes.Add(cube);
		Debug.Log("Cubecontroller: Cube placed: " + cube + "(" + placedCubes.Count + ")");
	}

	public static void CubeRemoved(GameObject cube) {
		placedCubes.Remove(cube);
		Debug.Log("Cubecontroller: Cube removed: " + cube + "(" + placedCubes.Count + " left)");
	}
	
}
