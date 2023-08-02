using UnityEngine;
using System.Collections;
using System.IO;

public class Experiment : MonoBehaviour {

    ExperimentSettings expSettings { get { return ExperimentSettings.Instance; } }
    
    //logging
    private string subjectLogfile; //gets set based on the current subject in Awake()
	public Logger_Threading subjectLog;
	//public Logger_Threading_New subjectLogNew;
	private string eegLogfile; //gets set based on the current subject in Awake()
	//public Logger_Threading eegLog;
	public string sessionDirectory;
	public static string sessionStartedFileName = "sessionStarted.txt";
	public static int sessionID;

	//avatar
	public Player player;

	public ShoplifterScript shopLift;


	//logging
	public ShopliftLogTrack shopLiftLog;

	//state enum
	public ExperimentState currentState = ExperimentState.instructionsState;

	//checkpoint
	public static bool shouldCheckpoint=false;
	public string[] checkpointData;
	public int checkpointedEnvIndex = 0;
	public string checkpointedPhaseName = "NONE";
	public int checkpointedReevalIndex = 0;
	public string leftRoomName="";
	public int leftReward=0;
	public string rightRoomName="";
	public int rightReward=0;
	public static bool loadFromCheckpoint = false;


	public SubjectSelectionController subjectController;


	private string wordsLogged="";
	public enum ExperimentState
	{
		instructionsState,
		inExperiment,
		inExperimentOver,
	}

	//bools for whether we have started the state coroutines
	bool isRunningInstructions = false;
	bool isRunningExperiment = false;


	//EXPERIMENT IS A SINGLETON
	private static Experiment _instance;

	public static Experiment Instance{
		get{
			return _instance;
		}
	}

	void Awake(){
        Cursor.visible = false;
		if (_instance != null) {
			Debug.Log("Instance already exists!");
			return;
		}
		_instance = this;

	//	cameraController.SetInGame();

		if (ExperimentSettings.isLogging) {
			InitLogging();
		}
//		else if(ExperimentSettings.isReplay) {
//			instructionsController.TurnOffInstructions();
//		}

	}


	//HANDLES LOGGING AND CHECKS FOR CHECKPOINT FILE
	void InitLogging(){
		string subjectDirectory = ExperimentSettings.defaultLoggingPath + ExperimentSettings.currentSubject.name + "/";
		sessionDirectory = subjectDirectory + "session_0" + "/";
        string checkpointFilePath = "";
        string chosenCheckpointFile = "";
        sessionID = 0;
		string sessionIDString = "_0";
		
		if(!Directory.Exists(subjectDirectory)){
			Directory.CreateDirectory(subjectDirectory);
		}
		Debug.Log ("does " + sessionDirectory + "and" + sessionStartedFileName + " exist");

		//check for directory
        while (File.Exists(sessionDirectory + sessionStartedFileName))
        {
            sessionIDString = "_" + sessionID.ToString();
            sessionDirectory = subjectDirectory + "session" + sessionIDString + "/";
            checkpointFilePath = sessionDirectory + "checkpoint.txt";

			//checking for checkpoint file
            if(File.Exists(checkpointFilePath))
            {
                chosenCheckpointFile = checkpointFilePath;
                Debug.Log("chosen checkpoint file sess id " + sessionID.ToString());
            }
            sessionID++;
        }
            //check if the session crashed
            if (loadFromCheckpoint) {
            Debug.Log("sanity check for " + chosenCheckpointFile);
				checkpointData = new string[8];
            if (File.Exists (chosenCheckpointFile)) {
					string checkpointText = File.ReadAllText (chosenCheckpointFile);
					if (checkpointText.Contains ("ONGOING")) {
						Debug.Log ("previous session crashed; use checkpoint details to resume that session");
						checkpointData = checkpointText.Split ("\t" [0]);
						shouldCheckpoint = true;
					}
					for (int i = 0; i < checkpointData.Length; i++) {
						Debug.Log (checkpointData [i]);
					}
					UpdateCheckpointedVariables (checkpointData);

				}
			}

		Debug.Log ("current session is: " + sessionID.ToString ());


		//delete old files.
		if(Directory.Exists(sessionDirectory)){
			DirectoryInfo info = new DirectoryInfo(sessionDirectory);
			FileInfo[] fileInfo = info.GetFiles();
			for(int i = 0; i < fileInfo.Length; i++){
				File.Delete(fileInfo[i].ToString());
			}
		}
		else{ //if directory didn't exist, make it!
			Directory.CreateDirectory(sessionDirectory);
		}
		
		subjectLog.fileName = sessionDirectory + ExperimentSettings.currentSubject.name + "Log" + ".txt";
		subjectLog.fileNameNew = sessionDirectory + ExperimentSettings.currentSubject.name + "LogNew" + ".txt";
		subjectLog.fileNameeeg = sessionDirectory + ExperimentSettings.currentSubject.name + "EEGLog" + ".txt";
		subjectLog.fileNamenatus = sessionDirectory + ExperimentSettings.currentSubject.name + "NatusAudioLog" + ".txt";
		//eegLog.fileName = sessionDirectory + ExperimentSettings.currentSubject.name + "EEGLog1" + ".txt";
		//eegLog.fileNameNew = sessionDirectory + ExperimentSettings.currentSubject.name + "EEGLogNew2" + ".txt";
		//eegLog.fileNameeeg = sessionDirectory + ExperimentSettings.currentSubject.name + "EEGLogNew3" + ".txt";
	}

