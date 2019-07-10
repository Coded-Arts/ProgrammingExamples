using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilComponent : MonoBehaviour {

    [SerializeField] private RecoilMode recoilMode = RecoilMode.Procedural;
    [SerializeField] private ProceduralRecoil proceduralRecoilReferenceData;
    [SerializeField] private bool useStartPosition = true;
    [SerializeField] private Vector3 defaultPosition = Vector3.zero;
    [SerializeField] private Vector3 defaultRotation = Vector3.zero;
    [SerializeField] private Transform weaponBody;

    [SerializeField] private Vector3 currentPosition;
    private Vector3 currentRotation;
    private Weapon weapon;
    private float refVelocityXPosition = 0f;
    private float refVelocityXRotation = 0f;
    private float refVelocityYRotation = 0f;
    private ProceduralRecoil proceduralRecoil;

    private void Start() {
        weapon = GetComponent<WeaponController>().Weapon;

        if (recoilMode == RecoilMode.Procedural) {
            proceduralRecoil = Instantiate(proceduralRecoilReferenceData);
        }

        if (useStartPosition)
            defaultPosition = weaponBody.localPosition;
    }

    public void Recoil() {
        if (recoilMode == RecoilMode.Procedural) {
            currentPosition.z -= proceduralRecoil.recoilAmount;
            currentRotation.x -= Random.Range(-proceduralRecoil.aimingRandomization, proceduralRecoil.aimingRandomization);
            currentRotation.y -= Random.Range(-proceduralRecoil.aimingRandomization, proceduralRecoil.aimingRandomization);
        }
    }

    private void LateUpdate() {
        SmoothPositionReset();
    }

    protected virtual void SmoothPositionReset() {
        if (!weapon.Reloading) {
            currentPosition.z = Mathf.SmoothDamp(currentPosition.z, defaultPosition.z, ref refVelocityXPosition, proceduralRecoil.recoilRecoverTime * Time.smoothDeltaTime);

            currentRotation.x = Mathf.SmoothDamp(currentRotation.x, defaultRotation.x, ref refVelocityXRotation, proceduralRecoil.recoilRotationRecoverTime * Time.smoothDeltaTime);
            currentRotation.y = Mathf.SmoothDamp(currentRotation.y, defaultRotation.y, ref refVelocityYRotation, proceduralRecoil.recoilRotationRecoverTime * Time.smoothDeltaTime);
        }

        weaponBody.localPosition = currentPosition;
        weaponBody.localEulerAngles = currentRotation;
    }

}
