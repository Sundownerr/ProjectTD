using System.Collections;
using UnityEngine;
using Game.Systems;

namespace Game.Data.Effects
{
    [CreateAssetMenu(fileName = "Stun", menuName = "Data/Effect/Stun")]
    public class Stun : Effect
    {
        public GameObject EffectPrefab;

        private GameObject effectPrefab;    

        public IEnumerator SetEffect(float delay)
        {
            yield return new WaitForSeconds(delay);
       
            End();
        }

        public override void Apply()
        {
            base.Apply();

            effectPrefab = Instantiate(EffectPrefab, 
                            Target.gameObject.transform.position, 
                            Quaternion.identity, 
                            Target.gameObject.transform);

            Target.IsOn = false;

            Target.EffectSystem.ApplyEffect(this);
            EffectCoroutine = GM.Instance.StartCoroutine(SetEffect(Duration));
        }

        public override void End()
        {
            if(Target != null)
                Target.IsOn = true;
            
            Destroy(effectPrefab);

            base.End();
        }
    }
}
