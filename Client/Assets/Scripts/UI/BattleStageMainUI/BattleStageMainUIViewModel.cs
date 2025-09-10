using UI.Base;

namespace UI.BattleStageMainUI
{
    public class BattleStageMainUIViewModel : BaseUIViewModel
    {
        public readonly Observable<float> PlayerMaxHp = new (999);
        public readonly Observable<float> PlayerCurrentHp = new (999);
        public readonly Observable<float> BossMaxHp = new (999);
        public readonly Observable<float> BossCurrentHp = new (999);
        public readonly Observable<bool> BossHpActive = new (false);
    }
}