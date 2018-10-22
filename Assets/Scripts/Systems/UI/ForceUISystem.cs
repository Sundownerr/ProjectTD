using UnityEngine.UI;
using TMPro;

namespace Game.System
{
    public class ForceUISystem : ExtendedMonoBehaviour
    {
        public Button Astral, Darkness, Ice, Iron, Storm, Nature, Fire;
        public TextMeshProUGUI AstralLevel, DarknessLevel, IceLevel, IronLevel, StormLevel, NatureLevel, FireLevel;

        protected override void Awake()
        {
            if ((object)CachedTransform == null)
            {
                CachedTransform = transform;
            }

            Astral.onClick.AddListener(LearnAstral);
            Darkness.onClick.AddListener(LearnDarkness);
            Ice.onClick.AddListener(LearnIce);
            Iron.onClick.AddListener(LearnIron);
            Storm.onClick.AddListener(LearnStorm);
            Nature.onClick.AddListener(LearnNature);
            Fire.onClick.AddListener(LearnFire);

            GM.Instance.ForceUISystem = this;

            gameObject.SetActive(false);
        }

        private void Start()
        {
            UpdateValues();
        }

        public void UpdateValues()
        {
            AstralLevel.text = GM.Instance.PlayerData.ElementLevelList["Astral"].ToString();
            DarknessLevel.text = GM.Instance.PlayerData.ElementLevelList["Darkness"].ToString();
            IceLevel.text = GM.Instance.PlayerData.ElementLevelList["Ice"].ToString();
            IronLevel.text = GM.Instance.PlayerData.ElementLevelList["Iron"].ToString();
            StormLevel.text = GM.Instance.PlayerData.ElementLevelList["Storm"].ToString();
            NatureLevel.text = GM.Instance.PlayerData.ElementLevelList["Nature"].ToString();
            FireLevel.text = GM.Instance.PlayerData.ElementLevelList["Fire"].ToString();
        }

        private void LearnAstral()
        {
            GM.Instance.ForceSystem.LearnElement("Astral");
            UpdateValues();
        }

        private void LearnDarkness()
        {
            GM.Instance.ForceSystem.LearnElement("Darkness");
            UpdateValues();
        }

        private void LearnIce()
        {
            GM.Instance.ForceSystem.LearnElement("Ice");
            UpdateValues();
        }

        private void LearnIron()
        {
            GM.Instance.ForceSystem.LearnElement("Iron");
            UpdateValues();
        }

        private void LearnStorm()
        {
            GM.Instance.ForceSystem.LearnElement("Storm");
            UpdateValues();
        }

        private void LearnNature()
        {
            GM.Instance.ForceSystem.LearnElement("Nature");
            UpdateValues();
        }

        private void LearnFire()
        {
            GM.Instance.ForceSystem.LearnElement("Fire");
            UpdateValues();
        }
    }
}
