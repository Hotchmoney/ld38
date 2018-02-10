using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

[RequireComponent (typeof (Rigidbody2D))]
public class Player : MonoBehaviour {
	public AudioClip jumpSFX;
	public AudioClip kickSFX;
	public GameObject restartPrompt;
    public int jumpPower = 10;
    public float speed = 1;
    public float maxSpeedNormal = 3.0f;
    public float maxSpeedCrouched = 3.7f;
    public bool debugTools;
	//[HideInInspector]
	public List<GameObject> cats = new List<GameObject>();
    Vector2 prevVelocity;

	Vector2 collisionNormal; //averaged normal for all collisions this frame
	int numCollisions; //number of collisions this frame
	bool wasGrounded; //true if grounded last frame
	bool jumped; //true if jump pressed last frame
	bool canKick; //kick can only be used once
	public bool grounded;
	float maxSpeed;
	Timer groundTimer;
    public bool debugwin;
    public bool haswon = false;
	
	ForceGravityAttractor[] planets;
	public Vector2 forcesApplied;
    public Rigidbody2D rb2d;
	AudioSource audioSource;
    GameObject CounterUI;

	void Awake () {
        Init();
	}

    void Start()
    {
        CounterUI = GameObject.Find("NumCatsCount");
        debugwin = false;
    }

    void OnEnable()
    {
        Init();
    }

    public void Init()
    {
		audioSource = GetComponent<AudioSource>();
        planets = FindObjectsOfType<ForceGravityAttractor>();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 0;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        prevVelocity = Vector2.zero;
		numCollisions = 0;
		collisionNormal = Vector2.zero;
        jumped = false;
        wasGrounded = false;
        grounded = false;
        canKick = true;

        maxSpeed = maxSpeedNormal;
        groundTimer = new Timer();
        restartPrompt.SetActive(false);
    }


	
	public void Update () {

        foreach (Follow f in FindObjectsOfType<Follow>())
        {
            if (f.enabled == true)
            {
                if (!cats.Contains(f.gameObject))
                {
                    cats.Add(f.gameObject);
                }
            }
        }

        if (debugwin)
        {
            foreach (GameObject c in FindObjectsOfType<GameObject>())
            {
                if (c.GetComponent<Cat>() != null)
                {
                    c.GetComponent<Cat>().OnTriggerEnter2D(this.GetComponent<Collider2D>());
                }
            }
        }


        if (CounterUI == null)
        {
            CounterUI = GameObject.Find("NumCatsCount");
        }

        int floorNumCat = Mathf.Clamp(cats.Count, 0, 10);
        CounterUI.GetComponent<Text>().text = floorNumCat+"/10";

        if (!haswon && cats.Count >= 10)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


		handleRestartPrompt();

		//space or up to jump
		bool jumpKeyDown = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow);

		if (((grounded || wasGrounded) && jumpKeyDown) || (grounded && jumped))
        {
			audioSource.clip = jumpSFX;
			audioSource.Play();
            ForceGravityAttractor closest = findClosestPlanet();
            Vector2 ForceToAdd = jumpPower * -closest.attractForce(rb2d);
            if(ForceToAdd.magnitude < 500)
            {
                ForceToAdd = ForceToAdd.normalized * 650;
            }
			rb2d.AddForce(ForceToAdd);
            grounded = false;
			canKick = true;
			jumped = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }

		// if not grounded and jump pressed this frame, don't jump.
		// jump next frame if grounded is true
		jumped = !grounded && jumpKeyDown;

		//shift or down to crouch
		if (grounded && (Input.GetKey(KeyCode.LeftShift))) {
            maxSpeed = maxSpeedCrouched;
		}
		else
		{
            maxSpeed = maxSpeedNormal;
        }

		if (!grounded)
        {
			//one big kick on press per jump
			if (canKick && Input.GetKeyDown(KeyCode.DownArrow)) {
                GetComponent<ParticleSystem>().Play();

				audioSource.clip = kickSFX;
				audioSource.Play();

				kick(ForceMode2D.Impulse);
			} else if (Input.GetKey(KeyCode.DownArrow)) {
				kick(ForceMode2D.Force);
			}
		}

