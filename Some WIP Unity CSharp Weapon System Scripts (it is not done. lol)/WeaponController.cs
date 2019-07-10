using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct WeaponParts {
    public string name;
    public Transform muzzle;
    public Transform weaponBody;
}

public enum NextFirePositionDeterminationMode {
    Random,
    RoundRobin,
    Simultaneous
}

public class WeaponController : MonoBehaviour {

    [SerializeField] private Weapon weaponData;
    [SerializeField] private NextFirePositionDeterminationMode nextFirePositionDeterminationMode = NextFirePositionDeterminationMode.RoundRobin;
    [SerializeField] private WeaponParts[] weaponParts = new WeaponParts[1];
    [SerializeField] private float multiwieldInterval = 0.1f;
    public UnityEvent OnWeaponFired;
    public UnityEvent OnBeganWeaponReload;
    public UnityEvent OnEndWeaponReload;
    public UnityEvent OnAmmoFinished;
    public UnityEvent OnAmmoDecreased;
    public UnityEvent OnAmmoIncreased;
    protected WaitForSeconds burstfireIntervalWait;
    protected Weapon weapon;
    protected WaitForSeconds multiweildIntervalYield;

    public WeaponParts[] WeaponParts {
        get {
            return weaponParts;
        }
    }

    public Weapon Weapon {
        get {
            return weapon;
        }
    }

    public NextFirePositionDeterminationMode NextFirePositionDeterminationMode {
        get {
            return nextFirePositionDeterminationMode;
        }
    }

    private void Awake() {
        weapon = Instantiate(weaponData);
        weapon.Initialize(gameObject);
        multiweildIntervalYield = new WaitForSeconds(multiwieldInterval);
        burstfireIntervalWait = new WaitForSeconds(weapon.BurstFireSettings.burstFireInterval);
    }

}
