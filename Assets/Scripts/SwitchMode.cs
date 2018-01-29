using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SwitchMode : MonoBehaviour {

	public List<VRTK_InteractableObject> tools;

	private List<VRTK_InteractableObject> _tempTools;
	public static SwitchMode _instance;

	private static GameObject _lastTool;
	public static bool OptionLock = false;

	private static bool _pointerEnabled = false;
	private static bool _headsetPointer = false; // Use controller pointer when false

	private enum CurrentMode {
		Build,
		Destroy,
		Paint
	};

	private CurrentMode _mode;

	private void Awake() {
		_instance = this;
		Debug.Log("Switchmode Awake: " + this.GetInstanceID());
	}

	private void Start() {
		_tempTools = new List<VRTK_InteractableObject>();
		_mode = CurrentMode.Build;
		GetComponent<VRTK_ControllerEvents>().GripPressed += DoGrabPressed;
		GetComponent<VRTK_ControllerEvents>().GripReleased += DoGrabReleased;
		GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed += DoButtonTwoPressed;
	}

	private void OnDestroy() {
		GetComponent<VRTK_ControllerEvents>().GripPressed -= DoGrabPressed;
		GetComponent<VRTK_ControllerEvents>().GripReleased -= DoGrabReleased;
		GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed -= DoButtonTwoPressed;
		_instance = null;
	}

	// When grab button pressed on right controller and the option menu is not open, drop current tool and show tool selection
	private void DoGrabPressed(object sender, ControllerInteractionEventArgs e) {
		if (!OptionLock) {
			ReleaseCurrentTool();
			ShowTools();
		}
	}

	public void ReleaseCurrentTool() {
		//Drop current tool
		if (GetComponent<VRTK_InteractGrab>().GetGrabbedObject() == null) return;
		HideUnpickedTools();
		_lastTool = GetComponent<VRTK_InteractGrab>().GetGrabbedObject();
		Debug.Log("About to release object: " + _lastTool + " on " + _instance.GetInstanceID());
		GetComponent<VRTK_InteractTouch>().ForceStopTouching();
		GetComponent<VRTK_InteractGrab>().ForceRelease();
		_lastTool.SetActive(false);

		//Remove script
		switch (_mode) {
			case CurrentMode.Build:
				DestroyImmediate(GetComponent<BuildMode>());
				break;
			case CurrentMode.Destroy:
				DestroyImmediate(GetComponent<DestroyMode>());
				break;
			case CurrentMode.Paint:
				DestroyImmediate(GetComponent<PaintMode>());
				break;
		}
		Debug.Log("Released object: " + _lastTool);
	}

	public void RestoreLastTool() {
		Debug.Log("Restore object: " + _lastTool + " on " + _instance.GetInstanceID());
		SwitchControllerModel(_lastTool);
	}

	private void SwitchControllerModel(GameObject obj) {
		Debug.Log("Trying to restore " + obj);
		//GameObject grabbedObject = Instantiate(obj, transform.position, Quaternion.identity);
		obj.SetActive(true);

		GetComponent<VRTK_InteractTouch>().ForceTouch(obj.gameObject);
		GetComponent<VRTK_InteractGrab>().AttemptGrab();
		AttachCurrentModeScript();
	}

	private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e) {
		//TogglePointerOrigin();
		//GameObject.Find("Benchmark").GetComponent<Benchmark>().SpawnCubes();
	}

	private void DoGrabReleased(object sender, ControllerInteractionEventArgs e) {
		HideUnpickedTools();
		
	}


	// Code für Kreis positionieurung mit Hilfe von https://answers.unity.com/questions/170413/position-objects-around-other-object-forming-a-cir.html
	private Vector3 CirclePosition(Vector3 center, float radius, int place) { 
		float angle = place * 60 - 60; // * 40 - 60
		Vector3 pos;
		pos.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
		pos.y = center.y + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
		pos.z = center.z;
		return pos; 
	}

	private void ShowTools() {
		Vector3 center = transform.position + new Vector3(0,-0.1f,0f);
		GameObject TempToolContainer = new GameObject();
		
		for (int i = 0; i < tools.Count; i++) {
			// Get Random angle/position
			Vector3 pos = CirclePosition(center, 0.3f, i);
			// make the object face the center
			var rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
			_tempTools.Add(Instantiate(tools[i], pos, Quaternion.identity) as VRTK_InteractableObject);
			_tempTools[i].transform.SetParent(TempToolContainer.transform);
		}
		TempToolContainer.transform.eulerAngles = new Vector3(0,GameObject.Find("Camera (eye)").transform.eulerAngles.y,0);
		TempToolContainer.transform.DetachChildren();
		Destroy(TempToolContainer);
	}

	private void HideUnpickedTools() {
		Debug.Log("Hiding tools");
		foreach (VRTK_InteractableObject tool in _tempTools) { 
			if(GetComponent<VRTK_InteractGrab>() != null && GetComponent<VRTK_InteractGrab>().GetGrabbedObject() != null) {
				// Something is holded
				if (tool != GetComponent<VRTK_InteractGrab>().GetGrabbedObject().GetComponent<VRTK_InteractableObject>()) {
					// Check if one of the tools got grabbed
					Destroy(tool.gameObject); // Destroy only objects that didnt get grabbed
				} else {
					AttachCurrentModeScript();
					GetComponent<VRTK_InteractGrab>().GetGrabbedObject().transform.localScale = new Vector3(2f, 2f, 2f);
				}
			} else {
				// If nothing is grabbed, destroy all
				Destroy(tool.gameObject);
			}
				
		}
		_tempTools.Clear();	
	}

	private void AttachCurrentModeScript() {
		switch (GetComponent<VRTK_InteractGrab>().GetGrabbedObject().tag) {
			case "Build":
				gameObject.AddComponent<BuildMode>();
				_mode = CurrentMode.Build;
				break;
			case "Destroy":
				gameObject.AddComponent<DestroyMode>();
				_mode = CurrentMode.Destroy;
				break;
			case "Paint":
				gameObject.AddComponent<PaintMode>();
				_mode = CurrentMode.Paint;
				break;
		}
	}

	private void Update() {
		if (!_pointerEnabled) return;

		//Hack: For some reason the pointer size changes after switching tools, this prevents it and resets the size on each frame
		if (GameObject.Find("[VRTK][AUTOGEN][Headset][BasePointerRenderer_ObjectInteractor_Container]") != null) {
			GameObject.Find("[VRTK][AUTOGEN][Headset][BasePointerRenderer_ObjectInteractor_Container]").transform.localScale = Vector3.one;
		}
	}

	

	public static bool IsHeadsetPointer() {
		return _headsetPointer;
	}

	public static bool IsColorPickerActive() {
		return (GameObject.Find("ColorPicker") != null);
	}

	public static void DisableColorPicker() {
		Debug.Log("DisableColorPicker");
		if (GameObject.Find("ColorPicker") == null) return;
		GameObject.Find("ColorPicker").transform.GetChild(0).gameObject.SetActive(false);
		Destroy(GameObject.Find("ColorPicker").GetComponent<Colorpicker>());
		GameObject.Find("ColorHistory").transform.GetChild(0).gameObject.SetActive(false);
	}

	public static void EnableColorPicker() {
		Debug.Log("EnableColorPicker");
		GameObject.Find("ColorPicker").transform.GetChild(0).gameObject.SetActive(true);
		//if(GameObject.Find("ColorPicker").GetComponent<Colorpicker>() == null)
		GameObject.Find("ColorPicker").AddComponent<Colorpicker>();
		GameObject.Find("ColorHistory").transform.GetChild(0).gameObject.SetActive(true);
	}

	public void TogglePointerOrigin() {
		if (!_pointerEnabled) {
			_headsetPointer = !_headsetPointer;
		} else {
			DisablePointer();
			_headsetPointer = !_headsetPointer;
			EnablePointer();
		}
		
	}

	public static void DisablePointer() {
		string origin = _headsetPointer ? "headset" : "controller";
		Debug.Log("Disable Pointer for " + origin);

		if (_headsetPointer) {
			GameObject headset = GameObject.Find("Headset");
			headset.GetComponent<VRTK_Pointer>().enabled = false;
			headset.GetComponent<VRTK_StraightPointerRenderer>().enabled = false;
		} else {
			_instance.gameObject.GetComponent<VRTK_StraightPointerRenderer>().enabled = false;
			_instance.gameObject.GetComponent<VRTK_Pointer>().enabled = false;
		}
		_pointerEnabled = false;
	}

	public static void EnablePointer() {
		string origin = _headsetPointer ? "headset" : "controller";
		Debug.Log("Enable Pointer for " + origin);

		if (_headsetPointer) {
			GameObject headset = GameObject.Find("Headset");
			headset.GetComponent<VRTK_StraightPointerRenderer>().enabled = true;
			headset.GetComponent<VRTK_Pointer>().enabled = true;
		} else {
			_instance.gameObject.GetComponent<VRTK_StraightPointerRenderer>().enabled = true;
			_instance.gameObject.GetComponent<VRTK_Pointer>().enabled = true;
		}
		_pointerEnabled = true;
	}

	static public SwitchMode Instance {
		get {
			if (_instance == null) {
				_instance = new SwitchMode();
				Debug.Log("New Switchmode Instance: " + _instance.GetInstanceID());
			}
			return _instance;
		}
	}
}
