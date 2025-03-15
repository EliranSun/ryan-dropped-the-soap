# Organize Desk Mini Game

## Dialog System

The Organize Desk mini game now includes a categorized dialog system that displays different dialog lines based on the game outcome (win or lose).

### How to Set Up

1. **Create a Dialog Lines Asset**:
   - In the Unity Editor, right-click in the Project window
   - Select `Create > Mini Games > Desk Dialog Lines`
   - Name your asset (e.g., "OrganizeDeskDialogLines")

2. **Configure Dialog Lines**:
   - Select the created asset in the Project window
   - In the Inspector, add dialog lines using the "+" button
   - For each dialog line:
     - Assign a `NarrationDialogLine` ScriptableObject
     - Set the `Type` to either `Good` (for winning) or `Bad` (for losing)

3. **Assign to the Mini Game**:
   - Select the GameObject with the `OrganizeDeskMiniGame` component
   - In the Inspector, assign your DeskDialogLines asset to the `Desk Dialog Lines` field

### How It Works

- When the mini game ends, the system checks if the player won or lost
- Based on the outcome, it selects a random dialog line of the appropriate type (Good/Bad)
- The selected dialog line is triggered using the `TriggerSpecificDialogLine` event
- You can customize the trigger event by changing the `Trigger Dialog Event` field

### Creating NarrationDialogLine Assets

To create new dialog lines:

1. Right-click in the Project window
2. Select `Create > Line`
3. Configure the dialog line properties in the Inspector
4. Assign this line to your DeskDialogLines asset

### Example Dialog Lines

**Good (Winning) Lines:**
- "Great job organizing your desk! A clean workspace leads to a clear mind."
- "You've successfully decluttered your workspace. Now you can focus on what matters!"
- "Perfect! Your desk is now optimized for productivity."

**Bad (Losing) Lines:**
- "Your desk is still cluttered. It might be hard to focus with all this mess."
- "You missed some non-essential items. A clean desk helps with concentration."
- "Try again! A well-organized workspace can boost your productivity." 