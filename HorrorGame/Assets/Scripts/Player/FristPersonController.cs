using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FristPersonController : MonoBehaviour
{
    [Header("[ Dependence ]")]
    [SerializeField] private  Transform playerBody;
    
    [Header("[ Move Info ]")]
    [SerializeField] private  float mouseSensitivity = 100f;
    [SerializeField] private  float moveSpeed = 5f;

    [Header("[ Rotation Info ]")]
    [SerializeField] private float rotSpeed = 2f;
    [SerializeField] private  float xRot = 0f;
    [SerializeField] private float yRot = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        Rotation();
        Move();
    }

    private void Rotation()
    {
        // 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 카메라 회전 (상하)
        xRot -= mouseY * rotSpeed;
        yRot += mouseX * rotSpeed;
        xRot = Mathf.Clamp(xRot, -45f, 45f);

        transform.localRotation = Quaternion.Euler(xRot, yRot, 0f);
    }
    private void Move()
    {
        // 키보드 입력 받기
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        playerBody.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }

   
}

