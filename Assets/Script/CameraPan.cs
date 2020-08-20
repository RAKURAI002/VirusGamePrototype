using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    
    float MouseZoomSpeed = 15.0f;
    float ZoomMinBound = 30f;
    float ZoomMaxBound = 130f;

    private Vector3 touchStart;
    public float groundZ = 0;
    float totalClickTime = 0;

    public static bool isPanning { get; set; }
    private void Start()
    {
        transform.position = new Vector3(-35f, 40f, -10);
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
    }
    private void PanCamera()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
          //  Debug.Log("Start drag" + touchStart.ToString());
            touchStart = GetWorldPosition(groundZ);
            
            totalClickTime = 0f;
            
        }
        if (Input.GetMouseButton(0) )
        {
            totalClickTime += Time.deltaTime;
            if(totalClickTime >= 0.2f)
            {
             //   Debug.Log("Long click : " + totalClickTime);
                isPanning = true;

            }
            Vector3 direction = touchStart - GetWorldPosition(groundZ);
            //Debug.Log($"end drag {touchStart.ToString()} - { GetWorldPosition(groundZ).ToString() } = { direction.ToString()}");
            Camera.main.transform.position += direction;
        }
        if (Input.GetMouseButtonUp(0) )
        {
           // Debug.Log("stoppan");

            StartCoroutine(DelaySetPanning());
        }
    }
    private void ZoomCamera()
    {
        float scroll =0;
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
       // Debug.Log(scroll);
        Camera.main.fieldOfView -= scroll * MouseZoomSpeed;
        // set min and max value of Clamp function upon your requirementb
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, ZoomMinBound, ZoomMaxBound);

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