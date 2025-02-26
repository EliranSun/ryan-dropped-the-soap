using System;
using System.Linq;
using UnityEngine;

namespace Ryan.Scripts
{
    public enum StateName
    {
        Dressed,
        Naked,
        Showering,
        Drowning,
        Dead
    }

    [Serializable]
    public class States
    {
        public StateName name;
        public Sprite spriteRenderer;
        public float gravity;
        public bool isControlledByPlayer;
    }

    [RequireComponent(typeof(Renderer))]
    public class PlayerChangeState : MonoBehaviour
    {
        [SerializeField] private GameObject defaultColliderObject;
        [SerializeField] private GameObject slipperyColliderObject;
        [SerializeField] private GameObject clothing;
        [SerializeField] public StateName currentState;
        [SerializeField] private bool isSlipperyShower;
        [SerializeField] private States[] states;
        [SerializeField] private float drownYOffset = 1;
        private int _activeStateIndex;
        private States[] _controlledByPlayerStates;

        private bool _isDead;
        private bool _isInWater;
        private Renderer _renderer;

        // private bool _isRoomWaterFilled;
        // private bool _isShowerWaterFilled;
        private SpriteRenderer _spriteRenderer;

        public static PlayerChangeState Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _controlledByPlayerStates = states.Where(state => state.isControlledByPlayer).ToArray();
            ChangePlayerState(currentState);
        }

        private void Update()
        {
            if (_isDead)
                return;

            // if (GameState.WaterFilledRoom && currentState != StateName.Drowning) {
            //     ChangePlayerState(StateName.Drowning);
            //     return;
            // }
            //
            // if (GameState.WaterFilledShower && GameState.IsPlayerInShower) {
            //     ChangePlayerState(StateName.Drowning);
            //     Invoke(nameof(PlayerAvoidableDeath), 5);
            //     return;
            // }
            //
            // if ((!GameState.WaterFilledRoom && !GameState.IsPlayerInShower && currentState == StateName.Drowning) ||
            //     (!GameState.WaterFilledShower && GameState.IsPlayerInShower && currentState == StateName.Drowning))
            //     ChangePlayerState(StateName.Naked);

            //
            // if (
            //     WaterLevel.CurrentWaterLevel >= transform.position.y + drownYOffset &&
            //     currentState != StateName.Drowning &&
            //     _isInWater
            // ) {
            //     ChangePlayerState(StateName.Drowning);
            //     Invoke(nameof(PlayerAvoidableDeath), 5);
            // }
            //
            // if (WaterLevel.CurrentWaterLevel < transform.position.y + drownYOffset &&
            //     currentState == StateName.Drowning) {
            //     ChangePlayerState(StateName.Naked);
            //     CancelInvoke(nameof(PlayerAvoidableDeath));
            // }
        }

        public void OnMouseDown()
        {
            if (CursorManager.Instance.IsActionCursor ||
                currentState == StateName.Drowning ||
                currentState == StateName.Dead)
                return;

            _activeStateIndex = _activeStateIndex + 1 > _controlledByPlayerStates.Length - 1
                ? _activeStateIndex = 0
                : _activeStateIndex + 1;

            var nextState = _controlledByPlayerStates[_activeStateIndex];
            ChangePlayerState(nextState.name);
        }

        public void ChangePlayerState(StateName newState)
        {
            if (newState == StateName.Naked)
                clothing.gameObject.SetActive(true);

            if (newState == StateName.Dressed)
                clothing.gameObject.SetActive(false);

            _spriteRenderer.sprite = states.First(state => state.name == newState).spriteRenderer;

            var isSlippery = isSlipperyShower && GameState.IsPlayerInShower;
            defaultColliderObject.SetActive(!isSlippery);
            slipperyColliderObject.SetActive(isSlippery);

            currentState = newState;
        }

        public void OnNotify(GameEventData eventData)
        {
            switch (eventData.Name)
            {
                case GameEvents.HitByConcrete:
                    HandleDeath();
                    break;

                // case GameEvents.InShower: {
                //     if (currentState == StateName.Dressed && _isShowerWaterFilled)
                //         ChangePlayerState(StateName.Drowning);
                //     break;
                // }

                case GameEvents.LevelLost:
                    HandleDeath();
                    break;
            }
        }

        private void PlayerAvoidableDeath()
        {
            // if (!GameState.IsPlayerInShower && !GameState.WaterFilledRoom) {
            //     print("Player outside shower and room is not filled with water");
            //     return;
            // }
            //
            // if (GameState.IsPlayerInShower && !GameState.WaterFilledShower) {
            //     print("Player inside shower and shower is not filled with water");
            //     return;
            // }

            // if (WaterLevel.CurrentWaterLevel >= transform.position.y + 1)
            //     HandleDeath();
        }

        private void HandleDeath()
        {
            ChangePlayerState(StateName.Dead);
            _isDead = true;
            Invoke(nameof(NotifyDeath), 5);
        }

        private void NotifyDeath()
        {
            EventManager.Instance.Publish(GameEvents.Dead);
        }
    }
}