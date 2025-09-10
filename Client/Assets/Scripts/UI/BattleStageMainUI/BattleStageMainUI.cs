using UI.Base;

namespace UI.BattleStageMainUI
{
    public class BattleStageMainUI : BaseUI<BattleStageMainUI, BattleStageMainUIViewModel, BattleStageMainUIView, BattleStageMainUIData>
    {
        public void SetPlayerCurrentHp(float playerCurrentHp)
        {
            ViewModel.PlayerCurrentHp.Set(playerCurrentHp);
        }
        
        public void SetPlayerMaxHp(float playerMaxHp)
        {
            ViewModel.PlayerMaxHp.Set(playerMaxHp);
        }
        
        public void SetBossCurrentHp(float bossCurrentHp)
        {
            ViewModel.BossCurrentHp.Set(bossCurrentHp);
        }
        
        public void SetBossMaxHp(float bossMaxHp)
        {
            ViewModel.BossMaxHp.Set(bossMaxHp);
        }
        
        public void SetBossHpBarActive(bool isHpBarActive)
        {
            ViewModel.BossHpActive.Set(isHpBarActive);
        }
    }
}