using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class SetupConnectionCanvas
{
    private const string SceneName = "00_Connection";

    private static readonly string[] ScreenNames =
    {
        "Screen_01_DevMenu",
        "Screen_02_Tap",
        "Screen_03_Waiting",
        "Screen_04_Confirmation",
        "Screen_05_Consent",
        "Screen_06_ConnectionMenu",
        "Screen_07_JoinCode",
        "Screen_08_RelayWaiting"
    };

    [MenuItem("Tools/Old Clean-ups/Setup Connection Canvas")]
    public static void Setup()
    {
        if (EditorSceneManager.GetActiveScene().name != SceneName)
        {
            Debug.LogError(
                "Open 00_Connection before running this tool."
            );
            return;
        }

        GameObject screens = GameObject.Find("Screens");

        if (screens == null)
        {
            Debug.LogError(
                "Could not find the 'Screens' GameObject."
            );
            return;
        }

        bool changed = false;

        // ---------------------------------------------------------
        // FIND OR CREATE UI CANVAS
        // ---------------------------------------------------------

        GameObject ui = FindDirectChild(screens, "UI");

        if (ui == null)
        {
            ui = new GameObject(
                "UI",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );

            ui.transform.SetParent(
                screens.transform,
                false
            );

            changed = true;

            Debug.Log(
                "Created Screens/UI."
            );
        }

        // ---------------------------------------------------------
        // ENSURE RECT TRANSFORM
        // ---------------------------------------------------------

        RectTransform uiRect =
            ui.GetComponent<RectTransform>();

        if (uiRect == null)
        {
            uiRect = ui.AddComponent<RectTransform>();
            changed = true;
        }

        // Canvas root should fill the screen.
        if (
            uiRect.anchorMin != Vector2.zero ||
            uiRect.anchorMax != Vector2.one ||
            uiRect.offsetMin != Vector2.zero ||
            uiRect.offsetMax != Vector2.zero
        )
        {
            uiRect.anchorMin = Vector2.zero;
            uiRect.anchorMax = Vector2.one;
            uiRect.offsetMin = Vector2.zero;
            uiRect.offsetMax = Vector2.zero;

            changed = true;
        }

        // ---------------------------------------------------------
        // CANVAS
        // ---------------------------------------------------------

        Canvas canvas =
            ui.GetComponent<Canvas>();

        if (canvas == null)
        {
            canvas = ui.AddComponent<Canvas>();
            changed = true;
        }

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvas.renderMode =
                RenderMode.ScreenSpaceOverlay;

            changed = true;
        }

        // Make sure it is the main UI canvas.
        if (!canvas.pixelPerfect)
        {
            canvas.pixelPerfect = true;
            changed = true;
        }

        // ---------------------------------------------------------
        // CANVAS SCALER
        // ---------------------------------------------------------

        CanvasScaler scaler =
            ui.GetComponent<CanvasScaler>();

        if (scaler == null)
        {
            scaler = ui.AddComponent<CanvasScaler>();
            changed = true;
        }

        /*
         * We use Scale With Screen Size so Hilary's artwork
         * scales consistently across phones.
         *
         * 1080 x 1920 is a common portrait reference resolution.
         *
         * We are NOT changing any screen artwork or RectTransforms.
         */
        if (
            scaler.uiScaleMode !=
            CanvasScaler.ScaleMode.ScaleWithScreenSize
        )
        {
            scaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;

            changed = true;
        }

        if (scaler.referenceResolution !=
            new Vector2(1080f, 1920f))
        {
            scaler.referenceResolution =
                new Vector2(1080f, 1920f);

            changed = true;
        }

        if (Mathf.Abs(scaler.matchWidthOrHeight - 0.5f) >
            0.001f)
        {
            scaler.matchWidthOrHeight = 0.5f;
            changed = true;
        }

        // ---------------------------------------------------------
        // GRAPHIC RAYCASTER
        // ---------------------------------------------------------

        GraphicRaycaster raycaster =
            ui.GetComponent<GraphicRaycaster>();

        if (raycaster == null)
        {
            ui.AddComponent<GraphicRaycaster>();
            changed = true;
        }

        // ---------------------------------------------------------
        // MOVE HILARY SCREENS UNDER UI
        // ---------------------------------------------------------

        foreach (string screenName in ScreenNames)
        {
            GameObject screen =
                FindDirectChild(screens, screenName);

            if (screen == null)
            {
                Debug.LogWarning(
                    "Could not find " + screenName
                );

                continue;
            }

            if (screen.transform.parent != ui.transform)
            {
                /*
                 * Preserve world position/scale while moving.
                 * This minimizes visual changes to Hilary's design.
                 */
                screen.transform.SetParent(
                    ui.transform,
                    true
                );

                changed = true;

                Debug.Log(
                    "Moved " +
                    screenName +
                    " under Screens/UI."
                );
            }
        }

        // ---------------------------------------------------------
        // MAKE UI ACTIVE
        // ---------------------------------------------------------

        if (!ui.activeSelf)
        {
            ui.SetActive(true);
            changed = true;
        }

        // ---------------------------------------------------------
        // DO NOT TOUCH OLD CANVAS
        // ---------------------------------------------------------

        GameObject oldCanvas =
            FindNestedObject(
                "Old",
                "OLD_Canvas"
            );

        if (oldCanvas != null &&
            oldCanvas.activeSelf)
        {
            oldCanvas.SetActive(false);
            changed = true;

            Debug.Log(
                "Kept Old/OLD_Canvas as backup and disabled it."
            );
        }

        // ---------------------------------------------------------
        // SAVE
        // ---------------------------------------------------------

        EditorUtility.SetDirty(ui);
        EditorUtility.SetDirty(canvas);
        EditorUtility.SetDirty(scaler);

        if (changed)
        {
            EditorSceneManager.MarkSceneDirty(
                EditorSceneManager.GetActiveScene()
            );

            EditorSceneManager.SaveScene(
                EditorSceneManager.GetActiveScene()
            );

            Debug.Log(
                "Connection Canvas setup complete."
            );
        }
        else
        {
            Debug.Log(
                "Connection Canvas is already set up. " +
                "No changes were necessary."
            );
        }

        AssetDatabase.Refresh();
    }

    // =============================================================
    // HELPERS
    // =============================================================

    private static GameObject FindDirectChild(
        GameObject parent,
        string childName
    )
    {
        if (parent == null)
            return null;

        Transform child =
            parent.transform.Find(childName);

        if (child == null)
            return null;

        return child.gameObject;
    }

    private static GameObject FindNestedObject(
        string parentName,
        string childName
    )
    {
        GameObject parent =
            GameObject.Find(parentName);

        if (parent == null)
            return null;

        return FindDirectChild(
            parent,
            childName
        );
    }
}
