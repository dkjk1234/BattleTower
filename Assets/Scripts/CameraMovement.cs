using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float cameraSpeed;
    private float xMin, xMax, yMin, yMax;
    private float initialZ;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            cam = FindObjectOfType<Camera>();
            if (cam == null)
            {
                Debug.LogError("Main Camera not found. Please ensure there is a Camera tagged as MainCamera in the scene.");
                return;
            }
        }
        initialZ = transform.position.z;  // 카메라의 초기 Z 값을 저장

        // 중앙에 위치하도록 초기 설정
        SetCameraToCenter();
    }

    void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            direction += Vector3.up;
        if (Input.GetKey(KeyCode.A))
            direction += Vector3.left;
        if (Input.GetKey(KeyCode.S))
            direction += Vector3.down;
        if (Input.GetKey(KeyCode.D))
            direction += Vector3.right;

        transform.Translate(direction.normalized * cameraSpeed * Time.deltaTime);

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, xMin, xMax),
            Mathf.Clamp(transform.position.y, yMin, yMax),
            initialZ);  // 초기 Z 값을 유지
    }

    public void SetLimits(Vector3 maxTile, Vector3 minTile)
    {
        if (cam == null)
        {
            cam = Camera.main;
            return;
        }

        Vector3 wp = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));  // 우상단
        Vector3 wp2 = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); // 좌하단

        xMin = minTile.x;
        xMax = maxTile.x;
        yMin = minTile.y;
        yMax = maxTile.y;

        SetCameraToCenter();
    }

    void SetCameraToCenter()
    {
        float centerX = (xMin + xMax) / 2;
        float centerY = (yMin + yMax) / 2;
        transform.position = new Vector3(centerX, centerY, initialZ);
    }
} 
