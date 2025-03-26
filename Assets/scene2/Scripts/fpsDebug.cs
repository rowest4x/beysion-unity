using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsDisplay : MonoBehaviour
{

    // �ϐ�
    int frameCount;
    float prevTime;
    float fps;

    // ����������
    void Start()
    {
        // �ϐ��̏�����
        frameCount = 0;
        prevTime = 0.0f;
    }

    // �X�V����
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

    // �\������
    private void OnGUI()
    {
        GUILayout.Label(fps.ToString());
    }
}
