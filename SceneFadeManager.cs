using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFadeManager : MonoBehaviour
{
    public enum SCENE_STATUS
    {
        START, LOGO, MEETING, GAME_PLAY, GAME_OVER, GAME_CLEAR, AUTO, NONE
    }

    public enum FADE_STATUS
    {
        FADE_IN, FADE_OUT, AUTO, NONE
    }

    [Header("Fade Settings")]
    [SerializeField] private float defaultFadeDuration = 0.5f;
    [SerializeField] private Color fadeColor = Color.black;

    public bool FadeStop { get; private set; } = true;

    private Canvas fadeCanvas;
    private CanvasGroup fadeCanvasGroup;
    private Image fadeImage;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        CreateFadeCanvas();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += SceneChangeEvent;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneChangeEvent;
    }

    private void CreateFadeCanvas()
    {
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(transform);

        fadeCanvas = canvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        fadeCanvasGroup = canvasObj.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasGroup.interactable = false;

        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);

        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = fadeColor;

        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
    /// <summary>
    /// 画面のフェードだけやる（シーン遷移なし）
    /// </summary>
    /// <param name="status"></param>
    /// <param name="duration"></param>
    public void FadeSystem(FADE_STATUS status = FADE_STATUS.NONE, float duration = -1f)
    {
        if (duration <= 0f)
            duration = defaultFadeDuration;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(StartFadeSystem(status, duration));
    }

    private IEnumerator StartFadeSystem(FADE_STATUS status, float duration)
    {
        FadeStop = false;

        switch (status)
        {
            case FADE_STATUS.FADE_IN:
                yield return FadeTo(1f, duration);
                break;

            case FADE_STATUS.FADE_OUT:
                yield return FadeTo(0f, duration);
                break;

            case FADE_STATUS.AUTO:
                float target = fadeCanvasGroup.alpha >= 0.5f ? 0f : 1f;
                yield return FadeTo(target, duration);
                break;

            case FADE_STATUS.NONE:
                break;
        }

        FadeStop = true;
        fadeCoroutine = null;
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        fadeCanvasGroup.blocksRaycasts = true;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        fadeCanvasGroup.blocksRaycasts = targetAlpha > 0f;
    }

    /// <summary>
    /// 遷移したいシーンを指定して即座に飛ぶ
    /// </summary>
    /// <param name="scene">飛びたいシーンを指定する。指定にはenumを使用する事</param>
    public void SceneChangeSystem(SCENE_STATUS scene = SCENE_STATUS.NONE)
    {
        string sceneName = GetSceneName(scene);

        if (string.IsNullOrEmpty(sceneName))
            return;

        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// フェード付きシーン遷移。遷移したいシーンはenumを使用する事
    /// </summary>
    /// <param name="duration">フェード速度を指定する</param>
    /// <param name="status">指定しない場合、自動的に登録された次のシーンに飛ぶ。</param>
    public void SceneOutAndChangeSystem(float duration = -1f, SCENE_STATUS status = SCENE_STATUS.AUTO)
    {
        if (duration <= 0f)
            duration = defaultFadeDuration;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutSceneChangeStart(duration, status));
    }

    private IEnumerator FadeOutSceneChangeStart(float duration, SCENE_STATUS status)
    {
        FadeStop = false;

        yield return FadeTo(1f, duration);

        string sceneName = GetSceneName(status);

        if (!string.IsNullOrEmpty(sceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
                yield return null;
        }

        yield return FadeTo(0f, duration);

        FadeStop = true;
        fadeCoroutine = null;
    }

    /// <summary>
    /// ゲームに応じてシーン名は都度変更すること
    /// </summary>
    /// <param name="scene">どのシーンに遷移するのかを指定する。指定方法は同ファイルのenumを使用する事</param>
    /// <returns></returns>
    private string GetSceneName(SCENE_STATUS scene)
    {
        string nowSceneName = SceneManager.GetActiveScene().name;

        switch (scene)
        {
            case SCENE_STATUS.LOGO:
                return "Logo";

            case SCENE_STATUS.START:
                return "Start";

            case SCENE_STATUS.MEETING:
                return "Meeting";

            case SCENE_STATUS.GAME_PLAY:
                return "GamePlay";

            case SCENE_STATUS.GAME_OVER:
                return "GameOver";

            case SCENE_STATUS.GAME_CLEAR:
                return "GameClear";

            case SCENE_STATUS.AUTO:
                if (nowSceneName == "Logo") return "Start";
                if (nowSceneName == "Start") return "Meeting";
                if (nowSceneName == "Meeting") return "GamePlay";
                return "Start";

            case SCENE_STATUS.NONE:
            default:
                return null;
        }
    }

    private void SceneChangeEvent(Scene from, Scene to)
    {
        Debug.Log($"{to.name}に遷移");
    }
}