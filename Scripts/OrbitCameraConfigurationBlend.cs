using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrbitCameraConfigurationBlend : IOrbitCameraDataGenerator {
    [SerializeField, SerializeReference, SubclassSelector]
    protected IOrbitCameraDataGenerator inputA;
    [SerializeField, SerializeReference, SubclassSelector]
    protected IOrbitCameraDataGenerator inputB;
    [SerializeField]
    protected float blend = 0.5f;

    public IOrbitCameraDataGenerator InputA {
        get => inputA;
        set => inputA = value;
    }

    public IOrbitCameraDataGenerator InputB {
        get => inputB;
        set => inputB = value;
    }

    public float Blend {
        get => blend;
        set => blend = value;
    }

    public OrbitCameraData GetData() {
        return OrbitCameraData.Lerp(inputA.GetData(), inputB.GetData(), blend);
    }
}
