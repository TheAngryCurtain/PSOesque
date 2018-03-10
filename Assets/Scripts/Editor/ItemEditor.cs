using UnityEngine;
using UnityEditor;

public class ItemEditor : EditorWindow
{
    private static int itemID = 0;

    private string newItemName = "New Item";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    [MenuItem("Tools/Item Editor")]
    static void Init()
    {
        ItemEditor window = (ItemEditor)EditorWindow.GetWindow(typeof(ItemEditor));
        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 400f, 600f);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Create Item", EditorStyles.boldLabel);
        newItemName = EditorGUILayout.TextField("Item Name: ", newItemName);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
    }
}
