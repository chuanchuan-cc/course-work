using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 followSpace;
    public Transform followPlayer;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.3f;
    private Vector3 originPosition;

    void Start()
    {
        if (followPlayer == null)
        {
            Debug.LogError("CameraFollow: followPlayer is not assigned!");
            return;
        }
        


        followSpace = new Vector3(6f, -4f, -2f)-followPlayer.position;
        originPosition=new Vector3(0f,-6.4f,-7.6f);
    }

    void LateUpdate()
    {
        if (RunGame.setFollow&&followPlayer != null )
        {
            Vector3 targetPosition = followPlayer.position + followSpace;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }

    public void SetTarget(Transform target)
    {
        followPlayer = target;
       
    }
    public void reset(){
        velocity = Vector3.zero;
        transform.position=originPosition;
    }
}
