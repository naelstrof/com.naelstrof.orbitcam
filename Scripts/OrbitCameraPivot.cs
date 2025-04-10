using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class OrbitCameraPivot : IOrbitCameraDataGenerator {
    [SerializeField, SerializeReference, SubclassSelector] protected IOrbitCameraDataGenerator input;

    [SerializeField] protected Transform pivotTransform;
    [SerializeField] protected Vector2 screenOffset = Vector2.one * 0.5f;
    [SerializeField] protected float desiredDistanceFromPivot = 1f;
    [SerializeField] protected float fov = 65f;
    [SerializeField] protected LayerMask cullingMask = ~0;

    public Transform PivotTransform {
        get => pivotTransform;
        set => pivotTransform = value;
    }

    public IOrbitCameraDataGenerator Input {
        get => input;
        set => input = value;
    }

    public Vector2 ScreenOffset {
        get => screenOffset;
        set => screenOffset = value;
    }

    public float DesiredDistanceFromPivot {
        get => desiredDistanceFromPivot;
        set => desiredDistanceFromPivot = value;
    }

    public float FOV {
        get => fov;
        set => fov = value;
    }

    public LayerMask CullingMask {
        get => cullingMask;
        set => cullingMask = value;
    }
    
    public virtual OrbitCameraData GetData() {
        var data = input.GetData();
        return new OrbitCameraData {
            screenPoint = screenOffset,
            position = pivotTransform.position,
            distance = desiredDistanceFromPivot,
            fov = fov,
            rotation = data.rotation,
            cullingMask = cullingMask,
        };
    }
}
