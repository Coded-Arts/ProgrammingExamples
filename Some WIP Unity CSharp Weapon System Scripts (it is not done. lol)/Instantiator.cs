using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] 
public class OnInstantiateEvent : UnityEvent<GameObject> { }

public class Instantiator : MonoBehaviour {

    [SerializeField] private GameObject[] gameObjects;
    [SerializeField] private OnInstantiateEvent OnInstantiate;

    private GameObject lastObject;

    public GameObject[] GameObjects {
        get {
            return gameObjects;
        }
    }

    public GameObject LastObject {
        get {
            return lastObject;
        }
    }

    public void InstantiateObjects () {
        GameObject obj;
        for (int i = 0; i < gameObjects.Length; i++) {
            obj = Instantiate(gameObjects[i], transform.position, Quaternion.identity);
            lastObject = obj;
            OnInstantiate.Invoke(obj);
        }
    }

    public void InstantiateObjects (Vector3 position) {
        GameObject obj;
        for (int i = 0; i < gameObjects.Length; i++) {
            obj = Instantiate(gameObjects[i], position, Quaternion.identity);
            lastObject = obj;
            OnInstantiate.Invoke(obj);
        }
    }

}
