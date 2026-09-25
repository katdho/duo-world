using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class DuoConnectUIPanels {
    public static Button BuildHomePanel(Transform parent) {
        DuoConnectUIComponents.GetOrCreateText(
            "HomeTitleText",
            parent,
            "DUO CONNECT",
            72,
            DuoConnectUIStyles.DarkText,
            new Vector2(0, 500),
            new Vector2(900, 160)
        );

        DuoConnectUIComponents.GetOrCreateText(
            "HomeSubtitleText",
            parent,
            "Learn through real conversation",
            32,
            DuoConnectUIStyles.DarkText,
            new Vector2(0, 350),
            new Vector2(900, 100)
        );

        GameObject tapObject = DuoConnectUIComponents.FindChild(parent, "CreateGameButton");
        if (tapObject == null) {
            tapObject = DuoConnectUIComponents.GetOrCreateObject("CreateGameButton", parent);
        }

        Button tapButton = tapObject.GetComponent<Button>();
        if (tapButton == null) {
            tapButton = tapObject.AddComponent<Button>();
        }

        DuoConnectUIComponents.MoveToParent(tapObject, parent);

        DuoConnectUIStyles.ConfigureButton(
            tapButton,
            "TAP PHONES TOGETHER",
            DuoConnectUIStyles.Green,
            DuoConnectUIStyles.White,
            34
        );

        DuoConnectUIComponents.SetRect(
            tapObject.GetComponent<RectTransform>(),
            new Vector2(0, -20),
            new Vector2(840, 150)
        );

        return tapButton;
    }

    public static Button BuildHomeJoinButton(Transform parent) {
        DuoConnectUIComponents.GetOrCreateText(
            "HomeOrText",
            parent,
            "OR",
            28,
            DuoConnectUIStyles.Gray,
            new Vector2(0, -210),
            new Vector2(300, 70)
        );

        GameObject joinObject = DuoConnectUIComponents.FindChild(parent, "JoinGameButton");
        if (joinObject == null) {
            joinObject = DuoConnectUIComponents.GetOrCreateObject("JoinGameButton", parent);
        }

        Button joinButton = joinObject.GetComponent<Button>();
        if (joinButton == null) {
            joinButton = joinObject.AddComponent<Button>();
        }

        DuoConnectUIStyles.ConfigureButton(
            joinButton,
            "JOIN WITH CODE",
            DuoConnectUIStyles.White,
            DuoConnectUIStyles.DarkText,
            34
        );

        DuoConnectUIComponents.SetRect(
            joinObject.GetComponent<RectTransform>(),
            new Vector2(0, -390),
            new Vector2(840, 150)
        );

        return joinButton;
    }

    public static void BuildJoinPanel(Transform parent, TMP_InputField joinInput,
                                       out Button joinButton, out Button createGameButton,
                                       out Button backButton) {
        DuoConnectUIComponents.GetOrCreateText(
            "JoinTitleText",
            parent,
            "JOIN GAME",
            64,
            DuoConnectUIStyles.DarkText,
            new Vector2(0, 500),
            new Vector2(900, 140)
        );

        DuoConnectUIComponents.GetOrCreateText(
            "JoinInstructionText",
            parent,
            "Enter your partner's game code",
            30,
            DuoConnectUIStyles.Gray,
            new Vector2(0, 360),
            new Vector2(900, 100)
        );

        if (joinInput == null) {
            Debug.LogError("DuoConnectUIPanels: JoinCodeInput was not found.");
        }
        else {
            GameObject inputObject = joinInput.gameObject;
            DuoConnectUIComponents.MoveToParent(inputObject, parent);
            DuoConnectUIStyles.ConfigureInputField(joinInput);

            DuoConnectUIComponents.SetRect(
                inputObject.GetComponent<RectTransform>(),
                new Vector2(0, 190),
                new Vector2(840, 150)
            );
        }

        joinButton = DuoConnectUIComponents.GetOrCreateButton(
            "JoinButton",
            parent,
            "JOIN",
            new Vector2(0, -20),
            new Vector2(840, 150),
            DuoConnectUIStyles.Green,
            DuoConnectUIStyles.White
        );

        createGameButton = DuoConnectUIComponents.GetOrCreateButton(
            "CreateGameFromCodeButton",
            parent,
            "CREATE A GAME",
            new Vector2(0, -210),
            new Vector2(840, 135),
            DuoConnectUIStyles.White,
            DuoConnectUIStyles.DarkGreen
        );

        backButton = DuoConnectUIComponents.GetOrCreateButton(
            "BackButton",
            parent,
            "BACK",
            new Vector2(0, -430),
            new Vector2(500, 120),
            DuoConnectUIStyles.LightGray,
            DuoConnectUIStyles.Gray
        );
    }

    public static void BuildWaitingPanel(Transform parent, out TMP_Text gameCodeText,
                                         out TMP_Text waitingText, out Button cancelButton) {
        DuoConnectUIComponents.GetOrCreateText(
            "WaitingTitleText",
            parent,
            "GAME CREATED",
            60,
            DuoConnectUIStyles.White,
            new Vector2(0, 500),
            new Vector2(900, 140)
        );

        gameCodeText = DuoConnectUIComponents.GetOrCreateText(
            "GameCodeText",
            parent,
            "Game Code",
            64,
            DuoConnectUIStyles.White,
            new Vector2(0, 250),
            new Vector2(900, 180)
        );

        waitingText = DuoConnectUIComponents.GetOrCreateText(
            "WaitingText",
            parent,
            "Waiting for your partner...",
            32,
            DuoConnectUIStyles.White,
            new Vector2(0, 60),
            new Vector2(900, 100)
        );

        cancelButton = DuoConnectUIComponents.GetOrCreateButton(
            "CancelButton",
            parent,
            "CANCEL",
            new Vector2(0, -430),
            new Vector2(500, 120),
            DuoConnectUIStyles.White,
            DuoConnectUIStyles.DarkText
        );
    }

    public static void BuildTapPanel(Transform parent, out TMP_Text tapStatusText,
                                     out Button cancelButton) {
        DuoConnectUIComponents.GetOrCreateText(
            "TapTitleText",
            parent,
            "TAP PHONES TOGETHER",
            56,
            DuoConnectUIStyles.White,
            new Vector2(0, 500),
            new Vector2(900, 180)
        );

        DuoConnectUIComponents.GetOrCreateText(
            "TapInstructionText",
            parent,
            "Hold your phones together\nand tap them gently.",
            34,
            DuoConnectUIStyles.White,
            new Vector2(0, 270),
            new Vector2(900, 180)
        );

        tapStatusText = DuoConnectUIComponents.GetOrCreateText(
            "TapStatusText",
            parent,
            "Waiting for another phone...",
            32,
            DuoConnectUIStyles.White,
            new Vector2(0, -80),
            new Vector2(900, 150)
        );

        cancelButton = DuoConnectUIComponents.GetOrCreateButton(
            "CancelTapButton",
            parent,
            "CANCEL",
            new Vector2(0, -430),
            new Vector2(500, 120),
            DuoConnectUIStyles.White,
            DuoConnectUIStyles.DarkText
        );
    }
}
