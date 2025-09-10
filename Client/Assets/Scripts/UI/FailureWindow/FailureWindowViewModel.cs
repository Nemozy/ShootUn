using System;
using UI.Base;

namespace UI.FailureWindow
{
    public class FailureWindowViewModel : BaseUIViewModel
    {
        public readonly Observable<Action> OnQuitAction = new ();
        public readonly Observable<Action> OnRetryAction = new ();
    }
}