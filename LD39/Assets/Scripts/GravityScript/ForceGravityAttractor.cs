using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGravityAttractor : MonoBehaviour {

    public float mass;
    private float G = 1000;
    private float r;

    //Attracts other transform to this body 
    public Vector2 attractForce(Rigidbody2D rb2d)
    {
		Vector2 gravityUp = (rb2d.position - new Vector2(transform.position.x,transform.position.y)).normalized;
        float dist = Vector2.Distance(this.transform.position, rb2d.position);
        dist = dist * dist;
        float force = G * mass / dist;

        gravityUp = gravityUp * -force;

        return gravityUp;
    }

}
