using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    private MoneyManager moneyManager;
    private Animator anim;
    public GameObject starParticle;
    private bool isTouched = false;

    private int timeLimit = 0;
    private float timeElapsed = 0f;
    private bool isActivated = false;

    private bool RandomizeAppear()
    {
        int i = Random.Range(0, 11);

        //33％の確率で出現させる
        switch (i)
        {
            case 0:
            case 1:
            case 2:
                Debug.Log("Coin出現する");
                return true;

            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
                Debug.Log("Coin出現しない");
                return false;

            default:
                return false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //出現するなら生成の準備をさせる
        if (RandomizeAppear())
        {
            moneyManager = GameObject.FindGameObjectWithTag("MoneyManager").GetComponent<MoneyManager>();
            this.transform.position = new Vector3(Random.Range(-8.0f, 8f), this.transform.position.y, Random.Range(-8.0f, 8f));
            anim = this.GetComponent<Animator>();

            anim.enabled = false;
            this.GetComponent<MeshRenderer>().enabled = false;

            timeLimit = Random.Range(4, 16);
        }
        else
        {
            DestroySelf();
        }

    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        
        if(timeElapsed > timeLimit && !isActivated)
        {
            anim.enabled = true;
            this.GetComponent<MeshRenderer>().enabled = true;
            isActivated = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (!isTouched)
            {
                anim.SetTrigger("Get");
                Instantiate(starParticle, this.transform.position, this.transform.rotation);
                moneyManager.ChangeCurrentMoney(moneyManager.GetCurrentMoney + 2);
                isTouched = true;
            }
        }


        if (other.tag == "Enemy")
        {
            if (!isTouched)
            {
                Instantiate(starParticle, this.transform.position, this.transform.rotation);
                anim.SetTrigger("Get");
                isTouched = true;
            }
        }
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

}
