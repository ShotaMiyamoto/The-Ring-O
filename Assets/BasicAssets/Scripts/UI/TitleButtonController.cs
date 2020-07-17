using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleButtonController : MonoBehaviour
{
    public GameObject frontImage;
    public GameObject leftImage;
    public GameObject rightImage;
    public GameObject backImage;

    public TitleManager titleManager;

    private GameObject currentActiveImage;

    private void Start()
    {
        currentActiveImage = frontImage;
        ChangeImage();
    }

    public void ChangeImage()
    {
        switch (titleManager.GetCurrentCamDir)
        {
            case TitleManager.CameraDir.Forward:

                currentActiveImage.SetActive(false); //現在アクティブなイメージを非アクティブ化
                frontImage.SetActive(true); //方向に対応したイメージをアクティブ化
                currentActiveImage = frontImage;　//現在アクティブなイメージに設定

                break;

            case TitleManager.CameraDir.Left:

                currentActiveImage.SetActive(false); //現在アクティブなイメージを非アクティブ化
                leftImage.SetActive(true); //方向に対応したイメージをアクティブ化
                currentActiveImage = leftImage;　//現在アクティブなイメージに設定

                break;

            case TitleManager.CameraDir.Right:

                currentActiveImage.SetActive(false); //現在アクティブなイメージを非アクティブ化
                rightImage.SetActive(true); //方向に対応したイメージをアクティブ化
                currentActiveImage = rightImage;　//現在アクティブなイメージに設定

                break;

            case TitleManager.CameraDir.Back:

                currentActiveImage.SetActive(false); //現在アクティブなイメージを非アクティブ化
                backImage.SetActive(true); //方向に対応したイメージをアクティブ化
                currentActiveImage = backImage;　//現在アクティブなイメージに設定

                break;

            default:
                Debug.Log("そんなイメージはアクティブにできない");
                break;
        }
    }

}
