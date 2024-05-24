using GoodLuckValley.Events;
using GoodLuckValley.Persistence;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    protected struct ImageData
    {
        public Image Image;
        public Color Color;

        public ImageData(Image image)
        {
            Image = image;
            Color = image.color;
        }
    }

    protected struct TextData
    {
        public Text Text;
        public Color Color;

        public TextData(Text text)
        {
            Text = text;
            Color = text.color;
        }
    }

    #region EVENTS
    [Header("Events")]
    [SerializeField] private GameEvent onUpdatePaused;
    #endregion

    #region FIELDS
    private float fadeDuration = 0.2f;
    private List<ImageData> imageDatas = new List<ImageData>();
    private List<TextData> textDatas = new List<TextData>();
    #endregion

    #region PROPERTIES
    public bool Paused { get; private set; }
    #endregion

    public void Awake()
    {
        // Store images and texts into lists
        List<Image> images = GetComponentsInChildren<Image>().ToList();
        List<Text> texts = GetComponentsInChildren<Text>().ToList();

        // Add ImageDatas
        foreach (Image image in images)
        {
            imageDatas.Add(new ImageData(image));
        }

        // Add TextDatas
        foreach (Text text in texts)
        {
            textDatas.Add(new TextData(text));
        }
    }

    public void Start()
    {
        // Set paused to false initially
        Paused = false;

        // Hide the UI
        HideUI();
    }

    /// <summary>
    /// Toggle the pause menu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    public void TogglePause(Component sender, object data)
    {
        // Toggle paused
        Paused = !Paused;

        // Set effects
        if (Paused)
        {
            // Freeze time
            Time.timeScale = 0f;
            ShowUI();
        } else
        {
            // Resume time
            Time.timeScale = 1f;
            HideUI();
        }

        // Update paused for any listeners
        // Calls to:
        //  - PlayerInputHandler.SetPaused
        onUpdatePaused.Raise(this, Paused);
    }

    /// <summary>
    /// Resume from the Pause Menu
    /// </summary>
    public void Resume()
    {
        // Since only called from the menu, TogglePause will unpause
        TogglePause(this, null);
    }

    /// <summary>
    /// Save from the Pause Menu
    /// </summary>
    public void Save()
    {
        // Save the game
        SaveLoadSystem.Instance.SaveGame();
    }

    public void ShowUI() => StartCoroutine(FadeIn());

    public void HideUI() => StartCoroutine(FadeOut());

    private IEnumerator FadeIn()
    {
        List<Coroutine> fadeCoroutines = new List<Coroutine>();

        foreach(ImageData imageData in imageDatas)
        {
            fadeCoroutines.Add(StartCoroutine(FadeInElement(imageData.Image, imageData.Color, fadeDuration)));
        }

        foreach(TextData textData in textDatas)
        {
            fadeCoroutines.Add(StartCoroutine(FadeInElement(textData.Text, textData.Color, fadeDuration)));
        }

        foreach(Coroutine coroutine in fadeCoroutines)
        {
            yield return coroutine;
        }
    }

    private IEnumerator FadeInElement(Graphic uiElement, Color targetColor, float duration)
    {
        // Create a timer and establish the starting color
        float elapsedTime = 0f;
        Color startColor = uiElement.color;

        // Loop while the timer is less than the duration
        while(elapsedTime < duration)
        {
            // Increase by unscaled deltaTime (pause menu freezes scaled deltaTime)
            elapsedTime += Time.unscaledDeltaTime;

            // Adjust alpha
            float alpha = Mathf.Clamp(elapsedTime / duration, 0f, targetColor.a);

            // Set alpha
            uiElement.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // Ensure the final color is set to the final color
        uiElement.color = targetColor;
    }

    private IEnumerator FadeOut()
    {
        List<Coroutine> fadeCoroutines = new List<Coroutine>();

        foreach (ImageData imageData in imageDatas)
        {
            fadeCoroutines.Add(StartCoroutine(FadeOutElement(imageData.Image, imageData.Color, fadeDuration)));
        }

        foreach (TextData textData in textDatas)
        {
            fadeCoroutines.Add(StartCoroutine(FadeOutElement(textData.Text, textData.Color, fadeDuration)));
        }

        foreach (Coroutine coroutine in fadeCoroutines)
        {
            yield return coroutine;
        }
    }

    private IEnumerator FadeOutElement(Graphic uiElement, Color startColor, float duration)
    {
        // Create a timer and establish the starting color
        float elapsedTime = 0f;
        Color color = uiElement.color;

        // Loop while the timer is less than the duration
        while (elapsedTime < duration)
        {
            // Increase by unscaled deltaTime (pause menu freezes scaled deltaTime)
            elapsedTime += Time.unscaledDeltaTime;

            // Adjust alpha
            float alpha = startColor.a - Mathf.Clamp(elapsedTime / duration, 0f, startColor.a);

            // Set alpha
            uiElement.color = new Color(color.r, color.g, color.b, alpha);

            yield return null;
        }

        // Ensure the final color is set to the final color
        uiElement.color = new Color(color.r, color.g, color.b, 0f);
    }
}
