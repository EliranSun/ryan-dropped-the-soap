public class PlungingVerticalDetector : CursorVerticalDetector {
    protected override void OnVerticalMotion(bool isDownwardMotion) {
        if (CursorManager.Instance.IsActionCursor)
            return;

        base.OnVerticalMotion(isDownwardMotion);

        EventManager.Instance.Publish(GameEvents.Pumping);
        EventManager.Instance.Publish(isDownwardMotion
            ? GameEvents.DownwardsControllerMotion
            : GameEvents.UpwardsControllerMotion);
    }

    protected override void OnBigUpwardsMotion() {
        if (CursorManager.Instance.IsActionCursor)
            return;

        base.OnBigUpwardsMotion();

        print("STRONG PULL!");
        EventManager.Instance.Publish(GameEvents.StrongPull);
        EventManager.Instance.Publish(GameEvents.TriggerNonStick);
    }
}