using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutPlanet2 : MonoBehaviour {
	Timer timer = new Timer();
	public GameObject tutorialText;

	void Start() {
	}

	void Update () {

		if (timer.time > 6) {
			tutorialText.SetActive(false);
			enabled = false;
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		Player player = collision.gameObject.GetComponent<Player>();

		if (player != null && !tutorialText.activeInHierarchy) {
			if (player.cats.Count > 0) {
				enabled = false;
			} else if (enabled) {
				tutorialText.SetActive(true);
				timer.Start();
			}
		}
	}
}
