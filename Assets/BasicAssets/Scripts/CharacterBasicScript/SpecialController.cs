using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialController : MonoBehaviour
{

    [SerializeField] private float specialPower = 0f;
    [SerializeField] private float maxPoint = 50f;
    [SerializeField] private Slider specialSlider = default;

    //private bool canDoSpecial = false;

    // Start is called before the first frame update
    void Start()
    {
        specialSlider.value = specialPower;
        specialSlider.maxValue = maxPoint;
    }

    // Update is called once per frame
    void Update()
    {
        specialSlider.value = specialPower;
    }

    public void SetSpecialPoint(float point)
    {
        specialPower += point;
        if(specialPower >= maxPoint)
        {
            specialPower = maxPoint;

        }
    }
}
