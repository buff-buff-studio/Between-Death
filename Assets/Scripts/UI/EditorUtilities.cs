using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class EditorUtilities
{
    #if UNITY_EDITOR
    public static GUIStyle btnLeft = new GUIStyle(EditorStyles.miniButtonLeft) { padding = new RectOffset(4, 4, 2, 2)};
    public static GUIStyle btnMid = new GUIStyle(EditorStyles.miniButtonMid) { padding = new RectOffset(4, 4, 2, 2) };
    public static GUIStyle btnRight = new GUIStyle(EditorStyles.miniButtonRight) { padding = new RectOffset(4, 4, 2, 2) };
    
    public static GUIStyle boldIcon = new GUIStyle(EditorStyles.boldLabel) {  };

    private static string tmproAssetFolderPath => TMPro.EditorUtilities.TMP_EditorUtility.packageRelativePath;
    public static Texture2D PosZ => Resources.Load<Texture2D>("Editor/AxisAnchor/ZPos");
    public static Texture2D NegZ => Resources.Load<Texture2D>("Editor/AxisAnchor/ZNeg");
    public static Texture2D PosX => Resources.Load<Texture2D>("Editor/AxisAnchor/XPos");
    public static Texture2D NegX => Resources.Load<Texture2D>("Editor/AxisAnchor/XNeg");
    public static Texture2D PosY => Resources.Load<Texture2D>("Editor/AxisAnchor/YPos");
    public static Texture2D NegY => Resources.Load<Texture2D>("Editor/AxisAnchor/YNeg");
    public static Texture2D Center => Resources.Load<Texture2D>("Editor/AxisAnchor/Center");

    public static bool EditorToggle(Rect position, bool value, GUIContent content, GUIStyle style)
    {
        var id = GUIUtility.GetControlID(content, FocusType.Keyboard, position);
        var evt = Event.current;

        // Toggle selected toggle on space or return key
        if (GUIUtility.keyboardControl == id && evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Space || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter))
        {
            value = !value;
            evt.Use();
            GUI.changed = true;
        }

        if (evt.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
        {
            GUIUtility.keyboardControl = id;
            EditorGUIUtility.editingTextField = false;
            HandleUtility.Repaint();
        }

        style.fixedHeight = 25;
        
        return GUI.Toggle(position, id, value, content, style);
    }
    #endif
}