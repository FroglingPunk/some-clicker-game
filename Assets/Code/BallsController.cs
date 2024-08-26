using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UniRx;

namespace SomeClickerGame
{
    public class BallsController : IDisposable
    {
        private Stack<Ball> _poolBalls = new();
        private List<Ball> _activeBalls = new();

        private Vector3 _minPosition;
        private Vector3 _maxPosition;
        private float _endYPosition;

        public event Action<Ball> OnBallClicked;
        public event Action<Ball> OnBallPassed;

        private CompositeDisposable _disposable = new();


        public BallsController()
        {
            CalcSpawnBounds();
            Observable.EveryUpdate().Subscribe((_) => Update()).AddTo(_disposable);
        }


        private void Update()
        {
            for (var i = _activeBalls.Count - 1; i >= 0; i--)
            {
                _activeBalls[i].Update();
            }
        }

        public void Dispose()
        {
            for (var i = 0; i < _poolBalls.Count; i++)
            {
                _poolBalls.Pop()?.Dispose();
            }

            for (var i = 0; i < _activeBalls.Count; i++)
            {
                _activeBalls[i]?.Dispose();
            }

            _disposable?.Dispose();
        }

        public void Clear()
        {
            for (var i = _activeBalls.Count - 1; i >= 0; i--)
            {
                ReleaseBall(_activeBalls[i]);
            }
        }


        public void CreateBall()
        {
            var ball = GetBall();

            var startPosition = Vector3.Lerp(_minPosition, _maxPosition, Random.Range(0f, 1f));
            var endPosition = new Vector3(startPosition.x, _endYPosition, startPosition.z);
            var scale = Random.Range(GameRules.MinBallScale, GameRules.MaxBallScale);
            var speed = Random.Range(GameRules.MinBallSpeed, GameRules.MaxBallSpeed);

            ball.Run(scale, speed, startPosition, endPosition, Random.ColorHSV());
        }


        private void CalcSpawnBounds()
        {
            var mainCamera = Camera.main;
            var maxBallScale_half = GameRules.MaxBallScale * 0.5f;

            _minPosition = mainCamera.ScreenToWorldPoint(Vector3.zero);
            _minPosition.x += maxBallScale_half;
            _minPosition.y -= maxBallScale_half;
            _minPosition.z = 0f;

            _maxPosition = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
            _maxPosition.x -= maxBallScale_half;
            _maxPosition.y -= maxBallScale_half;
            _maxPosition.z = 0f;

            _endYPosition = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y + maxBallScale_half;
        }

        private Ball GetBall()
        {
            if (_poolBalls.Count > 0)
            {
                var ball = _poolBalls.Pop();
                _activeBalls.Add(ball);
                return ball;
            }
            else
            {
                var ball = new Ball();
                ball.State.Subscribe(state =>
                {
                    if (state == EBallState.Passed)
                    {
                        OnBallPassed?.Invoke(ball);
                        ReleaseBall(ball);
                    }
                    else if (state == EBallState.Clicked)
                    {
                        OnBallClicked?.Invoke(ball);
                        ReleaseBall(ball);
                    }
                }).AddTo(_disposable);
                _activeBalls.Add(ball);

                return ball;
            }
        }

        private void ReleaseBall(Ball ball)
        {
            ball.Hide();
            _activeBalls.Remove(ball);
            _poolBalls.Push(ball);
        }
    }
}