using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class OnRatingEvent : UnityEvent<int> { }

[Serializable]
public class OnEnteredInteractionVolumeEvent : UnityEvent<UnityAction> { }

[Serializable]
public class OnExitInteractionVolumeEvent : UnityEvent<UnityAction> { }

[Serializable]
public struct RatingRanges {
    public int fiveStars, fourStars, threeStars, twoStars, oneStar;

    public RatingRanges(int five, int four, int three, int two, int one) {
        fiveStars = five;
        fourStars = four;
        threeStars = three;
        twoStars = two;
        oneStar = one;
    }
}

public class InteractionManager : MonoBehaviour {

    [SerializeField] private int totalNumberOfStars = 5;
    [SerializeField] private TextMeshProUGUI completedTasksText;
    [SerializeField] private Image[] starImages = new Image[5];
    [SerializeField] private RatingRanges ratingRanges = new RatingRanges(30, 60, 90, 120, 180);
    [SerializeField] private bool allowLevelCompletion = true;

    public UnityEvent OnAllTasksCompleted;
    [SerializeField] private UnityEvent OnTaskCompleted;
    [SerializeField] private UnityEvent OnXAmountOfTasksCompleted;
    [SerializeField] private int numberOfTasksForEvent = 2;
    [SerializeField] private UnityEvent OnEnteredInteractionVolume;
    [SerializeField] private UnityEvent OnStayInteractionVolume;
    [SerializeField] private UnityEvent OnExitInteractionVolume;
    public OnRatingEvent OnRating;

    private int totalTasks;
    private Interactable[] interactibles;
    private InteractionVolume[] interactionVolumes;

    public int CurrentCompletedTasksCount { get; set; }

    private void Awake() {
        interactibles = FindObjectsOfType<Interactable>();
        interactionVolumes = FindObjectsOfType<InteractionVolume>();

        for (int i = 0; i < interactibles.Length; i++) {
            totalTasks++;
            UpdateCompletedTasksText();
            interactibles[i].OnInteracted.AddListener(IncrementInteractions);
        }

        for (int i = 0; i < interactionVolumes.Length; i++) {
            interactionVolumes[i].OnEnteredInteractableRange.AddListener(EnteredInteractionSphere);
            interactionVolumes[i].OnStayInteractableRange.AddListener(StayInteractionSphere);
            interactionVolumes[i].OnExitInteractableRange.AddListener(ExitedInteractionSphere);
        }
    }

    private void UpdateCompletedTasksText() {
        completedTasksText.text = string.Concat(CurrentCompletedTasksCount, "/", totalTasks);
    }

    private int DetermineRating(int _time) {
        if (_time <= ratingRanges.fiveStars) {
            OnRating.Invoke(5);
            return 5;
        }
        else {
            if (_time > ratingRanges.fiveStars && _time <= ratingRanges.fourStars) {
                OnRating.Invoke(4);
                return 4;
            }
            else {
                if (_time > ratingRanges.fourStars && _time <= ratingRanges.threeStars) {
                    OnRating.Invoke(3);
                    return 3;
                }
                else {
                    if (_time > ratingRanges.threeStars && _time <= ratingRanges.twoStars) {
                        OnRating.Invoke(2);
                        return 2;
                    }
                    else {
                        OnRating.Invoke(1);
                        return 1;
                    }
                }
            }
        }
    }

    private void IncrementInteractions () {
        CurrentCompletedTasksCount++;
        OnTaskCompleted.Invoke();

        if (CurrentCompletedTasksCount == numberOfTasksForEvent) {
            OnXAmountOfTasksCompleted.Invoke();
        }

        if (CurrentCompletedTasksCount >= totalTasks) {
            CompleteLevel();
        }
        UpdateCompletedTasksText();
    }

    public void DisplayStarsImages () {
        int rating = DetermineRating(ElapsedTimer.currentTrueTime);

        for (int i = 0; i < rating; i++) {
            starImages[i].enabled = true;
        }
    }

    public void CompleteLevel () {
        if (allowLevelCompletion) {
            OnAllTasksCompleted.Invoke();
        }
    }

    public void EnteredInteractionSphere () {
        OnEnteredInteractionVolume.Invoke();
    }

    public void StayInteractionSphere () {
        OnStayInteractionVolume.Invoke();
    }

    public void ExitedInteractionSphere () {
        OnExitInteractionVolume.Invoke();
    }

}
