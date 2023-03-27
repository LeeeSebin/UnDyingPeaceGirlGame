using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputMgr : MonoBehaviour
{
    private PlayerMove moveMgr;
    private PlayerDock dockMgr;
    public static PlayerInputMgr instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        moveMgr = GetComponent<PlayerMove>();
        dockMgr = GetComponent<PlayerDock>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Horizontal"))
        {
            moveMgr.Run(Input.GetAxisRaw("Horizontal"));
        }
        if(Input.GetButtonDown("Horizontal"))
        {
            moveMgr.HorizontalKeyDownEvent();
        }
        if (Input.GetButtonDown("Vertical"))
        {
            moveMgr.VerticalKeyDownEvent();
        }

        if (Input.GetKeyDown(KeyCode.Insert))//ī�޶� ȿ�� �׽�Ʈ
        {
            MoveCam.instance.CameraShake(3, 0.3f);
        }
        if (Input.GetButtonDown("Jump"))
        {
            moveMgr.UpkeyEvent();
        }
        if (Input.GetButton("Jump"))
        {
            moveMgr.UpKeyPressingEvent();
        }
        if (Input.GetButtonUp("Jump"))
        {
            moveMgr.UpKeyUpEvent();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveMgr.DownKeyEvent();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))//�и�,��ü ���
        {
            moveMgr.DivideKeyEvent();
        }

        //--------------------------------------------------------------------���ݰ���
        //if (Input.GetKeyDown(KeyCode.X))
        if (Input.GetButtonDown("Attack"))
        {
            moveMgr.NormalAttackEvent();
        }
        //---------------------------------------------------------
        if (Input.GetKeyDown(KeyCode.Q))//����Ű
        {
            moveMgr.TransformKeyEvent();
        }


        //---------------------------------------------------------

        if (Input.GetButtonDown("Interaction"))
        {
            moveMgr.InteractionOn();
        }

        //------------------------------------------�������� ����
        if (!moveMgr.legJumping && !moveMgr.headJumping && moveMgr.bodyJumping && !PlayerDock.instance.combine && moveMgr.CanMoveCheck())//��ü���������� �����ʿ�
        {
            if (!PlayerDock.instance.combine)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    this.gameObject.GetComponent<PlayerMove>().ChangeMoveTarget("Head");
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    this.gameObject.GetComponent<PlayerMove>().ChangeMoveTarget("Body");
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    this.gameObject.GetComponent<PlayerMove>().ChangeMoveTarget("Leg");
            }
        }

        //--------------------------------------------------------------------������ ����

        if (moveMgr.action && moveMgr.headCatch && !DialogueMgr.instance.talking)
        {
            if (Input.GetButton("Horizontal"))
            {
                moveMgr.HeadCatchingHorizontal();
            }
            if (Input.GetButton("Vertical"))
            {
                moveMgr.HeadCatchingVertical();
            }
            if (Input.GetButtonUp("Interaction"))//������
            {
                moveMgr.HeadThrow();
            }
            if (Input.GetKeyDown(KeyCode.X))
                moveMgr.StopHeadCatch();
        }

        //----------------------------------------------------------------------------------------
        //�׽�Ʈ��
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
        if (Input.GetKeyDown(KeyCode.X))
            moveMgr.StopHeadCatch();
        //-----------------------------------------------

        //�÷��̾���� ���üũ��--------------------------------------------------------------------------------------------

        if (!moveMgr.moveObjHitting)
        {
            if (Input.GetButtonUp("Horizontal"))
            {
                moveMgr.StopRun();
            }
        }
    }
}
