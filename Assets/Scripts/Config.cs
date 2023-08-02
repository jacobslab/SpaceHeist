/*using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;*/
using UnityEngine;
using System.Collections;


public class Config : MonoBehaviour
{

    public enum Version
    {
        SpaceHeist
    }

    public enum SessionType
    {
        Adaptive,
        NonAdaptive
    }

    public static SessionType sessionType = SessionType.NonAdaptive;
    public static Version BuildVersion = Version.SpaceHeist; //TODO: change this for your experiment!
    public static string VersionNumber = "2.3"; //TODO: change this for your experiment!

    public static bool isGamified = false;


    public static bool isSystem2 = false;
    public static bool isSyncbox = false;
    public static bool isSystem3 = false;
    public static bool SyncBoxDisconnect = false;
#if DAY_THREE
    public static bool isDayThree=true;
#else
    public static bool isDayThree = false;

#endif
    public static bool shouldForceControl = false;
    public static bool shouldSkipInstructions = false;
    public static bool shouldAudioControlOn = false;

    //recall
    public static int recallTime = 30;
    public static int ffrTime = 300;
    //REPLAY
    public static int replayPadding = 6;

    //SOUNDTRACK
    public static bool isSoundtrack = false;

    //trial variables
    public static int numTestTrials = 25; //IF 50% 2 OBJ, [1obj, counter1, 2a, counter2a, 2b, counter2b, 3, counter3] --> MULTIPLE OF EIGHT
    public static Vector2 trialBlockDistribution = new Vector2(4, 4); //4 2-item trials, 4 3-item trials

    //practice settings
    public static int numTrialsPract = 1;
    public static bool doPracticeTrial = false;


    //JITTER
    public static float randomJitterMin = 0.0f;
    public static float randomJitterMax = 0.2f;


    public static float micLoudnessThreshold = 0.2f;

    //instructions
    public static float minInitialInstructionsTime = 0.0f; //TODO: change back to 5.0f

    //adaptive
    public static float minPercent = 0.3f;
    public static float maxPercent = 0.5f;

    void Awake()
    {
#if !GAMIFIED
        isGamified = false;
#else
		isGamified=true;
#endif
        DontDestroyOnLoad(transform.gameObject);
    }

    void Start()
    {

    }

    public static int GetTotalNumTrials()
    {
        if (!doPracticeTrial)
        {
            return numTestTrials;
        }
        else
        {
            return numTestTrials + numTrialsPract;
        }
    }

    /*public static object LoadFromText(string json)
    {
        JObject cfg = JObject.Parse(json);
        return CastToStatic(cfg);
    }

    private static object GetExperimentConfig()
    {
        if (experimentConfig == null)
        {
#if !UNITY_WEBGL

            UnityEngine.Debug.Log("data path " + Application.dataPath);
            UnityEngine.Debug.Log("one level up " + Directory.GetParent(Application.dataPath).FullName);
            UnityEngine.Debug.Log("another level up " + Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName);
            // Setup config file
            string configPath = System.IO.Path.Combine(
                            Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName,
                            "Configs");
            UnityEngine.Debug.Log("config path " + configPath);
            string text = "";
            text = File.ReadAllText(Path.Combine(configPath, Experiment.ExpName + "_behavioral.json"));

            experimentConfig = FlexibleConfig.LoadFromText(text);
            UnityEngine.Debug.Log("loaded experiment text " + text);
            experimentConfig = FlexibleConfig.LoadFromText(text);
#else
                if (onlineExperimentConfigText == null)
                    Debug.Log("Missing config from web");
                else
                    experimentConfig = FlexibleConfig.LoadFromText(onlineExperimentConfigText);
#endif
        }
        return experimentConfig;
    }*/

}

