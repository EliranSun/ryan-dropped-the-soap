using Dialog;
using UnityEngine;

public class SpriteColorController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color color;

    private void StepTowardsColor(Color targetColor, float stepSize)
    {
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, stepSize);
    }

    public void OnNotify(GameEventData gameEventData)
    {
        if (gameEventData.Name == GameEvents.LineNarrationStart)
        {
            var narrationDialogLine = gameEventData.Data as NarrationDialogLine;
            if (narrationDialogLine == null) return;

            print("angerLevel: " + narrationDialogLine.blurLevel);

            StepTowardsColor(color, narrationDialogLine.blurLevel);
        }
    }
}