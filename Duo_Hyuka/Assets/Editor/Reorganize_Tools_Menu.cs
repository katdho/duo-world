using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ReorganizeToolsMenu
{
    private const string EditorFolder = "Assets/Editor";

    [MenuItem("Tools/Old Clean-ups/Reorganize Tools Menu")]
    public static void Run()
    {
        if (!Directory.Exists(EditorFolder))
        {
            Debug.LogError(
                "Could not find Assets/Editor."
            );
            return;
        }

        int changedFiles = 0;

        string[] files =
            Directory.GetFiles(
                EditorFolder,
                "*.cs",
                SearchOption.AllDirectories
            );

        foreach (string file in files)
        {
            // Don't modify this migration script itself.
            if (
                Path.GetFileName(file) ==
                "ReorganizeToolsMenu.cs"
            )
            {
                continue;
            }

            if (UpdateFile(file))
            {
                changedFiles++;
            }
        }

        AssetDatabase.Refresh();

        Debug.Log(
            "========================================"
        );

        Debug.Log(
            "Tools menu reorganization complete."
        );

        Debug.Log(
            "Files changed: " +
            changedFiles
        );

        Debug.Log(
            "========================================"
        );
    }

    private static bool UpdateFile(string path)
    {
        string original =
            File.ReadAllText(path);

        string text = original;

        // =====================================================
        // BUILD UI
        // =====================================================

        text = ReplaceMenu(
            text,
            "Tools/Build UI",
            "Tools/Useful Utilities/Build UI"
        );

        text = ReplaceMenu(
            text,
            "Tools/Duo Connect/Build UI",
            "Tools/Useful Utilities/Build UI"
        );

        // =====================================================
        // PRINT SCENE HIERARCHY
        // =====================================================

        text = ReplaceMenu(
            text,
            "Tools/Print Scene Hierarchy",
            "Tools/Useful Utilities/Print Scene Hierarchy"
        );

        text = ReplaceMenu(
            text,
            "Tools/Duo Connect/Print Scene Hierarchy",
            "Tools/Useful Utilities/Print Scene Hierarchy"
        );

        // =====================================================
        // MIGRATE CONNECTION SCENE
        // =====================================================

        text = ReplaceMenu(
            text,
            "Tools/Migrate Connection Scene",
            "Tools/Cleanup/Migrate to Connection Scene"
        );

        text = ReplaceMenu(
            text,
            "Tools/Duo Connect/Migrate Connection Scene",
            "Tools/Cleanup/Migrate to Connection Scene"
        );

        text = ReplaceMenu(
            text,
            "Tools/Duo Connect/Migrate to Connection Scene",
            "Tools/Cleanup/Migrate to Connection Scene"
        );

        // =====================================================
        // ORGANIZE CONNECTION HIERARCHY
        // =====================================================

        text = ReplaceMenu(
            text,
            "Tools/Organize Connection Hierarchy",
            "Tools/Cleanup/Organize Connection Hierarchy"
        );

        text = ReplaceMenu(
            text,
            "Tools/Duo Connect/Organize Connection Hierarchy",
            "Tools/Cleanup/Organize Connection Hierarchy"
        );

        text = ReplaceMenu(
            text,
            "Tools/Duo Connect/Organize Connection Scene",
            "Tools/Cleanup/Organize Connection Hierarchy"
        );

        // =====================================================
        // ORGANIZE DUO SCENES
        // =====================================================

        text = ReplaceMenu(
            text,
            "Tools/Organize Duo Scenes",
            "Tools/Cleanup/Organize Duo Scenes"
        );

        text = ReplaceMenu(
            text,
            "Tools/Duo Connect/Organize Duo Scenes",
            "Tools/Cleanup/Organize Duo Scenes"
        );

        // =====================================================
        // REMOVE OLD DUO CONNECT MENU ENTRIES
        //
        // Only remove MenuItem attributes that still point to
        // the old Duo Connect menu.
        //
        // We deliberately do NOT delete the scripts.
        // =====================================================

        text =
            RemoveOldDuoConnectMenuItems(text);

        if (text == original)
            return false;

        File.WriteAllText(
            path,
            text
        );

        Debug.Log(
            "Updated Tools menu: " +
            path
        );

        return true;
    }

    // =========================================================
    // REPLACE MENU PATH
    // =========================================================

    private static string ReplaceMenu(
        string text,
        string oldMenu,
        string newMenu
    )
    {
        string oldAttribute =
            "[MenuItem(\"" +
            oldMenu +
            "\")]";

        string newAttribute =
            "[MenuItem(\"" +
            newMenu +
            "\")]";

        return text.Replace(
            oldAttribute,
            newAttribute
        );
    }

    // =========================================================
    // REMOVE OLD DUO CONNECT MENU ITEMS
    // =========================================================
    //
    // We only remove complete MenuItem attribute lines that
    // still use the old "Tools/Duo Connect/" path.
    //
    // The underlying methods remain untouched.
    // =========================================================

    private static string RemoveOldDuoConnectMenuItems(
        string text
    )
    {
        using (StringReader reader =
            new StringReader(text))
        using (StringWriter writer =
            new StringWriter())
        {
            string line;

            while (
                (line = reader.ReadLine()) != null
            )
            {
                string trimmed =
                    line.Trim();

                bool isOldDuoMenu =
                    trimmed.StartsWith(
                        "[MenuItem(\"Tools/Duo Connect/"
                    );

                if (isOldDuoMenu)
                {
                    continue;
                }

                writer.WriteLine(line);
            }

            return writer.ToString();
        }
    }
}
