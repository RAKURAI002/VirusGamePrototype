using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraPan : MonoBehaviour
{

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
        transform.position = new Vector3(-35f, 40f, -10);
        boundMin = boundRendererGO.GetComponent<SpriteRenderer>().bounds.min;
        boundMax = boundRendererGO.GetComponent<SpriteRenderer>().bounds.max;
    }
    // Update is called once per frame
    void Update()
    {
        if (MainCanvas.canvasActive)
        {
            return;

        }
        PanCamera();
        ZoomCamera();
        ClampPosition();
    }

    void ClampPosition()
    {
        Camera.main.transform.position = new Vector3(
                Mathf.Clamp(Camera.main.transform.position.x, boundMin.x + (Camera.main.aspect * Camera.main.orthographicSize),
                boundMax.x - (Camera.main.aspect * Camera.main.orthographicSize)),
               Mathf.Clamp(Camera.main.transform.position.y, boundMin.y + (Camera.main.orthographicSize),
               boundMax.y - (Camera.main.orthographicSize)),
               Camera.main.transform.position.z);
    }
    void OnDrawGizmos()
    {
        float verticalHeightSeen = Camera.main.orthographicSize * 2.0f;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3((verticalHeightSeen * Camera.main.aspect), verticalHeightSeen, 0));
    }
    private void PanCamera()
    {

        if (Input.GetMouseButtonDown(0))
        {
            touchStart = GetWorldPosition(groundZ);
            totalClickTime = 0f;

        }
        if (Input.GetMouseButton(0))
        {
            totalClickTime += Time.deltaTime;
            if (totalClickTime >= 0.2f)
            {
                isPanning = true;
            }
            Vector3 direction = touchStart - GetWorldPosition(groundZ);
            Camera.main.transform.position += direction;

        }
        if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(DelaySetPanning());

        }
  
    }
    private void ZoomCamera()
    {
        float scroll = 0;

#if UNITY_ANDROID
        {
            if (Input.touchCount >= 2)
            {
                Vector2 touch0, touch1;
                touch0 = Input.GetTouch(0).position;
                touch1 = Input.GetTouch(1).position;
                scroll = Vector2.Distance(touch0, touch1);
            }
            scroll = Input.GetAxis("Mouse ScrollWheel");

        }

#else
             scroll = Input.GetAxis("Mouse ScrollWheel");
#endif

        if (Camera.main.transform.position.x > boundMax.x || Camera.main.transform.position.x < boundMin.x)
        {
            return;
        }
       
        Camera.main.orthographicSize -= scroll * MouseZoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, ZoomMinBound, ZoomMaxBound);

    }
    IEnumerator DelaySetPanning()
    {
        yield return new WaitForEndOfFrame();
        isPanning = false;

    }
    private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);

    }
}