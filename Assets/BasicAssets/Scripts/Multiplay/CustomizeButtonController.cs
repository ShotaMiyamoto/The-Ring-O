using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeButtonController : MonoBehaviour
{
    private GameObject selectedIcon = default;
    [SerializeField] private int num = 0;
    [SerializeField] private CostumeCustomizer costumeCustomizer = default;

    private void Start()
    {
        selectedIcon = this.transform.GetChild(0).gameObject;
    }

    public void PushedBColBtnLeft()
    {
        costumeCustomizer.SetBodyColorLeft = num;
        costumeCustomizer.ChangeSelectBColIconLeft(selectedIcon);
        selectedIcon.SetActive(true);
    }

    public void PushedCosBtnLeft()
    {
        costumeCustomizer.SetCostumeLeft = num;
        costumeCustomizer.ChangeSelectCosIconLeft(selectedIcon);
        selectedIcon.SetActive(true);
    }

    public void PushedBColBtnRight()
    {
        costumeCustomizer.SetBodyColorRight = num;
        costumeCustomizer.ChangeSelectBColIconRight(selectedIcon);
        selectedIcon.SetActive(true);
    }

    public void PushedCosBtnRight()
    {
        costumeCustomizer.SetCostumeRight = num;
        costumeCustomizer.ChangeSelectCosIconRight(selectedIcon);
        selectedIcon.SetActive(true);
    }
}
