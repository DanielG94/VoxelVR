using UnityEngine;
using System.Collections;

//Script by https://answers.unity.com/questions/177611/show-error-when-running-in-application.html
public class Console : MonoBehaviour {
	private bool _enabled = false;
	float height = 550f;
	static private string text;
	Vector2 scrollPosition = new Vector2(0, 0);
	private void Awake() {
		Application.logMessageReceived += Add;
	}

	void OnGUI() {
		if (!_enabled) return;
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(height));
		GUILayout.TextArea(text, GUILayout.MinHeight(height));
		GUILayout.EndScrollView();
	}

	static public void Add(string line, string stackTrace, LogType type) {
		//if (type != LogType.Error) return;
		text += type + ": " + line + "\n";
	}

	private void Update() {
		if (Input.GetKeyUp(KeyCode.F1))
			_enabled = !_enabled;
	}

}