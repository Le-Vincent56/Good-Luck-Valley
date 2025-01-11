using UnityEngine;
using UnityEditor;
using GoodLuckValley.World.Triggers;

namespace GoodLuckValley.Editors.Triggers
{
    [CustomEditor(typeof(BaseTrigger), true)]
    public class BaseTriggerEditor : Editor
    {
        private void OnEnable()
        {
            BaseTrigger baseTrigger = (BaseTrigger)target;

            // Ensure the Rigidbody2D is set to static
            Rigidbody2D rb = baseTrigger.GetComponent<Rigidbody2D>();
            if (rb != null && rb.bodyType != RigidbodyType2D.Static)
            {
                rb.bodyType = RigidbodyType2D.Static;
            }

            // Ensure the BoxCollider2D is set as a trigger
            BoxCollider2D collider = baseTrigger.GetComponent<BoxCollider2D>();
            if (collider != null && !collider.isTrigger)
            {
                collider.isTrigger = true;
            }
        }
    }
}
