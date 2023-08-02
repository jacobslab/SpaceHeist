using UnityEngine;
using System.Collections;


using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
//using System.Runtime.InteropServices;
using System.Threading;



//CLASS BASED OFF OF: http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html

//SEE THREADEDJOB CLASS


public class LoggerQueue
{

	public Queue<String> logQueue;

	public LoggerQueue(){
		logQueue = new Queue<String> ();
	}

	public void AddToLogQueue(string newLogInfo){
		lock (logQueue) {
			logQueue.Enqueue (newLogInfo);
		}
	}

	public String GetFromLogQueue(){
		string toWrite = "";
		lock (logQueue) {
			toWrite = logQueue.Dequeue ();
			if (toWrite == null) {
				toWrite = "";
			}
		}
		return toWrite;
	}

}

public class LoggerWriter : ThreadedJob
{
	public bool isRunning = false;

	//LOGGING
	protected long microseconds = 1;
	protected string workingFile = "";
	private StreamWriter logfile;
	private LoggerQueue loggerQueue;

	public LoggerWriter(string filename, LoggerQueue newLoggerQueue) {
		workingFile = filename;
		logfile = new StreamWriter ( workingFile, true );

		loggerQueue = newLoggerQueue;
	}

	public LoggerWriter() {

	}

	protected override void ThreadFunction()
	{
		isRunning = true;
		// Do your threaded task. DON'T use the Unity API here
		while (isRunning) {
			while(loggerQueue.logQueue.Count > 0){
				log (loggerQueue.GetFromLogQueue());
			}
		}

		close ();

	}
		
	protected override void OnFinished()
	{
		// This is executed by the Unity main thread when the job is finished

	}

	public void End(){
		isRunning = false;
	}

	public virtual void close()
	{
		//logfile.WriteLine ("EOF");
		logfile.Flush ();
		logfile.Close();	
		Debug.Log ("flushing & closing");
	}


	public virtual void log(string msg) {

		logfile.WriteLine (msg);
	}

}

public class Logger_Threading : MonoBehaviour{
	public static string LogTextSeparator = "\t";

	LoggerQueue myLoggerQueue;
	LoggerQueue myLoggerQueueNew;
	LoggerQueue myLoggerQueueEEG;
	LoggerQueue myLoggerQueueNatus;
	LoggerWriter myLoggerWriter;
	public bool isRunning=false;
	long frameCount;
	public StreamWriter logfile;
	public StreamWriter logfileNew;

	public StreamWriter logfileeeg;
	public StreamWriter logfilenatus;

	public string fileName;
	public string fileNameNew;

	public string fileNameeeg;
	public string fileNamenatus;

	void Start ()
	{
		if (ExperimentSettings.isLogging) {
			myLoggerQueue = new LoggerQueue ();
			myLoggerQueueNew = new LoggerQueue();
			myLoggerQueueEEG = new LoggerQueue();
			myLoggerQueueNatus = new LoggerQueue();
			StartCoroutine ("LogWriter");
			//StartCoroutine("LogWriterNew");
			//			myLoggerWriter = new LoggerWriter (fileName, myLoggerQueue);
			//		
			//			myLoggerWriter.Start ();

			//	myLoggerWriter.log ("DATE: " + DateTime.Now.ToString ("M/d/yyyy")); //might not be needed
		}
	}

	public Logger_Threading(string file){
		UnityEngine.Debug.Log("running Constructor: " + file);
		fileName = file;
	}

