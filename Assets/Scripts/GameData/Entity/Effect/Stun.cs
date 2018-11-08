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

            if(target == null)
                End();
            else
            {
                effectPrefab = Instantiate(EffectPrefab, 
                                target.gameObject.transform.position, 
                                Quaternion.identity, 
                                Target.gameObject.transform);

                target.IsOn = false;

                target.EffectSystem.ApplyEffect(this);
                effectCoroutine = GM.Instance.StartCoroutine(SetEffect(Duration));
            }
        }

        public override void End()
        {
            if(target != null)
                target.IsOn = true;
            
            Destroy(effectPrefab);

            base.End();
        }
    }
}
