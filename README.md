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

로그인
----------------------
  아이디와 비밀번호를 통해 서버에서 JWT토큰을 발급받는 과정입니다. 아이디와 비밀번호를 Body에 담아 서버로 보내면 서버는 ID를 기반으로 유저를 조회해 비밀번호의 일치여부를 판단합니다. 문제가 없을경우 로그인 성공 메세지와 함께 해당 유저의 시크릿 키를 이용해 생성된 JWT토큰을 응답으로 보냅니다. 클라이언트는 이 JWT토큰을 통해 다시 서버에 유저정보를 요청함과 동시에 Home 씬을 로딩합니다. 프로세스를 도식화하면 다음과 같습니다.

![Login Flowchart](./Images/LoginFlowchart.png)

전투
--------------------
  전투는 플레이어가 선택한 스테이지에 입장하여 플레이어가 설정한 파티를 통해 적을 무찌르고 보상을 받는 과정입니다. 전투는 전투 씬에서 진행되며, 씬에 진입할 때 서버에 현재 스테이지 진입 요청을 하게됩니다. 그러면 서버는 요청의 유효성을 검사하고, 스테이지의 정보를 응답으로 보내주게됩니다. 이 정보를 바탕으로 전투 씬 로드시 스테이지와 적을 생성하여 전투를 진행하고, 스테이지는 각 서브스테이지의 클리어 조건과 스테이지 실패 조건을 검사하기 시작합니다.<br>
  모든 서브스테이지를 클리어하였을 경우, 클라이언트는 서버에 스테이지 성공 요청을 보내게됩니다. 요청을 받은 서버는 유효성 검사후 데이터베이스의 Stages 클러스터에서 성공 요청을 받은 스테이지를 검색하여 성공 보상을 계산하고, 보상중 Gold와 Jem을 감지하여 플레이어의 재화로 반영합니다. 이후 보상 리스트를 담은 응답을 전송하고, 클라이언트는 스테이지 완료 UI에 이를 표시하게됩니다. 프로세스를 도식화하면 다음과 같습니다.

![Battle Flowchart](./Images/BattleFlowchart.png)

#### 캐릭터 행동 및 스킬
  전투에서 캐릭터의 행동은 캐릭터 커맨드를 통해 제어됩니다. 캐릭터 커맨드는 [CharacterCommandBuffer](./Assets/Scripts/Objects/Character/CharacterCommandBuffer.cs)에 정의되어있습니다. 캐릭터 커맨드는 캐릭터의 캐릭터 커맨드 버퍼에 예약되고, 이를 캐릭터가 순차적으로 실행합니다. 캐릭터 커맨드에선 캐릭터의 이동, 애니메이션 재생등을 제어합니다.<br>
  캐릭터마다 스킬은, 캐릭터의 기본공격까지 포함하여 총 4개를 보유하며 [Skill 클래스](./Assets/Scripts/Battle/Skill/Skill.cs)를 상속하여 구현합니다. 사용자가 UI를 통해 캐릭터의 스킬을 사용하면 [BattleSystem](./Assets/Scripts/System/BattleSystem.cs)에 SkillRequest를 예약하며 BattleSystem은 이 SkillRequest를 순차적으로 Cast합니다. Skill이 Cast되면 시전 캐릭터에 SkillAction과 캐릭터 커맨드를 예약하고, 이 캐릭터 커맨드는 해당 스킬의 애니메이션 재생을 포함합니다. 해당 스킬의 애니메이션이 재생되면 Unity의 애니메이션 이벤트 기능을 통해 캐릭터에 예약된 SkillAction을 실행하고, 여기서 효과나 데미지 처리등을 합니다. 이를 도식화하면 다음과 같습니다.

![CharacterCommand Flowchart](./Images/CharacterCommandFlowchart.png)

홈
--------------------
  홈은 계정의 재화, 아이템, 보유 캐릭터를 확인하거나, 현재 파티의 구성을 변경, 새 캐릭터 모집 및 스테이지를 선택하여 전투씬으로 입장하는 것이 가능한 장소입니다.

