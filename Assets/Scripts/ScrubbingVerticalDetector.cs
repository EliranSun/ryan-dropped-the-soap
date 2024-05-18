public class ScrubbingVerticalDetector : CursorVerticalDetector {
    protected override void OnVerticalMotion(bool isDownwardMotion) {
        base.OnVerticalMotion(isDownwardMotion);
        Notify(GameEvents.IsScrubbing);
    }
}