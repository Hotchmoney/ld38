using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryOnLoad : MonoBehaviour {

	void Start() {
		bool dontDestroy = true;

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Music")) {
			if (obj.scene.name == "DontDestroyOnLoad") {
				// kill yourself
				Destroy(gameObject);
				dontDestroy = false;
			}
		}

		if (dontDestroy) {
			DontDestroyOnLoad(gameObject);
			GetComponent<AudioSource>().Play();
		}
	}
}
