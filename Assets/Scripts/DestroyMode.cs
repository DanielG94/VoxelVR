using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DestroyMode : MonoBehaviour {

	GameObject _headset;
	GameObject _lastHit;

	void Start () {
		SwitchMode.EnablePointer();

		_headset = GameObject.Find("Headset");

		if (GetComponent<VRTK_ControllerEvents>() == null) {
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerEvents_ListenerExample", "VRTK_ControllerEvents", "the same"));
			return;
		}



		_headset.GetComponent<VRTK_Pointer>().PointerStateValid += DoPointerEnter;
		_headset.GetComponent<VRTK_Pointer>().PointerStateInvalid += DoPointerExit;
		GetComponent<VRTK_Pointer>().PointerStateValid += DoPointerEnter;
		GetComponent<VRTK_Pointer>().PointerStateInvalid += DoPointerExit;
	}

	private void Update() {
		if (GetComponent<VRTK_ControllerEvents>().triggerPressed) {
			if ((_headset.GetComponent<VRTK_Pointer>().IsStateValid() || GetComponent<VRTK_Pointer>().IsStateValid()) && _lastHit != null) {
				CubeController.CubeRemoved(_lastHit);
				Destroy(_lastHit);
			}
				
		}
	}

	private void OnDestroy() {
		SwitchMode.DisablePointer();
		_headset.GetComponent<VRTK_Pointer>().PointerStateValid -= DoPointerEnter;
		_headset.GetComponent<VRTK_Pointer>().PointerStateValid -= DoPointerExit;
		GetComponent<VRTK_Pointer>().PointerStateValid -= DoPointerEnter;
		GetComponent<VRTK_Pointer>().PointerStateValid -= DoPointerExit;
	}


	private void DoPointerEnter(object sender, DestinationMarkerEventArgs e) {
		//Debug.Log("Pointer Enter: "+e.target.gameObject);
		_lastHit = e.target.gameObject;
		//DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "POINTER HOVER", e.target, e.raycastHit, e.distance, e.destinationPosition);
	}
	private void DoPointerExit(object sender, DestinationMarkerEventArgs e) {
		//Debug.Log("Pointer Exit");
		_lastHit = null;
		//DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "POINTER HOVER", e.target, e.raycastHit, e.distance, e.destinationPosition);
	}

}
