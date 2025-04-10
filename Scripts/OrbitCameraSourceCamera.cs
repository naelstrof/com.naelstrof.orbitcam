using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrbitCameraSourceCamera : IOrbitCameraDataGenerator {
    [SerializeField]
    protected Camera camera;

    public Camera Camera {
        get => camera;
        set => camera = value;
    }

    public OrbitCameraData GetData() {
        return new OrbitCameraData(camera);
    }
}
