using Assets.Scripts.Player;
using UnityEditor;
using UnityEngine;

public class MakeScriptableObject
{
    [MenuItem("Assets/Create/PlayerDresser")]
    public static void CreateMyAsset()
    {
        PlayerDresser asset = ScriptableObject.CreateInstance<PlayerDresser>();

        AssetDatabase.CreateAsset(asset, "Assets/ScriptableObjects/PlayerDresser.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
} 
