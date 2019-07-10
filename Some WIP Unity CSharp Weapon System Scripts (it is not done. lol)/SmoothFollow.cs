using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DampingMode {
    SmoothDamp,
    LinearInterpolation,
    SphericalInterpolation
}

public class SmoothFollow : MonoBehaviour {

    [SerializeField] private DampingMode DampingMode;
    [SerializeField] private Transform target;
    [SerializeField] private Transform lookTarget;
    [SerializeField] private float smoothness = 1f;

    private Vector3 refVelocity = Vector3.zero;
    new private Transform transform;

    private void Awake() {
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Update() {
        switch (DampingMode) {
            case DampingMode.SmoothDamp:
                transform.position = Vector3.SmoothDamp(transform.position, target.position, ref refVelocity, Time.deltaTime * smoothness);
                break;
            case DampingMode.LinearInterpolation:
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * smoothness);
                break;
            case DampingMode.SphericalInterpolation:
                transform.position = Vector3.Slerp(transform.position, target.position, Time.deltaTime * smoothness);
                break;
            default:
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * smoothness);
                break;
        }
        transform.LookAt(lookTarget);
    }

}
