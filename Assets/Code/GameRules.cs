namespace SomeClickerGame
{
    [System.Serializable]
    public static class GameRules
    {
        public static int LevelDuration { get; private set; } = 30;

        public static int BaseScoreIncrement { get; private set; } = 10;
        public static int BaseScoreDecrement { get; private set; } = 5;

        public static float MinBallSpeed { get; private set; } = 0.25f;
        public static float MaxBallSpeed { get; private set; } = 0.5f;

        public static float MinBallScale { get; private set; } = 0.5f;
        public static float MaxBallScale { get; private set; } = 2f;

        public static float MinBallsSpawnRate { get; private set; } = 0.4f;
        public static float MaxBallsSpawnRate { get; private set; } = 1f;
    }
}