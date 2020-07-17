using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : SingletonMonoBehaviour<CameraManager>
{
    public CinemachineVirtualCamera[] vCamera;
    public CinemachineVirtualCamera currentMainCamera;
    private int currentCameraNum = 0;
    public int GetCurrentCameraNum { get { return currentCameraNum; } }

    public void ChangeCamera(int cameraNum)
    {
        if(cameraNum >= vCamera.Length)
        {
            Debug.Log("そんなカメラはない");
        }
        else
        {
            currentMainCamera.Priority = 10;
            vCamera[cameraNum].Priority = 11;
            currentMainCamera = vCamera[cameraNum];
            currentCameraNum = cameraNum;
        }
    }

    public void ChangeLookAt(int cameraNum, GameObject obj)
    {
        vCamera[cameraNum].LookAt = obj.transform;
    }

    public void ChangeFollow(int cameraNum, GameObject obj)
    {
        vCamera[cameraNum].Follow = obj.transform;
    }

}
