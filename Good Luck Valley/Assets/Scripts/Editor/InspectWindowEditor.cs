using UnityEditor;
using GoodLuckValley.UI.Editor;
using GoodLuckValley.Journal.UI;

namespace GoodLuckValley.Journal.Editor
{
    [CustomEditor(typeof(InspectWindow))]
    public class InspectWindowEditor : FadePanelEditorBase
    {
        public override void OnInspectorGUI()
        {
            ShowFadePanelUI();
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}