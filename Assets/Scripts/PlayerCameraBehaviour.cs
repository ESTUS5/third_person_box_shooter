using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraBehaviour : MonoBehaviour
{
    void Update()
    {
        MoveCamera();
    }
    #region Look
    [SerializeField] public GameObject CameraTarget;

    [Header("Mouse Settings")]
    [SerializeField] private float Sensitivity = 0.6f;
    [SerializeField] private bool InverseVerticalLook = true;
    [SerializeField] private Vector2 MousePosition;
    [SerializeField] private float MinClamp,MaxClamp;
    private Vector2 MouseInput()
    {
        Vector2 mouseInput = Mouse.current.delta.ReadValue();
        if(InverseVerticalLook)
            mouseInput.y = -mouseInput.y;
        mouseInput *= Sensitivity;
        return mouseInput;
    }
    public void MoveCamera()
    {
        MousePosition += MouseInput();
        MousePosition.y = ClampAngle(MousePosition.y,MinClamp,MaxClamp);
        Quaternion rotation = Quaternion.Euler(MousePosition.y,MousePosition.x,0);
        CameraTarget.transform.rotation = rotation;
    }
    #endregion

    float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
