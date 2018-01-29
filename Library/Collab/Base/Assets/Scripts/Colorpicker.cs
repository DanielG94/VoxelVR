using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

//Code made with tutorial: https://www.youtube.com/watch?v=1HDdPX6NQKE

public class Colorpicker : MonoBehaviour {

	public static Color cubeColor;

	private float _hue, _saturation, _value = 1f;
	private GameObject _blackWheel;
	private static bool _gripTouched = false;

	// Use this for initialization
	void Start() {
		if (GetComponentInParent<VRTK_ControllerEvents>() == null) {
			Debug.LogError("ColorWheel is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
			return;
		}

		_blackWheel = transform.Find("Canvas/BlackWheel").gameObject;
		cubeColor = Color.white;

		GetComponentInParent<VRTK_ControllerEvents>().TouchpadAxisChanged += DoTouchpadAxisChanged;
		GetComponentInParent<VRTK_ControllerEvents>().TouchpadTouchEnd += DoTouchpadTouchEnd;
		GetComponentInParent<VRTK_ControllerEvents>().GripPressed += DoGripTouchStart;
		GetComponentInParent<VRTK_ControllerEvents>().GripReleased += DoGripTouchEnd;
	}

	private void DoGripTouchStart(object sender, ControllerInteractionEventArgs e) {
		Debug.LogWarning("Colorhistory opened");
		_gripTouched = true;
	}

	private void DoGripTouchEnd(object sender, ControllerInteractionEventArgs e) {
		Debug.LogWarning("Colorhistory closed");
		_gripTouched = false;

		
	}

	private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e) {
		Debug.Log("Color picked, Grip active: " + _gripTouched + " " +GetInstanceID());
		if (_gripTouched) return;
		if (GetComponentInParent<VRTK_ControllerEvents>().touchpadPressed) {
			ChangedValue(e.touchpadAxis);
		} else {
			ChangedHueSaturation(e.touchpadAxis, e.touchpadAngle);
		}
	}

	private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e) {
		if (_gripTouched) return;
		Debug.Log("touchpad touch end: " + _gripTouched);

		Custom_RadialMenu menu = GameObject.Find("ColorHistoryMenu").GetComponent<Custom_RadialMenu>();
		if (menu != null) {
			menu.AddButton(new Custom_RadialMenu.RadialMenuButton(), cubeColor);
		} else {
			Debug.LogError("Radial menu null");
		}

	}

	private void ChangedValue(Vector2 touchpadAxis) {
		

		this._value = (touchpadAxis.y + 1) / 2;
		Color currColor = _blackWheel.GetComponent<Image>().color;
		currColor.a = 1 - this._value;
		_blackWheel.GetComponent<Image>().color = currColor;
		Debug.Log("ChangeValue: Trackpad axis at: " + touchpadAxis + " val: " + currColor);
		UpdateColor();
	}

	private void ChangedHueSaturation(Vector2 touchpadAxis, float touchpadAngle) {
		float normalAngle = touchpadAngle - 90;
		if (normalAngle < 0)
			normalAngle = 360 + normalAngle;

		Debug.Log(gameObject.GetInstanceID() + " ChangeColor: Trackpad axis at: " + touchpadAxis + " (" + normalAngle + " degrees)");

		float rads = normalAngle * Mathf.PI / 180;
		float maxX = Mathf.Cos(rads);
		float maxY = Mathf.Sin(rads);

		float currX = touchpadAxis.x;
		float currY = touchpadAxis.y;

		float percentX = Mathf.Abs(currX / maxX);
		float percentY = Mathf.Abs(currY / maxY);

		this._hue = normalAngle / 360.0f;
		this._saturation = (percentX + percentY) / 2;

		UpdateColor();
	}

	private void UpdateColor() {
		cubeColor = Color.HSVToRGB(this._hue, this._saturation, this._value);
	}


	

	private void OnDestroy() {
		if (GetComponentInParent<VRTK_ControllerEvents>() == null) return;
		GetComponentInParent<VRTK_ControllerEvents>().TouchpadAxisChanged -= DoTouchpadAxisChanged;
		GetComponentInParent<VRTK_ControllerEvents>().TouchpadTouchEnd -= DoTouchpadTouchEnd;
		GetComponentInParent<VRTK_ControllerEvents>().GripPressed -= DoGripTouchStart;
		GetComponentInParent<VRTK_ControllerEvents>().GripReleased -= DoGripTouchEnd;
	}
}
