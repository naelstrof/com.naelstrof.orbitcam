using System;
using GraphProcessor;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class OrbitCameraBasicPlayerControl : OrbitCameraControllerNode {
    [Input("Input")]
    public OrbitCameraData input;
    
    [SerializeField]
    protected bool clampPitch = true;
    
    [Output("Output")]
    public OrbitCameraData output;


    protected override void Process() {
        var data = input;
        // Always let player control
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        if (Gamepad.current != null) {
            Vector2 gamepadLook = Gamepad.current.rightStick.ReadValue();
            float deadzone = 0.15f;
            gamepadLook = new Vector2(Mathf.MoveTowards(gamepadLook.x, 0f, 0.15f)/(1f-deadzone),
                                      Mathf.MoveTowards(gamepadLook.y, 0f, 0.15f)/(1f-deadzone));
            mouseDelta += gamepadLook * 40f;
        }

        //var sensitivity = mouseSensitivity?.GetValue() ?? 0.01f;
        mouseDelta *= 0.02f;

        var euler = data.rotation.eulerAngles;
        euler += new Vector3(-mouseDelta.y, mouseDelta.x, 0f);
        euler = new Vector3(Mathf.Repeat(euler.x + 180f, 360f) - 180f, Mathf.Repeat(euler.y + 180f, 360f) - 180f, euler.z);
        if (clampPitch) {
            euler = new Vector3(Mathf.Clamp(euler.x, -89f, 89f), euler.y, euler.z);
        }

        data.rotation = Quaternion.Euler(euler);
        output = data;
    }
}
