using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
public class SyncboxControl : MonoBehaviour {
    Experiment exp { get { return Experiment.Instance; } }

	[DllImport ("ASimplePlugin.dll")]
	private static extern int OpenUSB();
	[DllImport ("ASimplePlugin.dll")]
	private static extern int CloseUSB();
	[DllImport ("ASimplePlugin.dll")]
	private static extern IntPtr TurnLEDOn();
	[DllImport ("ASimplePlugin.dll")]
	private static extern IntPtr TurnLEDOff();
	[DllImport ("ASimplePlugin.dll")]
	private static extern int CheckUSB ();
    [DllImport("ASimplePlugin.dll")]
    private static extern int AddTwoIntegers(int a, int b);
    public bool ShouldSyncPulse = true;
	public bool DontRun = false;
	public float PulseOnSeconds;
	public float PulseOffSeconds;

	public bool isUSBOpen = false;
	public bool syncboxconnect_sent = false;



	//SINGLETON
	private static SyncboxControl _instance;

	public static SyncboxControl Instance{
		get{
			return _instance;
		}
	}

	void Awake(){

		if (_instance != null) {
			UnityEngine.Debug.Log("Instance already exists!");
			Destroy(transform.gameObject);
			return;
		}
		_instance = this;

	}

	// Use this for initialization
	void Start () {

		if(Config.isSyncbox){
           // LogSyncInfo("LOGSTART");
            UnityEngine.Debug.Log(AddTwoIntegers(4, 5));
			UnityEngine.Debug.Log("SyncboxControl: Start: isSyncBox");
			if (ExperimentSettings.isLogging == true)
			{
				syncboxconnect_sent = true;
				StartCoroutine(ConnectSyncbox());
			}
		}
	}

	IEnumerator ConnectSyncbox(){

		string connectionError = "";
		UnityEngine.Debug.Log("SyncboxControl: ConnectSyncbox: isSyncBox");
		while (!isUSBOpen){
			UnityEngine.Debug.Log ("attempting to connect");
			UnityEngine.Debug.Log("SyncboxControl: ConnectSyncbox: isUSBOpen: attempting to connect");
			LogZero(GameClock.SystemTime_Milliseconds);

			int usbOpenFeedback = OpenUSB();
            
			UnityEngine.Debug.Log("USB Open response: " + usbOpenFeedback.ToString());
			UnityEngine.Debug.Log("SyncboxControl: ConnectSyncbox: isUSBOpen: usbOpenFeedback: " + usbOpenFeedback.ToString());
			if (usbOpenFeedback != 0){
				// LogSyncInfo("USB Connected");
				UnityEngine.Debug.Log("SyncboxControl: ConnectSyncbox: isUSBOpen v2: usbOpenFeedback: " + usbOpenFeedback.ToString());
				LogOne(GameClock.SystemTime_Milliseconds);
				isUSBOpen = true;
			}

			yield return 0;
		}

		if (DontRun == false) /*If it is True, it is Disconnect*/
		{
			ShouldSyncPulse = true;
			//StartCoroutine ("CheckSyncboxConnection");
			StartCoroutine("RunSyncPulseManual");
		}
		yield return null;
	}

    public void LogSyncInfo(string info)
    {
        //using (StreamWriter outputFile = new StreamWriter(Application.dataPath + @"\syncboxInfo.txt"))
        //{
        //    outputFile.WriteLine(info);
        //}
    }

	// Update is called once per frame
	void Update () {
		if (Config.isSyncbox)
		{
			if (syncboxconnect_sent == false && ExperimentSettings.isLogging == true)
			{
				syncboxconnect_sent = true;
				StartCoroutine(ConnectSyncbox());
			}
		}

		if (Config.SyncBoxDisconnect == true)
		{
			Disconnect();
		}
		GetInput ();

		//		if (Input.GetKeyDown (KeyCode.A)) {
		//			int ok = CheckUSB ();
		//
		//			UnityEngine.Debug.Log (ok.ToString());
		//		}
	}

	void GetInput(){
		//use this for debugging if you'd like
	}

	//IEnumerator CheckSyncboxConnection()
	//{
	//	while (ShouldSyncPulse) {
	//		//int syncStatus = CheckUSB ();
 //           //LogSyncInfo("sync status is: " + syncStatus.ToString());
 //         //  UnityEngine.Debug.Log ("sync status is: " + syncStatus.ToString ());
	//		#if FREIBURG
	//		if (syncStatus == 0) {
 //           int syncStatus = CheckUSB ();
 //              // LogSyncInfo("Syncbox connected");
 //              // UnityEngine.Debug.Log ("Syncbox connected");
	//		} 
	//		#else
	//		if (syncStatus == 1) {
	//			UnityEngine.Debug.Log ("Syncbox connected");
	//		} 
	//		#endif
	//		else {
	//			isUSBOpen = false;
 //              // LogSyncInfo("disconnected; initiating reconnection procedure");
 //               //UnityEngine.Debug.Log ("disconnected; initiating reconnection procedure");
	//			StartCoroutine (ReconnectSyncbox ());
	//		}
	//		yield return new WaitForSeconds (2f); //check every 2 seconds
	//		yield return 0;
	//	}
	//	yield return null;
	//}

	IEnumerator ReconnectSyncbox()
	{
		//stop running coroutines
		StopCoroutine ("RunSyncPulseManual");
		StopCoroutine ("CheckSyncboxConnection");

		//close any lingering USB handles
		UnityEngine.Debug.Log(CloseUSB().ToString());

		ShouldSyncPulse = false;

        exp.shopLift.TogglePause (); //pause the game
		//		yield return new WaitForSeconds(1f);
      //  LogSyncInfo("attempting to reconnect");
      //  UnityEngine.Debug.Log ("attempting to reconnect");
		yield return StartCoroutine(ConnectSyncbox());
        exp.shopLift.TogglePause (); //unpause the game
		yield return null;
	}

