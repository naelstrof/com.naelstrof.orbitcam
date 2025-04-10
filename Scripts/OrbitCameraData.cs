using UnityEngine;
public struct OrbitCameraData {
    public Vector3 position;
    public float distance;
    public float fov;
    public Vector2 screenPoint;
    public Quaternion rotation; 
    public LayerMask cullingMask;
    public LayerMask collisionMask;
    
    public OrbitCameraData(Camera camera) {
        position = camera.transform.position;
        distance = 0f;
        fov = camera.fieldOfView;
        screenPoint = Vector2.one*0.5f;
        rotation = camera.transform.rotation;
        cullingMask = camera.cullingMask;
        collisionMask = ~(0);
    }

    public void ApplyTo(Camera cam) {
        Quaternion cameraRot = rotation;
        cam.fieldOfView = fov;
        cam.transform.rotation = cameraRot;
        Ray screenRay = OrbitCamera.GetScreenRay(cam, screenPoint);
        cam.transform.position = position - screenRay.direction * distance;
        cam.cullingMask = cullingMask;
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
            cullingMask = t > 0.5f ? pivotB.cullingMask : pivotA.cullingMask
        };
    }
}