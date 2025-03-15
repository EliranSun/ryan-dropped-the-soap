using UnityEngine;
using Mini_Games.Organize_Desk.scripts;

namespace Mini_Games.Organize_Desk
{
    /// <summary>
    /// This is an example class to demonstrate how to create a DeskDialogLines asset.
    /// In the Unity Editor:
    /// 1. Right-click in the Project window
    /// 2. Select Create > Mini Games > Desk Dialog Lines
    /// 3. Name your asset (e.g., "OrganizeDeskDialogLines")
    /// 4. In the Inspector, add dialog lines and set their types (Good/Bad)
    /// 5. Assign this asset to the OrganizeDeskMiniGame component
    /// </summary>
    public class DeskDialogLinesExample : MonoBehaviour
    {
        [SerializeField] private DeskDialogLines deskDialogLines;

        // This is just an example method to show how to use the DeskDialogLines asset
        private void ExampleUsage()
        {
            // Get a random "Good" dialog line
            var goodLine = deskDialogLines.GetRandomLine(DialogLineType.Good);

            // Get a random "Bad" dialog line
            var badLine = deskDialogLines.GetRandomLine(DialogLineType.Bad);

            // Use these lines in your game logic
            Debug.Log("Good line: " + (goodLine != null ? goodLine.name : "None"));
            Debug.Log("Bad line: " + (badLine != null ? badLine.name : "None"));
        }
    }
}