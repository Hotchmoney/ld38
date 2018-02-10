using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer {
	float startTime = 0;        // time when timer was started
	//float waitTime = 0;         // seconds constituting one iteration
	float timePaused = 0;  // time when the timer was paused

	public void Start()   {
		startTime = Time.time;
		timePaused = -1;
	}
	public void Restart() {
		startTime = Time.time;
		timePaused = -1;
	}

	public void Pause()   {
		//record pause time
		timePaused = Time.time;
	}
	public void UnPause() {
		// skip the time during which timer was paused
		startTime += (Time.time - timePaused);
		timePaused = -1;
	}


	//total running time
	public float time {
		get {
			if (timePaused != -1) {
				return startTime - timePaused;
			} else {
				return Time.time - startTime;
			}
		}
	}


	// public int iteration {
	// 	get { return (int) (time / waitTime); }
	// }
}
