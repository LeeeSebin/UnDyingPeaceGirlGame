using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloorCheckT : MonoBehaviour
{
    private GameObject parent;
    private bool onFloor;//바닥에 붙어있는지 체크하기

    private void Awake()
    {
        parent = transform.parent.gameObject;
        onFloor = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Floor") || collision.gameObject.tag.Equals("MoveFloor") || collision.gameObject.tag.Equals("PassFloor") || collision.gameObject.tag.Equals("Swtich") || collision.gameObject.tag.Equals("Enemy"))
        {
            PlayerMove.instance.HittingEnd(transform.parent.gameObject);//땅에 닿을때마다 일단 피격판정을 무조건 확인해봅시다.
            transform.parent.GetComponent<PlayerAnimeMgr>().LandingAnime();
        }
    }
    void OnTriggerStay2D(Collider2D trigger)
    {
        if (PlayerMove.instance.moveObjHitting)
        {
            if (trigger.gameObject.tag.Equals("Floor") || trigger.gameObject.tag.Equals("MoveFloor") || trigger.gameObject.tag.Equals("PassFloor") || trigger.gameObject.tag.Equals("Swtich") || trigger.gameObject.tag.Equals("Enemy"))
            {
                PlayerMove.instance.HittingEnd(transform.parent.gameObject);//땅에 닿을때마다 일단 피격판정을 무조건 확인해봅시다.
                transform.parent.GetComponent<PlayerAnimeMgr>().LandingAnime();
            }
        }

        //바닥에 닿으면 점프 다시하게 해주는용도. 각 플레이어 오브젝트별로 가지고있음
        if (!onFloor)
        {
            if (trigger.gameObject.tag.Equals("Floor") || trigger.gameObject.tag.Equals("MoveFloor") || trigger.gameObject.tag.Equals("PassFloor") || trigger.gameObject.tag.Equals("Swtich") || trigger.gameObject.tag.Equals("Enemy"))
            {
                //여러 오브젝트가 공용으로 쓰이기에 오브젝트별로 바닥에 닿을시 다른 작업이 들어갈경우 추가해줘야함
                if (parent.name.Equals("Spider") && PlayerMove.instance.playerJumping)//거미일경우
                {
                    PlayerMove.instance.PlayerLanding(transform.parent.gameObject.name);
                    parent.GetComponent<PlayerAnimeMgr>().LandingAnime();
                    if(PlayerMove.instance.spiderClimb && PlayerMove.instance.action)//천장타기일경우 다시 움직일수있게 해주기.
                    {
                        PlayerMove.instance.action = false;
                    }
                }
                else//점프판정과 애니메이션만 바꿔주는 공용
                {
                    PlayerMove.instance.PlayerLanding(transform.parent.gameObject.name);
                    parent.GetComponent<PlayerAnimeMgr>().LandingAnime();
                }

            }
            parent.GetComponent<PlayerAnimeMgr>().OnFloor();
            onFloor = true;
        }
    }

    private void OnTriggerExit2D(Collider2D trigger)
    {
        //if (trigger.gameObject.tag.Equals("Floor"))

        if (onFloor)
        {
            parent.GetComponent<PlayerAnimeMgr>().NoFloor();
            onFloor = false;
        }
    }
}
