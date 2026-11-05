using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using System.Runtime.CompilerServices;
using System;

// 遷移先シーン番号
public enum SceneID
{
    Title = 0,
    CharacterSelect = 1,
    SoloGame = 2,
    TrainingSelect = 3,
    Option = 4,
    PassTraining = 5,
    Result = 6,
}

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance { get; private set; }

    // デバイス情報
    public enum InputDeviceInfo
    {
        MouseAndKeyboard = 0,
        Gamepad = 1,
    }
    public InputDeviceInfo finallyInfo;

    // フェードエフェクト用キャンバスグループ
    [SerializeField]
    private CanvasGroup fadeCanvas;

    // フェード速度
    [SerializeField]
    private float fadeDuration = 0.5f;

    // 遷移可能かどうかのフラグ
    private bool isTransitioning = false;
    
    // 通ったシーン番号の履歴
    private Stack<int> sceneHistory = new();

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
    }

    public void ClearHistory()
    {
        sceneHistory.Clear();
    }

    // シーンの最後に使用した入力デバイス情報を保存する
    private void RecordFinalyInputDeviceInfo(bool gamepadButtonStatus)
    {
        if(Keyboard.current.anyKey.wasPressedThisFrame)
        {
            finallyInfo = InputDeviceInfo.MouseAndKeyboard;
            return;
        }
        else if(Gamepad.current != null && gamepadButtonStatus)
        {
            finallyInfo = InputDeviceInfo.Gamepad;
        }
    }

    // 最後に使用したデバイス情報の取得
    public bool GetFinallyDeviceInfo()
    {
        if (finallyInfo == InputDeviceInfo.Gamepad)
            return true;
        else return false;
    }

    // シーン切り替え
    public void LoadScene(int sceneId)
    {
        if (isTransitioning) return;
        Time.timeScale = 1f;
        // デバイス情報を保存
        bool isPadDecision = false;
        if (Gamepad.current != null)
        {
            isPadDecision = Gamepad.current.aButton.wasPressedThisFrame;
        }
        RecordFinalyInputDeviceInfo(isPadDecision);
        // 前のシーン番号として記録
        int previousId = SceneManager.GetActiveScene().buildIndex;
        sceneHistory.Push(previousId);
        // 遷移開始
        StartCoroutine(LoadSceneCoroutine(sceneId));
    }

    // 前のシーンに戻る
    public void LoadPreviousScene()
    {
        if (isTransitioning) return;

        Time.timeScale = 1f;

        // デバイス情報を保存
        bool isPadDecision = false;
        if (Gamepad.current != null)
        {
            isPadDecision = Gamepad.current.aButton.wasPressedThisFrame;
        }
        RecordFinalyInputDeviceInfo(isPadDecision);
        // 履歴から前のシーンのインデックスを取得して遷移
        if (sceneHistory.Count > 0)
        {
            int sceneId = sceneHistory.Pop();
            StartCoroutine(LoadSceneCoroutine(sceneId));
        }
        else
        {
            StartCoroutine(LoadSceneCoroutine((int)SceneID.Title));
        }
    }

    public void LoadNewScene(int sceneId)
    {
        if (isTransitioning) return;
        Time.timeScale = 1f;
        // デバイス情報を保存
        bool isPadDecision = false;
        if (Gamepad.current != null)
        {
            isPadDecision = Gamepad.current.aButton.wasPressedThisFrame;
        }
        RecordFinalyInputDeviceInfo(isPadDecision);
        
        // 遷移開始
        StartCoroutine(LoadSceneCoroutine(sceneId));
    }

    private IEnumerator LoadWhenReady(int sceneId)
    {
        //　シーンロード
        AsyncOperation operation =
            SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation = false; // 自動で切り替わらない

        // ロード終了時progressを0.9で止める
        while(operation.progress < 0.9f)
        {
            yield return null;
        }

        // プレイヤーの入力を待つ
        while(!Keyboard.current.enterKey.wasPressedThisFrame)
        {
            yield return null;
        }

        operation.allowSceneActivation = true; // シーンを切り替える
    }

    private IEnumerator LoadSceneCoroutine(int sceneId)
    {
        isTransitioning = true;

        // フェードアウト
        yield return Fade(0.0f, 1.0f);

        //　シーンロード
        AsyncOperation operation =
            SceneManager.LoadSceneAsync(sceneId);

        while (!operation.isDone)
        {
            yield return null;
        }

        // フェードイン
        yield return Fade(1.0f, 0.0f);

        isTransitioning = false;
    }

    private IEnumerator Fade(float start, float end)
    {
        float time = 0.0f;

        while(time < fadeDuration)
        {
            time += Time.deltaTime;

            float t = time / fadeDuration;

            fadeCanvas.alpha = Mathf.Lerp(start, end, t);

            yield return null;
        }
        fadeCanvas.alpha = end;
    }
}
