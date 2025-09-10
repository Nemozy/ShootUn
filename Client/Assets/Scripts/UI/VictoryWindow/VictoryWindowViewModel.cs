using System;
using UI.Base;

namespace UI.VictoryWindow
{
    public class VictoryWindowViewModel : BaseUIViewModel
    {
        public readonly Observable<Action> OnQuitAction = new ();
        public readonly Observable<Action> OnRetryAction = new ();
    }
}