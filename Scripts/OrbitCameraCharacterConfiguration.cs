using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[System.Serializable]
public class OrbitCameraCharacterConfiguration : OrbitCameraConfiguration {
    [SerializeField]
    private OrbitCameraPivotBase shoulderPivot;
    [SerializeField]
    private OrbitCameraPivotBase buttPivot;
    [SerializeField]
    private OrbitCameraPivotBase dickPivot;
    [SerializeField]
    private Transform characterTransform;

    public OrbitCameraPivotBase ShoulderPivot {
        get => shoulderPivot;
        set => shoulderPivot = value;
    }

    public OrbitCameraPivotBase ButtPivot {
        get => buttPivot;
        set => buttPivot = value;
    }

    public OrbitCameraPivotBase DickPivot {
        get => dickPivot;
        set => dickPivot = value;
    }

    public Transform CharacterTransform {
        get => characterTransform;
        set => characterTransform = value;
    }

    private OrbitCameraData? lastData;
    
    public override OrbitCameraData GetData(Camera cam) {
        if (shoulderPivot == null) {
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

        Quaternion camRotation = cam.transform.rotation;

        OrbitCameraData topData = shoulderPivot.GetData(cam);
        OrbitCameraData buttData = buttPivot.GetData(cam);
        OrbitCameraData dickData = dickPivot.GetData(cam);

        const float standOffToSideTarget = 0.33f;
        Vector3 forward = cam.transform.forward;
        float downUp = Easing.OutSine(Mathf.Clamp01(-forward.y*2f));
        float downUpSoft = Easing.InOutCubic(Mathf.Clamp01(-forward.y));
        float yaw = camRotation.eulerAngles.y;
        Quaternion rot = Quaternion.AngleAxis(yaw, Vector3.up);
        float forwardBack = Easing.InOutCubic((Vector3.Dot(characterTransform.forward, rot * Vector3.forward)+1f)/2f);

        var buttPivotBasic = (OrbitCameraPivotBasic)buttPivot;
        if (buttPivotBasic != null) {
            buttPivotBasic.ScreenOffset = new Vector2(Mathf.Lerp(0.5f, standOffToSideTarget, downUp), 0.4f);
            buttPivotBasic.DesiredDistanceFromPivot = 0.7f;
            buttPivotBasic.FOV = 65f;
        }

        var shoulderPivotBasic = (OrbitCameraPivotBasic)shoulderPivot;
        if (shoulderPivotBasic != null) {
            shoulderPivotBasic.ScreenOffset = new Vector2(Mathf.Lerp(standOffToSideTarget, 0.5f, downUpSoft), 0.5f);
            shoulderPivotBasic.DesiredDistanceFromPivot = 0.7f;
            shoulderPivotBasic.FOV = 65f;
        }

        OrbitCameraData forwardBackData = OrbitCameraData.Lerp(dickData, buttData, forwardBack);
        return OrbitCameraData.Lerp(forwardBackData, topData, downUp);
    }
}
