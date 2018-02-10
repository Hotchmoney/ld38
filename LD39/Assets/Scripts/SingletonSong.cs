using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonSong : MonoBehaviour {

    public static SingletonSong i = null;

	// Use this for initialization
	void Awake () {
		if (i != null)
        {
            Destroy(this.gameObject);
            return;
        }else
        {
            i = this;
        }
	}
}
