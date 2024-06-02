public class PlungingVerticalDetector : CursorVerticalDetector {
    protected override void OnVerticalMotion(bool isDownwardMotion) {
        if (CursorManager.Instance.IsScrubbingCursor)
            return;

        base.OnVerticalMotion(isDownwardMotion);

        EventManager.Instance.Publish(GameEvents.Pumping);
        EventManager.Instance.Publish(isDownwardMotion
            ? GameEvents.DownwardsControllerMotion
            : GameEvents.UpwardsControllerMotion);
    }

    protected override void OnBigUpwardsMotion() {
        if (CursorManager.Instance.IsScrubbingCursor)
            return;

        base.OnBigUpwardsMotion();

        print("STRONG PULL!");
        EventManager.Instance.Publish(GameEvents.StrongPull);
        EventManager.Instance.Publish(GameEvents.TriggerNonStick);
    }
}