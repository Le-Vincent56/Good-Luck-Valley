using UnityEditor;

namespace GoodLuckValley.UI.Editor
{
    [CustomEditor(typeof(PauseMenu))]
    public class PauseMenuEditor : FadePanelEditorBase
    {
        public override void OnInspectorGUI()
        {
            ShowFadePanelUI();
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}