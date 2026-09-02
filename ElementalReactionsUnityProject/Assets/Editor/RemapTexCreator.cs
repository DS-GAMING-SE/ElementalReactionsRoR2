using UnityEngine;
using UnityEditor;
using Unity.Collections;
using System.IO;

public class RemapTexCreator : EditorWindow
{
    [MenuItem("Assets/Create RemapTex", validate = true)]
    static bool Validate()
    {
        return !AssetDatabase.GetAssetPath(Selection.activeObject).Contains(".");
    }
    [MenuItem("Assets/Create RemapTex")]
    static void Init()
    {
        RemapTexCreator window = (RemapTexCreator)EditorWindow.GetWindow(typeof(RemapTexCreator), true, "RemapTex Creator");
        window.Show();
    }
    private Gradient gradient = new Gradient();
    private Vector2Int texScale = new Vector2Int(128, 4);
    private string texName = "Name";
    private void OnGUI()
    {
        gradient = EditorGUILayout.GradientField(gradient, GUILayout.Height(32f));
        texScale = EditorGUILayout.Vector2IntField("Texture Scale", texScale);
        texName = EditorGUILayout.TextField(texName);
        if (GUILayout.Button("Confirm"))
        {
            Texture2D tex = new Texture2D(texScale.x, texScale.y, TextureFormat.RGBA32, false);
            NativeArray<Color32> data = tex.GetRawTextureData<Color32>();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = gradient.Evaluate(((float)(i % texScale.x)) / texScale.x);
            }
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.name = "texRamp"+ texName + ".png";
            tex.Apply();
            string selectedObjectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string path = selectedObjectPath + "/" + tex.name;
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            AssetDatabase.ImportAsset(path);

            TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.readable = true;
            settings.mipmapEnabled = false;
            settings.wrapMode = TextureWrapMode.Clamp;
            settings.alphaIsTransparency = true;
            importer.SetTextureSettings(settings);
            TextureImporterPlatformSettings platformSettings = importer.GetDefaultPlatformTextureSettings();
            platformSettings.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.SetPlatformTextureSettings(platformSettings);
            importer.SaveAndReimport();

            Close();
        }
    }
}