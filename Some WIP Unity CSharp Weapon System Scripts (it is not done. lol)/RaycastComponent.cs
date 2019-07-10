using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnSingleRayHitEvent : UnityEvent<RaycastHit> { }
[System.Serializable]
public class OnMultiRayHitEvent : UnityEvent<RaycastHit> { }
[System.Serializable]
public class OnRayCastEvent : UnityEvent<Vector3> { }

public class RaycastComponent : MonoBehaviour {

    [SerializeField] private Transform originTraansform;
    [SerializeField] private float rayDistance = 1000f;
    [SerializeField] private bool multiTarget = false;
    [SerializeField] private OnSingleRayHitEvent OnSingleRayHit;
    [SerializeField] private OnMultiRayHitEvent OnMultiRayHit;
    [SerializeField] private OnRayCastEvent OnRayCast;

    private Weapon weapon;
    private Damageable[] damageables;

    private void Start() {
        weapon = GetComponent<WeaponController>().Weapon;
    }

    public void ShootRay () {
        Ray ray = new Ray(originTraansform.position, originTraansform.forward);
        RaycastHit hit;

        OnRayCast.Invoke(originTraansform.position);

        if (Physics.Raycast(ray, out hit, rayDistance)) {
            if (multiTarget) {
                RaycastHit[] hits;

                hits = Physics.RaycastAll(ray);
                for (int index = 0; index < hits.Length; index++) {
                    OnMultiRayHit.Invoke(hits[index]);
                }
            }
            else {
                OnSingleRayHit.Invoke(hit);
            }
        }
        else {

        }
    }

}
