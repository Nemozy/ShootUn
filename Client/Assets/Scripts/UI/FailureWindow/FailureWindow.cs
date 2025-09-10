using UI.Base;

namespace UI.FailureWindow
{
    public class FailureWindow : BaseUI<FailureWindow, FailureWindowViewModel, FailureWindowVew, FailureWindowData>
    {
        protected override void OnOpen()
        {
            ViewModel.OnQuitAction.Set(QuitGame);
            ViewModel.OnRetryAction.Set(Retry);
        }
        
        private void QuitGame()
        {
            Data.QuitGame?.Invoke();
            Close();
        }
        
        private void Retry()
        {
            Data.Retry?.Invoke();
            Close();
        }

        private void Close()
        {
            Close(this, true);
        }
    }
}