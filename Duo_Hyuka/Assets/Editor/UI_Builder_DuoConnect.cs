using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public static class DuoConnectUIBuilder {
    [MenuItem("Tools/Useful Utilities/Build UI")]
    public static void BuildUI() {
        Canvas canvas = FindExistingCanvas();
        if (canvas == null) {
            ShowError("No Canvas was found in the scene.");
            return;
        }

        RelayManager relayManager = Object.FindAnyObjectByType<RelayManager>();
        if (relayManager == null) {
            ShowError("No RelayManager was found in the scene.");
            return;
        }

        DuoConnectUIStyles.ConfigureCanvas(canvas);

        Transform canvasTransform = canvas.transform;

        GameObject homePanel = DuoConnectUIComponents.GetOrCreatePanel(
            "HomePanel",
            canvasTransform,
            DuoConnectUIStyles.LightGray
        );

        GameObject joinPanel = DuoConnectUIComponents.GetOrCreatePanel(
            "JoinPanel",
            canvasTransform,
            DuoConnectUIStyles.LightGray
        );

        GameObject waitingPanel = DuoConnectUIComponents.GetOrCreatePanel(
            "WaitingPanel",
            canvasTransform,
            DuoConnectUIStyles.Green
        );

        GameObject tapPanel = DuoConnectUIComponents.GetOrCreatePanel(
            "TapPanel",
            canvasTransform,
            DuoConnectUIStyles.Green
        );

        Button tapButton = DuoConnectUIPanels.BuildHomePanel(homePanel.transform);
        Button joinWithCodeButton =
            DuoConnectUIPanels.BuildHomeJoinButton(homePanel.transform);

        TMP_InputField joinInput = FindExistingJoinInput();
        if (joinInput == null) {
            ShowError(
                "No JoinCodeInput was found. " +
                "Please keep your existing JoinCodeInput GameObject in the scene."
            );
            return;
        }

        Button joinButton;
        Button createGameButton;
        Button backButton;

        DuoConnectUIPanels.BuildJoinPanel(
            joinPanel.transform,
            joinInput,
            out joinButton,
            out createGameButton,
            out backButton
        );

        TMP_Text gameCodeText;
        TMP_Text waitingText;
        Button cancelWaitingButton;

        DuoConnectUIPanels.BuildWaitingPanel(
            waitingPanel.transform,
            out gameCodeText,
            out waitingText,
            out cancelWaitingButton
        );

        TMP_Text tapStatusText;
        Button cancelTapButton;

        DuoConnectUIPanels.BuildTapPanel(
            tapPanel.transform,
            out tapStatusText,
            out cancelTapButton
        );

        DuoConnectUI controller = GetOrCreateController();
        controller.homePanel = homePanel;
        controller.joinPanel = joinPanel;
        controller.waitingPanel = waitingPanel;
        controller.tapPanel = tapPanel;
        controller.joinCodeInput = joinInput;
        controller.gameCodeText = gameCodeText;
        controller.waitingText = waitingText;
        controller.tapStatusText = tapStatusText;
        controller.relayManager = relayManager;

        WireButtons(
            controller,
            tapButton,
            joinWithCodeButton,
            joinButton,
            createGameButton,
            backButton,
            cancelWaitingButton,
            cancelTapButton
        );

        SetInitialState(
            homePanel,
            joinPanel,
            waitingPanel,
            tapPanel
        );

        EnsureEventSystem();

        Selection.activeGameObject = controller.gameObject;
        EditorUtility.SetDirty(controller);
        EditorUtility.DisplayDialog(
            "Duo Connect",
            "UI successfully built/reused.",
            "OK"
        );
        Debug.Log("Duo Connect UI successfully built/reused.");
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

    private static TMP_InputField FindExistingJoinInput() {
        Canvas canvas = FindExistingCanvas();
        if (canvas == null) return null;
        GameObject inputObject =
            DuoConnectUIComponents.FindChild(
                canvas.transform,
                "JoinCodeInput"
            );
        if (inputObject == null) return null;
        return inputObject.GetComponent<TMP_InputField>();
    }

    private static DuoConnectUI GetOrCreateController() {
        DuoConnectUI existing = Object.FindAnyObjectByType<DuoConnectUI>();
        if (existing != null) {
            return existing;
        }

        GameObject obj = new GameObject("DuoConnectUI");
        return obj.AddComponent<DuoConnectUI>();
    }

    private static void WireButtons(
        DuoConnectUI controller,
        Button tapButton,
        Button joinWithCodeButton,
        Button joinButton,
        Button createGameButton,
        Button backButton,
        Button cancelWaitingButton,
        Button cancelTapButton) {

        DuoConnectUIComponents.AddClick(
            tapButton,
            controller.ShowTap
        );

        DuoConnectUIComponents.AddClick(
            joinWithCodeButton,
            controller.ShowJoin
        );

        DuoConnectUIComponents.AddClick(
            joinButton,
            controller.JoinGame
        );

        DuoConnectUIComponents.AddClick(
            createGameButton,
            controller.CreateGame
        );

        DuoConnectUIComponents.AddClick(
            backButton,
            controller.ShowHome
        );

        DuoConnectUIComponents.AddClick(
            cancelWaitingButton,
            controller.ShowHome
        );

        DuoConnectUIComponents.AddClick(
            cancelTapButton,
            controller.ShowHome
        );
    }

    private static void SetInitialState(
        GameObject homePanel,
        GameObject joinPanel,
        GameObject waitingPanel,
        GameObject tapPanel) {

        homePanel.SetActive(true);
        joinPanel.SetActive(false);
        waitingPanel.SetActive(false);
        tapPanel.SetActive(false);

        DisableLegacyObject("StatusText");
        DisableLegacyObject("JoinCodeText");
    }

    private static void DisableLegacyObject(string objectName) {
        Canvas canvas = FindExistingCanvas();
        if (canvas == null) return;

        GameObject obj =
            DuoConnectUIComponents.FindChild(
                canvas.transform,
                objectName
            );

        if (obj != null) {
            obj.SetActive(false);
        }
    }
    private static void EnsureEventSystem() {
        EventSystem eventSystem = Object.FindAnyObjectByType<EventSystem>();
        if (eventSystem != null) {
            return;
        }

        GameObject obj = new GameObject("EventSystem");
        obj.AddComponent<EventSystem>();
        obj.AddComponent<StandaloneInputModule>();
    }

    private static void ShowError(string message) {
        Debug.LogError("DuoConnectUIBuilder: " + message);

        EditorUtility.DisplayDialog(
            "Duo Connect UI",
            message,
            "OK"
        );
    }
}
