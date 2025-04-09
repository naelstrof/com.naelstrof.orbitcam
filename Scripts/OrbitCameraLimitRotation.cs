using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrbitCameraLimitRotation : IOrbitCameraDataGenerator {
    [SerializeField, SerializeReference, SubclassSelector] protected IOrbitCameraDataGenerator input;
    [SerializeField] protected float angleFreedom = 30f;
    [SerializeField] protected Transform targetRotationTransform;
    private Quaternion GetRotation(Quaternion camRotation) {
        var rotation = targetRotationTransform.rotation;
        float angle = Quaternion.Angle(camRotation, rotation);
        if (angle > angleFreedom) {
            return Quaternion.RotateTowards(camRotation, rotation, angle - angleFreedom);
        }
        return camRotation;
    }

    public OrbitCameraData GetData() {
        var orbitCameraData = input.GetData();
        orbitCameraData.rotation = GetRotation(orbitCameraData.rotation);
        return orbitCameraData;
    }
}
