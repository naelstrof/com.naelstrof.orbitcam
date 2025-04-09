using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OrbitCameraLook {
    public float yaw;
    public float pitch;

    public OrbitCameraLook(float yaw, float pitch) {
        this.yaw = yaw;
        this.pitch = pitch;
    }

    public OrbitCameraLook(Vector3 direction) {
        Quaternion lookDir = QuaternionExtensions.LookRotationUpPriority(direction, Vector3.up);
        pitch = -lookDir.y;
        yaw = lookDir.x;
    }

    public Quaternion GetRotation() {
        return Quaternion.Euler(-pitch, yaw, 0f);
    }
    
    public Vector3 GetForward() {
        return GetRotation()*Vector3.forward;
    }
}
