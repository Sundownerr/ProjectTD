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
                                target.Prefab.transform.position, 
                                Quaternion.identity, 
                                target.Prefab.transform);

                target.IsOn = false;

                target.EffectSystem.Add(this);
                effectCoroutine = GM.I.StartCoroutine(SetEffect(Duration));
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
