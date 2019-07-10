using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnFireEnumTriggeredEvent : UnityEvent<int> { }

public class UserWeaponController : WeaponController {

    [SerializeField] private OnFireEnumTriggeredEvent OnFireEnumTriggered;
    private float nextFire = 0.5f;
    private float referenceTime = 0f;

    private void Update() {
        if (!weapon.Reloading) {
            if (weapon.RoundLoadingMode == RoundLoadingMode.Automatic) {
                referenceTime = referenceTime + Time.deltaTime;

                if (Input.GetButton("Fire1") && referenceTime > nextFire) {
                    StartCoroutine(FireEnumeration());

                }
            }
            if (weapon.RoundLoadingMode == RoundLoadingMode.Burst) {
                StartCoroutine(BurstFire());
            }
            else {
                if (weapon.RoundLoadingMode == RoundLoadingMode.SemiAutomatic) {
                    if (Input.GetButtonDown("Fire1")) {
                        weapon.Fire();
                    }
                }
            }
        }
    }

    private IEnumerator BurstFire() {
        referenceTime = referenceTime + Time.deltaTime;

        if (weapon) {
            if (Input.GetButton("Fire1") && referenceTime > nextFire) {
                for (int i = 0; i < weapon.BurstFireSettings.burstFireAmount; i++) {
                    nextFire = referenceTime + weapon.FireRate;
                    if (!weapon.Reloading) {
                        weapon.Fire();
                    }
                    nextFire = nextFire - referenceTime;
                    referenceTime = 0.0f;
                    yield return burstfireIntervalWait;
                }
            }
        }
    }

    private IEnumerator FireEnumeration () {
        for (int i = 0; i < WeaponParts.Length; i++) {
            OnFireEnumTriggered.Invoke(i);
            nextFire = referenceTime + weapon.FireRate;
            weapon.Fire();
            nextFire = nextFire - referenceTime;
            referenceTime = 0.0f;
            yield return multiweildIntervalYield;
        }
    }


}