	//In order to increment the session, this file must be present. Otherwise, the session has not actually started.
	//This accounts for when we don't successfully connect to hardware -- wouldn't want new session folders.
	//Gets created in ShoplifterScript when the task is run.
	public void CreateSessionStartedFile(){
		StreamWriter newSR = new StreamWriter (sessionDirectory + sessionStartedFileName);
	}


	//upon loading checkpoint file, update variables accordingly to move the task to the checkpointed state
	public void UpdateCheckpointedVariables(string[] checkpointData)
	{
		Debug.Log ("checkpoint " + checkpointData [3].ToString ());
		checkpointedEnvIndex = int.Parse(checkpointData [1]);
		checkpointedPhaseName = checkpointData [2];
		checkpointedReevalIndex = int.Parse(checkpointData [3]);
		leftRoomName = checkpointData [4];
		leftReward = int.Parse (checkpointData [5]);
		rightRoomName = checkpointData [6];
		rightReward = int.Parse (checkpointData [7]);
        expSettings.stage = ExperimentSettings.Stage.None;

        switch (checkpointedPhaseName) {
            case "PRE-TRAINING":
                expSettings.stage = ExperimentSettings.Stage.Pretraining;
                break;
		case "TRAINING":
              expSettings.stage = ExperimentSettings.Stage.Training;
			//no action needed, assuming all the flags point to true, as they do by default
			break;
		case "LEARNING":
                expSettings.stage = ExperimentSettings.Stage.Learning;
			break;
		case "REEVALUATION":
                expSettings.stage = ExperimentSettings.Stage.Reevaluation;
			break;
		case "TESTING":
                expSettings.stage = ExperimentSettings.Stage.Test;
			break;
		case "POST-TEST":
                expSettings.stage = ExperimentSettings.Stage.PostTest;
			break;
        case "MUSIC_BASELINE":
                expSettings.stage = ExperimentSettings.Stage.Baseline;
                break;
        case "IMAGE_BASELINE":
                expSettings.stage = ExperimentSettings.Stage.Baseline;
                break;
        case "SILENT_TRAVERSAL":
                expSettings.stage = ExperimentSettings.Stage.Baseline;
                break;
		case "NONE":
                expSettings.stage = ExperimentSettings.Stage.None;
                break;
        default:
                expSettings.stage = ExperimentSettings.Stage.None;
			    break;
		}
	}

	public IEnumerator RunInstructions(){
		isRunningInstructions = true;

		//IF THERE ARE ANY PRELIMINARY INSTRUCTIONS YOU WANT TO SHOW BEFORE THE EXPERIMENT STARTS, YOU COULD PUT THEM HERE...

		currentState = ExperimentState.inExperiment;
		isRunningInstructions = false;

		yield return 0;

	}



	public void EndExperiment(){
		Debug.Log ("Experiment Over");
		currentState = ExperimentState.inExperimentOver;
		isRunningExperiment = false;
	}

	//TODO: move to instructions controller...
	public IEnumerator ShowSingleInstruction(string line, bool isDark, bool waitForButton, bool addRandomPostJitter, float minDisplayTimeSeconds){
		if(isDark){
	//		instructionsController.SetInstructionsColorful();
		}
		else{
	//		instructionsController.SetInstructionsTransparentOverlay();
		}
	//	instructionsController.DisplayText(line);

		yield return new WaitForSeconds (minDisplayTimeSeconds);

		if (waitForButton) {
			yield return StartCoroutine (WaitForActionButton ());
		}

		if (addRandomPostJitter) {
			yield return StartCoroutine(WaitForJitter ( Config.randomJitterMin, Config.randomJitterMax ) );
		}

	//	instructionsController.TurnOffInstructions ();
	}
	
	public IEnumerator WaitForActionButton(){
		bool hasPressedButton = false;
		while(Input.GetAxis("Action Button") != 0f && Experiment.Instance.shopLift.isGamePaused == false)
		{
			yield return 0;
		}
		while(!hasPressedButton){
			if(Input.GetAxis("Action Button") == 1.0f && Experiment.Instance.shopLift.isGamePaused == false)
			{
				hasPressedButton = true;
			}
			yield return 0;
		}
	}

	public IEnumerator WaitForJitter(float minJitter, float maxJitter){
		float randomJitter = Random.Range(minJitter, maxJitter);
		
		float currentTime = 0.0f;
		while (currentTime < randomJitter) {
			currentTime += Time.deltaTime;
			yield return 0;
		}

	}


	public void OnExit(){ //call in scene controller when switching to another scene!
		if (ExperimentSettings.isLogging) {
			subjectLog.close ();
			//eegLog.close ();
			//subjectLogNew.close();
		}
	}

	public void OnExperimentEnd(){
		if (ExperimentSettings.isLogging) {
			subjectLog.close ();
			//eegLog.close ();
			//subjectLogNew.close();
			Application.Quit ();
		}
	}

	void OnApplicationQuit()
	{
#if UNITY_STANDALONE_OSX
			File.Copy ("/Users/" + System.Environment.UserName + "/Library/Logs/Unity/Player.log", sessionDirectory+"Player.log");
#endif
		subjectLog.close ();
		//subjectLogNew.close();
		//eegLog.close ();

	}


}
