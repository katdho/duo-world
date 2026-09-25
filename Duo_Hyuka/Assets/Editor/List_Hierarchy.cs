using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Text;

public class HierarchyLister {
    [MenuItem("Tools/Print Scene Hierarchy")]
    public static void PrintHierarchy() {
        StringBuilder sb = new StringBuilder();
        Scene scene = SceneManager.GetActiveScene();
        sb.AppendLine("=== SCENE HIERARCHY ===");
        sb.AppendLine("Scene: " + scene.name);
        sb.AppendLine();
        foreach (GameObject rootObject in scene.GetRootGameObjects())
            AddGameObject(rootObject.transform, sb, 0);
        Debug.Log(sb.ToString());
        GUIUtility.systemCopyBuffer = sb.ToString();
        Debug.Log("Hierarchy copied to clipboard!");
    }

    private static void AddGameObject(Transform obj, StringBuilder sb, int depth) {
        string indent = new string(' ', depth * 2);
        string status = obj.gameObject.activeSelf ? "" : " [INACTIVE]";
        sb.AppendLine(indent + "- " + obj.name + status);
        foreach (Transform child in obj)
            AddGameObject(child, sb, depth + 1);
    }
}

