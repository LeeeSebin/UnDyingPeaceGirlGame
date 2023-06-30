using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class PlayerAnimeMgr : MonoBehaviour
    {
        private Animator thisAni;
        public PlayerSpineState state;
        //public Animator SpineAni;
        // Start is called before the first frame update



        void Awake()
        {
            thisAni = this.gameObject.GetComponent<Animator>();
            //SpineAni = this.transform.Find("Spine").gameObject.GetComponent<Animator>();

        }

        private void OnEnable()
        {
            thisAni.SetTrigger("ResetTrigger");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void RunAnimeStart()
        {
            thisAni.SetBool("Run", true);
            thisAni.SetFloat("MoveX", 1);
            //SpineAni.SetFloat("MoveX", 1);
        }

        public void RunAnimeStop()
        {
            thisAni.SetBool("Run", false);
            thisAni.SetFloat("MoveX", 0);
            //SpineAni.SetFloat("MoveX", 0);

        }

        public void JumpAnimeStart()
        {
            thisAni.SetTrigger("JumpStartTrigger");
            thisAni.SetBool("Jump", true);
            state = PlayerSpineState.Jumping;
        }

        //-------------------------------------피격관련
        public void HitAnimeStart()
        {
            thisAni.SetTrigger("Hit");
        }

        public void HitAnimeStop()
        {
            thisAni.SetBool("Hit", false);
        }

        public void DestroyAnime()
        {
            thisAni.SetBool("Destroy", true);
        }
        //--------------------------------------
        //-----------------------------------점프와 주관련된
        public void FallingAnimeStart(float speed)
        {
            thisAni.SetFloat("MoveY", -1);
            thisAni.SetBool("Jump", true);
        }

        public void UppingAnimeStart()
        {
            thisAni.SetFloat("MoveY", 1);
            thisAni.SetBool("Jump", true);
        }

        
        public void OnFloor()
        {
            thisAni.SetBool("FloorCheck", true);
        }
        public void NoFloor()
        {
            thisAni.SetBool("FloorCheck", false);
        }

        public void SuperRobotTransformAnimeStart()
        {
            thisAni.SetTrigger("SuperRobotTransformTrigger");
        }

        public void SetBoolAttackEnd()//공격모션이 100%완주됬을때 애니메이션에서 호출해서 끝내는 용도
        {
            thisAni.SetBool("Attacking", false);
        }

        public void SetBoolAttackStart()
        {
            thisAni.SetBool("Attacking", true);
        }

        public void AttackEnd()
        {
            thisAni.SetBool("Attacking", false);
            GetComponent<PlayerMgr>().PlayerCanMove();
        }

        public void AttackCancel()
        {
            thisAni.SetBool("Attacking", false);
            thisAni.SetTrigger("AttackEnd");
            GetComponent<PlayerMgr>().PlayerCanMove();
        }

        public bool AttackCheck()
        {
            return thisAni.GetBool("Attacking");
        }

        //----------------------------------스파인 관련 제작중

        public void TryMoveAnime(float speed)
        {
            if (state != PlayerSpineState.Jumping)
            {
                state = (speed == 0) ? PlayerSpineState.Idle : PlayerSpineState.Running;
            }
            //else if (state == PlayerSpineState.Jumping)
            //{
            //    state = PlayerSpineState.Jumping;
            //}
        }

        public void LandingAnime()
        {
            thisAni.SetBool("Jump", false);
            thisAni.SetFloat("MoveY", 0);
            StopMoveAnime();
        }

        public void StopMoveAnime()
        {
            state = PlayerSpineState.Idle;
        }
    }
    public enum PlayerSpineState
    {
        Idle,
        Running,
        Jumping
    }