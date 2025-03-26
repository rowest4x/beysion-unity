using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Visual Effect Graph
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;

public class HitEffectMusicObj : MonoBehaviour
{
    // VisualEffect��`
    public VisualEffect effect;

    public GameObject gameObject;

    public AudioClip audioClip1;
    private AudioSource audioSource;

    public float time = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        // VisualEffect�R���|�[�l���g�擾
        effect = GetComponent<VisualEffect>();
        if (effect != null)
        {
            effect.Play();
            Debug.Log("Play Effect");
        }

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioClip1;
        audioSource.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Destroy(gameObject, time);
    }
}
