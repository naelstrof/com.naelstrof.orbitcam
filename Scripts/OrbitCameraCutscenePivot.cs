using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCameraCutscenePivot : OrbitCameraPivotBasic {
    [SerializeField] private float angleFreedom = 30f;
    private Quaternion GetRotation(Quaternion camRotation) {
        float angle = Quaternion.Angle(camRotation, transform.rotation);
        if (angle > angleFreedom) {
            return Quaternion.RotateTowards(camRotation, transform.rotation, angle - angleFreedom);
        }
        return camRotation;
    }
    public override OrbitCameraData GetData(Camera cam) {
        var rotation = GetRotation(cam.transform.rotation);
        return new OrbitCameraData {
            screenPoint = screenOffset,
            position = transform.position,
            distance = GetDistanceFromPivot(cam, rotation, screenOffset),
            fov = fov,
            rotation = rotation,
            mask = layerMask
        };
    }
}
