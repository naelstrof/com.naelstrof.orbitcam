using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class OrbitCameraPivotBasic : OrbitCameraPivotBase {
    [SerializeField] protected Vector2 screenOffset = Vector2.one * 0.5f;
    [SerializeField] protected float desiredDistanceFromPivot = 1f;
    [SerializeField] protected float fov = 65f;
    [SerializeField] protected LayerMask layerMask = ~0;
    
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

    public LayerMask LayerMask {
        get => layerMask;
        set => layerMask = value;
    }

    public override OrbitCameraData GetData(Camera cam) {
        return new OrbitCameraData {
            screenPoint = screenOffset,
            position = transform.position,
            distance = desiredDistanceFromPivot,
            fov = fov,
            rotation = cam.transform.rotation,
            mask = layerMask,
        };
    }
    
    protected float GetDistanceFromPivot(Camera cam, Quaternion rotation, Vector2 screenOffset) {
        return desiredDistanceFromPivot;
    }
}