	IEnumerator LogWriter()
	{
		isRunning = true;

		logfile = new StreamWriter ( fileName, true,Encoding.ASCII, 0x10000);
		logfileeeg = new StreamWriter(fileNameeeg, true, Encoding.ASCII, 0x10000);
		logfilenatus = new StreamWriter(fileNamenatus, true, Encoding.ASCII, 0x10000);
		UnityEngine.Debug.Log ("running logwriter coroutine writing at " + fileName);
		UnityEngine.Debug.Log("running logwriter coroutine writing at " + fileNameeeg);
		while (isRunning) {

			UnityEngine.Debug.Log("LogWriter Run: Running");
			while ((myLoggerQueue.logQueue.Count > 0) || (myLoggerQueueEEG.logQueue.Count > 0) || (myLoggerQueueNatus.logQueue.Count > 0)) {
				UnityEngine.Debug.Log("LogWriter Run: Running v2");
				string msg1, msg2;

				if (myLoggerQueue.logQueue.Count > 0)
				{
					msg1 = myLoggerQueue.GetFromLogQueue();

					//				UnityEngine.Debug.Log ("writing: " + msg);
					logfile.WriteLine(msg1);
				}

				if (myLoggerQueueEEG.logQueue.Count > 0)
				{
					msg2 = myLoggerQueueEEG.GetFromLogQueue();

					//				UnityEngine.Debug.Log ("writing: " + msg);
					logfileeeg.WriteLine(msg2);
				}

				if (myLoggerQueueNatus.logQueue.Count > 0)
				{
					msg2 = myLoggerQueueNatus.GetFromLogQueue();

					//				UnityEngine.Debug.Log ("writing: " + msg);
					logfilenatus.WriteLine(msg2);
				}

				yield return 0;
			}

			/*while (myLoggerQueueEEG.logQueue.Count > 0)
			{
				string msg = myLoggerQueueEEG.GetFromLogQueue();

				//				UnityEngine.Debug.Log ("writing: " + msg);
				logfileeeg.WriteLine(msg);
				yield return 0;
			}*/

			UnityEngine.Debug.Log("LogWriter FileName: " + fileName + "Queue Length " + myLoggerQueue.logQueue.Count);
			UnityEngine.Debug.Log("LogWriter FileName: " + fileNameeeg + "Queue Length " + myLoggerQueueEEG.logQueue.Count);
			UnityEngine.Debug.Log("LogWriter FileName: " + fileNamenatus + "Queue Length " + myLoggerQueueNatus.logQueue.Count);
			yield return 0;
		}
		UnityEngine.Debug.Log ("closing this");
		yield return null;
	}

	IEnumerator LogWriterNew()
	{
		isRunning = true;

		logfileNew = new StreamWriter(fileNameNew, true, Encoding.ASCII, 0x10000);
		UnityEngine.Debug.Log("running logwriter coroutine writing at New: " + fileNameNew);
		while (isRunning)
		{
			UnityEngine.Debug.Log("running Count New: " + myLoggerQueueNew.logQueue.Count);
			while (myLoggerQueueNew.logQueue.Count > 0)
			{
				string msg = myLoggerQueueNew.GetFromLogQueue();

				UnityEngine.Debug.Log ("writing New: " + msg);

				using (StreamWriter writer = new StreamWriter(fileNameNew, true, Encoding.ASCII))
				{
					writer.Write(msg + "\n");
				}
				yield return 0;
			}

			yield return 0;
		}
		UnityEngine.Debug.Log("closing this");
		yield return null;
	}

	//logging itself can happen in regular update. the rate at which ILoggable objects add to the log Queue should be in FixedUpdate for framerate independence.
	void Update()
	{
		frameCount++;
		//		if (myLoggerWriter != null)
		//		{
		//			if (myLoggerWriter.Update())
		//			{
		//				// Alternative to the OnFinished callback
		//				myLoggerWriter = null;
		//			}
		//		}
	}

	public long GetFrameCount(){
		return frameCount;
	}

	public void Log(long timeLogged,string newLogInfo){
		/*if (myLoggerQueueNew != null) {
			myLoggerQueueNew.AddToLogQueue (timeLogged + LogTextSeparator + newLogInfo);
		}*/
	}

	public void Log(long timeLogged, long frame, string newLogInfo){
		if (fileName == "qwertyyy/session_0/qwertyyyLogNew.txt")
			Debug.Log("I came hereNew");
		if (fileName == "qwertyyy/session_0/qwertyyyLog.txt")
			Debug.Log("I came here");

		/*if (myLoggerQueueNew != null) {
			myLoggerQueueNew.AddToLogQueue (timeLogged + LogTextSeparator + frame + LogTextSeparator + newLogInfo);
		}*/
	}

	public void LogNew(long timeLogged, long frame, string newLogInfo)
	{
		if (fileName == "qwertyyy/session_0/qwertyyyLogNew.txt")
			Debug.Log("I came hereNew");
		if (fileName == "qwertyyy/session_0/qwertyyyLog.txt")
			Debug.Log("I came here");

		if (myLoggerQueue != null)
		{
			myLoggerQueue.AddToLogQueue(timeLogged + LogTextSeparator + frame + LogTextSeparator + newLogInfo);
		}
		
	}


