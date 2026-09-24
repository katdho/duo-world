using UnityEditor;
using UnityEngine;

public static class DuoConnectUIPreview {
    [MenuItem("Tools/Duo Connect/Show Home")]
    public static void ShowHomePreview() {
        ShowPanelPreview("HomePanel");
    }

    [MenuItem("Tools/Duo Connect/Show Join")]
    public static void ShowJoinPreview() {
        ShowPanelPreview("JoinPanel");
    }

    [MenuItem("Tools/Duo Connect/Show Waiting")]
    public static void ShowWaitingPreview() {
        ShowPanelPreview("WaitingPanel");
    }

    [MenuItem("Tools/Duo Connect/Show Tap")]
    public static void ShowTapPreview() {
        ShowPanelPreview("TapPanel");
    }

    private static void ShowPanelPreview(string panelName) {
        Canvas canvas = FindExistingCanvas();
        if (canvas == null) {
            Debug.LogError("DuoConnectUIPreview: No Canvas found.");
            return;
        }

        string[] panelNames = {
            "HomePanel",
            "JoinPanel",
            "WaitingPanel",
            "TapPanel"
        };
        foreach (string name in panelNames) {
            GameObject panel = FindChild(canvas.transform, name);
            if (panel != null) {
                panel.SetActive(name == panelName);
            }
        }
        Debug.Log("Showing Duo Connect preview: " + panelName);
    }

    private static Canvas FindExistingCanvas() {
        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject != null) {
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            if (canvas != null) {
                return canvas;
            }
        }
        return Object.FindAnyObjectByType<Canvas>();
    }

    private static GameObject FindChild(Transform parent, string objectName) {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children) {
            if (child.name == objectName) {
                return child.gameObject;
            }
        }
        return null;
    }
}