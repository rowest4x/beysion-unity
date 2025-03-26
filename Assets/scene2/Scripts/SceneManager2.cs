using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Visual Effect Graph
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;
// UI
using UnityEngine.Video;
using UnityEngine.UI;

// �ȉ�����M�ɕK�v
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;


public class SceneManager2 : MonoBehaviour
{
    // UDP�ʐM�p
    static UdpClient udp;
    IPEndPoint remoteEP = null;

    //TCP�ʐM�p
    // TCP�ʐM�p
    TcpClient tcpClient;
    NetworkStream stream;
    bool isCalibrating = false;

    public Camera MainCamera;
    public GameObject CountdownPrefab;
    public GameObject[] HitEffectPrefab;
    public GameObject[] BeyObjPrefab;
    public int effectCount = 0;

    // VideoPlayer��`
    public VideoPlayer video;

    public VideoClip countdownClip; // �J�E���g�_�E���p�̓����Inspector����ݒ�
    public VideoClip calibrationClip; // �L�����u���[�V�����p�̓����Inspector����ݒ�

    // Dictionary�N���X�̐錾�Ə�����
    public Dictionary<int, GameObject> beys = new Dictionary<int, GameObject>();        // Key:BeyID, Value:GameObject
    public Dictionary<int, int> flames = new Dictionary<int, int>();                    // Key:BeyID, Value:Flame

    // Trail��ID���g���̂�public
    public int id1, id2;    // bey1 = id1, bey2 = id2
    // bey1 position
    public float x1 = 0.0f;
    public float y1 = 0.0f;
    // bey2 position
    public float x2 = 0.0f;
    public float y2 = 0.0f;
    // hit position
    public float hitX = 0.0f;
    public float hitY = 0.0f;


    public float cameraDistance = 50.0f;

    private bool isPlaying = false;
    private bool isChanging = false;

    // ���K�\�����g�p���ăJ�b�R���̐����𒊏o����p�^�[��
    string pattern = @"\((\d+), (\d+), (\d+)\)";    // 1���̍��W��ID
    string hitPattern = @"\((\d+), (\d+)\)";    // hits:�ȍ~�̍��W�̂�

    // Start is called before the first frame update
    void Start()
    {
        // 90FPS
        Application.targetFrameRate = 90;

        // Use this for initialization
        int LOCA_LPORT = 50007;

        // UDP�N���C�A���g�̃C���X�^���X��
        udp = new UdpClient(LOCA_LPORT);

        udp.Client.ReceiveTimeout = 2000;

        // TCP�ʐM�֘A
        try
        {
            tcpClient = new TcpClient("127.0.0.1", 50008);
            stream = tcpClient.GetStream();
            Debug.Log("TCP connection completed");
        }
        catch (Exception e)
        {
            Debug.LogError("TCP connection failed: " + e.Message);
        }

        // �J�[�\���\��OFF
        Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // get UDP data
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        string text = "empty";
        while(udp.Available > 0)
        {
            byte[] data = udp.Receive(ref remoteEP);
            text = Encoding.UTF8.GetString(data);
        }

        if (text == "empty") return;

        // �Ō�̍s��Ԃ�
        string str = splitLine(text);
        //Debug.Log(str);

        int flame = int.Parse(str.Split(',')[0]);

        // ���K�\���Ƀ}�b�`����S�Ă̌��ʂ��擾
        foreach (Match match in Regex.Matches(str, pattern))
        {
            int id = int.Parse(match.Groups[1].Value);
            float x = float.Parse(match.Groups[2].Value);
            float y = float.Parse(match.Groups[3].Value);

            // Dictionary�ɑ��݂���ID�Ȃ���W���X�V����
            if (beys.ContainsKey(id))
            {
                beys[id].transform.position = cvtPos(x, y);
                flames[id] = flame;
            }
            else
            {
                // id��Key�ɑ��݂��Ȃ��ꍇ�̓x�C����
                GameObject bey = Instantiate(BeyObjPrefab[effectCount], cvtPos(x, y), Quaternion.identity);
                beys.Add(id, bey);
                flames.Add(id, flame);

                // trail�G�t�F�N�g�̎���ID�Z�b�g
                if(effectCount == 0)
                {
                    BeyObjTrail1 tr1 = bey.GetComponent<BeyObjTrail1>();
                    tr1.setID(id);
                }
                
            }
        }

        // �X�V���Ȃ��x�C��Dictionary�ƃI�u�W�F�N�g���폜����
        foreach(int id in beys.Keys)
        {
            if (flame - flames[id] > 3)
            {
                Destroy(beys[id], 0.5f);
                beys.Remove(id);
                flames.Remove(id);
            }
        }

        // �Փ˂��Ă�����G�t�F�N�g����
        foreach (Match hitMatch in Regex.Matches(str, hitPattern))
        {
            hitX = float.Parse(hitMatch.Groups[1].Value);
            hitY = float.Parse(hitMatch.Groups[2].Value);

            Instantiate(HitEffectPrefab[effectCount], cvtPos(hitX, hitY), Quaternion.identity);
        }

        // S�L�[����͂���ƃJ�E���g�_�E���Đ�
        if (Input.GetKey(KeyCode.S) && !isPlaying && !isCalibrating)
        {
            isPlaying = true;
            StartCoroutine(playVideo(countdownClip));
        }

        // C�L�[����͂���ƃL�����u���[�V�����v��
        if (Input.GetKeyDown(KeyCode.C) && !isCalibrating && !isPlaying)
        {
            StartCoroutine(SendCalibrationRequest());
        }

        changeEffect();
    }

