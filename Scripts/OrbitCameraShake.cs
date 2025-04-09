using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

[System.Serializable]
public class OrbitCameraShake : IOrbitCameraDataGenerator {
    public const float SLAM_SHAKE_DURATION = 0.5f;
    public const float SLAM_SHAKE_AMP = 1f;
    
    public const float EXPLOSION_SHAKE_DURATION = 0.5f;
    public const float EXPLOSION_SHAKE_AMP = 1f;
    
    public const float FLOOR_QUAKE_SHAKE_DURATION = 0.8f;
    public const float FLOOR_QUAKE_SHAKE_AMP = 0.8f;
    
    [SerializeField, SerializeReference, SubclassSelector] private IOrbitCameraDataGenerator input;

    public IOrbitCameraDataGenerator Input {
        get => input;
        set => input = value;
    }

    private struct ShakeInstance {
        public int id;
        public float shakeStart;
        public float shakeDuration;
        public float shakeAmplitude;
        
        private float GetTimeEnvelope(float progress) {
            if (progress < 0.1f) {
                return Easing.InCubic(progress / 0.1f);
            } else if (progress is > 0.1f and < 0.3f) {
                return 1f;
            } else {
                return 1f-Easing.OutCubic((progress-0.3f)/0.7f);
            }
        }

        public Vector3 GetPositionOffset(OrbitCameraData data, float time) {
            float progress = Mathf.Clamp01((time - shakeStart) / Mathf.Max(shakeDuration, 0.01f));
            float amp = GetTriangleWave(progress*shakeDuration, 0.11f) * GetTimeEnvelope(progress) * shakeAmplitude;
            return Quaternion.AngleAxis(id * 89f, data.rotation * Vector3.forward) * data.rotation * Vector3.right * (amp * 0.1f);
        }
        
        public Quaternion GetRotationOffset(OrbitCameraData data, float time) {
            float progress = Mathf.Clamp01((time - shakeStart) / Mathf.Max(shakeDuration, 0.01f));
            float amp = GetTriangleWave(progress*shakeDuration, 0.13f) * GetTimeEnvelope(progress) * shakeAmplitude;
            return Quaternion.AngleAxis(amp * 10f, Quaternion.AngleAxis(id*109f, data.rotation * Vector3.forward)*data.rotation*Vector3.up);
        }

        public float GetFOVOffset(OrbitCameraData data, float time) {
            float progress = Mathf.Clamp01((time - shakeStart) / Mathf.Max(shakeDuration, 0.01f));
            float amp = GetTimeEnvelope(progress)*shakeAmplitude;
            return amp*15f;
        }

        public float GetAmp(float time) {
            float progress = Mathf.Clamp01((time - shakeStart) / Mathf.Max(shakeDuration, 0.01f));
            float amp = GetTriangleWave(progress*shakeDuration, 0.11f) * GetTimeEnvelope(progress) * shakeAmplitude;
            return amp;
        }
    }

    private static int currentId = 0;
    private static List<ShakeInstance> shakeInstances;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        shakeInstances?.Clear();
        currentId = 0;
    }
    
    public OrbitCameraData GetData() {
        var data = input.GetData();
        GetShake(data, out Vector3 position, out var rotation, out var fovChange);
        data.position += position;
        //data.rotation *= rotation;
        //data.fov += fovChange;
        return data;
    }

    public static void ApplyShake(float amplitude, float duration) {
        shakeInstances ??= new List<ShakeInstance>();
        shakeInstances.Add(new ShakeInstance() {
            id = currentId++,
            shakeStart = Time.unscaledTime,
            shakeDuration = duration,
            shakeAmplitude = amplitude,
        });
    }

    private static float lastTime;
    private static float lastAmp;
    public static void DrawShakeSamples() {
        shakeInstances ??= new();
        Vector3 offset = -Vector3.forward * 5f;
        float amp = 0f;
        foreach (var shakeInstance in shakeInstances) {
            amp += shakeInstance.GetAmp(Time.unscaledTime);
        }
        Debug.DrawLine(offset+ Vector3.right * lastTime + Vector3.up * lastAmp, offset+Vector3.right * Time.timeSinceLevelLoad + Vector3.up * amp, Color.blue, 10f);
        lastTime = Time.timeSinceLevelLoad;
        lastAmp = amp;
    }

    private static float GetTriangleWave(float time, float period) {
        float triangleWave = (2f * Mathf.Abs(time / period - Mathf.Floor(time / period + 0.5f)))-0.5f;
        return triangleWave;
    }

    protected static void GetShake(OrbitCameraData data, out Vector3 positionShake, out Quaternion rotationShake, out float fovChange) {
        shakeInstances ??= new List<ShakeInstance>();
        for (int i = 0; i < shakeInstances.Count; i++) {
            if (MatchExpiredShakeInstances(shakeInstances[i])) {
                shakeInstances.RemoveAt(i--);
            }
        }

        positionShake = Vector3.zero;
        rotationShake = Quaternion.identity;
        fovChange = 0f;
        
        foreach (var shakeInstance in shakeInstances) {
            positionShake += shakeInstance.GetPositionOffset(data, Time.unscaledTime);
            rotationShake *= shakeInstance.GetRotationOffset(data, Time.unscaledTime);
            fovChange += shakeInstance.GetFOVOffset(data, Time.unscaledTime);
        }
    }

    private static bool MatchExpiredShakeInstances(ShakeInstance shake) {
        return shake.shakeStart + shake.shakeDuration < Time.unscaledTime;
    }
}