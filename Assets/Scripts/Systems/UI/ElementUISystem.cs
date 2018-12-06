using UnityEngine.UI;
using TMPro;

namespace Game.Systems
{
    public class ElementUISystem : ExtendedMonoBehaviour
    {
        public Button Astral, Darkness, Ice, Iron, Storm, Nature, Fire;
        public TextMeshProUGUI AstralLevel, DarknessLevel, IceLevel, IronLevel, StormLevel, NatureLevel, FireLevel;

        protected override void Awake()
        {
            GM.I.ElementUISystem = this;

            base.Awake();

            Astral.onClick.AddListener(LearnAstral);
            Darkness.onClick.AddListener(LearnDarkness);
            Ice.onClick.AddListener(LearnIce);
            Iron.onClick.AddListener(LearnIron);
            Storm.onClick.AddListener(LearnStorm);
            Nature.onClick.AddListener(LearnNature);
            Fire.onClick.AddListener(LearnFire);       

            gameObject.SetActive(false);
        }

        private void OnEnable() => UpdateUI();

        public void UpdateUI()
        {
            AstralLevel.text    = GM.I.PlayerData.ElementLevels[0].ToString();
            DarknessLevel.text  = GM.I.PlayerData.ElementLevels[1].ToString();
            IceLevel.text       = GM.I.PlayerData.ElementLevels[2].ToString();
            IronLevel.text      = GM.I.PlayerData.ElementLevels[3].ToString();
            StormLevel.text     = GM.I.PlayerData.ElementLevels[4].ToString();
            NatureLevel.text    = GM.I.PlayerData.ElementLevels[5].ToString();
            FireLevel.text      = GM.I.PlayerData.ElementLevels[6].ToString();
        }

        private void LearnAstral()  => GM.I.ElementSystem.LearnElement(0);
        private void LearnDarkness() => GM.I.ElementSystem.LearnElement(1);
        private void LearnIce()     => GM.I.ElementSystem.LearnElement(2);
        private void LearnIron()    => GM.I.ElementSystem.LearnElement(3);
        private void LearnStorm()   => GM.I.ElementSystem.LearnElement(4);
        private void LearnNature()  => GM.I.ElementSystem.LearnElement(5);
        private void LearnFire()    => GM.I.ElementSystem.LearnElement(6);      
    }
}
