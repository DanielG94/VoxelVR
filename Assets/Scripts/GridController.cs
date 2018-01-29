using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GridController : MonoBehaviour {
	//public const float MINSIZE = 0.5f;
	//public const float MAXSIZE = 2f;
	//public const float STEPS = 4f; // (max - min)/steps = int
	public static float GridSize = 0.25f;
	private static bool _gridEnabled = true;

	public void SetSize(float size) {
		GridSize = size;
		Blackboard.DoGridsizeChanged();
	}

	public static bool IsGridEnabled() {
		return _gridEnabled;
	}

	public void ToggleGridEnabled() {
		_gridEnabled = !_gridEnabled;
		Debug.Log("Grid enabled set to " + _gridEnabled);
	}

	/*
	private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e) {

		float gridDelta = GridSize;
		GridSize = GridSize + (MAXSIZE / STEPS);
		if (GridSize > MAXSIZE)
			GridSize = MINSIZE;
		gridDelta = GridSize - gridDelta;
		Debug.Log("Gridsize changed to " + GridSize);

		//GameObject.Find("PlacedCubes").transform.localScale = new Vector3(GridSize, GridSize, GridSize);
		//GameObject.Find("PlacedCubes").transform.position += new Vector3(-gridDelta, gridDelta, gridDelta);

		Blackboard.DoGridsizeChanged();



		//DEBUG: benchmark
		//GameObject.Find("PlacedCubes").GetComponent<MeshCombiner>().CombineMeshes();

	}
	*/
}
