using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Elevator : MonoBehaviour {

	public float speed = 5f;

	private float startTime;
	private float distance;
	private Transform playArea;

	private float _xForce;
	private float _yForce;

	void Start () {
		if (GetComponent<VRTK_ControllerEvents>() == null) {
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerEvents_ListenerExample", "VRTK_ControllerEvents", "the same"));
			return;
		}

		GetComponent<VRTK_ControllerEvents>().TouchpadTouchStart += DoTouchpadTouchStart;
		GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += DoTouchpadAxisChanged;
		GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += DoTouchpadTouchEnd;

	}

	private void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e) {
		Debug.Log(e.touchpadAxis.y);
		playArea = VRTK_DeviceFinder.PlayAreaTransform();
		playArea.GetComponent<Rigidbody>().isKinematic = false;
		playArea.GetComponent<Rigidbody>().useGravity = false;
		playArea.GetComponent<Rigidbody>().mass = 0.1f;

		/*
		if (e.touchpadAxis.y < 0 && playArea.position.y <= 0) return;
		playArea.Translate(0, e.touchpadAxis.y, 0);
		startPos = new Vector3(playArea.position.x, playArea.position.y, playArea.position.z);
		endPos = new Vector3(playArea.position.x + e.touchpadAxis.x, playArea.position.y + e.touchpadAxis.y, playArea.position.z);
		distance = Vector3.Distance(startPos, endPos);
		startTime = Time.time;
		isMoving = true;*/
		_xForce = e.touchpadAxis.x;
		_yForce = e.touchpadAxis.y;
	}

	private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e) {
		_xForce = 0;
		_yForce = 0;
		playArea.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e) {
		_xForce = e.touchpadAxis.x;
		_yForce = e.touchpadAxis.y;
	}

	void Update () {
		
		if (playArea == null || !GetComponent<VRTK_ControllerEvents>().touchpadTouched) return;

		if (playArea.position.y <= 0 && _yForce < 0) {
			playArea.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			_yForce = 0;
		}

		//When right touchpad is pressed, apply force into direction
		if (Mathf.Abs(_xForce) > Mathf.Abs(_yForce))
			playArea.gameObject.GetComponent<Rigidbody>().AddForce(Camera.main.transform.right * _xForce * speed, ForceMode.Force);
		else
			playArea.gameObject.GetComponent<Rigidbody>().AddForce(playArea.transform.up * _yForce * speed, ForceMode.Force);

		//Debug.Log(_xForce + " : " + _yForce);
	}
}