        fixVelocity();
        
    }

    void fixVelocity()
    {
        Vector2 curVelocity = rb2d.velocity;
        Vector2 difference = curVelocity - prevVelocity;
        rb2d.velocity = curVelocity + 2*difference.normalized * Time.deltaTime;

        if (rb2d.velocity == Vector2.zero)
        {
            transform.Translate(transform.up * 0.75f);
            rb2d.velocity = (transform.right * maxSpeed);
        }
    }

	public void kick(ForceMode2D forceMode) {
        ForceGravityAttractor closest = findClosestPlanet();
		Vector2 closestPlanetPos = closest.transform.position;
		Vector2 diveDir = rb2d.position - closestPlanetPos;
		rb2d.AddForce(-diveDir.normalized * jumpPower * closest.attractForce(rb2d).magnitude / 5.0f, forceMode);

		canKick = false;
	}

    private void FixedUpdate()
    {
        // sum of all plant's gravity
        forcesApplied = new Vector2(0, 0);

		foreach (ForceGravityAttractor planet in planets)
        {
            forcesApplied += planet.attractForce(rb2d);
        }

        rb2d.AddForce(forcesApplied);

		// face transform.down towards sum of all forces
		/*
		transform.up = -forcesApplied;*/
		//proper normal rotation function
		if (numCollisions == 0) {
			RotatePlayerToGround();
		} else {
			transform.up = collisionNormal;
		}
        // draw original velocity
        if (debugTools)
        {
            Debug.DrawLine(rb2d.position, rb2d.position + rb2d.velocity, Color.blue);
        }

        // bring velocity into local space
        Vector2 localVelocity = transform.InverseTransformDirection(rb2d.velocity);

		if (localVelocity.x != maxSpeed)
        {
            Vector2 newDirVector = new Vector2(maxSpeed - localVelocity.x, 0);
			rb2d.AddRelativeForce(newDirVector.normalized*speed, ForceMode2D.Impulse);

			// draw correction vector
			if (debugTools)
            {
                Debug.DrawLine(rb2d.position, rb2d.position + (Vector2)(transform.right * (maxSpeed - localVelocity.x)));
            }
        }

        // draw resulting velocity
        if (debugTools)
        {
            Debug.DrawLine(rb2d.position, rb2d.position + rb2d.velocity, Color.red);
        }

		//reset for next frame
		collisionNormal = Vector2.zero;
		numCollisions = 0;
    }


	//collisions
    void OnCollisionEnter2D(Collision2D collision)
    {
		collide(collision);
        if (collision.gameObject.name.Contains("cat") && !collision.gameObject.name.Contains("planet"))
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }
    }
	void OnCollisionStay2D(Collision2D collision)
	{
		collide(collision);
	}
    void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
		canKick = true;
    }
    void collide(Collision2D collision)
	{
		//collided this frame
		numCollisions++;

		//get average collision normal
		Vector2 normal = Vector2.zero;
		foreach (ContactPoint2D contact in collision.contacts) {
			normal += contact.normal;
		}
		normal /= collision.contacts.Length;

		collisionNormal += normal;

		if (debugTools) {
			Debug.DrawLine(transform.position, transform.position + (Vector3)normal, new Color(1, 0, 1));
		}

		//set grounded
		groundTimer.Restart();
		grounded = true;
	}


	// ---------------- HELPER FUNCTIONS ---------------

	void handleRestartPrompt() {
		// show restart prompt if timeGrounded > 5 seconds
		restartPrompt.SetActive(groundTimer.time > 5);

		//if R pressed
		if (Input.GetKeyDown(KeyCode.R)) {
			// restart
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

    }

	public ForceGravityAttractor findClosestPlanet() {
		float minDist = Mathf.Infinity;
		ForceGravityAttractor closest = null;

		foreach (ForceGravityAttractor planet in planets) {
			float dist = Vector2.Distance(planet.transform.position, rb2d.transform.position);

			if (dist < minDist) {
				minDist = dist;
				closest = planet;
			}
		}

		return closest;
	}

    void RotatePlayerToGround()
    {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 0.5f, ~LayerMask.GetMask("Player"));

		if (debugTools) {
			Debug.DrawLine(transform.position, transform.position - transform.up * 0.5f, Color.yellow);
		}

		if (hit.collider != null) {
			grounded = true;
			transform.up = hit.normal;
		} else {
			transform.up = -forcesApplied;
			grounded = false;
		}
    }



    void OnApplicationQuit()
    {
        int numCats = cats.Count();
        Analytics.CustomEvent("ApplicationExit", new Dictionary<string, object> { { "Exit Time", Time.realtimeSinceStartup }, { "Number of Cats", numCats } });
    }
}
