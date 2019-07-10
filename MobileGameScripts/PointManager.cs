using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class PointManager : MonoBehaviour {

    [SerializeField] private int totalNumberOfStars = 5;
    [SerializeField] private TextMeshProUGUI toCollectText;
    [SerializeField] private UnityEvent OnMaxItemsCollected;
    [SerializeField] private Image[] starImages = new Image[5];
    [SerializeField] private RatingRanges ratingRanges = new RatingRanges(30, 60, 90, 120, 180);
    public UnityEvent OnLevelComplete;
    public bool isCollectionTypeLevel = true;
    [HideInInspector] public int maxPossiblePoints;
    public static Interactable[] interactibles;
    public int CurrentPoints { get; set; }

    private static TextMeshProUGUI textMeshPro;
    private int numberOfCollectedInteractables;

    private void Awake() {
        //maxPossiblePoints = FindObjectsOfType<Interactable>().Length;
        interactibles = FindObjectsOfType<Interactable>();
        for (int i = 0; i < interactibles.Length; i++) {
            //interactibles[i].OnCollected.AddListener(IncrementCollectedInteractables);
            maxPossiblePoints++;
            UpdateText();
        }

        textMeshPro = toCollectText;
    }

    public void IncrementPoints(int i) {
        CurrentPoints += i;

        if (CurrentPoints >= maxPossiblePoints) {
            if (isCollectionTypeLevel)
            {
                OnLevelComplete.Invoke();
            }
        }
    }

    //Calculates the number of stars the player has gotten in a level.
    private float CalculateRating(float maxPossiblePoints, float pointsCollected, int starCount) {
        return (pointsCollected / maxPossiblePoints) * starCount;
    }

    private int CalculateRating (int _time) {
        if (_time <= ratingRanges.fiveStars) {
            return 5;
        }
        else {
            if (_time > ratingRanges.fiveStars && _time <= ratingRanges.fourStars) {
                return 4;
            }
            else {
                if (_time > ratingRanges.fourStars && _time <= ratingRanges.threeStars) {
                    return 3;
                }
                else {
                    if (_time > ratingRanges.threeStars && _time <= ratingRanges.twoStars) {
                        return 2;
                    }
                    else {
                        return 1;
                    }
                }
            }
        }
    }

    public void CalculateRating () {
        //SetStars();
        SetStarsBasic();
    }

    private void IncrementCollectedInteractables (bool collected) {
        if (collected) {
            numberOfCollectedInteractables++;
        }
        else {
            numberOfCollectedInteractables--;
        }
        
        if (numberOfCollectedInteractables >= maxPossiblePoints) {
            OnMaxItemsCollected.Invoke();
            for (int i = 0; i < interactibles.Length; i++) {
                //interactibles[i].Triggerable = false;
            }
        }
        UpdateText();
    }

    public void UpdateText () {
        toCollectText.text = string.Concat(numberOfCollectedInteractables, "/", maxPossiblePoints);
    }

    private void SetStars () {
        float rating = CalculateRating(maxPossiblePoints, CurrentPoints, totalNumberOfStars);
        float remainder = (rating % 1);

        for (int i = 0; i < rating - remainder; i++) {
            starImages[i].fillAmount = 1;
            if (i == (rating - remainder) - 1) {
                if (remainder > 0) {
                    starImages[i + 1].fillAmount = remainder;
                }
            }
        }
    }

    private void SetStarsBasic () {
        int rating = CalculateRating(ElapsedTimer.currentTrueTime);
        
        for (int i = 0; i < rating; i++) {
            starImages[i].enabled = true;
        }
    }

    public void CompleteLevel ()
    {
        CurrentPoints = maxPossiblePoints;
        OnLevelComplete.Invoke();
    }

}