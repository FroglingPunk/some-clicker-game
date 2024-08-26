using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SomeClickerGame
{
    public class UIGameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameController _gameController;
        [SerializeField] private Text _textScore;
        [SerializeField] private Button _buttonRestart;
        [SerializeField] private Button _buttonQuit;


        public async UniTask<bool> ShowAsync()
        {
            _textScore.text = _gameController.Score.Value.ToString();
            gameObject.SetActive(true);

            return await UniTask.WhenAny
                (
                _buttonRestart.GetAsyncClickEventHandler().OnClickAsync(),
                _buttonQuit.GetAsyncClickEventHandler().OnClickAsync()
                ) == 0;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}