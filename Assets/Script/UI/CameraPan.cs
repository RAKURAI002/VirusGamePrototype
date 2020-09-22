using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraPan : MonoBehaviour
{
    Camera mainCamera;

    public float MouseZoomSpeed = 15.0f;
    public float ZoomMinBound = 30f;
    public float ZoomMaxBound = 130f;

    public Tilemap tilemap;
    public GameObject boundRendererGO;

    private Vector3 touchStart;
    public float groundZ = 0;
    float totalClickTime = 0;

    Vector3 boundMin;
    Vector3 boundMax;

    public static bool isPanning { get; set; }
    private void Start()
    {
        mainCamera = Camera.main;
        transform.position = new Vector3(-35f, 40f, -10);
        boundMin = boundRendererGO.GetComponent<SpriteRenderer>().bounds.min;
        boundMax = boundRendererGO.GetComponent<SpriteRenderer>().bounds.max;

    }

    void Update()
    {
        if (MainCanvas.FreezeCamera)
        {
            StartCoroutine(DelayOneFrame());
            return;
        }

        PanCamera();
        ZoomCamera();
        ClampPosition();
    }

    void ClampPosition()
    {
        mainCamera.transform.position = new Vector3(
                Mathf.Clamp(mainCamera.transform.position.x, boundMin.x + (mainCamera.aspect * mainCamera.orthographicSize),
                boundMax.x - (mainCamera.aspect * mainCamera.orthographicSize)),
               Mathf.Clamp(mainCamera.transform.position.y, boundMin.y + (mainCamera.orthographicSize),
               boundMax.y - (mainCamera.orthographicSize)),
               mainCamera.transform.position.z);
    }
    void OnDrawGizmos()
    {
        Camera targetCamera = Camera.main;
        float verticalHeightSeen = targetCamera.orthographicSize * 2.0f;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3((verticalHeightSeen * targetCamera.aspect), verticalHeightSeen, 0));

    }
    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = GetWorldPosition(groundZ);
            totalClickTime = 0f;
        }
        else if (Input.GetMouseButton(0))
        {
            totalClickTime += Time.deltaTime;
            if (totalClickTime >= 0.2f)
            {
                isPanning = true;
            }
            Vector3 direction = touchStart - GetWorldPosition(groundZ);
            mainCamera.transform.position += direction;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            StartCoroutine(DelaySetPanning());
        }
    }
    private void ZoomCamera()
    {
        float scroll = 0;

#if UNITY_ANDROID
        {
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                scroll = (difference / 100);
            }
        }
#else
             scroll = Input.GetAxis("Mouse ScrollWheel");
#endif

            if (mainCamera.transform.position.x > boundMax.x || mainCamera.transform.position.x < boundMin.x)
            {
                return;
            }

            mainCamera.orthographicSize -= scroll * MouseZoomSpeed;
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, ZoomMinBound, ZoomMaxBound);

        }
        IEnumerator DelaySetPanning()
        {
            yield return new WaitForEndOfFrame();
            isPanning = false;

        }
    IEnumerator DelayOneFrame()
    {
        yield return new WaitForEndOfFrame();


    }
    private Vector3 GetWorldPosition(float z)
        {
            Ray mousePos = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane ground = new Plane(Vector3.forward, new Vector3(0, 0, z));
            float distance;
            ground.Raycast(mousePos, out distance);
            return mousePos.GetPoint(distance);

        }
    }