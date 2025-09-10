namespace UI.Base
{
    public abstract class UIView<TViewModel> : BaseUIView where TViewModel : BaseUIViewModel, new()
    {
        public TViewModel ViewModel { get; private set; }
        
        protected sealed override void CreateViewModel()
        {
            ViewModel = new TViewModel();
        }
        
        public void Init(TViewModel viewModel)
        {
            BaseInitialization();
            ViewModel = viewModel;
            OnInit();
        }

        public void Hide()
        {
            Canvas.enabled = false;
        }

        public void Show()
        {
            Canvas.enabled = true;
        }
    }
}