#### 홈 씬 진입
 홈 씬에 진입할 경우, 클라이언트는 서버에 최신의 유저 정보와 글로벌 인포(현재 진행중인 이벤트 등)를 요청하게 됩니다. 먼저 로딩씬을 로드하고 유저 정보 요청을 보내며, 유저 정보가 정상적으로 도착하면 글로벌 인포를 요청합니다. 이때, 향후 OpenAPI등의 확장을 고려하여 따로 유효성 검사를 진행하진 않습니다. 이후 해당 정보들을 GameManager에 업데이트한 후, 홈 씬을 로드하게 됩니다. 이를 도식화 하면 다음과 같습니다.
 
 ![HomeSceneLoadFlowchart](./Images/HomeSceneLoadFlowchart.png)

 #### 모집
  홈 씬에서 하단 네비게이션을 통해 모집 UI로 넘어갈 수 있습니다. 현재 진행중인 모집에 젬을 사용하여 모집을 진행할 수 있으며, 이는 글로벌 인포에 담겨져 있으므로, UI가 로딩될 때 서버로 요청을 보내진 않습니다. 진행중인 모집은 이벤트 코드로 구분하며, 플레이어가 모집을 진행하면 이벤트 코드를 포함하는 모집 요청을 서버로 보내게됩니다. 그러면 서버는 유효성 검사와 함께 유저의 재화를 검사하며, 유저의 젬이 충분할 경우 서버는 DB에서 요청받은 이벤트를 검색하고, 모집 결과를 산출하게 됩니다. 산출된 모집 결과는 모집후 업데이트된 유저 정보와 함께 응답으로써 클라이언트에 보내지게됩니다. 응답을 받은 클라이언트는 유저 정보를 GameManager에 업데이트하고 모집 컷씬을 재생합니다. 이를 도식화하면 다음과 같습니다.

![RecruitFlowchart](./Images/RecruitFlowchart.png)


#### 아이템 사용
  모집과 마찬가지로 하단 네비게이션을 통해 가방 UI로 넘어갈 수 있으며, 현재 보유한 아이템을 확인하고, 사용가능한 아이템은 사용할 수 있습니다. 인벤토리 정보는 유저 정보에 담겨져 있기 때문에 UI 진입시 서버에 요청을 보내지는 않습니다. 사용가능한 아이템을 확인하게 되면 아이템 확인 UI에 "사용" 버튼이 활성화되고, 이를 클릭시 아이템 사용 요청을 보내게됩니다. 서버는 유효성 검사후 아이템 사용 처리를 하고, 결과 메세지와 업데이트된 유저 정보를 응답으로 반환합니다. 이를 받은 클라이언트는 UI에 결과 메세지를 띄우고, GameManager에 유저 정보를 업데이트합니다. 이를 도식화하면 다음과 같습니다.

![UseItemFlowchart](./Images/UseItemFlowchart.png)

#### 파티 편성
  모집, 가방과 마찬가지로 하단 네비게이션을 통해 캐릭터 UI로 넘어갈 수 있으며, 여기서 좌측 버튼을 통해 파티 편성 UI로 넘어갈 수 있습니다. 파티는 최대 네명의 캐릭터로 구성될수 있습니다. 파티 편성 UI의 각 자리 버튼을 클릭하면 캐릭터 선택 UI가 활성화 되며, 배치할 캐릭터를 클릭하면 서버로 파티 편성 요청을 보내게 됩니다. 이 요청에는 변경된 자리의 인덱스와 변경한 캐릭터 정보를 담고 있으며, 서버는 이를 받아서 유효성 검사후 유저 정보를 업데이트합니다. 이후, 업데이트된 유저 정보를 응답으로 보내며, 클라이언트는 이를 받아 유저 정보를 업데이트하고 파티 편성 UI를 갱신합니다. 이를 도식화 하면 다음과 같습니다.

![CharacterLineupFlowchart](./Images/CharacterLineupFlowchart.png)

확장성
--------------------
  온라인 서비스를 상정한 게임이므로 향후 확장성을 고려하여 시스템을 디자인했습니다. 게임내 확장 가능한 모든 컨텐츠가 일정한 프로세스에 맞춰 추가될 수 있도록 하는 것이 최우선시한 목표이며, 또한, 그 과정에서 최대한 프로그래밍 과정을 적게 하는것이 두번째 목표였습니다.
  
