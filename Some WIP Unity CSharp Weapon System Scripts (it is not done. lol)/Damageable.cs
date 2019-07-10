using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour {

    [SerializeField] private int health = 100;
    [SerializeField] private int obliterationThreshold = -10;
    [SerializeField] private bool obliterateable = true;
    [SerializeField] private bool acceptDamge = true;
    [SerializeField] private UnityEvent OnHealthDecreased;
    [SerializeField] private UnityEvent OnDestroyed;
    [SerializeField] private UnityEvent OnHealthIncreased;
    [SerializeField] private UnityEvent OnObliterate;

    public int Health {
        set {
            health = value;
            DecrementHelth(0);
        }
        get {
            return health;
        }
    }
    
    public void DecrementHelth (int damage) {
        health -= damage;
        OnHealthDecreased.Invoke();
        EvaluateIfDestroyed();
    }

    public void IncrementHelth (int _health) {
        health += _health;
        OnHealthIncreased.Invoke();
    }

    private void EvaluateIfDestroyed () {
        if (health <= 0 && health > obliterationThreshold) {
            OnDestroyed.Invoke();
        }
        else {
            if (health <= obliterationThreshold) {
                OnObliterate.Invoke();
            }
        }
    }

}
