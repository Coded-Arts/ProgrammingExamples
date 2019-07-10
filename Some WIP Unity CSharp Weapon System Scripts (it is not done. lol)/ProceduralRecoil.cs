using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Procedural Recoil Data")]
public class ProceduralRecoil : ScriptableObject {

    [Header("Recoil")]
    public float recoilAmount = 0.5f;
    public float recoilRecoverTime = 0.2f;
    public float recoilRotationRecoverTime = 0.2f;
    [Range(0, 90)] public float notAimingRandomization = 5f;
    [Range(0, 90)] public float aimingRandomization = 1f;

}
