using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Animations;
using UnityEngine;

namespace AtentsPro
{
    public enum TargetingMethod
    {
        Nearest,
        Furthest,
    }

    [RequireComponent(typeof(CharacterController))]
    public class Character : MonoBehaviour, IDamagable, IDamageCauser, ISkillCaster
    {
        public SubstageSlot currentSlot;

        [SerializeField] float preMoveTerm = 0.5f;

        CharacterAsset asset;

        private Status basicStatus;

        AnimatedCharacter animatedCharacter;
        CharacterController controller;

        TargetingMethod targetingMethod;

        Skill[] skills = new Skill[4];
        float[] skillCools = new float[4];

        CharacterCommandBuffer commandBuffer = new CharacterCommandBuffer();

        List<float> damages = new List<float>();
        List<float> consumedMp = new List<float>();

        private IDamagable target;
        [SerializeField] private bool bCanAttack;
        [SerializeField] private bool bCmdReady;

        HashSet<Buff> buffs = new HashSet<Buff>();
        Dictionary<Guid, Status> temporaryStatus = new Dictionary<Guid, Status>();


        #region ISkillCaster �������̽�
        public float GetMp()
        {
            float mp = basicStatus.maxMp;
            foreach(float f in consumedMp)
            {
                mp -= f;
            }
            return mp;
        }
        public bool ConsumMp(float value)
        {
            if (GetMp() < value) return false;
            consumedMp.Add(value);
            return true;
        }
        #endregion

        #region IDamagable �������̽�
        public void OnDamage(float value)
        {
            damages.Add(value);
            if(GetHp() <= 0)
            {
                OnDead();
            }
        }

        public void OnHeal(float value)
        {
            float heal = value;
            while(damages.Count > 0)
            {
                if (heal >= damages[0])
                {
                    heal -= damages[0];
                    damages.Remove(0);
                }
                else
                {
                    damages[0] -= heal;
                    break;
                }

            }
        }

        public void OnDead()
        {
            ReserveCommandBuffer(new CharacterCommand
            {
                action = () =>
                {
                    GetAnimator()?.SetTrigger("Die");
                }
            });
            ReserveCommandBuffer(new CharacterCommand{
                action = () =>
                {
                    BattleSystem.GetInstance().ReserveDeadRequest(new DeadRequest(this));
                }
            });
        }

        public float GetHp()
        {
            float hp = basicStatus.maxHp;
            foreach( float f in damages)
            {
                hp -= f;
            }
            return hp;
        }

        public Transform GetTransform()
        {
            return transform;
        }
        #endregion

        #region �������̽� ����
        public Status GetStatus()
        {
            Status result = new Status {
                maxHp = basicStatus.maxHp,
                maxMp = basicStatus.maxMp,
                attack = basicStatus.attack,
                defence = basicStatus.defence,
                criticalRate = basicStatus.criticalRate,
                criticalDamage = basicStatus.criticalDamage,
                attackSpeed = basicStatus.attackSpeed,

                hp = GetHp(),
                mp = GetMp(),
            };
            //���� ������ ���� ������ �ӽ� �������ͽ� ����
            foreach(var tmp in temporaryStatus.Values)
            {
                result.maxHp += tmp.maxHp;
                result.maxMp += tmp.maxMp;
                result.attack += tmp.attack;
                result.defence += tmp.defence;
                result.criticalRate += tmp.criticalRate;
                result.criticalDamage += tmp.criticalDamage;
                result.attackSpeed += tmp.attackSpeed;
            }
            return result;
        }

        public void SetStatus(Status status)
        {
            this.basicStatus = status;
        }
        #endregion

        public Status GetBasicStatus()
        {
            return basicStatus;
        }

        private void OnEnable()
        {
            //ĳ���� �ʱ�ȭ�߿��� Ŀ�ǵ� ���۰� �۵����� �ʰ�
            bCmdReady = false;
            if(asset != null)
                LoadAsset();
            #region Character �ʱ�ȭ
            controller = GetComponent<CharacterController>();

            commandBuffer.Clear();
            damages.Clear();
            consumedMp.Clear();

            bCanAttack = false;
            bCmdReady = true;
            #endregion
        }

