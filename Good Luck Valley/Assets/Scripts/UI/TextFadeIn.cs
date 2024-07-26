using System.Collections;
using UnityEngine;
using TMPro;

public class TextFadeIn : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public float fadeInDuration = 1f; // Duration for each word to fade in
    public float delayBetweenWords = 0.5f; // Delay between each word fade-in

    void Start()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TMP_Text>();
        }

        if (textMeshPro != null)
        {
            StartCoroutine(FadeInWords());
        }
    }

    IEnumerator FadeInWords()
    {
        textMeshPro.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMeshPro.textInfo;

        for (int w = 0; w < textInfo.wordCount; w++)
        {
            TMP_WordInfo wordInfo = textInfo.wordInfo[w];
            int firstCharacterIndex = wordInfo.firstCharacterIndex;
            int lastCharacterIndex = wordInfo.lastCharacterIndex;

            // Include punctuation immediately preceding the word
            while (firstCharacterIndex - 1 >= 0 &&
                  char.IsPunctuation(textInfo.characterInfo[firstCharacterIndex - 1].character))
            {
                firstCharacterIndex--;
            }

            // Include punctuation immediately following the word
            while (lastCharacterIndex + 1 < textInfo.characterCount &&
                  char.IsPunctuation(textInfo.characterInfo[lastCharacterIndex + 1].character))
            {
                lastCharacterIndex++;
            }

            StartCoroutine(FadeInWord(firstCharacterIndex, lastCharacterIndex, textInfo));

            yield return new WaitForSeconds(delayBetweenWords);
        }
    }

    IEnumerator FadeInWord(int firstCharacterIndex, int lastCharacterIndex, TMP_TextInfo textInfo)
    {
        Color32[] newVertexColors;

        for (float t = 0; t < fadeInDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / fadeInDuration);
            for (int i = firstCharacterIndex; i <= lastCharacterIndex; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                newVertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

                int vertexIndex = charInfo.vertexIndex;
                newVertexColors[vertexIndex + 0].a = (byte)(255 * alpha);
                newVertexColors[vertexIndex + 1].a = (byte)(255 * alpha);
                newVertexColors[vertexIndex + 2].a = (byte)(255 * alpha);
                newVertexColors[vertexIndex + 3].a = (byte)(255 * alpha);
            }

            textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            yield return null;
        }

        // Ensure the word and its punctuation are fully visible
        for (int i = firstCharacterIndex; i <= lastCharacterIndex; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            newVertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

            int vertexIndex = charInfo.vertexIndex;
            newVertexColors[vertexIndex + 0].a = 255;
            newVertexColors[vertexIndex + 1].a = 255;
            newVertexColors[vertexIndex + 2].a = 255;
            newVertexColors[vertexIndex + 3].a = 255;
        }

        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}
