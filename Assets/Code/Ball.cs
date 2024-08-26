using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace SomeClickerGame
{
    public class Ball : IDisposable
    {
        public float Speed { get; private set; }
        public float Scale { get; private set; }

        public IReadOnlyReactiveProperty<EBallState> State => _state;
        private ReactiveProperty<EBallState> _state = new(EBallState.None);

        private Transform _transform;

        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private float _progress;

        private CompositeDisposable _disposable = new();


        public Ball()
        {
            _transform = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;

            var collider = _transform.GetComponent<Collider>();
            collider.OnMouseDownAsObservable().Subscribe((_) =>
            {
                if (_state.Value == EBallState.Moving)
                {
                    _state.Value = EBallState.Clicked;
                }
            }).AddTo(_disposable);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }


        public void Update()
        {
            if (_state.Value != EBallState.Moving)
            {
                return;
            }

            _progress += Time.deltaTime * Speed;
            _transform.position = Vector3.Lerp(_startPosition, _endPosition, _progress);

            if (_progress >= 1f)
            {
                _state.Value = EBallState.Passed;
            }
        }

        public void Run(float scale, float speed, Vector3 startPosition, Vector3 endPosition, Color color)
        {
            _progress = 0f;

            Scale = scale;
            Speed = speed;
            _startPosition = startPosition;
            _endPosition = endPosition;

            _transform.localScale = Vector3.one * scale;
            _transform.position = startPosition;
            _transform.GetComponent<Renderer>().material.color = color;

            _state.Value = EBallState.Moving;

            _transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _transform.gameObject.SetActive(false);
            _state.Value = EBallState.None;
        }
    }
}