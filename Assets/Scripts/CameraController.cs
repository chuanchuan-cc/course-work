using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 5f;
    public bool canBeDragging=false;

    private Vector3 dragOrigin;
    private bool isDragging = false;
    private Vector3 initialPosition;


    public float zoomSpeed = 5f;
    public float minZ = -8f;
    public float maxZ = -3f;

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
        Camera.main.transform.position=initialPosition;
    }
     }

