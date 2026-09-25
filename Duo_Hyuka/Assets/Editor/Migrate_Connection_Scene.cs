#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public static class MigrateConnectionScene {
    private const string SceneName = "00_Connection";

    [MenuItem("Tools/Duo/Migrate 00_Connection")]
    public static void Migrate() {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name != SceneName) {
            EditorUtility.DisplayDialog(
                "Wrong Scene",
                "Open 00_Connection.unity before running this tool.",
                "OK"
            );
            return;
        }

        if (!EditorUtility.DisplayDialog(
            "Migrate 00_Connection",
            "This modifies ONLY the active 00_Connection scene.\n\n" +
            "00_Connection_BACKUP will not be changed.\n\n" +
            "The migration is idempotent, so it is safe to run again.",
            "Continue",
            "Cancel"
        ))
            return;

        Debug.Log("=== DUO CONNECTION MIGRATION START ===");

        GameObject ui = FindRoot("UI");

        if (ui == null) {
            Debug.LogError("Could not find root UI object.");
            return;
        }

        Transform safeArea = ui.transform.Find("SafeArea");

        if (safeArea == null) {
            Debug.LogError("Could not find UI/SafeArea.");
            return;
        }

        GameObject oldCanvas = FindRoot("Canvas");

        if (oldCanvas != null)
            MigrateFallbackPanels(oldCanvas, safeArea);

        EnsureConnectionUI(ui);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        AssetDatabase.SaveAssets();

        Debug.Log("=== DUO CONNECTION MIGRATION COMPLETE ===");
    }

    private static void MigrateFallbackPanels(
        GameObject oldCanvas,
        Transform safeArea
    ) {
        CopyIfMissing(
            oldCanvas,
            "HomePanel",
            safeArea,
            "ConnectionMenuScreen"
        );

        CopyIfMissing(
            oldCanvas,
            "JoinPanel",
            safeArea,
            "JoinCodeScreen"
        );

        CopyIfMissing(
            oldCanvas,
            "WaitingPanel",
            safeArea,
            "RelayWaitingScreen"
        );
    }

    private static void CopyIfMissing(
        GameObject sourceRoot,
        string sourceName,
        Transform destination,
        string destinationName
    ) {
        Transform existing =
            destination.Find(destinationName);

        if (existing != null) {
            Debug.Log(
                "[SKIP] Already exists: " +
                destinationName
            );
            return;
        }

        Transform source =
            FindChildRecursive(
                sourceRoot.transform,
                sourceName
            );

        if (source == null) {
            Debug.LogWarning(
                "[MISSING] Could not find: " +
                sourceName
            );
            return;
        }

        GameObject clone =
            UnityEngine.Object.Instantiate(
                source.gameObject,
                destination
            );

        clone.name = destinationName;

        RectTransform rect =
            clone.GetComponent<RectTransform>();

        if (rect != null) {
            rect.localPosition = Vector3.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;
        }

        clone.SetActive(false);

        Debug.Log(
            "[COPIED] " +
            sourceName +
            " -> " +
            destinationName
        );
    }

    private static void EnsureConnectionUI(
        GameObject uiObject
    ) {
        Type connectionUIType =
            FindConnectionUIType();

        if (connectionUIType == null) {
            Debug.LogError(
                "ConnectionUI class could not be found.\n" +
                "Make sure ConnectionUI.cs compiles first."
            );
            return;
        }

        Component connectionUI =
            uiObject.GetComponent(connectionUIType);

        if (connectionUI == null) {
            connectionUI =
                uiObject.AddComponent(connectionUIType);

            Debug.Log(
                "[ADDED] ConnectionUI component"
            );
        }
        else {
            Debug.Log(
                "[SKIP] ConnectionUI already exists"
            );
        }

        Transform safeArea =
            uiObject.transform.Find("SafeArea");

        if (safeArea == null)
            return;

        AssignGameObject(
            connectionUI,
            "tapScreen",
            FindChild(
                safeArea,
                "02_TapScreen_Transition"
            )
        );

        AssignGameObject(
            connectionUI,
            "waitingScreen",
            FindChild(
                safeArea,
                "RelayWaitingScreen"
            )
        );

        AssignGameObject(
            connectionUI,
            "confirmationScreen",
            FindChild(
                safeArea,
                "03_ConfirmationScreen"
            )
        );

        AssignGameObject(
            connectionUI,
            "consentScreen",
            FindChild(
                safeArea,
                "04_Consent Confirmation"
            )
        );

        AssignGameObject(
            connectionUI,
            "homeScreen",
            FindChild(
                safeArea,
                "ConnectionMenuScreen"
            )
        );

        AssignGameObject(
            connectionUI,
            "joinScreen",
            FindChild(
                safeArea,
                "JoinCodeScreen"
            )
        );

        AssignTMP(
            connectionUI,
            "tapStatusText",
            FindTMP(
                safeArea,
                "TapStatusText"
            )
        );

        AssignTMP(
            connectionUI,
            "waitingText",
            FindTMP(
                safeArea,
                "WaitingText"
            )
        );

        AssignTMP(
            connectionUI,
            "gameCodeText",
            FindTMP(
                safeArea,
                "GameCodeText"
            )
        );

        AssignInputField(
            connectionUI,
            "joinCodeInput",
            FindInputField(
                safeArea,
                "JoinCodeInput"
            )
        );

        AssignNetworking(
            connectionUI
        );

        Debug.Log(
            "[WIRED] ConnectionUI references"
        );
    }

    private static void AssignNetworking(
        Component connectionUI
    ) {
        RelayManager relay =
            UnityEngine.Object.FindAnyObjectByType<RelayManager>();

        PairingManager pairing =
            UnityEngine.Object.FindAnyObjectByType<PairingManager>();

        AssignObject(
            connectionUI,
            "relayManager",
            relay
        );

        AssignObject(
            connectionUI,
            "pairingManager",
            pairing
        );

        if (relay != null) {
            AssignObject(
                relay,
                "connectionUI",
                connectionUI
            );
        }

        if (pairing != null) {
            AssignObject(
                pairing,
                "connectionUI",
                connectionUI
            );
        }
    }

    private static void AssignGameObject(
        Component component,
        string propertyName,
        Transform target
    ) {
        if (target == null)
            return;

        AssignObject(
            component,
            propertyName,
            target.gameObject
        );
    }

    private static void AssignTMP(
        Component component,
        string propertyName,
        TMP_Text target
    ) {
        if (target == null)
            return;

        AssignObject(
            component,
            propertyName,
            target
        );
    }

    private static void AssignInputField(
        Component component,
        string propertyName,
        TMP_InputField target
    ) {
        if (target == null)
            return;

        AssignObject(
            component,
            propertyName,
            target
        );
    }

    private static void AssignObject(
        Component component,
        string propertyName,
        UnityEngine.Object value
    ) {
        SerializedObject serialized =
            new SerializedObject(component);

        SerializedProperty property =
            serialized.FindProperty(propertyName);

        if (property == null) {
            Debug.LogWarning(
                "[MISSING FIELD] " +
                component.GetType().Name +
                "." +
                propertyName
            );
            return;
        }

        if (property.objectReferenceValue == value)
            return;

        property.objectReferenceValue = value;

        serialized.ApplyModifiedProperties();

        Debug.Log(
            "[ASSIGNED] " +
            component.GetType().Name +
            "." +
            propertyName
        );
    }

    private static void WireButtons(
        Component connectionUI
    ) {
        Transform safeArea =
            connectionUI.transform.Find("SafeArea");

        if (safeArea == null)
            return;

        Transform home =
            safeArea.Find("ConnectionMenuScreen");

        Transform join =
            safeArea.Find("JoinCodeScreen");

        Transform waiting =
            safeArea.Find("RelayWaitingScreen");

        AddButtonListener(
            home,
            "CreateGameButton",
            connectionUI,
            "CreateGame"
        );

        AddButtonListener(
            home,
            "JoinGameButton",
            connectionUI,
            "ShowJoin"
        );

        AddButtonListener(
            join,
            "JoinButton",
            connectionUI,
            "JoinGame"
        );

        AddButtonListener(
            join,
            "CreateGameFromCodeButton",
            connectionUI,
            "CreateGame"
        );

        AddButtonListener(
            join,
            "BackButton",
            connectionUI,
            "ShowHome"
        );

        AddButtonListener(
            waiting,
            "CancelButton",
            connectionUI,
            "ShowHome"
        );
    }

    private static void AddButtonListener(
        Transform parent,
        string buttonName,
        Component target,
        string methodName
    ) {
        if (parent == null)
            return;

        Transform buttonTransform =
            FindChildRecursive(
                parent,
                buttonName
            );

        if (buttonTransform == null)
            return;

        Button button =
            buttonTransform.GetComponent<Button>();

        if (button == null)
            return;

        Type targetType =
            target.GetType();

        System.Reflection.MethodInfo method =
            targetType.GetMethod(
                methodName,
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public
            );

        if (method == null) {
            Debug.LogWarning(
                "[MISSING METHOD] " +
                targetType.Name +
                "." +
                methodName
            );
            return;
        }

        SerializedObject serialized =
            new SerializedObject(button);

        SerializedProperty calls =
            serialized.FindProperty("m_OnClick");

        if (calls != null) {
            for (int i = calls.arraySize - 1; i >= 0; i--) {
                SerializedProperty call =
                    calls.GetArrayElementAtIndex(i);

                SerializedProperty targetProperty =
                    call.FindPropertyRelative(
                        "m_Target"
                    );

                if (
                    targetProperty != null &&
                    targetProperty.objectReferenceValue ==
                    target
                ) {
                    calls.DeleteArrayElementAtIndex(i);
                }
            }
        }

        serialized.ApplyModifiedProperties();

        UnityEventTools.AddVoidPersistentListener(
            button.onClick,
            () => method.Invoke(target, null)
        );

        Debug.Log(
            "[WIRED BUTTON] " +
            buttonName +
            " -> " +
            methodName
        );
    }

    private static Type FindConnectionUIType() {
        foreach (var assembly in
                 AppDomain.CurrentDomain.GetAssemblies()) {

            Type type =
                assembly.GetType(
                    "ConnectionUI"
                );

            if (type != null)
                return type;
        }

        return null;
    }

    private static TMP_Text FindTMP(
        Transform parent,
        string name
    ) {
        TMP_Text[] texts =
            parent.GetComponentsInChildren<TMP_Text>(
                true
            );

        foreach (TMP_Text text in texts) {
            if (text.name == name)
                return text;
        }

        return null;
    }

    private static TMP_InputField FindInputField(
        Transform parent,
        string name
    ) {
        TMP_InputField[] inputs =
            parent.GetComponentsInChildren<TMP_InputField>(
                true
            );

        foreach (TMP_InputField input in inputs) {
            if (input.name == name)
                return input;
        }

        return null;
    }

    private static Transform FindChild(
        Transform parent,
        string name
    ) {
        return parent.Find(name);
    }

    private static Transform FindChildRecursive(
        Transform parent,
        string name
    ) {
        foreach (Transform child in parent) {
            if (child.name == name)
                return child;

            Transform result =
                FindChildRecursive(
                    child,
                    name
                );

            if (result != null)
                return result;
        }

        return null;
    }

    private static GameObject FindRoot(
        string name
    ) {
        for (int i = 0; i < SceneManager.sceneCount; i++) {
            Scene scene =
                SceneManager.GetSceneAt(i);

            foreach (GameObject obj in
                     scene.GetRootGameObjects()) {

                if (obj.name == name)
                    return obj;
            }
        }

        return null;
    }
}
#endif