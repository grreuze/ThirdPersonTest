using UnityEngine;

public class WrappableObject : MonoBehaviour {

    WorldWrapper wrapper;
    Transform my;

    void Start() {
        wrapper = FindObjectOfType<WorldWrapper>();
        my = transform;
    }

	void Update () {
        Vector3 pos = my.position;
        Vector3 wrapPos = wrapper.transform.position;
        Vector3 worldSize = wrapper.worldSize;

        if (wrapper.repeatAxes.x) {
            if (pos.x > wrapPos.x + worldSize.x / 2) {
                pos.x -= worldSize.x;
            } else if (pos.x < wrapPos.x - worldSize.x / 2) {
                pos.x += worldSize.x;
            }
        }

        if (wrapper.repeatAxes.y) {
            if (pos.y > wrapPos.y + worldSize.y / 2) {
                pos.y -= worldSize.y;
            } else if (pos.y < wrapPos.y - worldSize.y / 2) {
                pos.y += worldSize.y;
            }
        }

        if (wrapper.repeatAxes.z) {
            if (pos.z > wrapPos.z + worldSize.z / 2) {
                pos.z -= worldSize.z;
            } else if (pos.z < wrapPos.z - worldSize.z / 2) {
                pos.z += worldSize.z;
            }
        }

        my.position = pos;
	}
}
