using GoodLuckValley.Patterns.Commands;
using System.Collections;
using UnityEngine;

namespace GoodLuckValley.World.Decomposables
{
    public class DecomposableVine : MonoBehaviour, IDecomposable
    {
        public enum RetreatDirection
        {
            Up = 0, // Positive
            Down = 1, // Negative
            Right = 2, // Negative
            Left = 3, // Positive
        }

        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D boxCollider;

        [Header("Fields")]
        [SerializeField] private int index;
        [SerializeField] private RetreatDirection retreatDirection;
        private Coroutine fadeCoroutine;
        private float currentAlpha;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        /// <summary>
        /// Decompose the vine
        /// </summary>
        /// <param name="decomposeTime">The amount of time to recompose in</param>
        public void Decompose(float decomposeTime)
        {
            if(fadeCoroutine != null) 
                StopCoroutine(fadeCoroutine);

            Vector2 targetSize = new Vector2(boxCollider.size.x, 0.001f);
            Vector2 targetOffset = 
                (retreatDirection == RetreatDirection.Left || retreatDirection == RetreatDirection.Up) 
                ? new Vector2(boxCollider.offset.x, 0.5f) 
                : new Vector2(boxCollider.offset.x, -0.5f);

            fadeCoroutine = StartCoroutine(Fade(0f, targetSize, targetOffset, decomposeTime));
        }

        /// <summary>
        /// Recompose the vine
        /// </summary>
        /// <param name="decomposeTime">The amount of time to recompose in</param>
        public void Recompose(float recomposeTime)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            Vector2 targetSize = new Vector2(boxCollider.size.x, 1f);
            Vector2 targetOffset = new Vector2(boxCollider.offset.x, 0f);
            fadeCoroutine = StartCoroutine(Fade(1f, targetSize, targetOffset, recomposeTime));
        }

        public int GetIndex() => index;

        private IEnumerator Fade(float targetAlpha, Vector2 targetSize, Vector2 targetOffset, float decomposeTime)
        {
            float startAlpha = spriteRenderer.color.a;
            Vector2 startSize = boxCollider.size;
            Vector2 startOffset = boxCollider.offset;
            float elapsedTime = 0f;
            Color color = spriteRenderer.color;

            float adjustedDecomposeTime = Mathf.Abs(targetAlpha - startAlpha) * decomposeTime;

            while (elapsedTime < adjustedDecomposeTime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / adjustedDecomposeTime;

                // Fade sprite color
                color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
                spriteRenderer.color = color;

                // Adjust collider size and offset
                boxCollider.size = Vector2.Lerp(startSize, targetSize, t);
                boxCollider.offset = Vector2.Lerp(startOffset, targetOffset, t);

                yield return null;
            }

            // Ensure final values are set
            color.a = targetAlpha;
            spriteRenderer.color = color;
            boxCollider.size = targetSize;
            boxCollider.offset = targetOffset;
            currentAlpha = targetAlpha;

            fadeCoroutine = null;
        }
    }
}
