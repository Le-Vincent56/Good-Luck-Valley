using GoodLuckValley.Patterns.Commands;
using System.Collections;
using UnityEngine;

namespace GoodLuckValley.World.Decomposables
{
    public class DecomposableVine : MonoBehaviour, IDecomposable
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D boxCollider;

        [Header("Fields")]
        [SerializeField] private float decomposeTime;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        public void Decompose()
        {
            StartCoroutine(FadeOut());
        }

        public void Recompose()
        {
            StartCoroutine(FadeIn());
        }

        public IEnumerator FadeIn()
        {
            float elapsedTime = 0f;
            Color color = spriteRenderer.color;

            while(elapsedTime < decomposeTime)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsedTime / decomposeTime);
                spriteRenderer.color = color;
                yield return null;
            }

            color.a = 1f;
            spriteRenderer.color = color;
            gameObject.layer = 12;
        }

        public IEnumerator FadeOut()
        {
            float elapsedTime = 0f;
            Color color = spriteRenderer.color;

            while (elapsedTime < decomposeTime)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(1f - (elapsedTime / decomposeTime));
                spriteRenderer.color = color;
                yield return null;
            }

            color.a = 0f;
            spriteRenderer.color = color;
            gameObject.layer = 0;
        }
    }
}
