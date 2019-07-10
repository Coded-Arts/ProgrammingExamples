using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

public class VideoManager : MonoBehaviour {

    [SerializeField] private VideoPlayer videoPlayer;
    public UnityEvent OnVideoEnded;
    public UnityEvent OnDestroyed;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private GameObject videoGameObject;
    [SerializeField] private GameObject loadingImageGameObject;

    public bool hasVideoLoadingScreen;

    private void Start() {
        hasVideoLoadingScreen = videoPlayer.clip;

        if (hasVideoLoadingScreen) {
            videoGameObject.SetActive(hasVideoLoadingScreen);
            loadingImageGameObject.SetActive(!hasVideoLoadingScreen);
            StartCoroutine(VideoEnevtsCoroutine((float)videoPlayer.length));
        }
        else {
            videoGameObject.SetActive(hasVideoLoadingScreen);
            loadingImageGameObject.SetActive(!hasVideoLoadingScreen);
        }
    }


    private IEnumerator VideoEnevtsCoroutine(float waitDuration) {
        yield return new WaitForSeconds(waitDuration);
        OnVideoEnded.Invoke();
        Destroy(gameObject);
    }

    //UI Level Loading with the "Skip button".
    public void BeginLevel () {
        StartCoroutine(VideoEnevtsCoroutine(0));
    }

    public void ToggleSkipButton () {
        if (PlayerPrefs.GetInt(string.Concat(MenuManager.levelBegingLoaded, ProgressDataWriter.loadedLevelSuffix)) > 0 && hasVideoLoadingScreen) {
            skipButton.SetActive(true);
        }
        else {
            skipButton.SetActive(false);
        }
    }

}
