using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerOneTank;
    public Transform playerTwoTank;
    public Transform tfRef;
    public Camera followCam;
    public int depth = 0;
    public int height = 1;
    void Start()
    {
        if(tfRef == null)
        {
            tfRef = gameObject.GetComponent<Transform>();
        }
        if(followCam == null)
        {
            followCam = gameObject.GetComponent<Camera>();
        }
    }

    void Update()
    {
        FollowPlayer();
    }
    public void FollowPlayer()
    {
        if(gameObject.tag == "CameraFollowOne")
        {
            tfRef.position = playerOneTank.position + new Vector3(0, height, depth);
            tfRef.rotation = playerOneTank.rotation;
        }
        else if(gameObject.tag == "CameraFollowTwo")
        {
            tfRef.position = playerTwoTank.position + new Vector3(0, height, depth);
            tfRef.rotation = playerTwoTank.rotation;
        }
    }
}
