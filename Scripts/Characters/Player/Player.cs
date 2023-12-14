using Characters.Player.StateMachines;
using Characters.Player.StateMachines.TargetLock;
using Cinemachine;
using ScriptableObjects.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Player
{
    public class Player : MonoBehaviour
    {
        [field: Header("References")]
        [field: SerializeField] public PlayerSo Data { get; private set; }
    
        [field: Header("Animations")]
        [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }
        
        [field: Header("Weapon")]
        [field: SerializeField] public Weapon Weapon { get; set; }

        
        [field: Header("Else")]
        public Rigidbody Rigidbody { get; private set; }
        public Animator Animator { get; private set; }
        public PlayerInputModule InputModule { get; private set; }
        public ForceReceiver ForceReceiver { get; private set; }
        public CharacterController Controller { get; private set; }
        public TargetLockModule TargetLockModule { get; private set; }
        public CameraFollowModule CameraFollowModule { get; private set; }
        public EffectModule EffectModule { get; private set; }
        public PlayerStateMachine stateMachine;

        public ItemPickUp PickUP ;
        // 삭제 예정
        public TextMeshProUGUI currentState;
        public TextMeshProUGUI moveInput;
        public TextMeshProUGUI moveDir;
        public TextMeshProUGUI lookInput;
        public TextMeshProUGUI lookDir;
        
        private void Awake()
        {
            AnimationData.Initialize();
            Animator = GetComponent<Animator>();
            InputModule = GetComponent<PlayerInputModule>();
            Controller = GetComponent<CharacterController>();
            ForceReceiver = GetComponent<ForceReceiver>();
            PickUP = GetComponent<ItemPickUp>();
            EffectModule = GetComponent<EffectModule>();
            
            stateMachine = new PlayerStateMachine(this);

            TargetLockModule = GetComponent<TargetLockModule>();
        }

        private void Start()
        {
           // Cursor.lockState = CursorLockMode.Locked;
            stateMachine.ChangeState(stateMachine.TargetUnlockIdleState);
        }

        private void Update()
        {
            stateMachine.HandleInput();
            stateMachine.Update();

            //currentState.text = $"Current State : {stateMachine.currentState}";
            //moveInput.text = $"MoveInput | {stateMachine.MoveInput}";
            //moveDir.text = $"MoveDir | {stateMachine.MoveDirection}";
            //lookInput.text = $"LookInput {stateMachine.LookInput}";
            //lookDir.text = $"LookDir | {stateMachine.LookDirection}";
        }

        private void FixedUpdate()
        {
            stateMachine.PhysicsUpdate();
        }

        private void LateUpdate()
        {
            stateMachine.LateUpdate();
        }
    }
}