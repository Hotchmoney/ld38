using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class FakeOrbit : MonoBehaviour {
	float degreesPerSecond = 100;
    Player player;
    GameObject creditScreen;
    GameObject titleScreen;

    public bool win;

    void Start()
    {
        win = false;
        player = FindObjectOfType<Player>();
        creditScreen = GameObject.Find("CreditScreen");
        titleScreen = GameObject.Find("Title");
        creditScreen.SetActive(false);
    }

	void Update() {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        foreach(Follow c in FindObjectsOfType<Follow>())
        {
            if (c.enabled == true)
            {
                if (!player.cats.Contains(c.gameObject))
                {
                    player.cats.Add(c.gameObject);
                }
            }
        }

        if (player.cats.Count >= 10 && !player.haswon)
        {
            player.haswon = true;
            Analytics.CustomEvent("WinEvent", new Dictionary<string, object>() { { "Time Taken to Win", Time.realtimeSinceStartup } });
            
            titleScreen.SetActive(false);
            creditScreen.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        

        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

		if (Input.GetKeyDown(KeyCode.DownArrow)) {
            

            creditScreen.SetActive(false);
			titleScreen.SetActive(false);
            // get child, set parent to root
			Player player = transform.GetChild(0).GetComponent<Player>();
			player.transform.SetParent(transform.root);


			player.enabled = true;
            player.Init();
            if (player.cats.Count >= 10)
            {
                player.haswon = true;
            }
            player.Update();
            
            // kill yourself
            Destroy(gameObject);
		}
	}

	void FixedUpdate () {
		transform.Rotate(Vector3.back, degreesPerSecond * Time.fixedDeltaTime);
	}
}
