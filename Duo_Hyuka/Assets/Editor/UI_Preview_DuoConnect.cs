using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConnectionScreenPreview
{
    private const string ScreensPath = "Screens/UI";

    // ---------------------------------------------------------
    // PREVIEW SCREENS
    // ---------------------------------------------------------

    [MenuItem("Tools/Preview Screens/Connection: Dev Menu")]
    public static void ShowDevMenu()
    {
        ShowScreen("Screen_01_DevMenu");
    }

    [MenuItem("Tools/Preview Screens/Connection: Tap")]
    public static void ShowTap()
    {
        ShowScreen("Screen_02_Tap");
    }

    [MenuItem("Tools/Preview Screens/Connection: Waiting")]
    public static void ShowWaiting()
    {
        ShowScreen("Screen_03_Waiting");
    }

    [MenuItem("Tools/Preview Screens/Connection: Confirmation")]
    public static void ShowConfirmation()
    {
        ShowScreen("Screen_04_Confirmation");
    }

    [MenuItem("Tools/Preview Screens/Connection: Consent")]
    public static void ShowConsent()
    {
        ShowScreen("Screen_05_Consent");
    }

    [MenuItem("Tools/Preview Screens/Connection: Connection Menu")]
    public static void ShowConnectionMenu()
    {
        ShowScreen("Screen_06_ConnectionMenu");
    }

    [MenuItem("Tools/Preview Screens/Connection: Join Code")]
    public static void ShowJoinCode()
    {
        ShowScreen("Screen_07_JoinCode");
    }

    [MenuItem("Tools/Preview Screens/Connection: Relay Waiting")]
    public static void ShowRelayWaiting()
    {
        ShowScreen("Screen_08_RelayWaiting");
    }

    [MenuItem("Tools/Preview Screens/Connection: Show All")]
    public static void ShowAll()
    {
        Transform ui = FindUI();

        if (ui == null)
        {
            Debug.LogError(
                "ConnectionScreenPreview: Could not find Screens/UI."
            );
            return;
        }

        Transform[] screens = GetScreens(ui);

        if (screens.Length == 0)
        {
            Debug.LogError(
                "ConnectionScreenPreview: No connection screens found."
            );
            return;
        }

        // Make every screen visible.
        foreach (Transform screen in screens)
        {
            screen.gameObject.SetActive(true);
        }

        // -----------------------------------------------------
        // Arrange screens in a grid.
        //
        // These values are editor-preview positions only.
        // Individual preview restores the stacked layout.
        // -----------------------------------------------------

        const float horizontalSpacing = 1100f;
        const float verticalSpacing = 800f;

        int columns = 4;

        for (int i = 0; i < screens.Length; i++)
        {
            int row = i / columns;
            int column = i % columns;

            float x =
                (column - 1.5f) * horizontalSpacing;

            float y =
                -(row - 0.5f) * verticalSpacing;

            SetLocalPosition(
                screens[i],
                new Vector3(x, y, 0f)
            );

            screens[i].localRotation = Quaternion.identity;
            screens[i].localScale = Vector3.one;
        }

        MarkSceneDirty();

        Debug.Log(
            "Connection preview: showing all screens."
        );
    }

    // ---------------------------------------------------------
    // INDIVIDUAL SCREEN
    // ---------------------------------------------------------

    private static void ShowScreen(string screenName)
    {
        Transform ui = FindUI();

        if (ui == null)
        {
            Debug.LogError(
                "ConnectionScreenPreview: Could not find Screens/UI."
            );
            return;
        }

        Transform[] screens = GetScreens(ui);

        Transform target = null;

        foreach (Transform screen in screens)
        {
            if (screen.name == screenName)
            {
                target = screen;
                break;
            }
        }

        if (target == null)
        {
            Debug.LogError(
                "ConnectionScreenPreview: Could not find " +
                screenName
            );
            return;
        }

        // -----------------------------------------------------
        // IMPORTANT:
        // First restore ALL screens to their stacked layout.
        // -----------------------------------------------------

        RestoreStackedPositions(screens);

        // -----------------------------------------------------
        // Then only show the requested screen.
        // -----------------------------------------------------

        foreach (Transform screen in screens)
        {
            screen.gameObject.SetActive(
                screen == target
            );
        }

        MarkSceneDirty();

        Debug.Log(
            "Connection preview: showing " +
            screenName
        );
    }

    // ---------------------------------------------------------
    // RESTORE STACKED POSITIONS
    // ---------------------------------------------------------

    private static void RestoreStackedPositions(
        Transform[] screens
    )
    {
        foreach (Transform screen in screens)
        {
            SetLocalPosition(
                screen,
                Vector3.zero
            );

            screen.localRotation =
                Quaternion.identity;

            screen.localScale =
                Vector3.one;
        }
    }

    // ---------------------------------------------------------
    // FIND UI
    // ---------------------------------------------------------

    private static Transform FindUI()
    {
        GameObject screens =
            GameObject.Find("Screens");

        if (screens == null)
            return null;

        Transform ui =
            screens.transform.Find("UI");

        return ui;
    }

    // ---------------------------------------------------------
    // GET CONNECTION SCREENS
    // ---------------------------------------------------------

    private static Transform[] GetScreens(
        Transform ui
    )
    {
        Transform[] allChildren =
            ui.GetComponentsInChildren<Transform>(
                true
            );

        System.Collections.Generic.List<Transform>
            screens =
            new System.Collections.Generic.List<Transform>();

        foreach (Transform child in allChildren)
        {
            if (child.parent != ui)
                continue;

            if (child.name.StartsWith("Screen_"))
            {
                screens.Add(child);
            }
        }

        screens.Sort(
            (a, b) =>
                string.Compare(
                    a.name,
                    b.name,
                    System.StringComparison.Ordinal
                )
        );

        return screens.ToArray();
    }

    // ---------------------------------------------------------
    // POSITION HELPER
    // ---------------------------------------------------------

    private static void SetLocalPosition(
        Transform transform,
        Vector3 position
    )
    {
        transform.localPosition = position;
    }

    // ---------------------------------------------------------
    // SCENE
    // ---------------------------------------------------------

    private static void MarkSceneDirty()
    {
        EditorSceneManager.MarkSceneDirty(
            EditorSceneManager.GetActiveScene()
        );
    }
}
