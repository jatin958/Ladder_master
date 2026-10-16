using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ladder : MonoBehaviour
{
   
    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void destroyLadder()
    {
        rb.isKinematic = false;
        var randX = Random.Range(-3f, 3f);
        var randz = Random.Range(-3f, 3f);
        var dir =  new Vector3(randX, 1, randz);
        rb.AddForce(dir * 2f, ForceMode.Impulse);
        Destroy(gameObject, 2f);
    }
}
