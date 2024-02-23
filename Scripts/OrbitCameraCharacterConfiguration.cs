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

    private OrbitCameraData? lastData;
    
    public void SetPivots(OrbitCameraPivotBase shoulder, OrbitCameraPivotBase butt, OrbitCameraPivotBase dick) {
        shoulderPivot = shoulder;
        buttPivot = butt;
        dickPivot = dick;
    }
    
    public override OrbitCameraData GetData(Camera cam) {
        if (shoulderPivot == null) {
            lastData ??= OrbitCamera.GetCurrentCameraData();
            return lastData.Value;
        }

        OrbitCameraData topData = shoulderPivot.GetData(cam);
        OrbitCameraData buttData = buttPivot.GetData(cam);
        OrbitCameraData dickData = dickPivot.GetData(cam);

        const float standOffToSideTarget = 0.33f;
        Vector3 forward = cam.transform.forward;
        float downUp = Easing.OutSine(Mathf.Clamp01(-forward.y*2f));
        float downUpSoft = Easing.InOutCubic(Mathf.Clamp01(-forward.y));
        Quaternion rot = Quaternion.AngleAxis(OrbitCamera.GetPlayerIntendedScreenAim().x, Vector3.up);
        float forwardBack = Easing.InOutCubic((Vector3.Dot(characterTransform.forward, rot * Vector3.forward)+1f)/2f);
        (buttPivot as OrbitCameraPivotBasic)?.SetInfo(new Vector2(Mathf.Lerp(0.5f, standOffToSideTarget, downUp), 0.4f), 0.7f, 65f);
        (shoulderPivot as OrbitCameraPivotBasic)?.SetInfo(new Vector2(Mathf.Lerp(standOffToSideTarget, 0.5f, downUpSoft), 0.5f), Mathf.Lerp(0.7f,1.2f,downUpSoft), 65f);
        OrbitCameraData forwardBackData = OrbitCameraData.Lerp(dickData, buttData, forwardBack);
        return OrbitCameraData.Lerp(forwardBackData, topData, downUp);
    }
}
