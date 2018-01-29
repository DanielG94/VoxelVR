using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class BuildMode : MonoBehaviour {

	GameObject previewCube;

	void Start () {

		//Make sure controllerevents component exists
		if (GetComponent<VRTK_ControllerEvents>() == null) {
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerEvents_ListenerExample", "VRTK_ControllerEvents", "the same"));
			return;
		}

		Blackboard.OnGridsizeChanged += DoGridsizeChanged;

		previewCube = Instantiate(Resources.Load("Cube"),transform.position + new Vector3(0,0,3),Quaternion.identity) as GameObject;
		previewCube.transform.localScale = new Vector3(GridController.GridSize, GridController.GridSize, GridController.GridSize);
		//previewCube.transform.SetParent(transform);

		SwitchMode.EnableColorPicker();
	}

	private void DoGridsizeChanged() {
		previewCube.transform.localScale = new Vector3(GridController.GridSize, GridController.GridSize, GridController.GridSize);

	}

	private void Update() {
		if (previewCube != null && Camera.main != null) {
			
			if(GridController.IsGridEnabled()) // Position cube in front of the controller in invisible grid by rounding the x,y,z position
				previewCube.transform.position = new Vector3(Mathf.RoundToInt((transform.position.x + transform.forward.x * 2) / GridController.GridSize) * GridController.GridSize + (GridController.GridSize / 2), //* previewCube.transform.localScale.x,
															 Mathf.RoundToInt((transform.position.y + transform.forward.y) / GridController.GridSize) * GridController.GridSize - (GridController.GridSize / 2), // * previewCube.transform.localScale.y + previewCube.transform.localScale.y,
															 Mathf.RoundToInt((transform.position.z + transform.forward.z * 2) / GridController.GridSize) * GridController.GridSize - (GridController.GridSize / 2)); // * previewCube.transform.localScale.z);
			else
				previewCube.transform.position = new Vector3((transform.position.x + transform.forward.x*2),
															 (transform.position.y + transform.forward.y),
															 (transform.position.z + transform.forward.z*2));
			previewCube.GetComponent<Renderer>().material.color = Colorpicker.cubeColor;
		}
			

		if (!GetComponent<VRTK_ControllerEvents>().triggerPressed || previewCube.GetComponent<CubeCollisionCheck>().isColliding()) return;
		GameObject placedCube = Instantiate(previewCube, previewCube.transform.position, previewCube.transform.rotation) as GameObject; // Spawn new cube on preview position
		//placedCube.transform.localScale = Vector3.one;
		placedCube.transform.SetParent(GameObject.Find("PlacedCubes").transform);
		CubeController.CubePlaced(placedCube);
		placedCube.GetComponent<Renderer>().material.color = Colorpicker.cubeColor;
		Destroy(placedCube.GetComponent<CubeCollisionCheck>());
	}

	private void OnDestroy() {
		Destroy(previewCube);
		SwitchMode.DisableColorPicker();
		Blackboard.OnGridsizeChanged -= DoGridsizeChanged;
	}
}
