using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.VictoryWindow
{
    public class VictoryWindowVew : UIView<VictoryWindowViewModel>
    {
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _retryButton;
        
        protected override void OnInit()
        {
        }
        
        private void OnEnable()
        {
            _quitButton.onClick.AddListener(OnQuitClick);
            _retryButton.onClick.AddListener(OnRetryClick);
            
            PlayShow();
        }

        private void OnDisable()
        {
            _quitButton.onClick.RemoveListener(OnQuitClick);
            _retryButton.onClick.RemoveListener(OnRetryClick);
        }

        private void OnQuitClick()
        {
            ViewModel.OnQuitAction.Value?.Invoke();
        }

        private void OnRetryClick()
        {
            ViewModel.OnRetryAction.Value?.Invoke();
        }
        
        public void PlayShow()
        {
        }
    }
}