using TMPro;
using UnityEngine;
using UnityEngine.UI;
using View;

namespace UI.BattleStageMainUI.Components
{
    public class BossHpBarView : BaseView
    {
        [SerializeField] private Slider _bossHpSlider;
        [SerializeField] private TMP_Text _bossHpText;

        public void SetHp(float current, float maxHp)
        {
            _bossHpSlider.value = current / maxHp;
            _bossHpText.SetText($"{current}/{maxHp}");
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}