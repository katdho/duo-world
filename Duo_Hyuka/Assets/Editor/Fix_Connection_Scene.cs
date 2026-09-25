using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class FixConnectionArchitecture {
    private const string SceneName = "00_Connection";
    private const string ScriptsPath = "Assets/Scripts/_Connection/";

    [MenuItem("Tools/Old Clean-ups/Fix Connection Architecture")]
    public static void FixAll() {
        if (EditorSceneManager.GetActiveScene().name != SceneName) {
            Debug.LogError("Open 00_Connection before running this tool.");
            return;
        }

        bool sourceChanged = FixSourceFiles();
        bool sceneChanged = FixScene();

        if (sourceChanged)
            AssetDatabase.Refresh();

        if (sourceChanged || sceneChanged)
            Debug.Log("Connection architecture fixes applied.");
        else
            Debug.Log("Connection architecture already up to date.");
    }

    private static bool FixSourceFiles() {
        bool changed = false;

        changed |= ReplaceInFile(
            ScriptsPath + "RelayManager.cs",
            "DuoConnectUI",
            "ConnectionUI"
        );

        changed |= ReplaceInFile(
            ScriptsPath + "PairingManager.cs",
            "DuoConnectUI",
            "ConnectionUI"
        );

        return changed;
    }

    private static bool ReplaceInFile(
        string path,
        string oldText,
        string newText
    ) {
        if (!File.Exists(path)) {
            Debug.LogWarning("File not found: " + path);
            return false;
        }

        string text = File.ReadAllText(path);

        if (!text.Contains(oldText))
            return false;

        text = text.Replace(oldText, newText);

        File.WriteAllText(path, text);

        Debug.Log("Updated: " + path);
        return true;
    }

    private static bool FixScene() {
        bool changed = false;

        GameObject ui = GameObject.Find("Screens/UI");
        GameObject managers = GameObject.Find("Managers");

        if (ui == null) {
            Debug.LogError("Could not find Screens/UI.");
            return false;
        }

        if (managers == null) {
            Debug.LogError("Could not find Managers.");
            return false;
        }

        GameObject tap =
            FindChild(ui, "Screen_02_Tap");

        GameObject confirmation =
            FindChild(ui, "Screen_04_Confirmation");

        GameObject consent =
            FindChild(ui, "Screen_05_Consent");

        GameObject home =
            FindChild(ui, "Screen_06_ConnectionMenu");

        GameObject join =
            FindChild(ui, "Screen_07_JoinCode");

        GameObject relayWaiting =
            FindChild(ui, "Screen_08_RelayWaiting");

        GameObject pairingObject =
            FindChild(managers, "PairingManager");

        GameObject relayObject =
            FindChild(managers, "RelayManager");

        GameObject bumpObject =
            FindChild(managers, "BumpDetector");

        GameObject exchangeObject =
            FindChild(managers, "ExchangeSession");

        if (tap == null ||
            confirmation == null ||
            consent == null ||
            home == null ||
            join == null ||
            relayWaiting == null) {

            Debug.LogError(
                "One or more connection screens are missing."
            );

            return false;
        }

        if (pairingObject == null ||
            relayObject == null ||
            bumpObject == null ||
            exchangeObject == null) {

            Debug.LogError(
                "One or more Manager objects are missing."
            );

            return false;
        }

        ConnectionUI connectionUI =
            ui.GetComponent<ConnectionUI>();

        if (connectionUI == null) {
            connectionUI =
                ui.AddComponent<ConnectionUI>();

            changed = true;

            Debug.Log(
                "Added ConnectionUI to Screens/UI."
            );
        }

        PairingManager pairingManager =
            pairingObject.GetComponent<PairingManager>();

        RelayManager relayManager =
            relayObject.GetComponent<RelayManager>();

        BumpDetector bumpDetector =
            bumpObject.GetComponent<BumpDetector>();

        ExchangeSession exchangeSession =
            exchangeObject.GetComponent<ExchangeSession>();

        if (pairingManager == null) {
            Debug.LogError(
                "PairingManager component missing."
            );

            return false;
        }

        if (relayManager == null) {
            Debug.LogError(
                "RelayManager component missing."
            );

            return false;
        }

        if (bumpDetector == null) {
            Debug.LogError(
                "BumpDetector component missing."
            );

            return false;
        }

        if (exchangeSession == null) {
            Debug.LogError(
                "ExchangeSession component missing."
            );

            return false;
        }

        changed |= SetConnectionUIReferences(
            connectionUI,
            tap,
            relayWaiting,
            confirmation,
            consent,
            home,
            join,
            pairingManager,
            relayManager
        );

        changed |= SetPairingReferences(
            pairingManager,
            bumpDetector,
            relayManager,
            connectionUI,
            exchangeSession
        );

        changed |= SetRelayReferences(
            relayManager,
            connectionUI,
            pairingManager
        );

        changed |= SetBumpReferences(
            bumpDetector,
            pairingManager,
            exchangeSession
        );

        changed |= WireButtons(
            connectionUI,
            home,
            join,
            relayWaiting
        );

        changed |= DisableOldObjects();

        if (changed) {
            EditorSceneManager.MarkSceneDirty(
                EditorSceneManager.GetActiveScene()
            );

            EditorSceneManager.SaveScene(
                EditorSceneManager.GetActiveScene()
            );

            Debug.Log(
                "Scene references and buttons updated."
            );
        }

        return changed;
    }

    private static bool SetConnectionUIReferences(
        ConnectionUI ui,
        GameObject tap,
        GameObject relayWaiting,
        GameObject confirmation,
        GameObject consent,
        GameObject home,
        GameObject join,
        PairingManager pairingManager,
        RelayManager relayManager
    ) {
        bool changed = false;

        changed |= SetReference(
            ref ui.tapScreen,
            tap
        );

        changed |= SetReference(
            ref ui.waitingScreen,
            relayWaiting
        );

        changed |= SetReference(
            ref ui.confirmationScreen,
            confirmation
        );

        changed |= SetReference(
            ref ui.consentScreen,
            consent
        );

        changed |= SetReference(
            ref ui.homeScreen,
            home
        );

        changed |= SetReference(
            ref ui.joinScreen,
            join
        );

        GameObject tapStatus =
            FindChild(
                tap,
                "TapStatusText"
            );

        GameObject waitingText =
            FindChild(
                relayWaiting,
                "WaitingText"
            );

        GameObject gameCodeText =
            FindChild(
                relayWaiting,
                "GameCodeText"
            );

        GameObject joinInput =
            FindChild(
                join,
                "JoinCodeInput"
            );

        if (tapStatus != null) {
            TMP_Text text =
                tapStatus.GetComponent<TMP_Text>();

            if (text != null) {
                changed |= SetReference(
                    ref ui.tapStatusText,
                    text
                );
            }
        }

        if (waitingText != null) {
            TMP_Text text =
                waitingText.GetComponent<TMP_Text>();

            if (text != null) {
                changed |= SetReference(
                    ref ui.waitingText,
                    text
                );
            }
        }

        if (gameCodeText != null) {
            TMP_Text text =
                gameCodeText.GetComponent<TMP_Text>();

            if (text != null) {
                changed |= SetReference(
                    ref ui.gameCodeText,
                    text
                );
            }
        }

        if (joinInput != null) {
            TMP_InputField input =
                joinInput.GetComponent<TMP_InputField>();

            if (input != null) {
                changed |= SetReference(
                    ref ui.joinCodeInput,
                    input
                );
            }
        }

        changed |= SetReference(
            ref ui.pairingManager,
            pairingManager
        );

        changed |= SetReference(
            ref ui.relayManager,
            relayManager
        );

        if (changed)
            EditorUtility.SetDirty(ui);

        return changed;
    }

    private static bool SetPairingReferences(
        PairingManager pairingManager,
        BumpDetector bumpDetector,
        RelayManager relayManager,
        ConnectionUI connectionUI,
        ExchangeSession exchangeSession
    ) {
        bool changed = false;

        changed |= SetReference(
            ref pairingManager.bumpDetector,
            bumpDetector
        );

        changed |= SetReference(
            ref pairingManager.relayManager,
            relayManager
        );

        changed |= SetReference(
            ref pairingManager.connectionUI,
            connectionUI
        );

        changed |= SetReference(
            ref pairingManager.exchangeSession,
            exchangeSession
        );

        if (changed)
            EditorUtility.SetDirty(pairingManager);

        return changed;
    }

    private static bool SetRelayReferences(
        RelayManager relayManager,
        ConnectionUI connectionUI,
        PairingManager pairingManager
    ) {
        bool changed = false;

        SerializedObject serialized =
            new SerializedObject(relayManager);

        SerializedProperty connectionUIProperty =
            serialized.FindProperty("connectionUI");

        SerializedProperty pairingManagerProperty =
            serialized.FindProperty("pairingManager");

        if (connectionUIProperty == null) {
            Debug.LogError(
                "RelayManager does not contain a 'connectionUI' field."
            );
        }
        else if (
            connectionUIProperty.objectReferenceValue !=
            connectionUI
        ) {
            connectionUIProperty.objectReferenceValue =
                connectionUI;

            changed = true;
        }

        if (pairingManagerProperty == null) {
            Debug.LogError(
                "RelayManager does not contain a 'pairingManager' field."
            );
        }
        else if (
            pairingManagerProperty.objectReferenceValue !=
            pairingManager
        ) {
            pairingManagerProperty.objectReferenceValue =
                pairingManager;

            changed = true;
        }

        if (changed) {
            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(relayManager);
        }

        return changed;
    }

    private static bool SetBumpReferences(
        BumpDetector bumpDetector,
        PairingManager pairingManager,
        ExchangeSession exchangeSession
    ) {
        bool changed = false;

        changed |= SetReference(
            ref bumpDetector.pairingManager,
            pairingManager
        );

        changed |= SetReference(
            ref bumpDetector.exchangeSession,
            exchangeSession
        );

        if (changed)
            EditorUtility.SetDirty(bumpDetector);

        return changed;
    }

    private static bool WireButtons(
        ConnectionUI ui,
        GameObject home,
        GameObject join,
        GameObject relayWaiting
    ) {
        bool changed = false;

        changed |= SetButton(
            FindButton(
                home,
                "CreateGameButton"
            ),
            ui,
            "CreateGame"
        );

        changed |= SetButton(
            FindButton(
                home,
                "JoinGameButton"
            ),
            ui,
            "ShowJoin"
        );

        changed |= SetButton(
            FindButton(
                join,
                "JoinButton"
            ),
            ui,
            "JoinGameButton"
        );

        changed |= SetButton(
            FindButton(
                join,
                "CreateGameFromCodeButton"
            ),
            ui,
            "CreateGame"
        );

        changed |= SetButton(
            FindButton(
                join,
                "BackButton"
            ),
            ui,
            "ShowHome"
        );

        changed |= SetButton(
            FindButton(
                relayWaiting,
                "CancelButton"
            ),
            ui,
            "ShowHome"
        );

        return changed;
    }

    private static bool SetButton(
        Button button,
        ConnectionUI ui,
        string methodName
    ) {
        if (button == null) {
            Debug.LogWarning(
                "Button missing for: " +
                methodName
            );

            return false;
        }

        int count =
            button.onClick.GetPersistentEventCount();

        if (count == 1 &&
            button.onClick.GetPersistentTarget(0) == ui &&
            button.onClick.GetPersistentMethodName(0) ==
                methodName) {

            return false;
        }

        for (int i = count - 1; i >= 0; i--) {
            UnityEventTools.RemovePersistentListener(
                button.onClick,
                i
            );
        }

        switch (methodName) {
            case "CreateGame":
                UnityEventTools.AddPersistentListener(
                    button.onClick,
                    ui.CreateGame
                );
                break;

            case "ShowJoin":
                UnityEventTools.AddPersistentListener(
                    button.onClick,
                    ui.ShowJoin
                );
                break;

            case "JoinGameButton":
                UnityEventTools.AddPersistentListener(
                    button.onClick,
                    ui.JoinGameButton
                );
                break;

            case "ShowHome":
                UnityEventTools.AddPersistentListener(
                    button.onClick,
                    ui.ShowHome
                );
                break;

            default:
                Debug.LogError(
                    "Unknown ConnectionUI method: " +
                    methodName
                );

                return false;
        }

        EditorUtility.SetDirty(button);

        Debug.Log(
            "Wired " +
            button.name +
            " -> ConnectionUI." +
            methodName
        );

        return true;
    }

    private static bool DisableOldObjects() {
        bool changed = false;

        GameObject oldCanvas =
            FindSceneObject(
                "Old/OLD_Canvas"
            );

        GameObject oldUI =
            FindSceneObject(
                "Old/OLD_DuoConnectUI"
            );

        if (oldCanvas != null &&
            oldCanvas.activeSelf) {

            oldCanvas.SetActive(false);
            changed = true;

            Debug.Log(
                "Disabled Old/OLD_Canvas."
            );
        }

        if (oldUI != null &&
            oldUI.activeSelf) {

            oldUI.SetActive(false);
            changed = true;

            Debug.Log(
                "Disabled Old/OLD_DuoConnectUI."
            );
        }

        return changed;
    }

    private static bool SetReference<T>(
        ref T current,
        T value
    ) where T : Object {
        if (current == value)
            return false;

        current = value;
        return true;
    }

    private static Button FindButton(
        GameObject parent,
        string name
    ) {
        GameObject obj =
            FindChild(
                parent,
                name
            );

        if (obj == null)
            return null;

        return obj.GetComponent<Button>();
    }

    private static GameObject FindChild(
        GameObject parent,
        string name
    ) {
        if (parent == null)
            return null;

        Transform[] children =
            parent.GetComponentsInChildren<Transform>(
                true
            );

        foreach (Transform child in children) {
            if (child.name == name)
                return child.gameObject;
        }

        return null;
    }

    private static GameObject FindSceneObject(
        string path
    ) {
        string[] parts =
            path.Split('/');

        if (parts.Length == 0)
            return null;

        GameObject current =
            GameObject.Find(parts[0]);

        if (current == null)
            return null;

        for (int i = 1; i < parts.Length; i++) {
            Transform child =
                current.transform.Find(parts[i]);

            if (child == null)
                return null;

            current =
                child.gameObject;
        }

        return current;
    }
}
