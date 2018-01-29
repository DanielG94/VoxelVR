using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Debug class to lock the mouse cursor within the unityeditor, not needed
public class LockMouse : MonoBehaviour {

	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
	

	void Update () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
