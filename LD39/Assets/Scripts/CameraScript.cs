using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public float speedChange = 2.5f;
    public float dampTime = 0.15f;
    public float minDistChange = 6f;

	float regSizeCamera;

	Player player;
	ForceGravityAttractor[] planets;
	new Camera camera;

	void Start () {
        player = FindObjectOfType<Player>();
        planets = FindObjectsOfType<ForceGravityAttractor>();
        camera = Camera.main;
        regSizeCamera = camera.orthographicSize;

		transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -100);
	}

    public void ResetPlayer()
    {
        player = FindObjectOfType<Player>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float min = Mathf.Infinity;
        foreach (ForceGravityAttractor p in planets)
        {
            float dist = Vector2.Distance(p.transform.position, player.transform.position);
            if (dist < min)
            {
                min = dist;
            }
        }
        if (min < minDistChange)
        {
            min = regSizeCamera;
        }
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, min, speedChange * Time.fixedDeltaTime);
        transform.position = Vector3.Lerp(transform.position, player.transform.position + new Vector3(player.rb2d.velocity.normalized.x,player.rb2d.velocity.normalized.y,0)*8, 5 *Time.fixedDeltaTime);
        //transform.rotation = Quaternion.Slerp(transform.rotation, player.transform.rotation, 5 * Time.fixedDeltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -100);
	}
}
