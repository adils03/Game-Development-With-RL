using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class DragCamera : MonoBehaviour
{
    private Vector3 ResetCamera;
    private Vector3 Origin;
    private Vector3 Diference;
    private bool Drag = false;
    public float zoomSpeed = 4.0f;
    public float targetOrtho;
    public float minOrtho = 1.0f;
    public float maxOrtho = 20.0f;
    void Start()
    {
        ResetCamera = Camera.main.transform.position;
        targetOrtho = Camera.main.orthographicSize;
    }
    void LateUpdate()
    {
        #if UNITY_ANDROID
        DraggingForAndroid();
        #elif UNITY_STANDALONE
        Dragging();
        #endif
    }
    private void Update() { 
        #if UNITY_ANDROID
        ZoomForAndroid();
        #elif UNITY_STANDALONE
        Zoom(); 
        #endif 
    }

    void Dragging(){
        if (Input.GetMouseButton(1))
        {
            Diference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (Drag == false)
            {
                Drag = true;
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            Drag = false;
        }
        if (Drag == true)
        {
            Camera.main.transform.position = Origin - Diference;
        }
        //RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
        if (Input.GetKeyDown(KeyCode.R))
        {
            Camera.main.transform.position = ResetCamera;
        }
    }
    void Zoom(){
        
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        if (scrollData != 0.0f)
            targetOrtho -= scrollData * zoomSpeed;

        targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);

        Camera.main.orthographicSize = targetOrtho; // Kamerayı hemen hedef yakınlaştırma seviyesine ayarlıyoruz.
    }
    void DraggingForAndroid(){
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Diference = (Camera.main.ScreenToWorldPoint(touch.position)) - Camera.main.transform.position;
                if (Drag == false)
                {
                    Drag = true;
                    Origin = Camera.main.ScreenToWorldPoint(touch.position);
                }
            }
            else
            {
                Drag = false;
            }
            if (Drag == true)
            {
                Camera.main.transform.position = Origin - Diference;
            }
        }
        //RESET CAMERA TO STARTING POSITION WITH DOUBLE TAP
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(1).phase == TouchPhase.Began)
        {
            Camera.main.transform.position = ResetCamera;
        }
    }
    void ZoomForAndroid(){
        
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            targetOrtho += deltaMagnitudeDiff * zoomSpeed;
            targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);

            Camera.main.orthographicSize = targetOrtho;
        }
    }
}

