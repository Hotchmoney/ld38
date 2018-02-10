using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour {

	AudioSource audioSource;
    Player player;
    IEnumerator coroutine;
    bool launching;
    public int Strength;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
        player = FindObjectOfType<Player>();
        launching = false;
	}


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject == player.gameObject && !launching)
        {
			coroutine = LaunchPlayer();

			player.enabled = false;
            player.rb2d.velocity = Vector2.zero;
            player.transform.position = this.transform.position;
            launching = true;
            StartCoroutine(coroutine);     
        }
    }

    IEnumerator LaunchPlayer()
    {
        yield return new WaitForSeconds(0.3f);

		Vector2 directionMove = transform.up;
        player.rb2d.velocity = directionMove * Strength;

		// cannon sound
		audioSource.Play();

        yield return new WaitForSeconds(0.85f);

		player.enabled = true;
        launching = false;
    }

	void OnDrawGizmos() {
		Gizmos.DrawLine(transform.position, transform.position + transform.up * Strength * 0.85f);
	}
}
