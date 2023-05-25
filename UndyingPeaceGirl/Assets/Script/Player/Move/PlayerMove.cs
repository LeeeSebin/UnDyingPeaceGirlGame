using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour  //플레이어의 조종, 무엇을 조종중인가 상태등을 다룸. 애니메이션고나리도 여기서
{
    public ThrowRoute throwRoute = new ThrowRoute();

    public static PlayerMove instance;

    private void Awake()
    {
        if (instance == null)
        {
            throwRoute.Init();
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    SpiderMove spiderMove;
    [HideInInspector] public FlyingSquirrelMove squirrelMove;
    SuperRobotMove superRobotMove;

    private Vector3 vector;//움직일때 쓰는 벡터
    [HideInInspector] public SpriteRenderer moveObjSpriteRenderer;
    [SerializeField] private LayerMask layerMask;
    public GameObject weapon;
    public bool moving, headCatch, action, spiderClimb, moveObjHitting;
    public bool playerJumping, headJumping, bodyJumping, legJumping, moveObjJumping;
    public Animator playerAnime;
    public bool testbool;
    public bool transformWait;
    public GameObject interactionObj;
    private float angle, power, throwChangeSpeed;

    private float defaultSpeed, defaultJumpPower;//플레이어 속도관련
    public GameObject moveObject;

    private PlayerInputMgr inputMgr;
    private PlayerDock dockMgr;

    float moveY;

    public Vector3 loadScenePlayerVector;



    void OnSceneLoaded(Scene scene, LoadSceneMode level)//씬전환시 위치설정
    {
        moveObject.transform.position = loadScenePlayerVector;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {

        //전용 조작을 위한 하위 move스크립트들
        spiderMove = GetComponent<SpiderMove>();
        squirrelMove = GetComponent<FlyingSquirrelMove>();
        superRobotMove = GetComponent<SuperRobotMove>();

        //속도 초기화
        defaultSpeed = 10f;
        defaultJumpPower = 35f;

        moving = false;
        playerJumping = false;
        legJumping = false;
        moveObjJumping = false;
        headCatch = false;
        action = false;
        spiderClimb = false;
        moveObjHitting = false;
        throwRoute.InitTrajectory();
        vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), moveObject.transform.position.z);
        moveObjSpriteRenderer = moveObject.GetComponent<SpriteRenderer>();

        inputMgr = GetComponent<PlayerInputMgr>();
        dockMgr = GetComponent<PlayerDock>();
        throwChangeSpeed = 20f;
    }
    // Update is called once per frame
    void Update()
    {
        //상태체크후 적당한 함수실행하기
        if (!DialogueMgr.instance.talking)//대화도중이 아니며
        {
            if (!moveObjHitting)//피격도중도 아니고
            {
                if (!action)//액션중이 아님
                {
                    if (!playerJumping && !legJumping) //가만히 있을때 가능한 행동들
                    {
                    }
                }
                else
                {
                    if (moving)
                        StopRun();//액션이 활성화되면 걷기 멈추기
                }

                if (action && headCatch)
                {
                    ShowThrowRoute();
                }
                //----------------------------------------------------------------------------------------
                //테스트용
                //if (Input.GetKeyDown(KeyCode.O))
                //{
                //    AnimeMgr.instance.FadeStart();
                //}
                //if (Input.GetKeyDown(KeyCode.L))
                //{
                //    AnimeMgr.instance.PlayerHurrayStart();
                //}
                //if (Input.GetKeyDown(KeyCode.I))
                //{
                //    GameMgr.instance.PlayerHit(moveObject, 1);
                //}
                //-----------------------------------------------
            }
            else
                if(moving)
                    StopRun();//피격시 달리기멈추기
        }
        else//말하게될경우
        {
            moveObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, moveObject.GetComponent<Rigidbody2D>().velocity.y);
            if (moving)
                StopRun();
        }

        //플레이어상태 상시체크용--------------------------------------------------------------------------------------------
        moveY = moveObject.GetComponent<Rigidbody2D>().velocity.y;

        if (!moveObjHitting)
        {

        }
    }
    //-----------------------------------------------------------키이벤트 구역
    public void UpkeyEvent()//점프키 눌럿을때 (거미줄타기, 날기등등을 여기서 선별해 호출해줄것)
    {
        if (CanMoveCheck())
        {
            if (moveObject.name.Equals("Spider") && moveObject.GetComponent<Rigidbody2D>().gravityScale == -1 && spiderClimb)//거미천장타기 취소
            {
                spiderMove.SpiderClimbStop();
            }
            else if (moveObject.name.Equals("Player") && !playerJumping)//합체점프
            {
                Jump(moveObject);
            }
            else if (moveObject.name.Equals("Leg") && !legJumping)//다리점프
            {
                Jump(moveObject);
            }
            else if (moveObject.name.Equals("Head") && !headJumping)//다리점프
            {
                Jump(moveObject);
            }
            else if (moveObject.name.Equals("Body") && !bodyJumping)//다리점프
            {
                Jump(moveObject);
            }
            else if (moveObject.name.Equals("SuperRobot") && !playerJumping)
            {
                Jump(moveObject);
            }


            else if (moveObject.name.Equals("Spider") && !playerJumping)//거미점프
            {
                Jump(moveObject);
            }
            else if (moveObject.name.Equals("Spider") && playerJumping) //천장타기 시전
            {
                spiderMove.SpdierClimbCheck();
            }

            else if (moveObject.name.Equals("FlyingSquirrel") && !playerJumping)
            {
                Jump(moveObject);
            }
        }
    }
    public void UpKeyPressingEvent()
    {
        if (CanMoveCheck())
        {
            if (moveObject.name.Equals("FlyingSquirrel"))
            {
                if (moveObject.GetComponent<FallingCheck>().ReturnFalling())
                {
                    squirrelMove.GlidingStart();
                }
            }
        }
    }
    public void UpKeyUpEvent()
    {
        if (moveObject.name.Equals("FlyingSquirrel"))
        {
            if(playerJumping)
            {
                squirrelMove.GlidingEnd();
            }
        }
    }

    public void DownKeyEvent()
    {
        if (CanMoveCheck())
        {
            if (moveObject.name.Equals("Spider") && moveObject.GetComponent<Rigidbody2D>().gravityScale == -1)//거미천장타기 취소
            {
                GetComponent<SpiderMove>().SpiderClimbStop();//플레이어 무브로 조작감지 보내고 무브에서 스파이더 무브로 보내도록 수정해야함!!
            }
        }
    }

    public void HorizontalKeyDownEvent()
    {
        if (action && dockMgr.transformChoice && !DialogueMgr.instance.talking)//변신선택
        {
            if (Input.GetAxisRaw("Horizontal") == -1)
                dockMgr.InputTransformChoice(0);
            else
                dockMgr.InputTransformChoice(2);
            if (Input.GetAxisRaw("Vertical") == 1)
                dockMgr.InputTransformChoice(1);
            else
                dockMgr.InputTransformChoice(100);
        }
    }

    public void VerticalKeyDownEvent()
    {
        if (action && dockMgr.transformChoice && !DialogueMgr.instance.talking)//변신선택
        {
            if (Input.GetAxisRaw("Vertical") == 1)
                dockMgr.InputTransformChoice(1);
            else
                dockMgr.InputTransformChoice(100);
        }
    }

    public void DivideKeyEvent()
    {
        if (moveObject.name.Equals("SuperRobot"))
        {

        }
        else
            DivideOrCombine();
    }

    public void TransformKeyEvent()
    {
        if (moveObject.name.Equals("SuperRobot"))
        {
            superRobotMove.LaseBeamAttackEvent();
        }
        else
        {
            dockMgr.TransformChoice();
        }
    }

    //---------------------------------------------------------------------------키이벤트 구역

    public void InteractionTargeting(GameObject interactionObj)
    {
        if (interactionObj == null)
            this.interactionObj = null;
        else
        {
            this.interactionObj = interactionObj;
        }
    }

    public void NormalAttackEvent()
    {
        if(moveObject.name.Equals("Player") || moveObject.name.Equals("SuperRobot"))
        {
            //Attack(weapon);
            if (moveObject.GetComponent<PlayerAnimeMgr>().AttackCheck())//이미 공격도중일경우
            {
                //moveObject.GetComponent<PlayerAnimeMgr>().AttackCancel();
            }
            else
            {
                action = true;
                moveObject.GetComponent<Animator>().SetTrigger("NormalAttack");
                //playerAnime.SetTrigger("NormalAttack");
            }
        }
    }

    void HeadCatch()
    {
        if (Vector2.Distance(PlayerDock.instance.head.transform.position, PlayerDock.instance.body.transform.position) < 3f)
        {
            StopRun();
            action = true;
            headCatch = true;
            PlayerDock.instance.head.transform.position = new Vector3(PlayerDock.instance.body.transform.position.x, PlayerDock.instance.body.transform.position.y + 0.3f, 0);
            PlayerDock.instance.head.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            if (!moveObject.GetComponent<SpriteRenderer>().flipX)//우측보고 있을시
                angle = 60;
            else
                angle = 120;
            power = 30;
            throwRoute.thisObj.SetActive(true);

        }
    }

    public void HeadThrow()
    {
        action = false;
        headCatch = false;
        PlayerDock.instance.head.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        float radian = angle * Mathf.Deg2Rad; // 숫자부분에 각도 넣으면됨
        Vector2 vec2;
        //vec2.x = Mathf.Cos(radian) * power;//좌우로 미는힘
        //vec2.y = Mathf.Sin(radian) * power;//위아래로 미는힘 지금 3에는 원래 파워가 들어가야함.
        vec2 = throwRoute.GetDirection(angle, power);

        PlayerDock.instance.head.GetComponent<Rigidbody2D>().AddForce(vec2 * 60);

        throwRoute.thisObj.SetActive(false);
    }

    public void Falling(GameObject fallingObject)//플레이어가 아래로 떨어지는게 감지될경우 호출하시오. 매게변수는 어떤 오브젝트가 떨어지는지 알려줍니다.
    {
        //떨어지는경우 애니메이션을 바꿔주고 bool Jumping값을 변경해줍니다.
        if (!fallingObject.GetComponent<Animator>().GetBool("FloorCheck"))//공중에 떠있는상태일경우
        {
            fallingObject.GetComponent<PlayerAnimeMgr>().FallingAnimeStart(fallingObject.GetComponent<Rigidbody2D>().velocity.y);
            if (fallingObject.name.Equals("Player") && !playerJumping)
            {
                playerAnime.SetTrigger("SuddenlyJump");
            }
            else if (fallingObject.name.Equals("Head") && !headJumping)
            {
                GameMgr.instance.Head.GetComponent<Animator>().SetTrigger("SuddenlyJump");
            }
            else if (fallingObject.name.Equals("Body") && !bodyJumping)
            {
                GameMgr.instance.Body.GetComponent<Animator>().SetTrigger("SuddenlyJump");
            }
            else if (fallingObject.name.Equals("Leg") && !legJumping)
            {
                GameMgr.instance.Leg.GetComponent<Animator>().SetTrigger("SuddenlyJump");
            }
            else if (fallingObject.name.Equals("Spider") && !playerJumping)
            {
                GameMgr.instance.Spider.GetComponent<Animator>().SetTrigger("SuddenlyJump");
            }

            JumpingToTrue(fallingObject);
        }
    }

    public void Upping(GameObject uppingObject)//플레이어가 위로 올라가는게 감지될경우 호출하시오. 매게변수는 어떤 오브젝트가 올라가는지 알려줍니다.
    {
        uppingObject.GetComponent<PlayerAnimeMgr>().UppingAnimeStart();
        JumpingToTrue(uppingObject);
    }

    public void JumpingToTrue(GameObject obj)
    {
        if (obj.name.Equals("Player"))
        {
            playerJumping = true;
        }
        else if (obj.name.Equals("Head"))
        {
            headJumping = true;
        }
        else if (obj.name.Equals("Body"))
        {
            bodyJumping = true;
        }
        else if (obj.name.Equals("Leg"))
        {
            legJumping = true;
        }
        else
        {
            playerJumping = true;
        }

        if(obj.name.Equals(moveObject.name))
        {
            moveObjJumping = true;
        }
    }

    public void PlayerLanding(string landingName)//이것은 플레이어의 공중애니메이션을 취소시킨다. 플레이어가 땅에 닿을때 호출하면 긍정적이다.
    {
        if (landingName.Equals("Player"))
        {
            playerJumping = false;
        }
        else if (landingName.Equals("Head"))
        {
            headJumping = false;
        }
        else if (landingName.Equals("Body"))
        {
            bodyJumping = false;
        }
        else if(landingName.Equals("Leg"))
        {
            legJumping = false;
        }
        else
        {
            playerJumping = false;
        }

        if(landingName.Equals(moveObject.name))
        {
            moveObjJumping = false;
        }
    }


    public void PlayerHit(GameObject HitObj, int dir)//당신은 이것을 호출하면 피격으로인한 물리적결과값이 발생합니다.
    {
        //공격당할시 취소되야할것들 모두 취소해 줍시다.

        if (HitObj == moveObject)//조종중인 오브젝트가 피격시
        {
            StopRun();
            moveObjHitting = true; // 조작불가상태로 만들기위한 조건입니다.
        }
        HitObj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //프리징 걸어주기.

        moveObject.GetComponent<Rigidbody2D>().gravityScale = 1f;//다람쥐 활공중일경우 위해 초기화
        StopHeadCatch();//머리잡는도중일 경우를 위해 머리잡기 취소

        HitObj.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 650);
        if(dir == 1)//좌측에서 우측으로 날아온 공격
            HitObj.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 250);
        else
            HitObj.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 250);

    }

    //test ignore
    public void PlayerDestroy()
    {
        StopRun();
        action = true;
        moveObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //프리징 걸어주기.
        moveObject.GetComponent<PlayerAnimeMgr>().DestroyAnime();
        PlayerDock.instance.ResetPlayer();
    }
    void Jump(GameObject obj)//점프조작을 할경우 이것을 호출하시오.
    {
        float _jumpPower = defaultJumpPower * moveObject.GetComponent<CharStat>().stat["jumpPower"];
        if (moveObject.name.Equals("Head") || moveObject.name.Equals("Body"))
            moveObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);//점프해줌. 착지판정은 개별 오브젝트에 있음
        else
            moveObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);//점프해줌. 착지판정은 개별 오브젝트에 있음

        moveObject.GetComponent<PlayerAnimeMgr>().JumpAnimeStart();
        SoundManager.instance.JumpSoundPlay();
        JumpingToTrue(obj);
    }
    void RunStart()//걷기
    {
        //----------------------------------------------걷기 애니메이션 관리
        moveObject.GetComponent<PlayerAnimeMgr>().RunAnimeStart();
        //----------------------------------------------하단은 이동과 관련되어 있습니다.
        moving = true;
        moveObjSpriteRenderer = moveObject.GetComponent<SpriteRenderer>();
    } 
    public void Run(float Horizontal)
    {
        if (CanMoveCheck())
        {
            if (!moving)
            {
                RunStart();
                if (moveObject.GetComponent<PlayerAnimeMgr>().AttackCheck())//이미 공격도중일경우
                {
                    moveObject.GetComponent<PlayerAnimeMgr>().AttackCancel();
                }
            }

            if (moving)
            {
                vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), moveObject.transform.position.z);
                RaycastHit2D underHit, middleHit, topHit;
                Vector2 start;
                Vector2 end;

                if (Horizontal == -1)//왼쪽 스프라이트 전환//스파인 때문에 여기서 스케일값 변경필요
                {
                    moveObjSpriteRenderer.flipX = true;
                    moveObject.transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (Horizontal == 1 || Horizontal == 0)//오른쪽
                {
                    moveObjSpriteRenderer.flipX = false;
                    moveObject.transform.localScale = new Vector3(1, 1, 1);
                }
                moveObject.GetComponent<PlayerAnimeMgr>().TryMoveAnime(Horizontal);

                //---------------------------------이동할 방향 캐릭터 상중하단 체크로 벽비빔 방지
                start = new Vector2(moveObject.transform.position.x, moveObject.transform.position.y + (1 * moveObject.GetComponent<BoxCollider2D>().size.y / 2));//캐릭터 현재위치
                end = start + new Vector2(vector.x * (moveObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().size.x / 2 + 0.1f), 0);//캐릭터가 이동할 위치
                topHit = Physics2D.Linecast(start, end, layerMask);//스타트는 플레이어위치, 엔드는 도착할위치. 라인캐스트발사
                Debug.DrawLine(start, end, Color.red);

                start = new Vector2(moveObject.transform.position.x, moveObject.transform.position.y);
                end = start + new Vector2(vector.x * (moveObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().size.x / 2 + 0.1f), 0);
                middleHit = Physics2D.Linecast(start, end, layerMask);
                Debug.DrawLine(start, end, Color.red);

                start = new Vector2(moveObject.transform.position.x, moveObject.transform.position.y - (1 * moveObject.GetComponent<BoxCollider2D>().size.y / 2) + 0.1f);
                end = start + new Vector2(vector.x * (moveObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().size.x / 2 + 0.1f), 0);
                underHit = Physics2D.Linecast(start, end, layerMask);
                Debug.DrawLine(start, end, Color.red);
                //----------------------------------------------



                //if (hit.transform == null || hit.transform.tag.Equals("Passing") || hit.transform.tag.Equals("PasssFloor"))//아무것도 안걸리거나 passing 태그만 걸림. 이동해도됨
                if (!topHit && !middleHit & !underHit)//움직일수 있는경우
                {
                    float _speed = defaultSpeed * moveObject.GetComponent<CharStat>().speed;

                    if (spiderClimb)
                    {
                        if (Horizontal == 1 || Horizontal == 0)
                            moveObject.transform.Translate(_speed * Vector2.right * Time.deltaTime);
                        else if (Horizontal == -1)
                            moveObject.transform.Translate(_speed * Vector2.left * Time.deltaTime);
                    }
                    else
                    {
                        if (Horizontal == 1 || Horizontal == 0)
                            moveObject.transform.Translate(_speed * Vector2.right * Time.deltaTime);
                        else if (Horizontal == -1)
                            moveObject.transform.Translate(_speed * Vector2.left * Time.deltaTime);
                    }


                }
            }
        }
    }
    public void StopRun()
    {
        vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), moveObject.transform.position.z);
        //----------------------------------------------애니메이션 관리
        moveObject.GetComponent<PlayerAnimeMgr>().RunAnimeStop();
        moveObject.GetComponent<PlayerAnimeMgr>().StopMoveAnime();

        //----------------------------------------------
        moving = false;
        moveObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, moveObject.GetComponent<Rigidbody2D>().velocity.y);
    }

    public void DivideOrCombine()
    {
        if (CanMoveCheck() && !moveObjJumping)
        {
            StopRun();
            if (moveObject.name.Equals("Player"))//분리시켜야됨
            {
                playerAnime.SetTrigger("Decom");//애니메이션 먼저해주고 애니메이션에서 분리 호출
            }
            else if (moveObject.name.Equals("Head") || moveObject.name.Equals("Body") || moveObject.name.Equals("Leg"))//합체시켜야됨
            {
                PlayerDock.instance.Docking();//Player가 없이 때문에 합체호출해서 불러주고 Docking에서 애니메이션 호출
            }
        }
    }

    public void InteractionOn()
    {
        if (CanMoveCheck() && !moveObjJumping)
        {
            if (interactionObj != null)
            {
                //if(!interactionObj.tag.Equals("Door"))//상호작용대상이 문인경우는 제외
                interactionObj.SendMessage("InteractionOn", SendMessageOptions.DontRequireReceiver);
            }
            else if (moveObject.name.Equals("Body"))
                HeadCatch();
        }
    }



    public void StopHeadCatch()//머리잡아던지기 중간에 취소시키기.
    {
        if (headCatch)
        {
            action = false;
            headCatch = false;
            //PlayerDock.instance.Head.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            PlayerDock.instance.head.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            PlayerDock.instance.head.GetComponent<Rigidbody2D>().AddForce(Vector2.down * 0.1f);
            throwRoute.thisObj.SetActive(false);
        }
    }

    public void HeadCatchHitReset()
    {
        if (headCatch)
        {
            action = false;
            headCatch = false;
            //PlayerDock.instance.Head.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            PlayerDock.instance.head.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            PlayerDock.instance.head.GetComponent<Rigidbody2D>().AddForce(Vector2.down * 0.1f);
            throwRoute.thisObj.SetActive(false);
        }
    }

    public void ChangeMoveTarget(String targetName)//조종대상 바꾸기
    {
        StopRun();
        moveObject.GetComponent<AudioListener>().enabled = false;
        moveObject = GameObject.Find(targetName);
        MoveCam.instance.CamTargetChange(targetName);
        moveObject.GetComponent<AudioListener>().enabled = true;
        InteractionTargeting(null);
    }

    public void HittingEnd(GameObject target)//피격해제
    {
        if (target.name.Equals(moveObject.name)) //피격해제대상이 현재 조종중인 오브젝트와 같다면
            moveObjHitting = false; //조종불가용 피격값을 false로 해줘서 조종할수있게해줍시다.
        target.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    public void PlayerTeleport(GameObject destination)
    {
        moveObject.transform.position = destination.transform.position;
    }

    public void PlayerWaitStart(float time)
    {
        StartCoroutine(PlayerWait(time));
    }

    public bool CanMoveCheck()
    {
        if (!moveObjHitting && !action && !DialogueMgr.instance.talking)
            return true;
        else
            return false;
    }

    //private void OnLevelWasLoaded(int level)
    //{
    //    moveObject.transform.position = loadScenePlayerVector;
    //}

    IEnumerator PlayerWait(float stopTime)//일정시간 플레이어 이동막기
    {
        StopRun();
        action = true;
        yield return new WaitForSeconds(stopTime);
        action = false;
    }

    //IEnumerator AttackCoroutine(GameObject _Weapon)
    //{
    //    action = true;
    //    yield return new WaitForSeconds(0.5f);//공격준비시간


    //    Vector3 spawnerVec3 = new Vector3();//공격생성좌표
    //    Quaternion weaponQuaternion = Quaternion.identity;//공격회전값

    //    float moveObjSizeX = moveObject.GetComponent<BoxCollider2D>().size.x / 2 * moveObject.GetComponent<Transform>().localScale.x;
    //    float weaponSizeX = _Weapon.GetComponent<BoxCollider2D>().size.x / 2 * _Weapon.GetComponent<Transform>().localScale.x;
    //    if (!moveObjSpriteRenderer.flipX)//오른쪽을 볼경우
    //    {
    //        weaponQuaternion.eulerAngles = Vector3.zero;
    //        //spawnerVec3 = new Vector3(moveObject.GetComponent<BoxCollider2D>().size.x / 2 + _Weapon.GetComponent<BoxCollider2D>().size.x / 2, 0, 0);
    //        spawnerVec3 = new Vector3(moveObjSizeX + weaponSizeX, 0, 0);

    //        Debug.Log(moveObject.GetComponent<BoxCollider2D>().size.x / 2 + " + " + _Weapon.GetComponent<BoxCollider2D>().size.x / 2);
    //    }
    //    else if (moveObjSpriteRenderer.flipX)//왼쪽볼경우
    //    {
    //        weaponQuaternion.eulerAngles = new Vector3(0, 180, 0);
    //        spawnerVec3 = new Vector3(moveObject.GetComponent<SpriteRenderer>().size.x / 2 * -1, 0, 0);
    //    }
    //    //Instantiate(weapon, moveObject.transform.position, weaponQuaternion);

    //    GameObject _projectile = projectilePool.GetPoolObjectParent(_Weapon.name, moveObject);
    //    _projectile.transform.localPosition = spawnerVec3;
    //    _projectile.transform.rotation = weaponQuaternion;

    //    yield return new WaitForSeconds(1f);//공격후딜레이
    //    action = false;
    //}

    public void ShowThrowRoute()//포탄경로 보여주는 기능. muzzle = 발사구멍. 대포의 각도, 파워를 기반으로 cosin 계산을통해 vec.x y를 기반으로 포탄경로 계산.
    {
        throwRoute.UpdateTrajectory(throwRoute.trajectory.trajectoryRoot.transform.position, throwRoute.GetDirection(angle, power));
    }

    public void HeadCatchingHorizontal()
    {
        float vec = Input.GetAxisRaw("Horizontal") * -1;//각도는 오른쪽이 0도라서 오른쪽이 마이너스 여야함
        float angleValue = angle + throwChangeSpeed * vec * Time.deltaTime;
        if (angleValue > 15 && angleValue < 165)
            angle = angleValue;
    }

    public void HeadCatchingVertical()
    {
        float vec = Input.GetAxisRaw("Vertical");
        float powerValue = power + throwChangeSpeed * vec * Time.deltaTime;
        if (powerValue > 20 && powerValue < 100)
            power = powerValue;
    }


    public bool GetMoveplayerJumpCheck()
    {
        if (moveObject.GetComponent<Animator>().GetFloat("MoveY") == 0)//평지에 있는중
            return false;
        else
            return true;//점프중이면 true 반환
    }

    public void SetChangeSceneSpawnVector(Vector3 _vec)
    {
        loadScenePlayerVector = _vec;
    }

    public void HitReset()
    {
        HeadCatchHitReset();
        squirrelMove.GlidingEnd();
    }
}
