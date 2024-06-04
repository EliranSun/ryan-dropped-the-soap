public class ScrubbingVerticalDetector : CursorVerticalDetector {
    protected override void OnVerticalMotion(bool isDownwardMotion) {
        base.OnVerticalMotion(isDownwardMotion);
        print("OnVerticalMotion");
        EventManager.Instance.Publish(GameEvents.IsScrubbing);
    }
}