using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace GoodLuckValley.World.AreaTriggers.Editor
{
    [CustomEditor(typeof(AreaCollider))]
    public class AreaColliderEditor : UnityEditor.Editor
    {
        private SerializedProperty colliderType;
        private SerializedProperty layerMask;
        private SerializedProperty bounds;

        private AreaCollider areaCollider;

        private void OnEnable()
        {
            areaCollider = (AreaCollider)target;

            colliderType = serializedObject.FindProperty("colliderType");
            layerMask = serializedObject.FindProperty("layerMask");
            bounds = serializedObject.FindProperty("bounds");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(colliderType);
            EditorGUILayout.PropertyField(layerMask);
            EditorGUILayout.PropertyField(bounds);

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                HandleColliderTypeChange();
        }

        private void HandleColliderTypeChange()
        {
            switch((AreaCollider.Type)colliderType.enumValueIndex)
            {
                case AreaCollider.Type.Box:
                    ToggleCompositeCollider(false);
                    SetBoxColliderUsedByComposite(false);
                    break;

                case AreaCollider.Type.Composite:
                    ToggleCompositeCollider(true);
                    SetBoxColliderUsedByComposite(true);
                    ChangeRigidbody2DToStatic();
                    RearrangeComponents();
                    break;
            }

            EditorUtility.SetDirty(areaCollider.gameObject);
        }

        private void ToggleCompositeCollider(bool add)
        {
            if(add)
            {
                if(!areaCollider.TryGetComponent(out CompositeCollider2D compositeCollider))
                    areaCollider.gameObject.AddComponent<CompositeCollider2D>();
            } else
            {
                if (areaCollider.TryGetComponent(out CompositeCollider2D compositeCollider))
                    DestroyImmediate(compositeCollider);

                if (areaCollider.TryGetComponent(out Rigidbody2D rigidbody))
                    DestroyImmediate(rigidbody);
            }
        }

        private void SetBoxColliderUsedByComposite(bool usedByComposite)
        {
            BoxCollider2D boxCollider = areaCollider.GetComponent<BoxCollider2D>();
            if(boxCollider != null)
                boxCollider.usedByComposite = usedByComposite;
        }

        private void RearrangeComponents()
        {
            if(areaCollider.TryGetComponent(out CompositeCollider2D compositeCollider) &&
                areaCollider.TryGetComponent(out Rigidbody2D rigidbody))
            {
                MoveComponentToTop(compositeCollider);
                MoveComponentToTop(rigidbody);
            }
        }

        private void MoveComponentToTop(Component component)
        {
            if (component == null) return;

            Component[] components = component.gameObject.GetComponents<Component>();

            for(int i = 1; i < components.Length; i++)
            {
                if (components[i] == component)
                {
                    for(int j = i; j > 1; j--)
                    {
                        UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
                    }
                    break;
                }
            }
        }

        private void ChangeRigidbody2DToStatic()
        {
            if (areaCollider.TryGetComponent(out Rigidbody2D rigidbody))
                rigidbody.bodyType = RigidbodyType2D.Static;
        }
    }
}