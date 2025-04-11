using GraphProcessor;
using UnityEngine;

[System.Serializable, NodeMenuItem("OrbitCamera/Memory")]
public class OrbitCameraDataConstant : OrbitCameraControllerNode {
    
    //public OrbitCameraData data;
    [SerializeField, Input("Pivot Position")]
    public Vector3 pivotPosition = Vector3.zero;
    [SerializeField, Input("Screen Offset")]
    public Vector2 screenOffset = Vector2.one * 0.5f;
    [SerializeField, Input("Desired Distance from Pivot")]
    public float desiredDistanceFromPivot = 1f;
    [SerializeField, Input("Field of View")]
    public float fov = 65f;
    [SerializeField, Input("Rotation")]
    public Quaternion rotation = Quaternion.identity;
    [SerializeField]
    public LayerMask cullingMask = ~0;
    
    [Output(name = "Output")]
    public OrbitCameraData output;

    protected override void Process() {
        output = new OrbitCameraData() {
            cullingMask = cullingMask,
            distance = desiredDistanceFromPivot,
            fov = fov,
            position = pivotPosition,
            rotation = rotation,
            screenPoint = screenOffset
        };
    }
}