        public void OnDisable()
        {
            if(animatedCharacter != null)
                GameObject.Destroy(animatedCharacter.gameObject);
        }

        #region Character �ʱ�ȭ ����
        private void LoadAsset()
        {
            GameObject obj = GameObject.Instantiate(asset.model, transform);
            
            animatedCharacter = obj.AddComponent<AnimatedCharacter>();
            if(!obj.TryGetComponent<Animator>(out Animator animator))
            {
                obj.AddComponent<Animator>();
            }

            animatedCharacter.GetAnimator().runtimeAnimatorController = asset.animationInfo.controller;
            animatedCharacter.GetAnimator().avatar = asset.animationInfo.avatar;
            animatedCharacter.GetAnimator().applyRootMotion = false;
        }

        public void SetAsset(CharacterAsset asset)
        {
            this.asset = asset;
        }

        public void SetSkills(string[] skills)
        {
            this.skills = new Skill[skills.Length];
            for(int i = 0; i < skills.Length; i++)
            {
                this.skills[i] = SkillBuilder.Build(skills[i]);
            }
        }
        #endregion

        private void Update()
        {
            bCmdReady = commandBuffer.IsReady();
            if (BattleSystem.GetInstance() != null && BattleSystem.GetInstance().bOnBattle)
            {
                #region ���� ���� ó��
                //��Ŀ�̵� Ÿ���� ������ Ÿ���� ����
                if (target == null)
                {
                    SetTarget();
                }
                //��ų ��Ÿ�� ������
                for (int i = 0; i < skillCools.Length; i++)
                {
                    if (skillCools[i] > 0)
                        skillCools[i] -= Time.deltaTime;
                }
                if (bCanAttack)
                {
                    //�⺻���� ����
                    if (skillCools[0] <= 0 && target != null)
                    {
                        BattleSystem.GetInstance().ReserveSkillRequest(new SkillRequest(this, target, skills[0]));
                        skillCools[0] = 1 / GetStatus().attackSpeed;
                    }
                }
                //Ÿ���� �׾ ��Ȱ��ȭ�Ǿ��ٸ� ���ο� Ÿ�� ����
                if (target != null && !target.GetTransform().gameObject.activeSelf)
                {
                    target = null;
                    SetTarget();
                }
                #endregion
            }
            #region Ŀ�ǵ� ���� ���� ���� ó��
            commandBuffer.Execute();
            #endregion
        }

        #region �Ϲ� ���� ����
        public void SetCanAttack(bool pBool)
        {
            bCanAttack = pBool;
        }
        public void SetTarget()
        {
            HashSet<Character> targetables = BattleSystem.GetInstance().GetEnemies(this);
            switch (targetingMethod)
            {
                case TargetingMethod.Nearest:
                    Character nearest = null;
                    foreach(Character targetable in targetables)
                    {
                        if(nearest == null)
                            nearest = targetable;
                        else
                        {
                            if(Vector3.SqrMagnitude(nearest.transform.position - this.transform.position) > Vector3.SqrMagnitude(targetable.transform.position - this.transform.position))
                            {
                                nearest = targetable;
                            }
                        }
                    }
                    target = nearest;
                    break;
                case TargetingMethod.Furthest:
                    Character furthest = null;
                    foreach (Character targetable in targetables)
                    {
                        if (furthest == null)
                            furthest = targetable;
                        else
                        {
                            if (Vector3.SqrMagnitude(furthest.transform.position - this.transform.position) < Vector3.SqrMagnitude(targetable.transform.position - this.transform.position))
                            {
                                furthest = targetable;
                            }
                        }
                    }
                    target = furthest;
                    break;
            }
        }
        #endregion

        #region ��ų ���
        public Skill GetSkill(int idx)
        {
            return skills[idx];
        }

        public float GetSkillCooltime(int idx)
        {
            return skillCools[idx];
        }

