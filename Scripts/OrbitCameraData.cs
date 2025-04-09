using UnityEngine;
public struct OrbitCameraData {
    public Vector3 position;
    public float distance;
    public float fov;
    public Vector2 screenPoint;
    public Quaternion rotation; 
    public LayerMask mask;
    
    private static Vector3[] frustumCorners = new Vector3[4];
    private static RaycastHit[] raycastHits = new RaycastHit[32];
    private static bool CastNearPlane(Camera cam, LayerMask hitMask, Quaternion camRotation, Vector2 screenOffset, Vector3 from, Vector3 to, out float distance) {
        const float minDistance = 0.1f;
        var camTransform = cam.transform;
        var rot = camTransform.rotation;
        camTransform.rotation = camRotation;
        
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
        Vector3 extents = new Vector3(Vector3.Distance(frustumCorners[0], frustumCorners[2]), Vector3.Distance(frustumCorners[0], frustumCorners[1]), cam.nearClipPlane);
        Vector3 diff = to - from;
        Vector3 dir = -OrbitCamera.GetScreenRay(cam, screenOffset).direction;
        Vector3 cameraStart = from;// - dir * (cam.nearClipPlane*0.5f);
        //for(int i=0;i<4;i++) {
            //Debug.DrawLine(cameraStart + camRotation * frustumCorners[i], cameraStart + camRotation * frustumCorners[i]+dir*diff.magnitude);
        //}
        int hitCount = Physics.BoxCastNonAlloc(cameraStart, extents * 0.5f, dir, raycastHits, camRotation, diff.magnitude, hitMask);
        distance = Vector3.Distance(to, from);
        if (hitCount == 0) {
            cam.transform.rotation = rot;
            return false;
        }

        bool insideProp = false;
        for (int i = 0; i < hitCount; i++) {
            if (raycastHits[i].point == Vector3.zero) {
                insideProp = true;
                break;
            }
            distance = Mathf.Min(distance, Mathf.Max(raycastHits[i].distance-cam.nearClipPlane-minDistance, 0f));
        }

        // If we're inside something, then we panic and try to use a regular raycasts to figure out what to do.
        if (insideProp) {
            // Center-camera
            hitCount = Physics.RaycastNonAlloc(cameraStart, dir, raycastHits, distance, hitMask);
            for (int j = 0; j < hitCount; j++) {
                distance = Mathf.Min(distance, Mathf.Max(raycastHits[j].distance-cam.nearClipPlane-minDistance, 0f));
            }
            // All four corners
            for (int i = 0; i < 4; i++) {
                hitCount = Physics.RaycastNonAlloc(cameraStart + camRotation*frustumCorners[i], dir, raycastHits, distance, hitMask);
                for (int j = 0; j < hitCount; j++) {
                    distance = Mathf.Min(distance, Mathf.Max(raycastHits[j].distance-cam.nearClipPlane-minDistance,0f));
                }
            }
        }

        // Bring the camera in a bit so that the occlusion culling doesn't flicker...
        distance -= cam.nearClipPlane;
        cam.transform.rotation = rot;
        return true;
    }

    public void ApplyTo(Camera cam) {
        Quaternion cameraRot = rotation;
        cam.fieldOfView = fov;
        cam.transform.rotation = cameraRot;
        Ray screenRay = OrbitCamera.GetScreenRay(cam, screenPoint);
        cam.transform.position = position - screenRay.direction * distance;
        cam.cullingMask = mask;
    }

    public bool IsValid() {
        if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z)) {
            return false;
        }

        if (float.IsNaN(screenPoint.x) || float.IsNaN(screenPoint.y)) {
            return false;
        }

        if (rotation.normalized != rotation) {
            return false;
        }

        if (float.IsNaN(distance)) {
            return false;
        }

        return true;
    }

    public static OrbitCameraData Lerp(OrbitCameraData pivotA, OrbitCameraData pivotB, float t) {
        if (float.IsNaN(t)) {
            Debug.LogError("Tried to lerp with nan t");
        }
        if (!pivotA.IsValid()) {
            Debug.LogError("Tried to lerp with nan pivot A");
        }
        if (!pivotB.IsValid()) {
            Debug.LogError("Tried to lerp with nan pivot B");
        }

        return new OrbitCameraData {
            position = Vector3.Lerp(pivotA.position, pivotB.position, t),
            distance = Mathf.Lerp(pivotA.distance, pivotB.distance, t),
            fov = Mathf.Lerp(pivotA.fov, pivotB.fov, t),
            screenPoint = Vector2.Lerp(pivotA.screenPoint, pivotB.screenPoint, t),
            rotation = Quaternion.Lerp(pivotA.rotation, pivotB.rotation, t),
            mask = t > 0.5f ? pivotB.mask : pivotA.mask
        };
    }
}