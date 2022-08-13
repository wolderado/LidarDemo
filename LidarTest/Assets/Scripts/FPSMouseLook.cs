using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FPSMouseLook : MonoBehaviour
{

    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool ClampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;
    public bool lockCursor = true;



    private Quaternion characterTargetRot;
    private Quaternion cameraTargetRot;
    private bool CursorIsLocked = true;

    [HideInInspector]
    public float DefaultXSensivity;
    [HideInInspector]
    public float DefaultYSensivity;
    [HideInInspector]
    public float DefaultSmooth;

    private Transform camera;

    public void Init(Transform character)
    {
        camera = GetComponent<Transform>();
        DefaultSmooth = smoothTime;
        DefaultXSensivity = XSensitivity;
        DefaultYSensivity = YSensitivity;
        characterTargetRot = character.localRotation;
        cameraTargetRot = camera.localRotation;
    }

    void Start()
    {

    }

    public void LookRotation(Transform character)
    {
        float yRot = Input.GetAxis("Mouse X") * XSensitivity;
        float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

        characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        cameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (ClampVerticalRotation)
            cameraTargetRot = ClampRotationAroundXAxis(cameraTargetRot);

        if (smooth)
        {
            character.localRotation = Quaternion.Lerp(character.localRotation, characterTargetRot, smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Lerp(camera.localRotation, cameraTargetRot, smoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = characterTargetRot;
            camera.localRotation = cameraTargetRot;
        }

        UpdateCursorLock();

        if (Input.GetKeyDown(KeyCode.Escape) && Application.isEditor)
            SetCursorLock(false);
    }


    private void OnDisable()
    {
        if (Application.isEditor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        if (lockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {
        if (CursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!CursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
