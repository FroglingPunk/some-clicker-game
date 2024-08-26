using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SomeClickerGame
{
    public class UITimer : MonoBehaviour
    {
        [SerializeField] private GameController _gameController;
        [SerializeField] private Text _textTime;

        private CompositeDisposable _disposable = new();


        private void Start()
        {
            _gameController.LeftTime.Subscribe(leftTime => _textTime.text = leftTime.ToString()).AddTo(_disposable);
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }


        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}