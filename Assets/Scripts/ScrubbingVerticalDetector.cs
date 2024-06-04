public class ScrubbingVerticalDetector : CursorVerticalDetector {
    protected override void OnVerticalMotion(bool isDownwardMotion) {
        base.OnVerticalMotion(isDownwardMotion);
        EventManager.Instance.Publish(GameEvents.IsScrubbing);
    }
}