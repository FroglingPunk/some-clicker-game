using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SomeClickerGame
{
    public class UIScoreBoard : MonoBehaviour
    {
        [SerializeField] private GameController _gameController;
        [SerializeField] private Text _textScore;

        private CompositeDisposable _disposable = new();


        private void Start()
        {
            _gameController.Score.Subscribe(score => _textScore.text = score.ToString()).AddTo(_disposable);
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