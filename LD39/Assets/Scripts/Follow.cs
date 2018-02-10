using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {
	public Collider2D target;

	new Collider2D collider;

	// Use this for initialization
	void Start () {
		collider = GetComponent<Collider2D>();
        collider.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (target == null)
        {
            Player p = FindObjectOfType<Player>();
            //if no cats
            if (p.cats.Count == 0)
            {
                //follow player
                target = p.GetComponent<Collider2D>();
            }
            else
            {
                //follow most recently added cat
                for (int i = 1; i < p.cats.Count; i++) {
                    if (this.gameObject.GetInstanceID() == p.cats[p.cats.Count - i].GetInstanceID())
                    {
                        continue;
                    }
                    target = p.cats[p.cats.Count - i].GetComponent<Collider2D>();
                }
                if (target == null)
                {
                    target = p.GetComponent<Collider2D>();
                }
            }
            if (!p.cats.Contains(this.gameObject))
            {
                p.cats.Add(this.gameObject);
            }
        }
		Vector2 distVector = transform.position - target.transform.position;
		transform.position = (Vector2)target.transform.position + distVector.normalized * (target.bounds.extents.magnitude + collider.bounds.extents.magnitude);

		transform.up = Vector3.up;
	}
}
