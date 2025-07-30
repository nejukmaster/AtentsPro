using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    #region Request 구조체
    public struct SkillRequest
    {
        private ISkillCaster caster;
        public ISkillCaster Caster { get { return caster; } }

        private IDamagable target;
        public IDamagable Target { get { return target; } }

        private Skill skill;
        public Skill Skill { get { return skill; } }

        public SkillRequest(ISkillCaster caster, IDamagable target, Skill skill)
        {
            this.caster = caster;
            this.target = target;
            this.skill = skill;
        }
    }
    public struct DamageRequest
    {
        private IDamageCauser causer;
        public IDamageCauser Causer { get { return causer; } }

        private IDamagable target;
        public IDamagable Target { get { return target; } }

        private float value;
        public float Value { get { return value; } }

        public DamageRequest(IDamageCauser causer, IDamagable target, float value)
        {
            this.causer = causer;
            this.target = target;
            this.value = value;
        }
    }

    public struct DeadRequest
    {
        private IDamagable dead;
        public IDamagable Dead { get { return dead; } }
        public DeadRequest(IDamagable dead)
        {
            this.dead = dead;
        }
    }
    #endregion

    public class BattleSystem : MonoBehaviour
    {
        public static BattleSystem instance;
        public static BattleSystem GetInstance() { return instance; }

        public Queue<SkillRequest> skillRequestQueue = new Queue<SkillRequest>();
        public Queue<DamageRequest> damageRequestQueue = new Queue<DamageRequest>();
        public Queue<DeadRequest> deadRequestQueue = new Queue<DeadRequest>();
        public int queueRefrashCount;

        public bool bOnBattle = false;

        private int skillQueueExecuteCount;
        private int damageQueueExecuteCount;

        private string currentStageName;

        ObjectPool characterPool = new ObjectPool();
        [Header("Object Pool 설정")]
        [SerializeField] int poolingCount;
        [SerializeField] GameObject poolingPrefab;

        HashSet<Character> team = new HashSet<Character>();
        HashSet<Character> enemies = new HashSet<Character>();

        private void Awake()
        {
            instance = this;
            characterPool.Init(poolingPrefab, poolingCount);
            
        }

        private void Start()
        {
            InitStage();
        }

        private void Update()
        {
            ExecuteSkillRequest();
            ExecuteDamageRequest();
            ExecuteDeadRequest();
        }

        #region 스테이지 초기화
        private void InitStage()
        {
            currentStageName = GameManager.GetInstance().homeToBattle.stageName;
            GameObject stage = Resources.Load<GameObject>("Prefabs/Stage/" + currentStageName);
            GameObject.Instantiate(stage);
        }
        #endregion

        public void SetOnBattle(bool pBool)
        {
            bOnBattle = pBool;
        }

        public void EndStage(bool bIsClear)
        {
            UINavigationSystem.GetInstance().ChangeState("end",new UINavigateParam { pBool = bIsClear });
            if(bIsClear)
                WebClient.GetInstance().MakeEndStageRequest(currentStageName);
        }

        #region SkillRequest 처리
        private void ExecuteSkillRequest()
        {
            if (skillRequestQueue.Count > 0)
            {
                SkillRequest req = skillRequestQueue.Dequeue();
                req.Skill.Cast(req.Caster, req.Target);
                skillQueueExecuteCount++;
                if (skillQueueExecuteCount >= queueRefrashCount)
                {
                    skillRequestQueue.TrimExcess();
                    skillQueueExecuteCount = 0;
                }
            }
            else
            {
                skillRequestQueue.TrimExcess();
            }
        }

        public void ReserveSkillRequest(SkillRequest request)
        {
            skillRequestQueue.Enqueue(request);
        }
        #endregion

        #region DamageRequest 처리
        private void ExecuteDamageRequest()
        {
            if (damageRequestQueue.Count > 0)
            {
                DamageRequest req = damageRequestQueue.Dequeue();
                req.Target.OnDamage(req.Value);
                damageQueueExecuteCount++;
                if (damageQueueExecuteCount >= queueRefrashCount)
                {
                    damageRequestQueue.TrimExcess();
                    damageQueueExecuteCount = 0;
                }
            }
            else
            {
                damageRequestQueue.TrimExcess();
            }
        }
        public void ReserveDamage(DamageRequest request)
        {
            damageRequestQueue.Enqueue(request);
        }
        #endregion

        #region DeadRequest처리
        public void ExecuteDeadRequest()
        {
            if (deadRequestQueue.Count > 0)
            {
                DeadRequest req = deadRequestQueue.Dequeue();
                if(req.Dead is Character)
                {
                    GetTeam(req.Dead as Character).Remove(req.Dead as Character);
                    characterPool.Enqueue(req.Dead.GetTransform().gameObject, null);
                }
                else
                {
                    req.Dead.GetTransform().gameObject.SetActive(false);
                }
            }
        }

        public void ReserveDeadRequest(DeadRequest request)
        {
            deadRequestQueue.Enqueue(request);
        }
        #endregion

        #region 아군, 적군에 대한 정보 처리
        public HashSet<Character> GetTeam(Character character)
        {
            if (team.Contains(character)) { return team; }
            else { return enemies; }
        }

        public HashSet<Character> GetTeam()
        {
            return team;
        }

        public void RegisterTeam(Character character)
        {
            team.Add(character);
        }

        public HashSet<Character> GetEnemies(Character character)
        {
            if (team.Contains(character)) { return enemies; }
            else { return team; }
        }

        public HashSet<Character> GetEnemies()
        {
            return enemies;
        }

        public void RegisterEnemy(Character character)
        {
            enemies.Add(character);
        }
        #endregion

        #region 오브젝트 풀링
        public Character SpawnCharacter(string characterName)
        {
            if (characterName == "None") return null;
            GameObject obj = characterPool.Dequeue((o) => { 
                CharacterAsset asset = CharacterAsset.Load(characterName);
                o.GetComponent<Character>()?.SetAsset(asset);
                o.GetComponent<Character>()?.SetStatus(GameManager.GetInstance().homeToBattle.characterDic[characterName].status);
                o.GetComponent<Character>()?.SetSkills(GameManager.GetInstance().homeToBattle.characterDic[characterName].skillSet);
            });
            Character character = obj.GetComponent<Character>();
            Marker marker = MarkerSystem.GetInstance().DequeueMarker("character_status", Camera.main.WorldToScreenPoint(obj.transform.position + new Vector3(0f,1f,0f)));
            marker.SetMarkedObj(obj);
            marker.SetTracking((o) => {
                Vector2 newPos = Camera.main.WorldToScreenPoint(obj.transform.position + new Vector3(0f, 1f, 0f));
                return newPos;
            });
            (marker as CharacterStatusMarker).SetCharacter(character);

            return obj.GetComponent<Character>();
        }

        public void DespawnCharacter(Character character)
        {
            characterPool.Enqueue(character.gameObject, null);
        }
        #endregion
    }
}
