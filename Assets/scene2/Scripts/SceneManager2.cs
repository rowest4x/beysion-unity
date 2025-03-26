using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Visual Effect Graph
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;
// UI
using UnityEngine.Video;
using UnityEngine.UI;

// 以下が受信に必要
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;


public class SceneManager2 : MonoBehaviour
{
    // UDP通信用
    static UdpClient udp;
    IPEndPoint remoteEP = null;

    //TCP通信用
    // TCP通信用
    TcpClient tcpClient;
    NetworkStream stream;
    bool isCalibrating = false;

    public Camera MainCamera;
    public GameObject CountdownPrefab;
    public GameObject[] HitEffectPrefab;
    public GameObject[] BeyObjPrefab;
    public int effectCount = 0;

    // VideoPlayer定義
    public VideoPlayer video;

    public VideoClip countdownClip; // カウントダウン用の動画をInspectorから設定
    public VideoClip calibrationClip; // キャリブレーション用の動画をInspectorから設定

    // Dictionaryクラスの宣言と初期化
    public Dictionary<int, GameObject> beys = new Dictionary<int, GameObject>();        // Key:BeyID, Value:GameObject
    public Dictionary<int, int> flames = new Dictionary<int, int>();                    // Key:BeyID, Value:Flame

    // TrailでIDを使うのでpublic
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

    // 正規表現を使用してカッコ内の数字を抽出するパターン
    string pattern = @"\((\d+), (\d+), (\d+)\)";    // 1つ分の座標とID
    string hitPattern = @"\((\d+), (\d+)\)";    // hits:以降の座標のみ

    // Start is called before the first frame update
    void Start()
    {
        // 90FPS
        Application.targetFrameRate = 90;

        // Use this for initialization
        int LOCA_LPORT = 50007;

        // UDPクライアントのインスタンス化
        udp = new UdpClient(LOCA_LPORT);

        udp.Client.ReceiveTimeout = 2000;

        // TCP通信関連
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

        // カーソル表示OFF
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

        // 最後の行を返す
        string str = splitLine(text);
        //Debug.Log(str);

        int flame = int.Parse(str.Split(',')[0]);

        // 正規表現にマッチする全ての結果を取得
        foreach (Match match in Regex.Matches(str, pattern))
        {
            int id = int.Parse(match.Groups[1].Value);
            float x = float.Parse(match.Groups[2].Value);
            float y = float.Parse(match.Groups[3].Value);

            // Dictionaryに存在するIDなら座標を更新する
            if (beys.ContainsKey(id))
            {
                beys[id].transform.position = cvtPos(x, y);
                flames[id] = flame;
            }
            else
            {
                // idがKeyに存在しない場合はベイ生成
                GameObject bey = Instantiate(BeyObjPrefab[effectCount], cvtPos(x, y), Quaternion.identity);
                beys.Add(id, bey);
                flames.Add(id, flame);

                // trailエフェクトの時はIDセット
                if(effectCount == 0)
                {
                    BeyObjTrail1 tr1 = bey.GetComponent<BeyObjTrail1>();
                    tr1.setID(id);
                }
                
            }
        }

        // 更新がないベイのDictionaryとオブジェクトを削除する
        foreach(int id in beys.Keys)
        {
            if (flame - flames[id] > 3)
            {
                Destroy(beys[id], 0.5f);
                beys.Remove(id);
                flames.Remove(id);
            }
        }

        // 衝突していたらエフェクト生成
        foreach (Match hitMatch in Regex.Matches(str, hitPattern))
        {
            hitX = float.Parse(hitMatch.Groups[1].Value);
            hitY = float.Parse(hitMatch.Groups[2].Value);

            Instantiate(HitEffectPrefab[effectCount], cvtPos(hitX, hitY), Quaternion.identity);
        }

        // Sキーを入力するとカウントダウン再生
        if (Input.GetKey(KeyCode.S) && !isPlaying && !isCalibrating)
        {
            isPlaying = true;
            StartCoroutine(playVideo(countdownClip));
        }

        // Cキーを入力するとキャリブレーション要求
        if (Input.GetKeyDown(KeyCode.C) && !isCalibrating && !isPlaying)
        {
            StartCoroutine(SendCalibrationRequest());
        }

        changeEffect();
    }

    private string splitLine(string receivedData)
    {
        // 受信したデータの最後の行を出力する
        return receivedData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).Last();

    }

    private Vector3 cvtPos(float x, float y)
    {
        // pythonデータの調整

        // Desktop
        //x = (-(x / 335.0f) * 13.5f + 8.0f) * 10.0f;    // プロジェクターから見て左右
        //y = (-(y / 350.0f) * 13.0f + 6.0f) * 10.0f;     // プロジェクターから見て上下

        // Laptop?
        x = (-(x / 335.0f) * 13.5f + 7.625f) * 10.0f;    // プロジェクターから見て左右
        y = (-(y / 350.0f) * 13.0f + 6.25f) * 10.0f;     // プロジェクターから見て上下

        return new Vector3(x, 1.0f, y);
    }

    // エフェクトの切り替え入力
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

    // キャリブレーション要求
    private IEnumerator SendCalibrationRequest()
    {
        isCalibrating = true;

        // 動画を再生
        StartCoroutine(playVideo(calibrationClip));

        // 3秒待ってからキャリブレーション要求送信
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

        // 応答を待つ（必要なら）
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

    // 動画再生
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
