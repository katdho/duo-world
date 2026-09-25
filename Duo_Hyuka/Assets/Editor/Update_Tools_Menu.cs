using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FinalizeToolsMenu
{
    private const string EditorFolder = "Assets/Editor";

    [MenuItem("Tools/Old Clean-ups/Finalize Tools Menu")]
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
            if (
                Path.GetFileName(file) ==
                "FinalizeToolsMenu.cs"
            )
            {
                continue;
            }

            if (ProcessFile(file))
                changedFiles++;
        }

        AssetDatabase.Refresh();

        Debug.Log(
            "Tools menu finalized. Files changed: " +
            changedFiles
        );
    }

    private static bool ProcessFile(string path)
    {
        string original =
            File.ReadAllText(path);

        string text = original;

        // -----------------------------------------------------
        // SPECIFIC RENAMES
        // -----------------------------------------------------

        text = ReplaceMenu(
            text,
            "Tools/Organize 00_Connection",
            "Tools/Old Clean-ups/Organize Connection Hierarchy"
        );

        text = ReplaceMenu(
            text,
            "Tools/Duo/Organize 00_Connection",
            "Tools/Old Clean-ups/Organize Connection Hierarchy"
        );

        text = ReplaceMenu(
            text,
            "Tools/Duo/Organize Connection Hierarchy",
            "Tools/Old Clean-ups/Organize Connection Hierarchy"
        );

        text = ReplaceMenu(
            text,
            "Tools/Migrate 00_Connection",
            "Tools/Old Clean-ups/Migrate to Connection Scene"
        );

        text = ReplaceMenu(
            text,
            "Tools/Duo/Migrate 00_Connection",
            "Tools/Old Clean-ups/Migrate to Connection Scene"
        );

        text = ReplaceMenu(
            text,
            "Tools/Duo/Migrate to Connection Scene",
            "Tools/Old Clean-ups/Migrate to Connection Scene"
        );

        // -----------------------------------------------------
        // RENAME EXISTING CLEANUP COMMANDS
        // -----------------------------------------------------

        text =
            MoveCleanupCommandsToOldCleanups(text);

        // -----------------------------------------------------
        // MOVE ALL REMAINING TOOLS/DUO COMMANDS
        // -----------------------------------------------------

        text =
            MoveDuoCommandsToOldCleanups(text);

        if (text == original)
            return false;

        File.WriteAllText(
            path,
            text
        );

        Debug.Log(
            "Updated menu paths: " +
            path
        );

        return true;
    }

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

    private static string MoveCleanupCommandsToOldCleanups(
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

                const string prefix =
                    "[MenuItem(\"Tools/Cleanup/";

                if (trimmed.StartsWith(prefix))
                {
                    int start =
                        trimmed.IndexOf(
                            prefix,
                            StringComparison.Ordinal
                        );

                    int end =
                        trimmed.IndexOf(
                            "\")]",
                            start,
                            StringComparison.Ordinal
                        );

                    if (start >= 0 && end > start)
                    {
                        string command =
                            trimmed.Substring(
                                start + prefix.Length,
                                end -
                                (start + prefix.Length)
                            );

                        string replacement =
                            "[MenuItem(\"Tools/Old Clean-ups/" +
                            command +
                            "\")]";

                        string indentation =
                            line.Substring(
                                0,
                                line.Length -
                                line.TrimStart().Length
                            );

                        writer.WriteLine(
                            indentation +
                            replacement
                        );

                        continue;
                    }
                }

                writer.WriteLine(line);
            }

            return writer.ToString();
        }
    }

    private static string MoveDuoCommandsToOldCleanups(
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

                const string prefix =
                    "[MenuItem(\"Tools/Duo/";

                if (trimmed.StartsWith(prefix))
                {
                    int start =
                        trimmed.IndexOf(
                            prefix,
                            StringComparison.Ordinal
                        );

                    int end =
                        trimmed.IndexOf(
                            "\")]",
                            start,
                            StringComparison.Ordinal
                        );

                    if (start >= 0 && end > start)
                    {
                        string command =
                            trimmed.Substring(
                                start + prefix.Length,
                                end -
                                (start + prefix.Length)
                            );

                        string replacement =
                            "[MenuItem(\"Tools/Old Clean-ups/" +
                            command +
                            "\")]";

                        string indentation =
                            line.Substring(
                                0,
                                line.Length -
                                line.TrimStart().Length
                            );

                        writer.WriteLine(
                            indentation +
                            replacement
                        );

                        continue;
                    }
                }

                writer.WriteLine(line);
            }

            return writer.ToString();
        }
    }
}
