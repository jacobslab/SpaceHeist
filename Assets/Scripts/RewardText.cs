﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardText : MonoBehaviour {

	private Text text;
	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowScore(int score)
	{
		if (score < 0) {
			text.text = "-$" + score.ToString ();
		} else {
			text.text = "$" + score.ToString ();
		}
	}
}

