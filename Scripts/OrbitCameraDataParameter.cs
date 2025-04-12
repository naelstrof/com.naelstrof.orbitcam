using System;
using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

[Serializable]
public class OrbitCameraDataParameter : ExposedParameter {
    [SerializeField] private OrbitCameraData val = new OrbitCameraData() {
        rotation = Quaternion.identity
    };

    public override object value {
        get => val;
        set => val = (OrbitCameraData)value;
    }
}
