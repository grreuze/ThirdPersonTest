using System.Collections.Generic;
using UnityEngine;

public class EchoManager : MonoBehaviour {

    [SerializeField]
    Echo echoPrefab;

    public List<Echo> echoes;
    public int maxEchoes = 3;

    [HideInInspector]
    public Transform pool;

    new EchoCameraEffect camera;

    void Start() {
        camera = Camera.main.GetComponent<EchoCameraEffect>();
        pool = new GameObject().transform;
        pool.name = "Echo Pool";
    }

	void Update () {
		
        // Drift
        if (Input.GetKeyUp(KeyCode.A)) {
            if (echoes.Count > 0) {
                camera.SetFov(70, 0.15f, true);
                transform.position = echoes[echoes.Count - 1].transform.position;
            }
        }

        // Echo
        if (Input.GetKeyUp(KeyCode.E)) {
            CreateEcho();
        }
	}

    void CreateEcho() {

        Echo newEcho = pool.GetComponentInChildren<Echo>();
        if (newEcho) {
            newEcho.transform.parent = null;
            newEcho.transform.position = transform.position;
            newEcho.gameObject.SetActive(true);

        } else {
            newEcho = Instantiate(echoPrefab, transform.position, Quaternion.identity);
            
        }
        echoes.Add(newEcho);
        if (echoes.Count > maxEchoes) {
            echoes[0].Break();
        }
    }
}
