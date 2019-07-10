using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Impacts {
    public GameObject[] dirt, flesh, glass, liquid, liquidFilled, metal, plastic, sand, stone, wildcard, wood;
}

public class ImpactManager : MonoBehaviour {

    [SerializeField] private Impacts impacts;

    private GameObject SpawnImpact(string _impactTag, Vector3 position, Vector3 rotation) {
        GameObject impact;

        switch (_impactTag) {
            case ImpactTagManager.dirtImpactTag:
                impact = Instantiate(impacts.dirt[Random.Range(0, impacts.dirt.Length)].gameObject, position, rotation);
                break;
            case ImpactTagManager.fleshImpactTag:
                impact = Instantiate(impacts.flesh[Random.Range(0, impacts.flesh.Length)].gameObject, position, rotation);
                break;
            case ImpactTagManager.glassImpactTag:
                impact = Instantiate(impacts.glass[Random.Range(0, impacts.glass.Length)].gameObject, position, rotation);
                break;
            case ImpactTagManager.liquidImpactTag:
                impact = Instantiate(impacts.liquid[Random.Range(0, impacts.liquid.Length)].gameObject, position, rotation);
                break;
            case ImpactTagManager.liquidFilledImpactTag:
                impact = Instantiate(impacts.liquidFilled[Random.Range(0, impacts.liquidFilled.Length)].gameObject, position, rotation);
                break;
            case ImpactTagManager.metalImpactTag:
                impact = Instantiate(impacts.metal[Random.Range(0, impacts.metal.Length)].gameObject, position, rotation);
                break;
            case ImpactTagManager.plasticImpactTag:
                impact = Instantiate(impacts.plastic[Random.Range(0, impacts.plastic.Length)].gameObject, position, rotation);
                break;
            case ImpactTagManager.sandImpactTag:
                impact = Instantiate(impacts.sand[Random.Range(0, impacts.sand.Length)].gameObject, position, rotation);
                break;
            case ImpactTagManager.stoneImpactTag:
                impact = Instantiate(impacts.stone[Random.Range(0, impacts.stone.Length)].gameObject, position, rotation);
                break;
            case ImpactTagManager.woodImpactTag:
                impact = Instantiate(impacts.wood[Random.Range(0, impacts.wood.Length)].gameObject, position, rotation);
                break;
            default:
                impact = Instantiate(impacts.wildcard[Random.Range(0, impacts.wildcard.Length)].gameObject, position, rotation);
                break;

        }
        return impact;
    }

    private GameObject Instantiate (GameObject prefab, Vector3 position, Vector3 rotation) {
        GameObject impact;

        if (prefab != null) {
            impact = Instantiate(prefab, position, Quaternion.Euler(rotation));
            return impact;
        }
        else {
            return null;
        }
    }

    public void SpawnImpactRay (RaycastHit raycastHit) {
        SpawnImpact(raycastHit.transform.tag, raycastHit.point, Vector3.zero);
    }

}