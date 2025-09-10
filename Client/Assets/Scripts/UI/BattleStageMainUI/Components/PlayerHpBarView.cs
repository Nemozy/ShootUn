using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View;

namespace UI.BattleStageMainUI.Components
{
    public class PlayerHpBarView : BaseView
    {
        [SerializeField] private Slider _playerHpSlider;
        [SerializeField] private TMP_Text _playerHpText;

        public void SetPlayerHp(float current, float maxHp)
        {
            _playerHpSlider.value = current / maxHp;
            _playerHpText.SetText($"{current}/{maxHp}");
        }
    }
}