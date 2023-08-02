using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Diagnostics;
public class NatusAudio : MonoBehaviour
{

	public int position = 0;
	public int samplerate = 44100;
	public float frequency = 440;

	float syncPulseInterval = 10f;
	float syncPulseDuration = 0.05f;

	AudioClip myClip_on;
	AudioClip myClip_off;
	public AudioSource aud_on;
	public AudioSource aud_off;

	bool audio_on_off = false;

	public static bool ShouldSyncPulse = true;

	// Use this for initialization
	void Start()
	{
		myClip_on = AudioClip.Create("Digital1", 10* samplerate, 1, samplerate, true, OnAudioRead_on, OnAudioSetPosition);
		position = 0;
		myClip_off = AudioClip.Create("Digital2", 100 * samplerate, 1, samplerate, true, OnAudioRead_off, OnAudioSetPosition);
		position = 0;
		aud_on.clip = myClip_on;
		aud_on.Stop();
		aud_off.clip = myClip_off;
		aud_off.Stop();

		StartCoroutine("RunSyncPulseManual");
	}

	void OnAudioRead_on(float[] data)
	{
		int count = 0;
		while (count < data.Length)
		{
			data[count] = 1f;
			position++;
			count++;
		}
	}

	void OnAudioRead_off(float[] data)
	{
		int count = 0;
		while (count < data.Length)
		{
			data[count] = 0f;
			position++;
			count++;
		}
	}

	void OnAudioSetPosition(int newPosition)
	{
		position = newPosition;
	}

	// Update is called once per frame
	void Update()
	{
		if (ShouldSyncPulse == true)
		{
			if (audio_on_off == false)
			{
				ToggleNatusAudioOff();
			}
			else
			{
				ToggleNatusAudioOn();
			}
		}
		else {
			StopCoroutine("RunSyncPulseManual");
		}
	}

	public void StopSyncPulsing()
	{
		StopCoroutine("RunSyncPulseManual");
	}

	IEnumerator RunSyncPulseManual()
	{
		UnityEngine.Debug.Log("SyncPulser: RunsyncPulseManual: SyncBox");
		float jitterMin = 2f;
		float jitterMax = 4f;

		Stopwatch executionStopwatch = new Stopwatch();

		while (ShouldSyncPulse)
		{
			executionStopwatch.Reset();
			//	UnityEngine.Debug.Log ("pulse running");

			float jitter = UnityEngine.Random.Range(jitterMin, jitterMax);//syncPulseInterval - syncPulseDuration);
			yield return StartCoroutine(WaitForShortTime(jitter));

			aud_off.Stop();
			audio_on_off = true;
			aud_on.Play();
			/*StopCoroutine(ToggleLEDOff());
			StartCoroutine(ToggleLEDOn());*/
			yield return StartCoroutine(WaitForShortTime(syncPulseDuration));
			aud_on.Stop();
			audio_on_off = false;
			aud_off.Play();
			/*StopCoroutine(ToggleLEDOn());
			StartCoroutine(ToggleLEDOff());*/

			float timeToWait = (syncPulseInterval - syncPulseDuration) - jitter;
			if (timeToWait < 0)
			{
				timeToWait = 0;
			}

			UnityEngine.Debug.Log("NatusAudio: On: timeToWait: " + timeToWait);
			yield return StartCoroutine(WaitForShortTime(timeToWait));

			executionStopwatch.Stop();
			UnityEngine.Debug.Log("NatusAudio: On: Finally Success: ");
			
		}
		yield return null;
	}

	 void ToggleNatusAudioOn()
	{
		UnityEngine.Debug.Log("NatusAudio: On: Sending 1");
		//aud_on.
		//aud_on.Play();
		Experiment.Instance.shopLiftLog.LogNatusAudioOn();
	}

	void ToggleNatusAudioOff()
	{
		UnityEngine.Debug.Log("NatusAudio: On: Sending 0");
		//aud_off.Play();
		Experiment.Instance.shopLiftLog.LogNatusAudioOff();
		//yield return null;
	}

	IEnumerator WaitForShortTime(float jitter)
	{
		float currentTime = 0.0f;
		while (currentTime < jitter)
		{
			currentTime += Time.deltaTime;
			UnityEngine.Debug.Log("NatusAudio: On: currentTime: " + currentTime + " jitter: " + jitter);
			yield return 0;
		}
		yield return null;
	}
}
