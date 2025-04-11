using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrbitCameraLimitRotation : IOrbitCameraDataGenerator {
    [SerializeField, SerializeReference, SubclassSelector] protected IOrbitCameraDataGenerator input;
    [SerializeField] protected float angleFreedom = 30f;
    [SerializeField] protected Quaternion targetRotation;
    
    public IOrbitCameraDataGenerator Input {
        get => input;
        set => input = value;
    }

    public float AngleFreedom {
        get => angleFreedom;
        set => angleFreedom = value;
    }

    public Quaternion TargetRotation {
        get => targetRotation;
        set => targetRotation = value;
    }

    private Quaternion GetRotation(Quaternion camRotation) {
        var rotation = targetRotation;
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
