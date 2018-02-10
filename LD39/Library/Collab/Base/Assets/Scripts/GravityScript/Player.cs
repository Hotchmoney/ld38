using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent (typeof (Rigidbody2D))]
public class Player : MonoBehaviour {
	public ForceGravityAttractor[] planets;
	public GameObject restartPrompt;

    Rigidbody2D rb2d;
    public int jumpPower = 5;
    public int crouchPower = 5;

	void Awake () {
        rb2d = GetComponent<Rigidbody2D>();

        rb2d.gravityScale = 0;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
	}
	
	void Update () {
		// get distance to closest planet
		float min = planets.Min(planet => Vector3.Distance(planet.transform.position, this.transform.position));

		if (min > 100) {
			// show restart prompt
			restartPrompt.SetActive(true);
		}

		//if R pressed
		if (Input.GetKeyDown(KeyCode.R)) {
			// restart
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
    
		//space or up to jump
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) {
        rb2d.AddRelativeForce((rb2d.position - new Vector2(0, 100)).normalized * jumpPower);
		}
    
		//shift or down to crouch
		if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.DownArrow)) {
        rb2d.AddRelativeForce((rb2d.position - new Vector2(100, 0)).normalized * crouchPower);
		}

	}

    private void FixedUpdate()
    {
        foreach(ForceGravityAttractor p in planets)
        {
            p.attract(rb2d);
        }
    }
}
