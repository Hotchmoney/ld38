using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour {

    private void Awake()
    {
        foreach (GameObject g in FindObjectsOfType<GameObject>())
        {
            if(g.gameObject.GetInstanceID() == this.gameObject.GetInstanceID())
            {
                continue;
            }
            if (g.gameObject.name == this.gameObject.name)
            {
                Destroy(this.gameObject);
            }
        }
    }

    

	public void OnTriggerEnter2D(Collider2D collider) {
		Player player = collider.GetComponent<Player>();


		if (player != null) {
			// meow
			AudioSource audio = GetComponent<AudioSource>();
            audio.pitch = Random.Range(0.7f,2.0f);
			audio.Play();
			
			// disable this script
			enabled = false;

			// follow the player
			Follow followScript = GetComponent<Follow>();
			followScript.enabled = true;

			//if no cats
			if (player.cats.Count == 0) {
				//follow player
				followScript.target = player.GetComponent<Collider2D>();
			} else {
				//follow most recently added cat
				followScript.target = player.cats[player.cats.Count - 1].GetComponent<Collider2D>();
			}
            this.gameObject.transform.parent = null;
            DontDestroyOnLoad(this.gameObject);
            if (!player.cats.Contains(this.gameObject)){
                player.cats.Add(this.gameObject);
            }
            Destroy(this);
		}

	}
}
