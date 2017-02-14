using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Echo : MonoBehaviour {

    new Collider collider;
    bool isActive;

    EchoManager echoManager;
    Transform pool;

	void Start () {
        collider = GetComponent<Collider>();
        collider.isTrigger = true;
        echoManager = FindObjectOfType<EchoManager>();
        pool = echoManager.pool;
	}

    void OnTriggerEnter(Collider col) {
        if (isActive && col.tag == "Player") {
            Break();
        }
    }

    void OnTriggerExit(Collider col) {
        if (col.tag == "Player") {
            isActive = true;
        }
    }

    public void Break() {
        gameObject.SetActive(false);
        transform.parent = pool;
        echoManager.echoes.Remove(this);
    }
}
