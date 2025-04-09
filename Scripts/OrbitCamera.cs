using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour {
    [SerializeField, SerializeReference, SubclassSelector]
    private OrbitCameraConfiguration configuration;

    [SerializeField] private LayerMask collisionMask;
    
    private Camera cam;
    private bool tracking = true;
    private OrbitCameraData currentCameraData = new() {
        rotation = Quaternion.identity,
        position = Vector3.zero,
        fov = 65f,
        screenPoint = Vector2.one*0.5f,
        distance = 1f,
        mask = ~0
    };
    private Coroutine tween;
    public delegate void OrbitCameraConfigurationChangedAction(OrbitCameraConfiguration previousConfiguration, OrbitCameraConfiguration newConfiguration);

    public event OrbitCameraConfigurationChangedAction configurationChanged;

    public OrbitCameraConfiguration GetConfiguration() => configuration;
    
    public void SetConfiguration(OrbitCameraConfiguration newConfig, float tweenDuration = 0.4f) {
        if (configuration == newConfig) {
            return;
        }

        if (tweenDuration == 0f) {
            configurationChanged?.Invoke(configuration, newConfig);
            configuration = newConfig;
            return;
        }
        BeginTween(GetCurrentCameraData(), newConfig, tweenDuration);
    }

    private void Awake() {
        cam = GetComponent<Camera>();
        SetOrbit(configuration?.GetData(cam) ?? new OrbitCameraData());
    }

    public static Ray GetScreenRay(Camera cam, Vector2 screenPoint) {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 desiredScreenPosition = screenPoint*screenSize;
        return cam.ScreenPointToRay(desiredScreenPosition);
    }

    private void SetOrbit(OrbitCameraData data) {
        data.ApplyTo(cam, collisionMask);
        currentCameraData = data;
    }

    private void BeginTween(OrbitCameraData from, OrbitCameraConfiguration next, float duration) {
        if (tween != null) {
            StopCoroutine(tween);
            tween = null;
        }
        configurationChanged?.Invoke(configuration, next);
        configuration = next;
        tween = StartCoroutine(TweenTo(from, next, duration));
    }

    IEnumerator TweenTo(OrbitCameraData from, OrbitCameraConfiguration next, float duration) {
        yield return new WaitForEndOfFrame();
        try {
            float timer = 0f;
            while (timer < duration) {
                timer += Time.deltaTime;
                float t = timer / duration;
                SetOrbit(OrbitCameraData.Lerp(from, next.GetData(cam), t));
                yield return new WaitForEndOfFrame();
            }
            SetOrbit(next.GetData(cam));
        } finally {
            tween = null;
        }
    }
    
    private void LateUpdate() {
        if (tween != null || configuration == null) {
            return;
        }
        SetOrbit(configuration.GetData(cam));
    }
    
    public OrbitCameraData GetCurrentCameraData() => currentCameraData;
}
