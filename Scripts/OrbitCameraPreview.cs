using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine.Experimental.Playables;

[EditorTool("Orbit Camera Preview Tool", typeof(OrbitCamera))]
public class OrbitCameraPreview : EditorTool {
    private static RenderTexture renderTexture;
    private static Camera tempCamera;
    private static OrbitCameraData lastData;

    private static OrbitCameraPreviewStatus status;

    enum OrbitCameraPreviewStatus {
        Okay,
        NotSupportedSRP,
    }
    
    public static Camera GetPreviewCamera() {
        if (tempCamera == null) {
            var cameraObj = new GameObject("OrbitCameraPreview", typeof(Camera)) {
                hideFlags = HideFlags.HideAndDontSave
            };
            tempCamera = cameraObj.GetComponent<Camera>();
            if (Selection.activeObject != null && Selection.activeObject is GameObject selectedGameObject) {
                var currentCamera = selectedGameObject.GetComponent<Camera>();
                if (currentCamera != null) {
                    // FIXME: Possibly should be part of OrbitCameraData
                    tempCamera.nearClipPlane = currentCamera.nearClipPlane;
                    tempCamera.farClipPlane = currentCamera.farClipPlane;
                    tempCamera.orthographic = currentCamera.orthographic;
                    tempCamera.orthographicSize = currentCamera.orthographicSize;
                }
            }
        }

        return tempCamera;
    }
    
    private static RenderTexture GetRenderTexture() {
        if (renderTexture == null) {
            renderTexture = new RenderTexture(256, 256, 32, RenderTextureFormat.ARGB32);
        }
        return renderTexture;
    }
    
    private static RenderTexture RenderPreview(OrbitCameraData data) {
        lastData = data;
        RenderTexture temp = GetRenderTexture();
        var camera = GetPreviewCamera();
        lastData.ApplyTo(camera, new Vector2(temp.width, temp.height));
        
        RenderPipeline.StandardRequest request = new RenderPipeline.StandardRequest();
        if (RenderPipeline.SupportsRenderRequest(camera, request)) {
            request.destination = temp;
            RenderPipeline.SubmitRenderRequest(camera, request);
            status = OrbitCameraPreviewStatus.Okay;
        } else {
            status = OrbitCameraPreviewStatus.NotSupportedSRP;
        }
        return temp;
    }
    
    private void OnEnable() {
    }
    private void OnDisable() {
        if (tempCamera != null) {
            DestroyImmediate(tempCamera.gameObject, true);
        }
        if (renderTexture != null) {
            DestroyImmediate(renderTexture);
        }
        renderTexture = null;
        tempCamera = null;
    }

    public override void OnToolGUI(EditorWindow window) {
        if (!(window is SceneView sceneView)) {
            return;
        }
        if (target is not OrbitCamera camera) {
            return;
        }
        
        camera.Process();
        lastData = camera.GetData();

        if (Event.current.type is not EventType.Repaint) {
            return;
        }
        
        var rt = RenderPreview(lastData);
        Handles.BeginGUI();
        EditorGUI.DrawPreviewTexture(new Rect(0, 0, 256, 256), rt);
        Handles.EndGUI();
        sceneView.Repaint();
    }
    
    public override void OnActivated() {
        SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Entering Orbit Camera Tool"), .1f);
    }
    
    public override void OnWillBeDeactivated() {
        SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Exiting Orbit Camera Tool"), .1f);
    }
}

#endif