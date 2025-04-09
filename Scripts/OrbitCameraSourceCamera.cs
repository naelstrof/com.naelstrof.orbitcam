using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrbitCameraSourceCamera : IOrbitCameraDataGenerator {
    [SerializeField]
    private Camera camera;
    public OrbitCameraData GetData() {
        return new OrbitCameraData(camera);
    }
}
