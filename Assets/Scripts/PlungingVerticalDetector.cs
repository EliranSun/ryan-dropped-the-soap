public class PlungingVerticalDetector : CursorVerticalDetector {
    // private void OnMouseEnter() {
    //     print($"ENTER {gameObject.name}");
    // }
    //
    // private void OnMouseExit() {
    //     print($"EXIT {gameObject.name}");
    // }

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

        EventManager.Instance.Publish(GameEvents.StrongPull);
        EventManager.Instance.Publish(GameEvents.TriggerNonStick);
    }
}