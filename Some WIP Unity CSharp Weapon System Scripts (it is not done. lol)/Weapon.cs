using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct BurstFireSettings {
    [Range(0, 10)]
    public uint burstFireAmount;
    public float burstFireInterval;

    public BurstFireSettings(uint burst, float interval) {
        burstFireAmount = burst;
        burstFireInterval = interval;
    }
}

public enum RoundLoadingMode {
    Automatic,
    SemiAutomatic,
    Burst
}

public enum RecoilMode {
    Procedural,
    Animation,
    None
}

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Weapon", order = 0)]
public class Weapon : ScriptableObject {

    [Header("Weapon Details")]
    [SerializeField] private string weaponName = "Weapon Name";
    [SerializeField] private int weaponIndex = 0;
    [SerializeField] [TextArea] private string weaponDescription = "Moderately detailed description of the weapon goes here.";
    [Header("Firing")]
    [SerializeField] private RoundLoadingMode roundLoadingType = RoundLoadingMode.Automatic;
    [SerializeField] [Range(0, 5)] private float fireRate = 0.3f;
    [SerializeField] private BurstFireSettings burstFireSettings = new BurstFireSettings(3, 0.07f);
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private AudioClip outOfAmmoSound;
    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 500;
    [SerializeField] protected int magazineSize = 30;
    [SerializeField] private bool decrementAmmo = true;
    [Header("Reloading")]
    [SerializeField] private float reloadTime = 3f;
    [SerializeField] private AudioClip reloadSound;
    [Header("Damage")]
    [SerializeField] protected int maxDamage = 20;
    [SerializeField] private float damageReductionFactor = 0.1f;
    [Header("Impacts")]
    [SerializeField] private Vector3 impactPrefabOffset = new Vector3(0, 0, 0);
    [SerializeField] private float impactForce = 50f;
    [SerializeField] private ForceMode impactForceMode = ForceMode.Impulse;
    [Header("Enabling")]
    [SerializeField] private AudioClip enableAudioClip;

    protected bool outOfAmmo = false;
    protected bool reloading = false;
    protected GameObject gameObject;
    protected WeaponController weaponController;
    protected Transform transform;

    private int totalRemainingAmmo;
    private int magazineRemainingAmmo;
    private WaitForSeconds reloadWait;
    private RoundLoadingMode tempRoundLoadingType;
    private string[] impactSurfaces = {
        "Metal",
        "Wood",
        "Stone",
        "Plastic",
        "Sand",
        "Flesh",
        "Water",
        "Glass",
        "LiquidFilled",
        "Explosions",
        "Special"
    };

    public float FireRate {
        get {
            return fireRate;
        }
    }

    public BurstFireSettings BurstFireSettings {
        get {
            return burstFireSettings;
        }
    }

    public int MagazineRemainingAmmo {
        get {
            return magazineRemainingAmmo;
        }
    }

    public bool Reloading {
        get {
            return reloading;
        }
    }

    public int TotalRemainingAmmo {
        get {
            return totalRemainingAmmo;
        }
    }

    public bool OutOfAmmo {
        get {
            return outOfAmmo;
        }
        set {
            outOfAmmo = value;
        }
    }

    public int  MaxDamage {
        get {
            return maxDamage;
        }
    }

    public int MagazineSize {
        get {
            return magazineSize;
        }
    }

    public RoundLoadingMode RoundLoadingMode {
        get {
            return roundLoadingType;
        }
    }

    public virtual void Initialize(GameObject weaponObject) {
        gameObject = weaponObject;
        totalRemainingAmmo = maxAmmo;
        magazineRemainingAmmo = magazineSize;
        gameObject.name = weaponName;
        transform = gameObject.transform;
        outOfAmmo = false;
        reloading = false;
        weaponController = gameObject.GetComponent<WeaponController>();
        reloadWait = new WaitForSeconds(reloadTime);
    }

    public virtual void Fire() {
        if (!reloading) {
            if (totalRemainingAmmo > 0) {
                if (magazineRemainingAmmo > 0) {
                    weaponController.OnWeaponFired.Invoke();
                }

                if (decrementAmmo) {
                    DecrementAmmo(1);
                }
                else {
                    DecrementAmmo(0);
                }
            }
            else {
                outOfAmmo = true;
                weaponController.OnAmmoFinished.Invoke();
            }
        }
    }

