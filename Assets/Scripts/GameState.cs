public static class GameState {
    public static bool WaterFilledShower { get; set; }
    public static bool WaterFilledRoom { get; set; }
    public static bool IsPlayerInShower { get; set; }

    public static void Reset() {
        WaterFilledShower = false;
        WaterFilledRoom = false;
        IsPlayerInShower = false;
    }
}