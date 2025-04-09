using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour {
    [SerializeField, SerializeReference, SubclassSelector]
    private IOrbitCameraDataGenerator configuration;
    
    private Camera cam;
    private bool tracking = true;
    private OrbitCameraData currentCameraData = new() {
        rotation = Quaternion.identity,
        position = Vector3.zero,
        fov = 65f,
        screenPoint = Vector2.one*0.5f,
        distance = 1f,
        cullingMask = ~0
    };
    private Coroutine tween;
    public delegate void OrbitCameraConfigurationChangedAction(IOrbitCameraDataGenerator previousConfiguration, IOrbitCameraDataGenerator newConfiguration);

    public event OrbitCameraConfigurationChangedAction configurationChanged;

    public IOrbitCameraDataGenerator GetConfiguration() => configuration;
    
    public void SetConfiguration(IOrbitCameraDataGenerator newConfig, float tweenDuration = 0.4f) {
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
        SetOrbit(configuration?.GetData() ?? new OrbitCameraData());
    }

    public static Ray GetScreenRay(Camera cam, Vector2 screenPoint) {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 desiredScreenPosition = screenPoint*screenSize;
        return cam.ScreenPointToRay(desiredScreenPosition);
    }

    private void SetOrbit(OrbitCameraData data) {
        data.ApplyTo(cam);
        currentCameraData = data;
    }

    private void BeginTween(OrbitCameraData from, IOrbitCameraDataGenerator next, float duration) {
        if (tween != null) {
            StopCoroutine(tween);
            tween = null;
        }
        configurationChanged?.Invoke(configuration, next);
        configuration = next;
        tween = StartCoroutine(TweenTo(from, next, duration));
    }

    IEnumerator TweenTo(OrbitCameraData from, IOrbitCameraDataGenerator next, float duration) {
        yield return new WaitForEndOfFrame();
        try {
            float timer = 0f;
            while (timer < duration) {
                timer += Time.deltaTime;
                float t = timer / duration;
                SetOrbit(OrbitCameraData.Lerp(from, next.GetData(), t));
                yield return new WaitForEndOfFrame();
            }
            SetOrbit(next.GetData());
        } finally {
            tween = null;
        }
    }
    
    private void LateUpdate() {
        if (tween != null || configuration == null) {
            return;
        }
        SetOrbit(configuration.GetData());
    }
    
    public OrbitCameraData GetCurrentCameraData() => currentCameraData;

    private void OnDrawGizmosSelected() {
        if (configuration != null) {
            OrbitCameraPreview.RenderPreview(configuration);
        }
    }
}
