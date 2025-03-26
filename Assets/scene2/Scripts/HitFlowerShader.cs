using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFlowerShader : MonoBehaviour
{
    public GameObject Object;
    public Material materialFlower;

    public float desObjTime = 0.5f;

    private GameObject flowerObj;

    private float startTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        materialFlower = Object.GetComponent<Renderer>().material;

        int rotY = Random.Range(-180, 180);

        if(flowerObj == null)
        {
            //flowerObj = Instantiate(flowerShaderObj, transform.position, Quaternion.Euler(0.0f, rotY, 0.0f)) as GameObject;
            this.transform.rotation = Quaternion.Euler(0.0f, rotY, 0.0f);
            startTime = Time.time;
        }
        

        //Destroy(flowerObj, desFlowerTime);
        Destroy(Object, desObjTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        materialFlower.SetFloat("_dT", Time.time - startTime);

        //Destroy(Object, desObjTime);
    }
}