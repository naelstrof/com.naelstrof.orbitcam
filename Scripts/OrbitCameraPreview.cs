using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using System.Net;
using UnityEditor.Overlays;

[Overlay(typeof(SceneView), "Orbit Camera Preview", false)]
public class OrbitCameraPreview : Overlay {
    private static RenderTexture renderTexture;
    private static Camera tempCamera;
    private static VisualTreeAsset visualTree;
    private static VisualElement root;
    private static IOrbitCameraDataGenerator lastGenerator;
    public override void OnWillBeDestroyed() {
        base.OnWillBeDestroyed();
        if (tempCamera != null) {
            Object.DestroyImmediate(tempCamera.gameObject, true);
        }
        if (renderTexture != null) {
            Object.DestroyImmediate(renderTexture);
        }
        renderTexture = null;
        tempCamera = null;
        visualTree = null;
        root = null;
    }

    private static Camera GetTempCamera() {
        if (tempCamera == null) {
            var cameraObj = new GameObject("OrbitCameraPreview", typeof(Camera)) {
                hideFlags = HideFlags.HideAndDontSave
            };
            tempCamera = cameraObj.GetComponent<Camera>();
            tempCamera.nearClipPlane = 0.03f;
        }

        return tempCamera;
    }
    private static RenderTexture GetRenderTexture() {
        if (renderTexture == null) {
            renderTexture = new RenderTexture(256, 256, 32, RenderTextureFormat.ARGB32);
        }
        return renderTexture;
    }

    private VisualTreeAsset GetVisualTreeAsset() {
        if (visualTree == null) {
            var visualTreePath = AssetDatabase.GUIDToAssetPath("2a11c5c36a921194599e1cae66f5f2c6");
            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreePath);
        }
        return visualTree;
    }
    
    public static void RenderPreview(IOrbitCameraDataGenerator generator) {
        lastGenerator = generator;
        if (root == null) {
            return;
        }
        var camera = GetTempCamera();
        var data = generator.GetData();
        data.ApplyTo(camera);
        
        RenderPipeline.StandardRequest request = new RenderPipeline.StandardRequest();
        if (RenderPipeline.SupportsRenderRequest(camera, request)) {
            request.destination = GetRenderTexture();
            RenderPipeline.SubmitRenderRequest(camera, request);
            root.Q<Label>("info").text = "Click and drag within the image to simulate player look.";
            root.Q<VisualElement>("image").style.backgroundImage = Background.FromRenderTexture(GetRenderTexture());
        } else {
            root.Q<Label>("info").text = "This render pipeline doesn't support render requests... Sorry.";
        }
    }

    private class TrackballManipulator : MouseManipulator {
        private bool dragging = false;
        public float mouseSensitivity = 0.1f;
        protected override void RegisterCallbacksOnTarget() {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        }

        private void OnMouseMove(MouseMoveEvent evt) {
            if (!dragging) {
                return;
            }
            var camera = GetTempCamera();
            var mouseDelta = evt.mouseDelta;
            //var sensitivity = mouseSensitivity?.GetValue() ?? 0.01f;
            mouseDelta *= mouseSensitivity;

            var euler = camera.transform.rotation.eulerAngles;
            euler += new Vector3(-mouseDelta.y, mouseDelta.x, 0f);
            euler = new Vector3(Mathf.Repeat(euler.x + 180f, 360f) - 180f, Mathf.Repeat(euler.y + 180f, 360f) - 180f, euler.z);
            euler = new Vector3(Mathf.Clamp(euler.x, -89f, 89f), euler.y, euler.z);
            camera.transform.rotation = Quaternion.Euler(euler);
        }

        private void OnMouseUp(MouseUpEvent evt) {
            dragging = false;
        }

        private void OnMouseDown(MouseDownEvent evt) {
            dragging = true;
        }

        protected override void UnregisterCallbacksFromTarget() {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        }
    }

    public override VisualElement CreatePanelContent() {
        root = new VisualElement();
        GetVisualTreeAsset().CloneTree(root);
        root.Q<Label>("info").text = "Select a pivot to preview.";
        var image = root.Q<VisualElement>("image");
        image.style.backgroundImage = Background.FromRenderTexture(GetRenderTexture());
        
        var trackballManipulator = new TrackballManipulator();
        image.AddManipulator(trackballManipulator);
        
        var slider = root.Q<Slider>("mouseSensitivity");
        slider.SetValueWithoutNotify(trackballManipulator.mouseSensitivity);
        slider.RegisterValueChangedCallback((v) => {
            trackballManipulator.mouseSensitivity = v.newValue;
        });
        return root;
    }
}

#endif