    private string splitLine(string receivedData)
    {
        // ��M�����f�[�^�̍Ō�̍s���o�͂���
        return receivedData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).Last();

    }

    private Vector3 cvtPos(float x, float y)
    {
        // python�f�[�^�̒���

        // Desktop
        //x = (-(x / 335.0f) * 13.5f + 8.0f) * 10.0f;    // �v���W�F�N�^�[���猩�č��E
        //y = (-(y / 350.0f) * 13.0f + 6.0f) * 10.0f;     // �v���W�F�N�^�[���猩�ď㉺

        // Laptop?
        x = (-(x / 335.0f) * 13.5f + 7.625f) * 10.0f;    // �v���W�F�N�^�[���猩�č��E
        y = (-(y / 350.0f) * 13.0f + 6.25f) * 10.0f;     // �v���W�F�N�^�[���猩�ď㉺

        return new Vector3(x, 1.0f, y);
    }

    // �G�t�F�N�g�̐؂�ւ�����
    private void changeEffect()
    {
        if (isChanging) return;

        isChanging = true;

        bool inputKey = false;

        if (Input.GetKeyDown(KeyCode.RightArrow) && !inputKey)
        {
            inputKey = true;
            effectCount++;

            if (effectCount == BeyObjPrefab.Length)
            {
                effectCount = 0;
            }

            Instantiate(HitEffectPrefab[effectCount], new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                inputKey = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && !inputKey)
        {
            inputKey = true;
            effectCount--;

            if (effectCount < 0)
            {
                effectCount = BeyObjPrefab.Length - 1;
            }

            Instantiate(HitEffectPrefab[effectCount], new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity);

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                inputKey = false;
            }
        }

        isChanging = false;
    }

    // �L�����u���[�V�����v��
    private IEnumerator SendCalibrationRequest()
    {
        isCalibrating = true;

        // ������Đ�
        StartCoroutine(playVideo(calibrationClip));

        // 3�b�҂��Ă���L�����u���[�V�����v�����M
        yield return new WaitForSeconds(3f);

        try
        {
            Debug.Log("send calibration request");
            byte[] data = Encoding.UTF8.GetBytes("calibrate");
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogError("TCP transmission failed: " + e.Message);
        }

        // ������҂i�K�v�Ȃ�j
        byte[] buffer = new byte[1024];
        string response = "";
        float timeout = 4.0f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            if (stream.DataAvailable)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("response received: " + response);
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        isCalibrating = false;
    }

    // ����Đ�
    private IEnumerator playVideo(VideoClip clip)
    {
        isPlaying = true;

        GameObject countdownObj = Instantiate(CountdownPrefab, Vector3.zero, Quaternion.identity);
        video = countdownObj.GetComponentInChildren<VideoPlayer>();

        Canvas canvas = CountdownPrefab.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = MainCamera;
        canvas.planeDistance = cameraDistance;

        video.clip = clip;
        video.Prepare();
        while (!video.isPrepared)
            yield return null;

        video.Play();
        while (video.isPlaying)
            yield return null;

        Destroy(countdownObj);
        isPlaying = false;
    }
}
