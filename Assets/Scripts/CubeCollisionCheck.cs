using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCollisionCheck : MonoBehaviour {

	private bool _collision = false;


	private void OnTriggerExit(Collider other) {
		_collision = false;
		//Debug.Log("Collision exit: " + _collision);
	}

	private void OnTriggerStay(Collider other) {
		if (other.gameObject.CompareTag("Cube"))
			_collision = true;

		//Debug.Log("Collision: " + _collision);
	}

	public bool isColliding() {
		return _collision;
	}
}