	public void LogNewEEG(long timeLogged, long frame, string newLogInfo)
	{
		if (fileName == "qwertyyy/session_0/qwertyyyLogNew.txt")
			Debug.Log("I came hereNew");
		if (fileName == "qwertyyy/session_0/qwertyyyLog.txt")
			Debug.Log("I came here");

		if (myLoggerQueueEEG != null)
		{
			myLoggerQueueEEG.AddToLogQueue(timeLogged + LogTextSeparator + frame + LogTextSeparator + newLogInfo);
		}

	}

	public void LogNewNatus(long timeLogged, long frame, string newLogInfo)
	{
		if (fileName == "qwertyyy/session_0/qwertyyyLogNew.txt")
			Debug.Log("I came hereNew");
		if (fileName == "qwertyyy/session_0/qwertyyyLog.txt")
			Debug.Log("I came here");

		if (myLoggerQueueNatus != null)
		{
			myLoggerQueueNatus.AddToLogQueue(timeLogged + LogTextSeparator + frame + LogTextSeparator + newLogInfo);
		}

	}

	void OnApplicationQuit()
	{
		isRunning = false;
	}

	//must be called by the experiment class OnApplicationQuit()
	public void close(){
		//Application stopped running -- close() was called
		//applicationIsRunning = false;
//		UnityEngine.Debug.Log("is running will be false");
		logfile.Flush ();
		//logfileNew.Flush();
		logfileeeg.Flush();
		logfilenatus.Flush();
		logfile.Close();
		//logfileNew.Close();
		logfileeeg.Close();
		logfilenatus.Close();

		isRunning =false;
		//		myLoggerWriter.End ();
	}

	public IEnumerator QueueisEmpty() {
		UnityEngine.Debug.Log("This is the Count: " + myLoggerQueue.logQueue.Count);
		UnityEngine.Debug.Log("This is the Count EEG: " + myLoggerQueueEEG.logQueue.Count);
		UnityEngine.Debug.Log("LogWriter FileName:" + fileName + "Queue Length " + myLoggerQueue.logQueue.Count + " Final");

		StopCoroutine("LogWriter");
		UnityEngine.Debug.Log("LogWriter Run: Stopped Running");
		while (myLoggerQueue.logQueue.Count > 0) {
			UnityEngine.Debug.Log("LogWriter Run: Running v3");
			//string msg = 
			logfile.WriteLine(myLoggerQueue.GetFromLogQueue());
			yield return 0;
		}
		yield return null;

	}

	public IEnumerator QueueisEmptyEEG()
	{
		UnityEngine.Debug.Log("This is the Count: " + myLoggerQueue.logQueue.Count);
		UnityEngine.Debug.Log("This is the Count EEG: " + myLoggerQueueEEG.logQueue.Count);
		UnityEngine.Debug.Log("LogWriter FileName:" + fileName + "Queue Length " + myLoggerQueue.logQueue.Count + " Final");

		while (myLoggerQueueEEG.logQueue.Count > 0)
		{
			UnityEngine.Debug.Log("LogWriter Run: Running v4");
			//string msg = 
			logfileeeg.WriteLine(myLoggerQueueEEG.GetFromLogQueue());
			yield return 0;
		}

		yield return null;

	}

	public IEnumerator QueueisEmptyNatus()
	{
		UnityEngine.Debug.Log("This is the Count: " + myLoggerQueue.logQueue.Count);
		UnityEngine.Debug.Log("This is the Count EEG: " + myLoggerQueueEEG.logQueue.Count);
		UnityEngine.Debug.Log("This is the Count Natus: " + myLoggerQueueNatus.logQueue.Count);
		UnityEngine.Debug.Log("LogWriter FileName:" + fileName + "Queue Length " + myLoggerQueue.logQueue.Count + " Final");

		while (myLoggerQueueNatus.logQueue.Count > 0)
		{
			UnityEngine.Debug.Log("LogWriter Run: Running v5");
			//string msg = 
			logfilenatus.WriteLine(myLoggerQueueNatus.GetFromLogQueue());
			yield return 0;
		}

		yield return null;

	}


}