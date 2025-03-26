using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Visual Effect Graph
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;

public class HitEffectObj : MonoBehaviour
{
    // VisualEffect定義
    public VisualEffect effect;

    public GameObject gameObject;

    public float time = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        // VisualEffectコンポーネント取得
        effect = GetComponent<VisualEffect>();
        if(effect != null)
        {
            effect.Play();
            Debug.Log("Play Effect");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Destroy(gameObject, time);
    }
}
