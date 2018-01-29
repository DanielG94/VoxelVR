using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class OptionController : MonoBehaviour {

	private static bool _showOptions = false;
	private static bool _originalPointerState;
	private static bool _originalPointerOrigin = false; // True = Headset ; False = Tool
	private static bool _originalColorpickerState = false;

	void Start () {
		transform.GetChild(0).gameObject.SetActive(false);

		GetComponentInParent<VRTK_ControllerEvents>().ButtonTwoPressed += DoButtonTwoPressed;
	}

	private void OnDestroy() {
		if (GetComponentInParent<VRTK_ControllerEvents>() == null) return;
		GetComponentInParent<VRTK_ControllerEvents>().ButtonTwoPressed -= DoButtonTwoPressed;
	}

	private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e) {
		_showOptions = !_showOptions;

		

		if (_showOptions) {
			SwitchMode.OptionLock = true;
			_originalPointerState = GameObject.Find("RightController").GetComponent<VRTK_Pointer>().enabled;
			GameObject.Find("RightController").GetComponent<VRTK_UIPointer>().enabled = true;
			SwitchMode.Instance.ReleaseCurrentTool();

			_originalPointerOrigin = SwitchMode.IsHeadsetPointer();
			if (SwitchMode.IsHeadsetPointer()) SwitchMode.Instance.TogglePointerOrigin();
			SwitchMode.EnablePointer();
			_originalColorpickerState = SwitchMode.IsColorPickerActive();
			SwitchMode.DisableColorPicker();
		} else {
			GameObject.Find("RightController").GetComponent<VRTK_UIPointer>().enabled = false;
			if (!_originalPointerState) SwitchMode.DisablePointer();
			SwitchMode.Instance.RestoreLastTool();
			SwitchMode.OptionLock = false;

			if (_originalPointerOrigin) SwitchMode.Instance.TogglePointerOrigin();
			if (_originalColorpickerState) SwitchMode.EnableColorPicker();
		}

		transform.GetChild(0).gameObject.SetActive(_showOptions);
		
	}

}
