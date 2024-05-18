public class PlungingVerticalDetector : CursorVerticalDetector {
    protected override void OnVerticalMotion(bool isDownwardMotion) {
        base.OnVerticalMotion(isDownwardMotion);

        Notify(GameEvents.Pumping);
        Notify(isDownwardMotion
            ? GameEvents.DownwardsControllerMotion
            : GameEvents.UpwardsControllerMotion);
    }

    protected override void OnBigUpwardsMotion() {
        base.OnBigUpwardsMotion();

        print("STRONG PULL!");
        Notify(GameEvents.StrongPull);
        Notify(GameEvents.TriggerNonStick);
    }
}