using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideController : MonoBehaviour
{
    //＝＝＝＝＝＝＝＝＝＝＝＝衝突処理系＝＝＝＝＝＝＝＝＝＝＝
    private Rigidbody rb;
    [SerializeField] private float force = 5f;
    [SerializeField] private GameObject hitEffectPrefab = default;
    [SerializeField] private GameObject hitHardEffectPrefab = default;
    private float speed = 0f;

    //＝＝＝＝＝＝＝＝＝＝＝＝スペシャル処理系＝＝＝＝＝＝＝＝＝＝＝

    //private SpecialController specialController;

    // Start is called before the first frame update
    void Start()
    {
        //specialController = GetComponent<SpecialController>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = rb.velocity.magnitude;
        force = Mathf.Clamp(speed * 1.2f, 3f, 8.5f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
        {

            //Debug.Log(this.name +":  " + "speed:" + speed + "  force:" + force);
            Vector3 dir = other.gameObject.transform.position - this.transform.position;
            other.gameObject.GetComponent<Rigidbody>().AddForce(dir * force, ForceMode.VelocityChange); //質量無視のImpulse
            
            if(force > 8.2f) //強打なら赤いエフェクト
            {
                Instantiate(hitHardEffectPrefab, other.contacts[0].point, hitHardEffectPrefab.transform.rotation);
            }
            else
            {
                Instantiate(hitEffectPrefab, other.contacts[0].point, hitEffectPrefab.transform.rotation);
            }

            ////スペシャル関係
            //specialController.SetSpecialPoint(force);
        }
    }
}
