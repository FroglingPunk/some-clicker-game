using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace SomeClickerGame
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private UIGameOverPanel _gameOverPanel;

        public IReadOnlyReactiveProperty<int> Score => _score;
        private readonly IntReactiveProperty _score = new(0);

        public IReadOnlyReactiveProperty<int> LeftTime => _leftTime;
        private readonly IntReactiveProperty _leftTime = new(0);

        private BallsController _ballsController;
        private CancellationTokenSource _gameCycleCslTokenSource;


        private void Start()
        {
            _ballsController = new BallsController();
            _ballsController.OnBallPassed += OnBallPassed;
            _ballsController.OnBallClicked += OnBallClicked;

            _ = LifeCycleAsync();
        }

        private void OnDestroy()
        {
            _gameCycleCslTokenSource?.Dispose();
        }


        private async UniTask LifeCycleAsync()
        {
            while (true)
            {
                _gameCycleCslTokenSource = new();

                await GameCycleAsync(_gameCycleCslTokenSource.Token);

                _gameCycleCslTokenSource?.Dispose();

                _ballsController.Clear();

                var restartGame = await _gameOverPanel.ShowAsync();

                if (restartGame)
                {
                    _gameOverPanel.Hide();
                    continue;
                }
                else
                {
                    Application.Quit();
                    break;
                }
            }
        }

        private async UniTask GameCycleAsync(CancellationToken cslToken)
        {
            _score.Value = 0;
            _leftTime.Value = GameRules.LevelDuration;

            var timeHandler = UniTask.RunOnThreadPool(async () =>
            {
                while (_leftTime.Value > 0)
                {
                    await UniTask.WaitForSeconds(1f, false, PlayerLoopTiming.Update, cslToken);

                    if (cslToken.IsCancellationRequested)
                    {
                        return;
                    }

                    _leftTime.Value--;
                }
            });

            while (timeHandler.Status == UniTaskStatus.Pending)
            {
                _ballsController.CreateBall();
                await UniTask.WaitForSeconds(Random.Range(GameRules.MinBallsSpawnRate, GameRules.MaxBallsSpawnRate), false, PlayerLoopTiming.Update, cslToken);

                if (cslToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }


        private void OnBallPassed(Ball ball)
        {
            var scoreForSpeed = (GameRules.MaxBallSpeed - ball.Speed) / (GameRules.MaxBallSpeed - GameRules.MinBallSpeed);
            var scoreForScale = (ball.Scale - GameRules.MinBallScale) / (GameRules.MaxBallScale - GameRules.MinBallScale);
            var decrement = (int)((scoreForSpeed + scoreForScale) * GameRules.BaseScoreDecrement);
            _score.Value -= decrement;
        }

        private void OnBallClicked(Ball ball)
        {
            var scoreForSpeed = (ball.Speed - GameRules.MinBallSpeed) / (GameRules.MaxBallSpeed - GameRules.MinBallSpeed);
            var scoreForScale = (GameRules.MaxBallScale - ball.Scale) / (GameRules.MaxBallScale - GameRules.MinBallScale);
            var increment = (int)((scoreForSpeed + scoreForScale) * GameRules.BaseScoreIncrement);
            _score.Value += increment;
        }
    }
}