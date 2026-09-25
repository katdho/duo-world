using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConnectionToolsMenu
{
    private const string SceneName = "00_Connection";
    private const string ScreensPath = "Screens/UI";

    // =========================================================
    // PREVIEW SCREENS
    // =========================================================

    [MenuItem("Tools/Preview Screens/Connection: Dev Menu")]
    public static void PreviewDevMenu()
    {
        ShowScreen("Screen_01_DevMenu");
    }

    [MenuItem("Tools/Preview Screens/Connection: Tap")]
    public static void PreviewTap()
    {
        ShowScreen("Screen_02_Tap");
    }

    [MenuItem("Tools/Preview Screens/Connection: Waiting")]
    public static void PreviewWaiting()
    {
        ShowScreen("Screen_03_Waiting");
    }

    [MenuItem("Tools/Preview Screens/Connection: Confirmation")]
    public static void PreviewConfirmation()
    {
        ShowScreen("Screen_04_Confirmation");
    }

    [MenuItem("Tools/Preview Screens/Connection: Consent")]
    public static void PreviewConsent()
    {
        ShowScreen("Screen_05_Consent");
    }

    [MenuItem("Tools/Preview Screens/Connection: Connection Menu")]
    public static void PreviewConnectionMenu()
    {
        ShowScreen("Screen_06_ConnectionMenu");
    }

    [MenuItem("Tools/Preview Screens/Connection: Join Code")]
    public static void PreviewJoinCode()
    {
        ShowScreen("Screen_07_JoinCode");
    }

    [MenuItem("Tools/Preview Screens/Connection: Relay Waiting")]
    public static void PreviewRelayWaiting()
    {
        ShowScreen("Screen_08_RelayWaiting");
    }

    [MenuItem("Tools/Preview Screens/Connection: Show All")]
    public static void PreviewAll()
    {
        ShowAllScreens();
    }

    // =========================================================
    // USEFUL UTILITIES
    // =========================================================

    [MenuItem("Tools/Useful Utilities/Print Scene Hierarchy")]
    public static void PrintSceneHierarchy()
    {
        if (!IsCorrectScene())
            return;

        GameObject[] roots =
            UnityEngine.SceneManagement
                .SceneManager
                .GetActiveScene()
                .GetRootGameObjects();

        Debug.Log(
            "\n========== SCENE HIERARCHY ==========\n"
        );

        foreach (GameObject root in roots)
        {
            PrintTransform(root.transform, 0);
        }

        Debug.Log(
            "\n========== END HIERARCHY ==========\n"
        );
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    [MenuItem("Tools/Old Clean-ups/Delete Old Tools Menu")]
    public static void DeleteOldToolsMenu()
    {
        string oldScript =
            "Assets/Editor/DuoConnectUIPreview.cs";

        if (AssetDatabase.LoadAssetAtPath<MonoScript>(
            oldScript
        ) != null)
        {
            AssetDatabase.DeleteAsset(oldScript);

            Debug.Log(
                "Deleted old DuoConnectUIPreview.cs"
            );
        }
        else
        {
            Debug.Log(
                "Old DuoConnectUIPreview.cs was already deleted."
            );
        }

        AssetDatabase.Refresh();
    }

    // =========================================================
    // SCREEN PREVIEW
    // =========================================================

    private static void ShowScreen(string screenName)
    {
        if (!IsCorrectScene())
            return;

        Transform ui = FindUI();

        if (ui == null)
        {
            Debug.LogError(
                "Could not find Screens/UI."
            );
            return;
        }

        Transform[] screens =
            GetConnectionScreens(ui);

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
                "Could not find screen: " +
                screenName
            );

            return;
        }

        // -----------------------------------------------------
        // Every individual preview starts from the normal
        // stacked layout.
        // -----------------------------------------------------

        RestoreStackedLayout(screens);

        // -----------------------------------------------------
        // Only the selected screen is visible.
        // -----------------------------------------------------

        foreach (Transform screen in screens)
        {
            screen.gameObject.SetActive(
                screen == target
            );
        }

        SaveScene();

        Debug.Log(
            "Previewing: " +
            screenName
        );
    }

    // =========================================================
    // SHOW ALL
    // =========================================================

    private static void ShowAllScreens()
    {
        if (!IsCorrectScene())
            return;

        Transform ui = FindUI();

        if (ui == null)
        {
            Debug.LogError(
                "Could not find Screens/UI."
            );
            return;
        }

        Transform[] screens =
            GetConnectionScreens(ui);

        if (screens.Length == 0)
        {
            Debug.LogError(
                "No connection screens found."
            );

            return;
        }

        // -----------------------------------------------------
        // Temporarily show every screen.
        // -----------------------------------------------------

        foreach (Transform screen in screens)
        {
            screen.gameObject.SetActive(true);
        }

        // -----------------------------------------------------
        // Arrange them in a 4 x 2 grid.
        // -----------------------------------------------------

        const float horizontalSpacing = 1100f;
        const float verticalSpacing = 800f;

        const int columns = 4;

        for (int i = 0; i < screens.Length; i++)
        {
            int row = i / columns;
            int column = i % columns;

            float x =
                (column - 1.5f) *
                horizontalSpacing;

            float y =
                (0.5f - row) *
                verticalSpacing;

            screens[i].localPosition =
                new Vector3(
                    x,
                    y,
                    0f
                );

            screens[i].localRotation =
                Quaternion.identity;

            screens[i].localScale =
                Vector3.one;
        }

        SaveScene();

        Debug.Log(
            "Showing all connection screens."
        );
    }

    // =========================================================
    // STACKED LAYOUT
    // =========================================================

    private static void RestoreStackedLayout(
        Transform[] screens
    )
    {
        foreach (Transform screen in screens)
        {
            screen.localPosition =
                Vector3.zero;

            screen.localRotation =
                Quaternion.identity;

            screen.localScale =
                Vector3.one;
        }
    }

    // =========================================================
    // FIND UI
    // =========================================================

    private static Transform FindUI()
    {
        GameObject screens =
            GameObject.Find("Screens");

        if (screens == null)
            return null;

        return screens.transform.Find("UI");
    }

    // =========================================================
    // FIND CONNECTION SCREENS
    // =========================================================

    private static Transform[] GetConnectionScreens(
        Transform ui
    )
    {
        System.Collections.Generic.List<Transform>
            result =
                new System.Collections.Generic.List<Transform>();

        for (int i = 0; i < ui.childCount; i++)
        {
            Transform child =
                ui.GetChild(i);

            if (child.name.StartsWith("Screen_"))
            {
                result.Add(child);
            }
        }

        result.Sort(
            (a, b) =>
                string.Compare(
                    a.name,
                    b.name,
                    System.StringComparison.Ordinal
                )
        );

        return result.ToArray();
    }

    // =========================================================
    // SCENE VALIDATION
    // =========================================================

    private static bool IsCorrectScene()
    {
        if (
            UnityEngine.SceneManagement
                .SceneManager
                .GetActiveScene()
                .name != SceneName
        )
        {
            Debug.LogError(
                "Open " +
                SceneName +
                " before using this tool."
            );

            return false;
        }

        return true;
    }

    // =========================================================
    // SAVE
    // =========================================================

    private static void SaveScene()
    {
        EditorSceneManager.MarkSceneDirty(
            EditorSceneManager.GetActiveScene()
        );

        EditorSceneManager.SaveScene(
            EditorSceneManager.GetActiveScene()
        );
    }

    // =========================================================
    // PRINT HIERARCHY
    // =========================================================

    private static void PrintTransform(
        Transform transform,
        int depth
    )
    {
        string indent =
            new string(
                ' ',
                depth * 2
            );

        Debug.Log(
            indent +
            "- " +
            transform.name
        );

        for (int i = 0; i < transform.childCount; i++)
        {
            PrintTransform(
                transform.GetChild(i),
                depth + 1
            );
        }
    }
}

