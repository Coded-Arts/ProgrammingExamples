using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct LevelManagerEvents {
    public UnityEvent OnMainSceneLoaded;
    public UnityEvent OnDesktopPlatformLoaded;
    public UnityEvent OnMobilePlatformLoaded;
    public UnityEvent OnEscapePressed;
    public UnityEvent OnEnvironmentLoaded;
    public UnityEvent OnLevelReady;
    public UnityEvent OnBeginLevel;
}

public class LevelManager : MonoBehaviour {

    [SerializeField] private KeyCode debugBeginLevelKeyCode = KeyCode.F1;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private bool loadEnvironmentLevel = true;
    [SerializeField] private string environmentLevelName = "InteriorEnvironment01";
    [SerializeField] private LevelManagerEvents levelManagerEvents;
    [SerializeField] private bool startOnLoad = true;
    //[SerializeField] private UnityEvent OnLevelLoaded;
    public UnityEvent OnBeginLevel;

    private VideoManager videoManager;
    private Toggler[] togglers;
    private Scene environmentScene;
    private bool environmentSceneLoaded = false;
    private bool restarting = false;    
    private bool started = false;
    private bool levelReady = false;
    private bool levelBegan = false;
    private bool readied = false;

    public LevelManagerEvents LevelManagerEvents {
        get {
            return levelManagerEvents;
        }
    }

    private void Awake() {
        #region External Environment Loading
        //Additively loads a specified environment level if required. 
        //This will function as the environment for a given level if the level shares
        //an environment with others.
        if (loadEnvironmentLevel) {
            LoadSceneParameters loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive);
            environmentScene = SceneManager.LoadScene(environmentLevelName, loadSceneParameters);
        }
        #endregion

        togglers = GameObject.FindObjectsOfType<Toggler>();

        #region Platform Detection 
        //Determines the platforms 
#if UNITY_EDITOR || UNITY_STANDALONE
        levelManagerEvents.OnDesktopPlatformLoaded.Invoke();
#elif UNITY_ANDROID || UNITY_IOS
        levelManagerEvents.OnMobilePlatformLoaded.Invoke();
#endif
        #endregion

    }

    private void Start() {
        #region Old Level Loading
        //if (videoManager) {
        //    if (videoManager.hasVideoLoadingScreen) {
        //        OnLevelLoaded.AddListener(videoManager.ToggleSkipButton);
        //        videoManager.OnVideoEnded.AddListener(BeginLevel);
        //    }
        //    else {
        //        if (loadEnvironmentLevel || restarting || PlayerPrefs.HasKey("Restarting School Flood")) {
        //            StartCoroutine(EnvironmentSceneLoadingCheckCoroutine());
        //        }
        //    }
        //}
        //else {
        //    if (PlayerPrefs.HasKey("Restarting School Flood")) {
        //        StartCoroutine(EnvironmentSceneLoadingCheckCoroutine());
        //    }
        //}

        ////restarting = PlayerPrefs.HasKey("Restarting School Flood");//

        //OnLevelLoaded.Invoke();

        //if (!loadEnvironmentLevel) {
        //    if (videoManager) {
        //        Destroy(videoManager.gameObject);
        //    }
        //    BeginLevel();
        //}
        #endregion
        levelManagerEvents.OnMainSceneLoaded.Invoke();
    }

    private void Update() {
#if UNITY_EDITOR
        if (Input.GetKeyDown(debugBeginLevelKeyCode)) {
            BeginLevel();
        }
#endif
        if (Input.GetKeyDown(KeyCode.Escape)) {
            levelManagerEvents.OnEscapePressed.Invoke();
        }

        if (loadEnvironmentLevel) {
            if (!environmentSceneLoaded) {
                if (environmentScene.isLoaded) {
                    levelManagerEvents.OnEnvironmentLoaded.Invoke();
                    environmentSceneLoaded = true;
                    levelReady = true;
                }
            }
        }
        else {
            levelManagerEvents.OnEnvironmentLoaded.Invoke();
            environmentSceneLoaded = true;
            levelReady = true;
        }

        if (environmentSceneLoaded && levelReady && !readied) {
            levelManagerEvents.OnLevelReady.AddListener(ToggleSkipButton);
            levelManagerEvents.OnLevelReady.Invoke();
            readied = true;
        }

        if (startOnLoad && environmentSceneLoaded && levelReady && !levelBegan) {
            BeginLevel();
        }

        if (videoManager) {
            if (!videoManager.hasVideoLoadingScreen) {
                if (loadEnvironmentLevel) {
                    if (environmentSceneLoaded && !started) {
                        BeginLevel();
                        Destroy(videoManager.gameObject);
                        started = true;
                    }
                }
            }
        }
        else {
            if (PlayerPrefs.HasKey("Restarting School Flood")) {
                if (loadEnvironmentLevel) {
                    if (environmentSceneLoaded && !started) {
                        BeginLevel();
                        if (videoManager) {
                            Destroy(videoManager.gameObject);
                        }
                        started = true;
                    }
                }
            }
        }
    }

    public void BeginLevel() {
        OnBeginLevel.Invoke();
        levelManagerEvents.OnBeginLevel.Invoke();
        levelBegan = true;
    }

    public void SaveStarRating (int starCount) {
        string sceneName = SceneManager.GetActiveScene().name;
        int currentHighestStars = PlayerPrefs.GetInt(sceneName);

        if (starCount > currentHighestStars) {
            PlayerPrefs.SetInt(sceneName, starCount);
        }
    }

    public void ToggleClickInteractions () {
        for (int i = 0; i < togglers.Length; i++) {
            togglers[i].Toggle();
        }
    }

    private IEnumerator EnvironmentSceneLoadingCheckCoroutine () {
        while (!environmentSceneLoaded) {
            if (environmentScene.isLoaded) {
                environmentSceneLoaded = true;
            }
            yield return null;
        }
    }

    public void  Restart () {
        restarting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy() {
        if (restarting) {
            PlayerPrefs.SetInt("Restarting School Flood", 1);
            Debug.Log("Writing");
        }
        else {
            PlayerPrefs.DeleteKey("Restarting School Flood");
            Debug.Log("Not Writing");
        }
    }

    public void ToggleSkipButton() {
        if (PlayerPrefs.GetInt(string.Concat(SceneManager.GetActiveScene().name, ProgressDataWriter.loadedLevelSuffix)) > 0) {
            skipButton.SetActive(true);
        }
        else {
            skipButton.SetActive(false);
        }
    }

    public void AddFunctionToSkipButton () {
        GameObject.FindWithTag("FlyThroughTriggerEnder")?.GetComponent<InteractionVolume>().OnEnteredInteractableRange.Invoke();
    }

}