        public bool UseSkill(int idx)
        {
            if(skillCools[idx] > 0 || !ConsumMp(skills[idx].MpRequire))
            {
                return false;
            }
            BattleSystem.GetInstance().ReserveSkillRequest(new SkillRequest(this, target, skills[idx]));
            skillCools[idx] = skills[idx].Cooltime;
            return true;
        }

        public bool UseSkill(int idx, IDamagable pTarget)
        {
            if (skillCools[idx] > 0 || !ConsumMp(skills[idx].MpRequire))
            {
                return false;
            }
            BattleSystem.GetInstance().ReserveSkillRequest(new SkillRequest(this, pTarget, skills[idx]));
            skillCools[idx] = skills[idx].Cooltime;
            return true;
        }
        #endregion

        #region ����
        public void AddTemporaryStatus(Guid guid, Status status)
        {
            temporaryStatus.Add(guid, status);
        }
        public void RemoveTemporaryStatus(Guid guid)
        {
            temporaryStatus.Remove(guid);
        }
        public void AddBuff(Buff buff)
        {
            buffs.Add(buff);
            buff.OnEffect(this);
        }
        public void EffectBuffs()
        {
            foreach(var buff in buffs)
            {
                buff.Effect(this);
                buff.duration -= Time.deltaTime;
                if(buff.duration <= 0)
                {
                    buff.OnUneffect(this);
                    buffs.Remove(buff);
                }
            }
        }

        public HashSet<Buff> GetBuffs()
        {
            return new HashSet<Buff>(buffs);
        }
        #endregion

        #region ĳ���� Ŀ�ǵ� ����
        public void ReserveCommandBuffer(CharacterCommand cmd)
        {
            commandBuffer.Enqueue(cmd);
        }

        public void ClearCommandBuffer()
        {
            commandBuffer.Clear();
        }

        public void CommandBufferEndCallback()
        {
            commandBuffer.Ready();
        }

        public bool IsCharacterCommandBufferReady()
        {
            return commandBuffer.IsReady();
        }
        #endregion

        #region ��ų �ִϸ��̼� ó��
        public Animator? GetAnimator()
        {
            if(animatedCharacter != null)
                return animatedCharacter.GetAnimator();
            else return null;
        }

        public void ReserveSkillAction(Action action)
        {
            AnimatedCharacter character = GetComponentInChildren<AnimatedCharacter>();
            character.ReserveSkillAction(action);
        }
        #endregion

        #region ĳ���� �̵�����

        public void Teleport(Vector3 destination)
        {
            controller.enabled = false;
            transform.position = destination;
            controller.enabled = true;
        }
        public void Teleport(Vector3 destination, Quaternion quaternion)
        {
            controller.enabled = false;
            transform.position = destination;
            transform.rotation = quaternion;
            controller.enabled = true;
        }
        //CharacterController�� ������Ʈ�� Transform ������ Cancel�ϳ�, ȸ���� �������� �����Ƿ� ���� �޼��带 ����� ����
        public void RotateTo(Vector3 destination)
        {
            controller.enabled = false;
            transform.LookAt(destination);
            controller.enabled = true;
        }
        public void RotateTo(Quaternion quaternion)
        {
            controller.enabled = false;
            transform.rotation = quaternion;
            controller.enabled = true;
        }
        public void MoveTo(Vector3 destination, float duration, Action? preMoveAction, Action? postMoveAction)
        {
            StartCoroutine(MoveToCo(destination, duration, preMoveAction, postMoveAction));
        }

        IEnumerator MoveToCo(Vector3 destination, float duration, Action? preMoveAction, Action? postMoveAction)
        {
            preMoveAction?.Invoke();
            GetAnimator().SetBool("Running", true);

            yield return new WaitForSeconds(preMoveTerm);

            Vector3 startPos = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // ���� ��ġ���� ���������� t ������ŭ ���� ����
                Vector3 nextPos = Vector3.Lerp(startPos, destination, t);
                Vector3 delta = nextPos - transform.position;

                controller.Move(delta);

                yield return null;
            }

            Vector3 finalDelta = destination - transform.position;
            controller.Move(finalDelta);

            GetAnimator().SetBool("Running", false);
            postMoveAction?.Invoke();
        }
        #endregion
    }
}
