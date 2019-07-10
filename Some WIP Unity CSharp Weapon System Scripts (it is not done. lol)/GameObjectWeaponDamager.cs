using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ProjectileMovementMode {
    StraightLine,
    ContinuousUpdate
}

public class GameObjectWeaponDamager : MonoBehaviour {

    [SerializeField] private int damage = 20;
    [SerializeField] private float movementSpeed = 50f;
    [SerializeField] private ProjectileMovementMode projectileMovementMode = ProjectileMovementMode.StraightLine;
    [SerializeField] private ForceMode forceMode;
    [SerializeField] private bool monitorTimeSinceStart;
    [SerializeField] private int timeToTriggerDisabling = 3;
    [SerializeField] private UnityEvent OnCollisionBegan;
    [SerializeField] private UnityEvent OnTriggerBegan;
    [SerializeField] private UnityEvent OnTimerEnd;
    [SerializeField] private UnityEvent OnProjectileEnabled;

    new private Rigidbody rigidbody;
    private int instantiationCount;
    private Weapon weapon;
    private WeaponController weaponController;
    private int time = 10;
    private int timeAtStrt;
    private int disableTimeAtStrt;

    public Weapon Weapon {
        set {
            weapon = value;
        }
    }

    public int Damage {
        set {
            damage = value;
        }
    }

    public float MovementSpeed {
        set {
            movementSpeed = value;
        }
        get {
            return movementSpeed;
        }
    }

    public int Time {
        set {
            time = value;
        }
        get {
            return time;
        }
    }

    private void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        timeAtStrt = time;
        //disableTimeAtStrt = timeToTriggerDisabling;
    }

    private void OnEnable() {
        if (monitorTimeSinceStart) {
            instantiationCount++;
            if (instantiationCount > 1) {
                InvokeRepeating("DecrementTime", 0, 1);
            }
        }
    }

    private void OnDisable() {
        CancelInvoke("DecrementTime");
    }

    private void OnCollisionEnter(Collision collision) {
        //InvokeRepeating("DecrementDisableTime", 0, 1);
        OnCollisionBegan.Invoke();
    }

    private void OnTriggerEnter(Collider other) {
        OnTriggerBegan.Invoke();
    }

    private void DecrementTime () {
        time -= 1;

        if (time <= 0) {
            OnTimerEnd.Invoke();
            time = timeAtStrt;
            CancelInvoke("DecrementTime");
        }
    }

    private void DecrementDisableTime () {
        timeToTriggerDisabling -= 1;

        if (timeToTriggerDisabling <= 0) {
            time = disableTimeAtStrt;
            gameObject.SetActive(false);
            CancelInvoke("DecrementDisableTime");
        }
    }

    public void EnableProjectile () {
        OnProjectileEnabled.Invoke();
    }

}