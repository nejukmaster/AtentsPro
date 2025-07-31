개요
----------------------
**블루 아카이브**나 **붕괴:스타레일**과 같은 턴제 전투 방식을 차용한 3D 수집형 RPG 포트폴리오입니다.<br>
https://drive.google.com/file/d/1NDoOAa8mQ1nVqf0_cqEylDw2j1dW9sIC/view?usp=sharing

기능
---------------------
구현 목표로 하는 기능은 다음과 같습니다.

**서버**
> 1.유저 어카운트 관리(로그인, 재화, 아이템등)<br>
> 2.캐릭터 관리(캐릭터 획득, 목록 확인, 파티 편성등)<br>
> 3.전투 개시 및 보상 시스템<br>

**클라이언트**
> 1.로그인 기능<br>
> 2.보유 아이템 확인 및 아이템 사용 요청<br>
> 3.재화및 각종 프로필 확인 기능<br>
> 4.캐릭터 모집, 편성, 목록 확인 기능<br>
> 5.전투 기능<br>
> 6.카툰 랜더링<br>

캐릭터
--------------------
캐릭터는 **스테이지**에 등장하여 전투를 하는 아군/적군을 총칭합니다. [Character 클래스](https://github.com/nejukmaster/AtentsPro/blob/main/Assets/Scripts/Objects/Character/Character.cs)를 통해 구현합니다.

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

#### 캐릭터의 스킬
각 캐릭터는 기본 공격과 3개의 액티브 스킬을 가지고, 각 스킬들은 [Skill 클래스](https://github.com/nejukmaster/AtentsPro/blob/main/Assets/Scripts/Battle/Skill/Skill.cs)를 상속하여 구현합니다.
> mpRequire: 이 Skill이 소모하는 MP의 양<br>
> cooltime: 이 Skill의 쿨타임<br>
> Cast: 이 Skill이 사용되었을 때, 호출될 메서드<br>
> GetTargetables: 이 Skill을 사용가능한 대상을 반환하는 메서드<br>

Character의 스킬은 다음과 같은 매커니즘으로 실행됩니다.
  