    private IEnumerator ReloadCoroutine() {
        reloading = true;
        int refillAmount = magazineSize - magazineRemainingAmmo;
        magazineRemainingAmmo = magazineRemainingAmmo += refillAmount;

        weaponController.OnBeganWeaponReload.Invoke();

        yield return reloadWait;

        reloading = false;
        weaponController.OnEndWeaponReload.Invoke();
    }

    private void DecrementAmmo(int _decrementAmount) {
        if (totalRemainingAmmo <= magazineSize) {
            magazineRemainingAmmo = totalRemainingAmmo;
        }

        totalRemainingAmmo -= _decrementAmount;
        magazineRemainingAmmo -= _decrementAmount;

        weaponController.OnAmmoDecreased.Invoke();
    }

    public void IncrementAmmo(int _incrementAmount) {
        totalRemainingAmmo += _incrementAmount;
        weaponController.OnAmmoIncreased.Invoke();
    }

    public void DealDamage(GameObject target, int _damageAmount) {
        if (target.GetComponentInParent<Damageable>()) {
            Damageable damageTarget = target.GetComponentInParent<Damageable>();

            damageTarget.DecrementHelth(_damageAmount);
        }
    }

    /*protected virtual void SpawnImpact(Transform hitObject, RaycastHit hit, GameObject impact) {
        //GameObject spawnedDecal = Instantiate(impact, hit.point + impactPrefabOffset, Quaternion.LookRotation(hit.normal));
        GameObject spawnedDecal = InstantiateRelevantDecal(hitObject.tag, hit, impactPrefabs.useMultipleImpactTypes);
        Renderer spawnedDecalRenderer;
        Renderer hitObjectRenderer;

        if (impactPrefabs.useMultipleImpactTypes) {
            spawnedDecalRenderer = spawnedDecal.GetComponent<DecalMaterialReference>().referenceMaterial.GetComponent<Renderer>();
            hitObjectRenderer = hitObject.GetComponent<Renderer>();


            if (hitObject.GetComponent<Renderer>()) {
                //Colours:
                if (spawnedDecalRenderer.material.HasProperty("_Color") && hitObjectRenderer.material.HasProperty("_Color")) {
                    spawnedDecalRenderer.material.SetColor("_Color", hitObjectRenderer.material.GetColor("_Color"));
                }
                if (spawnedDecalRenderer.material.HasProperty("_TintColor") && hitObjectRenderer.material.HasProperty("_TintColor")) {
                    spawnedDecalRenderer.material.SetColor("_TintColor", hitObjectRenderer.material.GetColor("_TintColor"));
                }

                //Floats
                if (spawnedDecalRenderer.material.HasProperty("_Glossiness") && hitObjectRenderer.material.HasProperty("_Glossiness")) {
                    spawnedDecalRenderer.material.SetFloat("_Glossiness", hitObjectRenderer.material.GetFloat("_Glossiness"));
                }
                if (spawnedDecalRenderer.material.HasProperty("_Metallic") && hitObjectRenderer.material.HasProperty("_Metallic")) {
                    spawnedDecalRenderer.material.SetFloat("_Metallic", hitObjectRenderer.material.GetFloat("_Metallic"));
                }
            }

        }
        else {
            spawnedDecalRenderer = null;
            hitObjectRenderer = null;
        }

        if (spawnedDecal)
            spawnedDecal.transform.SetParent(hitObject.transform);
    }*/

    protected virtual void SpawnImpact(Transform hitObject, RaycastHit hit, GameObject[] impactType, int impactIndex) {
        GameObject spawnedDecal = Instantiate(impactType[impactIndex], hit.point + impactPrefabOffset, Quaternion.LookRotation(hit.normal));
        spawnedDecal.transform.SetParent(hitObject.transform);
    }

    public virtual void SpawnImpact(Transform hitObject, Vector3 position, GameObject impact, bool setParent) {
        GameObject spawnedDecal = Instantiate(impact, position, Quaternion.LookRotation(position.normalized));

        if (setParent) {
            spawnedDecal.transform.SetParent(hitObject.transform);
        }
        else {
            return;
        }
    }

    /*protected void DetermineSpawnedImpact(Transform hitObject, RaycastHit hit, GameObject[] impactParticles, int impactIndex) {
        for (int i = 0; i < impactSurfaces.Length; i++) {
            if (hitObject.CompareTag(impactSurfaces[i])) {
                SpawnImpact(hitObject, hit, impactParticles[impactIndex]);
            }
            else {
                return;
            }
        }
    }*/

}