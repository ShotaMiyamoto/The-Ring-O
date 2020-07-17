using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpWallController : MonoBehaviour
{
    [SerializeField] private float force = 15;
    [SerializeField] private Transform middlePos = default;
    private Animator animator;
    public GameObject[] hitEffect;
    private bool isTouching = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy" || other.gameObject.tag == "LeftPlayer"|| other.gameObject.tag == "RightPlayer")
        {
            if (!isTouching)
            {
                isTouching = true;
                other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                Vector3 dir = middlePos.position - this.gameObject.transform.position;
                other.gameObject.GetComponent<Rigidbody>().AddForce(dir * force, ForceMode.VelocityChange); //質量無視のImpulse

                animator.SetTrigger("isHit"); //ヒットアニメーション
                foreach (GameObject i in hitEffect) //接点にパーティクルの生成
                {
                    Instantiate(i, other.contacts[0].point, i.transform.rotation);
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "LeftPlayer" || collision.gameObject.tag == "RightPlayer")
        {
            isTouching = false;
        }
    }
}
