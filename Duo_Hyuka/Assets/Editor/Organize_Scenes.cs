using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public static class OrganizeScenes {
    private const string SceneFolder = "Assets/Scenes/";

    [MenuItem("Tools/Organize Duo Scenes")]
    public static void Organize() {
        string hilaryPath = SceneFolder + "Hilary.unity";
        string phonePath = SceneFolder + "PhoneConnection.unity";

        if (!System.IO.File.Exists(hilaryPath)) {
            Debug.LogError("Could not find " + hilaryPath);
            return;
        }

        if (!System.IO.File.Exists(phonePath)) {
            Debug.LogError("Could not find " + phonePath);
            return;
        }

        Scene hilary = EditorSceneManager.OpenScene(hilaryPath, OpenSceneMode.Single);
        Scene phone = EditorSceneManager.OpenScene(phonePath, OpenSceneMode.Additive);

        Scene connection = CreateOrOpenScene("00_Connection");
        Scene game = CreateOrOpenScene("01_Game");
        Scene end = CreateOrOpenScene("02_End");

        OrganizeConnection(hilary, phone, connection);
        OrganizeGame(hilary, game);
        OrganizeEnd(hilary, end);

        EditorSceneManager.SaveScene(connection);
        EditorSceneManager.SaveScene(game);
        EditorSceneManager.SaveScene(end);

        EditorSceneManager.CloseScene(phone, true);
        EditorSceneManager.CloseScene(connection, true);
        EditorSceneManager.CloseScene(game, true);
        EditorSceneManager.CloseScene(end, true);

        EditorSceneManager.OpenScene(hilaryPath, OpenSceneMode.Single);

        Debug.Log("=== DUO SCENE ORGANIZATION COMPLETE ===");
        Debug.Log("Created/updated: 00_Connection, 01_Game, 02_End");
        Debug.Log("Hilary was NOT modified.");
        Debug.Log("PhoneConnection was NOT modified.");
    }

    private static void OrganizeConnection(Scene hilary, Scene phone, Scene connection) {
        CopyRoot(hilary, connection, "Main Camera");
        CopyRoot(hilary, connection, "EventSystem");

        CopyRoot(hilary, connection, "BumpDetector");
        CopyRoot(hilary, connection, "ExchangeSeesion");
        CopyRoot(hilary, connection, "PairingManager");

        CopyRoot(phone, connection, "RelayManager");
        CopyRoot(phone, connection, "NetworkManager");
        CopyRoot(phone, connection, "NetworkTestManager");
        CopyRoot(phone, connection, "NetworkTestObject");
        CopyRoot(phone, connection, "DuoConnectUI");

        CopyHilaryConnectionUI(hilary, connection);
        CopyPhoneConnectionUI(phone, connection);
    }

    private static void OrganizeGame(Scene hilary, Scene game) {
        CopyRoot(hilary, game, "Main Camera");
        CopyRoot(hilary, game, "EventSystem");

        CopyRoot(hilary, game, "PrototypeManager");
        CopyRoot(hilary, game, "ContentGenerator");
        CopyRoot(hilary, game, "ConversationListener");
        CopyRoot(hilary, game, "SFX/VFX");

        CopyChild(hilary, game, "UI", "SafeArea", "05_ConversationStarterScreen_A/B");
        CopyChild(hilary, game, "UI", "SafeArea", "xx");
        CopyChild(hilary, game, "UI", "SafeArea", "06_Conversation Listening");
        CopyChild(hilary, game, "UI", "SafeArea", "07_ConvoGoalMet");
        CopyChild(hilary, game, "UI", "SafeArea", "Progess");
    }

    private static void OrganizeEnd(Scene hilary, Scene end) {
        CopyRoot(hilary, end, "Main Camera");
        CopyRoot(hilary, end, "EventSystem");

        CopyChild(hilary, end, "UI", "SafeArea", "08_CompleteScreen");
        CopyChild(hilary, end, "UI", "SafeArea", "08_ReflectionScreen");
        CopyChild(hilary, end, "UI", "SafeArea", "09_LearnAgainScreen");
    }

    private static Scene CreateOrOpenScene(string sceneName) {
        string path = SceneFolder + sceneName + ".unity";

        if (System.IO.File.Exists(path))
            return EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        scene.name = sceneName;
        EditorSceneManager.SaveScene(scene, path);
        return scene;
    }

    private static void CopyRoot(Scene source, Scene destination, string objectName) {
        GameObject sourceObject = FindRoot(source, objectName);

        if (sourceObject == null) {
            Debug.LogWarning("Could not find root object '" + objectName + "' in " + source.name + ".");
            return;
        }

        if (FindRoot(destination, objectName) != null)
            return;

        GameObject copy = Object.Instantiate(sourceObject);
        copy.name = sourceObject.name;
        SceneManager.MoveGameObjectToScene(copy, destination);
    }

    private static void CopyChild(Scene source, Scene destination, string parent1, string parent2, string objectName) {
        GameObject sourceParent = FindRoot(source, parent1);

        if (sourceParent == null) {
            Debug.LogWarning("Could not find '" + parent1 + "' in " + source.name + ".");
            return;
        }

        Transform parent = FindChild(sourceParent.transform, parent2);

        if (parent == null) {
            Debug.LogWarning("Could not find '" + parent2 + "' under '" + parent1 + "'.");
            return;
        }

        Transform sourceObject = FindChild(parent, objectName);

        if (sourceObject == null) {
            Debug.LogWarning("Could not find '" + objectName + "' in " + source.name + ".");
            return;
        }

        if (FindRoot(destination, objectName) != null)
            return;

        GameObject copy = Object.Instantiate(sourceObject.gameObject);
        copy.name = sourceObject.name;
        SceneManager.MoveGameObjectToScene(copy, destination);
    }

    private static void CopyHilaryConnectionUI(Scene source, Scene destination) {
        GameObject sourceUI = FindRoot(source, "UI");

        if (sourceUI == null) {
            Debug.LogWarning("Could not find UI in Hilary.");
            return;
        }

        GameObject ui = FindRoot(destination, "UI");

        if (ui == null) {
            ui = new GameObject("UI");
            SceneManager.MoveGameObjectToScene(ui, destination);
        }

        Transform sourceSafeArea = FindChild(sourceUI.transform, "SafeArea");

        if (sourceSafeArea == null)
            return;

        Transform safeArea = FindChild(ui.transform, "SafeArea");

        if (safeArea == null) {
            GameObject safeAreaObject = new GameObject("SafeArea");
            safeAreaObject.transform.SetParent(ui.transform, false);
            safeArea = safeAreaObject.transform;
        }

        string[] keep = {
            "01_DevMenu",
            "02_TapScreen_Transition",
            "00_WaitingScreen",
            "03_ConfirmationScreen",
            "04_Consent Confirmation"
        };

        foreach (string objectName in keep) {
            Transform sourceObject = FindChild(sourceSafeArea, objectName);

            if (sourceObject == null)
                continue;

            if (FindChild(safeArea, objectName) != null)
                continue;

            GameObject copy = Object.Instantiate(sourceObject.gameObject);
            copy.name = sourceObject.name;
            copy.transform.SetParent(safeArea, false);
        }
    }

    private static void CopyPhoneConnectionUI(Scene source, Scene destination) {
        GameObject sourceCanvas = FindRoot(source, "Canvas");

        if (sourceCanvas == null) {
            Debug.LogWarning("Could not find Canvas in PhoneConnection.");
            return;
        }

        GameObject canvas = FindRoot(destination, "Canvas");

        if (canvas == null) {
            canvas = Object.Instantiate(sourceCanvas);
            canvas.name = "Canvas";
            SceneManager.MoveGameObjectToScene(canvas, destination);
            return;
        }

        string[] keep = {
            "StatusText",
            "JoinCodeText",
            "HomePanel",
            "JoinPanel",
            "WaitingPanel",
            "TapPanel"
        };

        foreach (string objectName in keep) {
            Transform sourceObject = FindChild(sourceCanvas.transform, objectName);

            if (sourceObject == null)
                continue;

            if (FindChild(canvas.transform, objectName) != null)
                continue;

            GameObject copy = Object.Instantiate(sourceObject.gameObject);
            copy.name = sourceObject.name;
            copy.transform.SetParent(canvas.transform, false);
        }
    }

    private static GameObject FindRoot(Scene scene, string objectName) {
        foreach (GameObject obj in scene.GetRootGameObjects()) {
            if (obj.name == objectName)
                return obj;
        }

        return null;
    }

    private static Transform FindChild(Transform parent, string childName) {
        foreach (Transform child in parent) {
            if (child.name == childName)
                return child;
        }

        return null;
    }
}