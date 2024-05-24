public class ScrubbingVerticalDetector : CursorVerticalDetector {
    protected override void OnVerticalMotion(bool isDownwardMotion) {
        base.OnVerticalMotion(isDownwardMotion);
        print("SCURBBING");
        EventManager.Instance.Publish(GameEvents.IsScrubbing);
    }
}