#### 캐릭터
캐릭터는 스테이지에 등장하여 전투를 하는 아군/적군을 총칭하며 [Character 클래스](./Assets/Scripts/Objects/Character/Character.cs)를 통해 구현합니다.

#### AnimatedCharacter
[AnimatedCharacter 클래스](./Assets/Scripts/Objects/Character/AnimatedCharacter.cs)는 Animator 컴포넌트와 상호작용하는 컴포넌트이며, Character클래스가 CharacterAsset을 참조하여 Prefab을 생성할 때, 생성된 GameObject에 붙습니다. 수행하는 기능은 다음과 같습니다.
> 1. 스킬의 효과를 나타내는 SkillAction을 순차적으로 처리<br>
> 2. 애니메이션 재생이 끝났을 경우 AnimationEndCallback을 통해 Character에 전달<br>
> 3. 해당 Character가 사용하는 Animator를 GetAnimator를 통해 반환

이는 유니티 애니메이션의 Event 기능이 Animator가 붙은 GameObject를 기준으로 하므로 Character 클래스를 분리해놓은 것 입니다.

#### 새로운 캐릭터 제작 프로세스
> 1. 캐릭터로 사용할 모델, 애니메이션 데이터를 준비하고, 해당 캐릭터용 AnimatorController를 생성한다.<br>
> 2. 애니메이션 클립에 Event를 설정한다.<br>
> 3. AtentsPro > CharacterAsset을 생성하고 이 데이터를 바인딩한다<br>
> 4. 데이터베이스의 Characters 콜렉션에 해당 캐릭터의 데이터를 작성한다.<br>
> 5. 캐릭터의 스킬 4개를 기획하여 Resources/DataSheet/SkillTable.csv에 작성한다.<br>
> 6. 기획을 바탕으로 Skill 클래스를 상속한 캐릭터의 스킬 클래스를 제작한다.<br>
> 7. SkillBuilder 스태틱 클래스에 해당 스킬의 코드로 스킬의 객체를 생성하는 코드를 추가한다.<br>
> 8. 스킬의 아이콘을 제작하고 아틀라스로 묶는다.<br>
> 9. 해당 캐릭터의 초상화와 전신 일러스트를 제작한다.<br>

#### 스테이지
스테이지는 [Stage 클래스](./Assets/Scripts/Battle/Stage/Stage.cs)를 상속하여 구현합니다.  [StageTable](./Assets/Resources/DataSheet/StageTable.csv)에 각 스테이지에 대한 정보를 저장하고있습니다. [StageEditor 클래스](./Assets/Scripts/Editor/Battle/StageEditor.cs)를 제작하여 스테이지 제작시에 GUI를 사용하여 보다 빠르고 효율적으로 제작할 수 있도록 하였습니다.

**Substage**는 스테이지를 구성하는 작은 스테이지들입니다. 아군들과 적들이 위치할 포지션에 대한 정보와 등장하는 적들의 정보를 담고있습니다. 여기서 적들의 정보는 Resources/DataSheet/StageTable.csv에 저장되어있으나, 실제로는 클라이언트 변조를 예방하기 위하여 전투 개시시에 서버에 요청하여 받아옵니다.

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

#### 아이템
아이템은 캐릭터가 가방 UI에서 확인하거나 사용할 수 있는 요소입니다. 아이템의 정보는 [ItemTable](./Assets/Resources/DataSheet/ItemTable.csv)에 저장되어있으며, 아이디, 이름, 설명, 사용가능 여부로 구성됩니다

#### 새로운 아이템 제작 프로세스
> 1. Resources/DataSheet/ItemTable.csv에 아이템 정보 추가
> 2. Resources/Texture/ItemIcon에 아이템 아이콘 텍스쳐를 제작하여 추가
> 3. 사용가능한 아이템일 경우 Server/AtentsServer.js에 아이템 사용 요청시 처리될 함수를 추가
> 4. 해당 함수를 UserItem 메서드 분기에 추가

카툰 랜더링
--------------------------
  
