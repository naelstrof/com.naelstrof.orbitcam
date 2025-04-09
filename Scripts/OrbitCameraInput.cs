using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCameraInput : MonoBehaviour {
    private bool tracking = true;
    private bool clampYaw = false;
    private bool clampPitch = true;
    public void Update() {
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

        if (tracking) {
            var euler = transform.rotation.eulerAngles;
            euler += new Vector3(-mouseDelta.y, mouseDelta.x, 0f);
            euler = new Vector3(Mathf.Repeat(euler.x + 180f, 360f) - 180f, Mathf.Repeat(euler.y + 180f, 360f) - 180f, euler.z);
            euler = new Vector3(Mathf.Clamp(euler.x, -89f, 89f), euler.y, euler.z);
            transform.rotation = Quaternion.Euler(euler);
        }
    }
}
