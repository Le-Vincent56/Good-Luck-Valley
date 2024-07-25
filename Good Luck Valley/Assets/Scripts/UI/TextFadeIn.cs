using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFadeIn : MonoBehaviour
{
    [SerializeField] private TMP_Text textMeshPro;
    [SerializeField] private float fadeInDuration = 1f; // Duration for each word to fade in
    [SerializeField] private float delayBetweenWords = 0.5f; // Delay between each word fade-in

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
        Color32[] newVertexColors;

        for (int w = 0; w < textInfo.wordCount; w++)
        {
            TMP_WordInfo wordInfo = textInfo.wordInfo[w];

            for (float t = 0; t < fadeInDuration; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(0, 1, t / fadeInDuration);
                for (int i = 0; i < wordInfo.characterCount; i++)
                {
                    int characterIndex = wordInfo.firstCharacterIndex + i;
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[characterIndex];
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

            // Ensure the word is fully visible
            for (int i = 0; i < wordInfo.characterCount; i++)
            {
                int characterIndex = wordInfo.firstCharacterIndex + i;
                TMP_CharacterInfo charInfo = textInfo.characterInfo[characterIndex];
                newVertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

                int vertexIndex = charInfo.vertexIndex;
                newVertexColors[vertexIndex + 0].a = 255;
                newVertexColors[vertexIndex + 1].a = 255;
                newVertexColors[vertexIndex + 2].a = 255;
                newVertexColors[vertexIndex + 3].a = 255;
            }

            textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            yield return new WaitForSeconds(delayBetweenWords);
        }
    }
}
