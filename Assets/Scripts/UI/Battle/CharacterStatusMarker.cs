using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace AtentsPro
{
    public class CharacterStatusMarker : Marker
    {
        [SerializeField] Image hpValueBar;
        [SerializeField] Image mpValueBar;
        [SerializeField] GameObject buffDisplayItem;
        [SerializeField] Transform buffMarkerContainer;

        ObjectPool buffMarkerPool = new ObjectPool();
        Character character;
        HashSet<Buff> displayingBuffs;

        public override void Update()
        {
            base.Update();
            if(character != null)
            {
                Status status = character.GetStatus();
                SetHp(status.hp / status.maxHp);
                SetMp(status.mp / status.maxMp);

                foreach(Buff added in character.GetBuffs().Except(displayingBuffs))
                {
                    DisplayBuff(added);
                }
                foreach(Buff expired in displayingBuffs.Except(character.GetBuffs()))
                {
                    HideBuff(expired);
                }
                displayingBuffs = character.GetBuffs();
            }
        }

        public void SetCharacter(Character character)
        {
            this.character = character;
            displayingBuffs = new HashSet<Buff>();
            buffMarkerPool.Init(buffDisplayItem, 10, buffMarkerContainer);
        }

        public void SetHp(float hp)
        {
            hpValueBar.fillAmount = hp;
        }

        public void SetMp(float mp)
        {
            mpValueBar.fillAmount = mp;
        }

        public void DisplayBuff(Buff buff)
        {
            buffMarkerPool.Dequeue((o) => {
                o.GetComponent<BuffDisplayItem>()?.SetBuff(buff);
            });
        }

        public void HideBuff(Buff buff)
        {
            foreach(var display in buffMarkerContainer.GetComponentsInChildren<BuffDisplayItem>())
            {
                if(display.buffId == buff.id)
                {
                    buffMarkerPool.Enqueue(display.gameObject, null);
                }
            }
        }
    }
}
