using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsDisplay : MonoBehaviour
{

    // ïœêî
    int frameCount;
    float prevTime;
    float fps;

    // èâä˙âªèàóù
    void Start()
    {
        // ïœêîÇÃèâä˙âª
        frameCount = 0;
        prevTime = 0.0f;
    }

    // çXêVèàóù
    void FixedUpdate()
    {
        frameCount++;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            fps = frameCount / time;
            Debug.Log(fps);

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
    }

    // ï\é¶èàóù
    private void OnGUI()
    {
        GUILayout.Label(fps.ToString());
    }
}
