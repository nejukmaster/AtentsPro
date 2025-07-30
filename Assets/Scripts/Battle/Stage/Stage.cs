using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public abstract class Stage : MonoBehaviour
    {
        public Vector3[] spawnPos = new Vector3[4];
        public Substage[] substages;

        [SerializeField] CinemachineSmoothPath track;
        [SerializeField] float substageChangeDelay;
        int currentSubstageIdx = 0;

        bool bIsEnd = false;

        private void Start()
        {
            Initialize(GameManager.GetInstance().homeToBattle);
            EnterSubstage(0);
        }

        private void Update()
        {
            if (IsSubstageEnd() && !bIsEnd)
            {
                currentSubstageIdx++;
                if (currentSubstageIdx < substages.Length)
                    EnterSubstage(currentSubstageIdx);
                else
                {
                    EndStage(true);
                    bIsEnd = true;
                }
            }
            else if(IsFailed())
            {
                EndStage(false);
                bIsEnd=true;
            }
        }

        public void Initialize(HomeToBattle Info)
        {
            bIsEnd = false;
            for (int i = 0; i < Info.team.Length; i++)
            {
                Character character = BattleSystem.GetInstance().SpawnCharacter(Info.team[i]);
                if (character == null) continue;
                character.Teleport(spawnPos[i]);
                BattleSystem.GetInstance().RegisterTeam(character);
            }
            for(int i = 0; i < Info.substages.Length; i++)
            {
                substages[i].enemiesCharacter = Info.substages[i].enemies;
            }
            CameraSystem.GetInstance().GetCamera("_main").SetDollyTrack(track);
        }

        public void EnterSubstage(int idx)
        {
            BattleSystem.GetInstance().SetOnBattle(false);
            UINavigationSystem.GetInstance().ChangeState("main", new UINavigateParam());
            CameraSystem.GetInstance().GetCamera("_main").MoveTo(idx + 1, substageChangeDelay);
            int i = 0;
            foreach(Character character in BattleSystem.GetInstance().GetTeam())
            {
                if (i < BattleSystem.GetInstance().GetTeam().Count - 1)
                {
                    Vector3 pos = substages[idx].teamPos[i].position;
                    character.ReserveCommandBuffer(new CharacterCommand{
                        action = () =>
                        {
                            character.MoveTo(pos, substageChangeDelay, () => {
                                character.SetCanAttack(false);
                            }, 
                            () => {
                                character.currentSlot = substages[idx].teamPos[i];
                                character.RotateTo(substages[idx].teamPos[i].rotation);
                                character.CommandBufferEndCallback();
                            });
                        }
                    });
                }
                else
                {
                    Vector3 pos = substages[idx].teamPos[i].position;
                    character.ReserveCommandBuffer(new CharacterCommand{
                        action = () =>
                        {
                            character.MoveTo(pos, substageChangeDelay, () => {
                                character.SetCanAttack(false);
                            },
                            () =>
                            {
                                character.currentSlot = substages[idx].teamPos[i];
                                character.RotateTo(substages[idx].teamPos[i].rotation);
                                foreach(Character team in  BattleSystem.GetInstance().GetTeam())
                                {
                                    team.SetCanAttack(true);
                                }
                                BattleSystem.GetInstance().SetOnBattle(true);
                                character.CommandBufferEndCallback();
                            });
                        }
                    });
                }
                i++;
            }
            for(i = 0; i <  substages[idx].enemiesCharacter.Length; i++)
            {
                Character character = BattleSystem.GetInstance().SpawnCharacter(substages[idx].enemiesCharacter[i]);
                character.currentSlot = substages[idx].enemyPos[i];
                character.Teleport(substages[idx].enemyPos[i].position);
                character.RotateTo(substages[idx].enemyPos[i].rotation);
                character.SetCanAttack(true);
                BattleSystem.GetInstance().RegisterEnemy(character);
            }
        }

        public void EndStage(bool bIsClear)
        {
            BattleSystem.GetInstance().SetOnBattle(false);
            foreach(Character character in BattleSystem.GetInstance().GetTeam()){
                character.ClearCommandBuffer();
            }
            BattleSystem.GetInstance().EndStage(bIsClear);
        }

        public abstract bool IsSubstageEnd();
        public abstract bool IsFailed();
    }
}
