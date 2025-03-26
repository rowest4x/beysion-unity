using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Visual Effect Graph
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;

public class HitEffectExplosion : MonoBehaviour
{
    // VisualEffect定義
    public VisualEffect effect;

    public GameObject gameObject;

    public AudioClip audioClip1;
    private AudioSource audioSource;

    public float time = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        // VisualEffectコンポーネント取得
        effect = GetComponent<VisualEffect>();
        if (effect != null)
        {
            effect.Play();
        }

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioClip1;
        audioSource.Play();
    }

    void FixedUpdate()
    {
        Destroy(gameObject, time);
    }
}
