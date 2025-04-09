using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrbitCameraBasicConfiguration : OrbitCameraConfiguration {
    [SerializeField]
    protected OrbitCameraPivotBase pivot;

    public OrbitCameraPivotBase Pivot {
        get => pivot;
        set => pivot = value;
    }

    protected OrbitCameraData? lastData;
    public override OrbitCameraData GetData(Camera cam) {
        if (pivot == null) {
            lastData ??= new OrbitCameraData() {
                rotation = Quaternion.identity,
                position = Vector3.zero,
                fov = 65f,
                screenPoint = Vector2.one*0.5f,
                distance = 1f,
                mask = ~0
            };
            return lastData.Value;
        }
        lastData = pivot.GetData(cam);
        return lastData.Value;
    }
}
