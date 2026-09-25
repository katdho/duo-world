using System.IO;
using UnityEditor;
using UnityEngine;

public static class DeleteOldToolsMenu
{
    private const string OldPreviewScript =
        "Assets/Editor/DuoConnectUIPreview.cs";

    [MenuItem("Tools/Old Clean-ups/Delete Old Tools Menu")]
    public static void DeleteOldMenu()
    {
        bool deletedSomething = false;

        // ---------------------------------------------------------
        // Delete old preview script
        // ---------------------------------------------------------

        if (File.Exists(OldPreviewScript))
        {
            AssetDatabase.DeleteAsset(OldPreviewScript);
            deletedSomething = true;

            Debug.Log(
                "Deleted old preview script: " +
                OldPreviewScript
            );
        }

        // ---------------------------------------------------------
        // Remove old menu entries by deleting known old scripts.
        //
        // AssetDatabase.Refresh causes Unity to rebuild the menu.
        // ---------------------------------------------------------

        AssetDatabase.Refresh();

        if (deletedSomething)
        {
            Debug.Log(
                "Old Duo Connect preview menu removed."
            );
        }
        else
        {
            Debug.Log(
                "Old Duo Connect preview menu script was already gone."
            );
        }
    }
}
