using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using FSM;

namespace CharacterScripts
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private FSMSystem fsm;
        private float battleRadius;
        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
            battleRadius = 1;
            MakeFSM();
        }

        private void MakeFSM()
        {
            BattleState battle = new BattleState();
            battle.AddTransition(Transition.TransitionToNormalState, StateID.NormalStateID);
            NormalState normal = new NormalState();
            normal.AddTransition(Transition.TransitionToBattleState, StateID.BattleStateID);

            fsm = new FSMSystem();
            fsm.AddState(battle);
            fsm.AddState(normal);

            SetTransition(Transition.TransitionToNormalState);

        }

        public float BRadius
        {
            get { return battleRadius; }
        }
        public void SetTransition(Transition t)
        {
            fsm.PerformTransition(t);
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyGroup");
            for (int x = 0; x < enemies.Length; x++)
            {
                Group currentGroup = enemies[x].GetComponent<Group>();
                fsm.CurrentState.Reason(this.gameObject, currentGroup.Leader);
                fsm.CurrentState.Act(this.gameObject, currentGroup.Leader);
            }

            // read inputs
            float h, v;
            bool crouch;
            if(fsm.CurrentStateID == StateID.BattleStateID)
            {
                h = 0; v = 0; crouch = false;
            }
            else
            { 
                h = CrossPlatformInputManager.GetAxis("Horizontal");
                v = CrossPlatformInputManager.GetAxis("Vertical");
                crouch = Input.GetKey(KeyCode.C);
            }
            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v*Vector3.forward + h*Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }
    }
}
