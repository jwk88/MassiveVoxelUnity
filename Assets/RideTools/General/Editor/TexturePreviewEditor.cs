namespace RideTools
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(NoiseTexturer))]
    public class TextureGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            NoiseTexturer generator = (NoiseTexturer)target;
            DrawDefaultInspector();

            if (GUILayout.Button("Generate Texture"))
            {
                generator.GenerateTexture();
                EditorUtility.SetDirty(target);
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Flush Texture"))
            {
                generator.FlushTexture();
                EditorUtility.SetDirty(target);
                SceneView.RepaintAll();
            }

            if (generator.TexturePreview != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Texture Preview:", EditorStyles.boldLabel);
                EditorGUILayout.ObjectField("Generated Texture:", generator.TexturePreview, typeof(Texture2D), false);
                GUILayout.Label("Preview:");
                Rect rect = GUILayoutUtility.GetRect(200, 200);
                EditorGUI.DrawPreviewTexture(rect, generator.TexturePreview);
                EditorGUILayout.LabelField($"Texture dimensions: {generator.TexturePreview.width}x{generator.TexturePreview.height}");
                EditorGUILayout.LabelField($"Texture format: {generator.TexturePreview.format}");
            }
            else
            {
                EditorGUILayout.HelpBox("No texture generated yet", MessageType.Info);
            }
        }
    }
}