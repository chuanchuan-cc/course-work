using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 5f;
    public bool canBeDragging=false;

    private Vector3 dragOrigin;
    private bool isDragging = false;
    private Vector3 initialPosition;


    public float zoomSpeed = 5f;
    private float minZ = -8f;
    private float maxZ = -3f;
    public bool isCameraMoving=false;

    void Update()
    {
        if(canBeDragging){
  if (Input.GetMouseButtonDown(0)) {
            dragOrigin = Input.mousePosition;
            isDragging = true;

        }


        if (Input.GetMouseButtonUp(0)) {
       
         isDragging = false;
        }


        if (isDragging)
        {

            Vector3 currentMousePos = Input.mousePosition;
            Vector3 difference = Camera.main.ScreenToViewportPoint(dragOrigin - currentMousePos);

            Vector3 move = new Vector3(difference.x * dragSpeed, 0.707f*difference.y * dragSpeed, 0);

            transform.Translate(move, Space.World);

            dragOrigin = currentMousePos;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            Vector3 pos = transform.position;
            pos.z += scroll * zoomSpeed;
            pos.y += scroll * zoomSpeed;
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
            if(pos.z!=minZ&&pos.z!=maxZ)
            transform.position = pos;
        }
    }
        
        }
   
    public void setInitialPosition(){
        initialPosition = Camera.main.transform.position;

    }
    public void resetPosition(){
        StartCoroutine(movePosition(initialPosition));
    }
    private IEnumerator movePosition(Vector3 vs3){
        isCameraMoving=true;
        Vector3 iniPos=Camera.main.transform.position;
        float total=0.3f;
        float time=0;
        while(time<total){
            Vector3 curPosition=Vector3.Lerp(iniPos,vs3,time/total);
            Camera.main.transform.position=curPosition;
            time+=Time.deltaTime;
            yield return null;

        }
        Camera.main.transform.position=vs3;
        isCameraMoving=false;


    }
     }

