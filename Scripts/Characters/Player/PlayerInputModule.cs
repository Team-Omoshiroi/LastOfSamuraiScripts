using UnityEngine;

namespace Characters.Player
{
    public class PlayerInputModule : MonoBehaviour
    {
        private PlayerInputActions InputActions { get; set; }
        public PlayerInputActions.PlayerActions PlayerActions { get; private set; }
        
        private void Awake()
        {
            InputActions = new PlayerInputActions();

            PlayerActions = InputActions.Player;
        }
        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }

    }
}