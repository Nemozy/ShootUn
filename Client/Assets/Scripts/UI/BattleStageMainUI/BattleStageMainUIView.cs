using TMPro;
using UI.Base;
using UI.BattleStageMainUI.Components;
using UnityEngine;

namespace UI.BattleStageMainUI
{
    public class BattleStageMainUIView : UIView<BattleStageMainUIViewModel>
    {
        [SerializeField] private PlayerHpBarView _playerHpBarView;
        [SerializeField] private BossHpBarView _bossHpBarView;
        
        protected override void OnInit()
        {
            ViewModel.PlayerMaxHp.OnChange += OnPlayerMaxHpChanged;
            ViewModel.PlayerCurrentHp.OnChange += OnPlayerCurrentHpChanged;
            ViewModel.BossMaxHp.OnChange += OnBossMaxHpChanged;
            ViewModel.BossCurrentHp.OnChange += OnBossCurrentHpChanged;
            ViewModel.BossHpActive.OnChange += OnBossHpActiveChanged;

            OnPlayerMaxHpChanged(ViewModel.PlayerMaxHp);
            OnPlayerCurrentHpChanged(ViewModel.PlayerCurrentHp);
            OnBossMaxHpChanged(ViewModel.BossMaxHp);
            OnBossCurrentHpChanged(ViewModel.BossCurrentHp);
            OnBossHpActiveChanged(ViewModel.BossHpActive);
        }

        private void OnPlayerMaxHpChanged(float value)
        {
            _playerHpBarView.SetPlayerHp(ViewModel.PlayerCurrentHp, value);
        }

        private void OnPlayerCurrentHpChanged(float value)
        {
            _playerHpBarView.SetPlayerHp(value, ViewModel.PlayerMaxHp);
        }

        private void OnBossHpActiveChanged(bool isHpBarActive)
        {
            _bossHpBarView.SetActive(isHpBarActive);
        }

        private void OnBossMaxHpChanged(float value)
        {
            _bossHpBarView.SetHp(ViewModel.BossCurrentHp, value);
        }

        private void OnBossCurrentHpChanged(float value)
        {
            _bossHpBarView.SetHp(value, ViewModel.BossMaxHp);
        }
    }
}