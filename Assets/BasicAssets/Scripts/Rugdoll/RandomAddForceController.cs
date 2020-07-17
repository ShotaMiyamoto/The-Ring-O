using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAddForceController : MonoBehaviour
{
    private Rigidbody[] rbs;
 
    // Start is called before the first frame update
    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();  
        foreach(Rigidbody i in rbs)
        {
            i.AddForce(new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5)), ForceMode.VelocityChange);
        }
    }

}
