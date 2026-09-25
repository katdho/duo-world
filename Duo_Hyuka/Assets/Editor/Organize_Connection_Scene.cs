#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class OrganizeConnectionScene {
    private const string SceneName = "00_Connection";

    [MenuItem("Tools/Duo/Organize 00_Connection")]
    public static void Organize() {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name != SceneName) {
            EditorUtility.DisplayDialog(
                "Wrong Scene",
                "Open 00_Connection.unity before running this tool.",
                "OK"
            );
            return;
        }

        GameObject managers = GetOrCreateRoot("Managers");
        GameObject ui = FindRoot("UI");

        if (ui == null) {
            Debug.LogError("Could not find UI.");
            return;
        }

        MoveToParent("PairingManager", managers.transform);
        MoveToParent("RelayManager", managers.transform);
        MoveToParent("BumpDetector", managers.transform);
        MoveToParent("ExchangeSession", managers.transform);
        MoveToParent("NetworkManager", managers.transform);

        RenameObject(managers.transform, "ExchangeSession", "ExchangeSession");

        MoveToParent("DuoConnectUI", null);
        MoveToParent("Canvas", null);

        RenameObject(scene.GetRootGameObjects(), "DuoConnectUI", "OLD_DuoConnectUI");
        RenameObject(scene.GetRootGameObjects(), "Canvas", "OLD_Canvas");

        RenameScreen(ui.transform, "01_DevMenu", "Screen_01_DevMenu");
        RenameScreen(ui.transform, "02_TapScreen_Transition", "Screen_02_Tap");
        RenameScreen(ui.transform, "00_WaitingScreen", "Screen_03_Waiting");
        RenameScreen(ui.transform, "03_ConfirmationScreen", "Screen_04_Confirmation");
        RenameScreen(ui.transform, "04_Consent Confirmation", "Screen_05_Consent");
        RenameScreen(ui.transform, "ConnectionMenuScreen", "Screen_06_ConnectionMenu");
        RenameScreen(ui.transform, "JoinCodeScreen", "Screen_07_JoinCode");
        RenameScreen(ui.transform, "RelayWaitingScreen", "Screen_08_RelayWaiting");

        RenameChild(
            ui.transform,
            "SafeArea/Screen_02_Tap",
            "Waiting",
            "TapStatusText"
        );

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log("=== 00_Connection ORGANIZATION COMPLETE ===");
    }

    private static GameObject GetOrCreateRoot(string name) {
        GameObject existing = FindRoot(name);

        if (existing != null)
            return existing;

        GameObject created = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(created, "Create " + name);

        Debug.Log("[CREATED] " + name);

        return created;
    }

    private static void MoveToParent(string objectName, Transform parent) {
        GameObject obj = FindRoot(objectName);

        if (obj == null) {
            Debug.LogWarning("[MISSING] " + objectName);
            return;
        }

        if (parent != null && obj.transform.parent != parent) {
            Undo.SetTransformParent(
                obj.transform,
                parent,
                "Move " + objectName
            );

            Debug.Log("[MOVED] " + objectName + " -> " + parent.name);
        }
    }

    private static void RenameObject(
        Transform parent,
        string oldName,
        string newName
    ) {
        Transform child = parent.Find(oldName);

        if (child == null)
            return;

        child.name = newName;

        Debug.Log("[RENAMED] " + oldName + " -> " + newName);
    }

    private static void RenameObject(
        GameObject[] roots,
        string oldName,
        string newName
    ) {
        foreach (GameObject root in roots) {
            if (root.name == oldName) {
                root.name = newName;
                Debug.Log("[RENAMED] " + oldName + " -> " + newName);
                return;
            }
        }
    }

    private static void RenameScreen(
        Transform ui,
        string oldName,
        string newName
    ) {
        Transform safeArea = ui.Find("SafeArea");

        if (safeArea == null) {
            Debug.LogError("Could not find UI/SafeArea.");
            return;
        }

        Transform screen = safeArea.Find(oldName);

        if (screen == null) {
            screen = safeArea.Find(newName);

            if (screen != null)
                return;

            Debug.LogWarning("[MISSING] " + oldName);
            return;
        }

        screen.name = newName;

        Debug.Log("[RENAMED] " + oldName + " -> " + newName);
    }

    private static void RenameChild(
        Transform root,
        string path,
        string oldName,
        string newName
    ) {
        Transform parent = root.Find(path);

        if (parent == null) {
            Debug.LogWarning("[MISSING PATH] " + path);
            return;
        }

        Transform child = parent.Find(oldName);

        if (child == null) {
            if (parent.Find(newName) != null)
                return;

            Debug.LogWarning("[MISSING] " + path + "/" + oldName);
            return;
        }

        child.name = newName;

        Debug.Log(
            "[RENAMED] " +
            oldName +
            " -> " +
            newName
        );
    }

    private static GameObject FindRoot(string name) {
        Scene scene = SceneManager.GetActiveScene();

        foreach (GameObject obj in scene.GetRootGameObjects()) {
            if (obj.name == name)
                return obj;
        }

        return null;
    }
}
#endif