	float syncPulseDuration = 0.05f;
	float syncPulseInterval = 1.0f;
	/*
		IEnumerator RunSyncPulse(){
			Stopwatch executionStopwatch = new Stopwatch ();

			while (ShouldSyncPulse) {
				executionStopwatch.Reset();

				SyncPulse(); //executes pulse, then waits for the rest of the 1 second interval

				executionStopwatch.Start();
				long syncPulseOnTime = SyncPulse();
				LogSYNCOn(syncPulseOnTime);
				while(executionStopwatch.ElapsedMilliseconds < 1500){
					yield return 0;
				}

				executionStopwatch.Stop();

			}
		}
*/

	//WE'RE USING THIS FUNCTION
	IEnumerator RunSyncPulseManual(){
		UnityEngine.Debug.Log("SyncboxControl: RunSyncPulseManual");

		UnityEngine.Debug.Log("SyncboxControl: RunsyncPulseManual: ");
		float jitterMin = 0.1f;
		float jitterMax = syncPulseInterval - syncPulseDuration;

		Stopwatch executionStopwatch = new Stopwatch ();
		LogTwo(GameClock.SystemTime_Milliseconds);
		while (ShouldSyncPulse) {
			UnityEngine.Debug.Log("SyncboxControl: RunSyncPulseManual: ShouldSyncPulse");

			LogThree(GameClock.SystemTime_Milliseconds);
			executionStopwatch.Reset();
		//	UnityEngine.Debug.Log ("pulse running");

			float jitter = UnityEngine.Random.Range(jitterMin, jitterMax);//syncPulseInterval - syncPulseDuration);
			yield return StartCoroutine(WaitForShortTime(jitter));

			ToggleLEDOn();
			yield return StartCoroutine(WaitForShortTime(syncPulseDuration));
			ToggleLEDOff();

			float timeToWait = (syncPulseInterval - syncPulseDuration) - jitter;
			if(timeToWait < 0){
				timeToWait = 0;
			}

			yield return StartCoroutine(WaitForShortTime(timeToWait));

			executionStopwatch.Stop();
		}
	}
	public void Disconnect() {
		if (Config.isSyncbox)
		{
			
			ShouldSyncPulse = false;
			DontRun = true;
			isUSBOpen = true;
			StopCoroutine("RunSyncPulseManual");
			
			
		}
	}

	//return microseconds it took to turn on LED
	void ToggleLEDOn(){
		UnityEngine.Debug.Log("SyncboxControl: RunsyncPulseManual: ToggleLEDOn");
		TurnLEDOn ();
		LogSYNCOn (GameClock.SystemTime_Milliseconds);
	}

	void ToggleLEDOff(){
		UnityEngine.Debug.Log("SyncboxControl: RunsyncPulseManual: ToggleLEDOff");
		TurnLEDOff();
		LogSYNCOff (GameClock.SystemTime_Milliseconds);

	}

	long GetMicroseconds(long ticks){
		long microseconds = ticks / (TimeSpan.TicksPerMillisecond / 1000);
		return microseconds;
	}

	IEnumerator WaitForShortTime(float jitter){
		float currentTime = 0.0f;
		while (currentTime < jitter) {
			currentTime += Time.deltaTime;
			yield return 0;
		}

	}

	void LogSYNCOn(long time){
        if (ExperimentSettings.isLogging) {
			exp.subjectLog.LogNewEEG (time, exp.subjectLog.GetFrameCount(), "ON"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
		}
	}

	void LogSYNCOff(long time){
        if (ExperimentSettings.isLogging) {
			exp.subjectLog.LogNewEEG(time, exp.subjectLog.GetFrameCount(), "OFF"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
		}
	}

	void LogZero(long time)
	{
		if (ExperimentSettings.isLogging)
		{
			exp.subjectLog.LogNewEEG(time, exp.subjectLog.GetFrameCount(), "Zero"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
		}
	}

	void LogOne(long time)
	{
		if (ExperimentSettings.isLogging)
		{
			exp.subjectLog.LogNewEEG(time, exp.subjectLog.GetFrameCount(), "One"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
		}
	}

	void LogTwo(long time)
	{
		if (ExperimentSettings.isLogging)
		{
			exp.subjectLog.LogNewEEG(time, exp.subjectLog.GetFrameCount(), "Two"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
		}
	}

	void LogThree(long time)
	{
		if (ExperimentSettings.isLogging)
		{
			exp.subjectLog.LogNewEEG(time, exp.subjectLog.GetFrameCount(), "Three"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
		}
	}



	void LogSYNCStarted(long time, float duration){
        if (ExperimentSettings.isLogging) {
			exp.subjectLog.LogNewEEG(time, exp.subjectLog.GetFrameCount (), "SYNC PULSE STARTED" + Logger_Threading.LogTextSeparator + duration);
		}
	}

	void LogSYNCPulseInfo(long time, float timeBeforePulseSeconds){
        if (ExperimentSettings.isLogging) {
			exp.subjectLog.LogNewEEG(time, exp.subjectLog.GetFrameCount (), "SYNC PULSE INFO" + Logger_Threading.LogTextSeparator + timeBeforePulseSeconds*1000); //log milliseconds
		}
	}

	void OnApplicationQuit(){
		UnityEngine.Debug.Log(CloseUSB().ToString());
	}

}
