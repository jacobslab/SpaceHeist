using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Diagnostics;
public class AudioSyncPulser : MonoBehaviour
{

	float syncPulseInterval = 10f;
	public DataPrint dataprint;
	public int pulseCount = 0;

	public bool ShouldSyncPulse = true;
	public AudioSource audio_mp3;

	// Use this for initialization
	void Start()
	{
		pulseCount = 0;
		audio_mp3.Stop();
		StartCoroutine("RunSyncPulseManual");

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void StopSyncPulsing()
	{
		StopCoroutine("RunSyncPulseManual");
	}

	IEnumerator RunSyncPulseManual()
	{
		UnityEngine.Debug.Log("SyncPulser: RunsyncPulseManual: AudioSyncPulse");
		float jitterMin = 2f;
		float jitterMax = 4f;

		Stopwatch executionStopwatch = new Stopwatch();

		while (ShouldSyncPulse)
		{
			executionStopwatch.Reset();
			//	UnityEngine.Debug.Log ("pulse running");

			float jitter = UnityEngine.Random.Range(jitterMin, jitterMax);//syncPulseInterval - syncPulseDuration);
			yield return StartCoroutine(WaitForShortTime(jitter));

			ToggleLEDOn();

			float timeToWait = (syncPulseInterval) - jitter;

			yield return StartCoroutine(WaitForShortTime(timeToWait));

			executionStopwatch.Stop();
		}
		yield return null;
	}

	void ToggleLEDOn()
	{
		UnityEngine.Debug.Log("SyncPulser: RunsyncPulseManual: AudioSyncPulseSent");
		pulseCount++;

		if (Config.shouldAudioControlOn)
			audio_mp3.Play();

		try
		{
			dataprint.sendttlPulse(1, 10);
			Experiment.Instance.shopLiftLog.LogAudioSyncSent(pulseCount);
		}
		catch (Exception e)
		{
			Experiment.Instance.shopLiftLog.LogAudioSyncException(pulseCount, e.Message);
		}
		
	}


	IEnumerator WaitForShortTime(float jitter)
	{
		float currentTime = 0.0f;
		while (currentTime < jitter)
		{
			currentTime += Time.deltaTime;
			yield return 0;
		}
		yield return null;
	}
}
