개요
----------------------
**블루 아카이브**나 **붕괴:스타레일**과 같은 턴제 전투 방식을 차용한 3D 수집형 RPG 포트폴리오입니다.

**작동 영상**
https://drive.google.com/file/d/1NDoOAa8mQ1nVqf0_cqEylDw2j1dW9sIC/view?usp=sharing

기능
---------------------
구현 목표로 한 기능은 다음과 같습니다.

**서버**
> 1. 유저 어카운트 관리(로그인, 재화, 아이템등)<br>
> 2. 캐릭터 관리(캐릭터 획득, 목록 확인, 파티 편성등)<br>
> 3. 전투 개시 및 보상 시스템<br>

**클라이언트**
> 1. 로그인 기능<br>
> 2. 보유 아이템 확인 및 아이템 사용 요청<br>
> 3. 재화및 각종 프로필 확인 기능<br>
> 4. 캐릭터 모집, 편성, 목록 확인 기능<br>
> 5. 전투 기능<br>
> 6. 카툰 랜더링<br>

캐릭터
--------------------
**캐릭터**는 **스테이지**에 등장하여 전투를 하는 아군/적군을 총칭합니다. [Character 클래스](https://github.com/nejukmaster/AtentsPro/blob/main/Assets/Scripts/Objects/Character/Character.cs)를 통해 구현합니다.

#### 캐릭터 스테이터스
캐릭터 스테이터스는 [Status 구조체](https://github.com/nejukmaster/AtentsPro/blob/main/Assets/Scripts/Objects/Character/Status.cs)를 통해 구현합니다. 각 항목은 다음과 같습니다.
> maxHp: 캐릭터의 최대 HP<br>
> maxMp: 캐릭터의 최대 MP<br>
> attack: 캐릭터의 공격력<br>
> defence: 캐릭터의 방어력<br>
> criticalRate: 캐릭터의 크리티컬 확률<br>
> criticalDamage: 캐릭터의 크리티컬시 데미지 증가량<br>
> attackSpeed: 캐릭터의 기본공격 속도<br>
> hp: 캐릭터의 현재 HP<br>
> mp: 캐릭터의 현재 MP<br>

#### CharacterAsset
[CharacterAsset 클래스](https://github.com/nejukmaster/AtentsPro/blob/main/Assets/Scripts/Objects/Character/CharacterAsset.cs)는 Character 클래스가 활성화될 때 생성할 모델과, 생성된 GameObject에 적용될 애니메이션 데이터를 담고있는 ScriptableObject 클래스입니다.

#### CharacterCommandBuffer
[CharacterCommandBuffer 클래스](https://github.com/nejukmaster/AtentsPro/blob/main/Assets/Scripts/Objects/Character/CharacterCommandBuffer.cs)는 Character의 행동을 제어하는 CharacterCommand를 관리하는 클래스입니다. 캐릭터는 다른 장소로 이동하거나 애니메이션을 실행하는데 비해, 사용자의 UI조작은 언제든 일어날 수 있으므로, UI를 통해 발생된 캐릭터 조작 명령을 CharacterCommand 형태로 큐잉하여 순차적으로 관리합니다. 이는 "bCmdReady" 프로퍼티를 통해 한 CharacterCommand가 종료되어야 다음 CharacterCommand를 실행하는 구조로 되어있습니다.

#### AnimatedCharacter
[AnimatedCharacter 클래스](https://github.com/nejukmaster/AtentsPro/blob/main/Assets/Scripts/Objects/Character/AnimatedCharacter.cs)는 Animator 컴포넌트와 상호작용하는 컴포넌트이며, Character클래스가 CharacterAsset을 참조하여 Prefab을 생성할 때, 생성된 GameObject에 붙습니다. 수행하는 기능은 다음과 같습니다.
> 1. 스킬의 효과를 나타내는 SkillAction을 순차적으로 처리<br>
> 2. 애니메이션 재생이 끝났을 경우 AnimationEndCallback을 통해 Character에 전달<br>
> 3. 해당 Character가 사용하는 Animator를 GetAnimator를 통해 반환

이는 유니티 애니메이션의 Event 기능이 Animator가 붙은 GameObject를 기준으로 하므로 Character 클래스를 분리해놓은 것 입니다.

#### 캐릭터의 스킬
각 캐릭터는 기본 공격과 3개의 액티브 스킬을 가지고, 각 스킬들은 [Skill 클래스](https://github.com/nejukmaster/AtentsPro/blob/main/Assets/Scripts/Battle/Skill/Skill.cs)를 상속하여 구현합니다.
> mpRequire: 이 Skill이 소모하는 MP의 양<br>
> cooltime: 이 Skill의 쿨타임<br>
> Cast: 이 Skill이 사용되었을 때, 호출될 메서드<br>
> GetTargetables: 이 Skill을 사용가능한 대상을 반환하는 메서드<br>

Character의 스킬은 기본적으로 다음과 같은 매커니즘으로 실행됩니다.<br>
> 1. Character의 UseSkill을 통해 BattleSystem에 SkillRequest를 예약<br>
> 2. BattleSystem이 SkillRequest의 Skill에 순차적으로 Cast 호출<br>
> 3. Character에 ReserveSkillAction을 통해 AnimatedCharacter에 SkillAction을 예약<br>
> 4. Character의 ReserveCommandBuffer를 통해 커맨드 버퍼를 예약하여 Skill의 애니메이션을 트리그<br>
> 5. 애니메이션의 Event를 통해 AnimatedCharacter의 SkillAction을 순차적으로 실행

#### 새로운 캐릭터 제작 프로세스
> 1. 캐릭터로 사용할 모델, 애니메이션 데이터를 준비하고, 해당 캐릭터용 AnimatorController를 생성한다.<br>
> 2. 애니메이션 클립에 Event를 설정한다.<br>
> 3. AtentsPro > CharacterAsset을 생성하고 이 데이터를 바인딩한다<br>
> 4. 데이터베이스의 Characters 콜렉션에 해당 캐릭터의 데이터를 작성한다.<br>
> 5. 캐릭터의 스킬 4개를 기획하여 Resources/DataSheet/SkillTable에 작성한다.<br>
> 6. 기획을 바탕으로 Skill 클래스를 상속한 캐릭터의 스킬 클래스를 제작한다.<br>
> 7. SkillBuilder 스태틱 클래스에 해당 스킬의 코드로 스킬의 객체를 생성하는 코드를 추가한다.<br>
> 8. 스킬의 아이콘을 제작하고 아틀라스로 묶는다.<br>
> 9. 해당 캐릭터의 초상화와 전신 일러스트를 제작한다.<br>

스테이지
-----------------------------------------
**스테이지**는 전투가 이루어지는 스테이지를 총칭합니다. [Stage 클래스](https://github.com/nejukmaster/AtentsPro/blob/main/Assets/Scripts/Battle/Stage/Stage.cs)를 상속하여 구현합니다. 스테이지의 구성 다음과 같습니다.
> spawnPos: 스테이지가 아군 캐릭터들을 최초로 생성하는 포지션<br>
> substages: 스테이지를 구성하는 Substage 배열<br>
> track: Substage가 클리어 됨에 따라 카메라가 움직일 DollyTrack<br>
> substageChangeDelay: 카메라가 다음 Track Index까지 가는 속도를 조절<br>
> bIsEnd: 스테이지가 끝났는지 여부<br>
> Initialize: 스테이지를 초기화하는 메서드. 캐릭터를 생성하고, 메인 카메라를 track에 배치하는 작동을 합니다.<br>
> EnterSubstage: 지정된 Substage를 시작<br>
> EndStage: 스테이지를 완료 처리<br>
> IsSubstageEnd: Substage의 종료 조건을 결정하는 추상메서드<br>
> IsFailed: 스테이지의 실패 조건을 결정하는 추상메서드<br>

또한 **Substage**의 구성은 다음과 같습니다.
> teamPos: 해당 Substage에서 아군 캐릭터들이 위치할 Position<br>
> enemyPos: 해당 Substage에서 적 캐릭터들이 위치할 Position<br>
> enemiesCharacter: 해당 Substage에 등장할 적 목록. 클라이언트 변조를 막기 위해 전투 개시시 서버로부터 받아옵니다.<br>

#### 새로운 스테이지 제작 프로세스
> 1. 게임 오브젝트를 생성하고 Stage 컴포넌트를 추가<br>
> 2. 스테이지 환경을 조성<br>
> 3. SpawnPos를 추가하고 GUI를 통해 위치를 설정<br>
> 4. Substage를 추가하고 GUI를 통해 teamPos및 enemyPos를 설정<br>
> 5. 게임 오브젝트를 프리팹으로 제작<br>
> 6. 데이터베이스 Stages 클러스터에 해당 Stage를 추가하고, enemies및 rewards 설정<br>
> 7. 데이터베이스 Users 클러스터에 원하는 유저의 enableStages에 해당 Stage를 추가<br>
> 8. Resources/DataSheet에 해당 스테이지의 정보를 추가<br>
> 9. Resources/Texture/Illustration/StagePreveiw에 Stage의 프리뷰 이미지를 제작해서 저장<br>

아이템
----------------------------------------
**아이템**은 캐릭터가 **가방 UI**에서 확인하거나 사용할 수 있는 요소입니다. 아이템의 정보는 Resources/DataSheet/ItemTable.csv에 저장되어있으며, 이름, 일러스트, 설명, 사용가능 여부로 구성됩니다. 아이템 사용시 프로세스는 다음과 같습니다.
> 1. 아이템 확인시 사용이 가능한 아이템일 경우 "사용" 버튼 활성화<br>
> 2. "사용" 버튼을 클릭시 서버로 아이템 사용 요청을 보냄<br>
> 3. 서버는 요청의 유효성을 검사하고, 아이템을 사용처리<br>
> 4. 이후 결과 메세지와 업데이트된 유저 정보를 담은 응답을 반환하고, 클라이언트는 UI에 이를 업데이트<br>
