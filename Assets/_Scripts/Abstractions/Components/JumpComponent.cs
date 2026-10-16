using System;
using UnityEngine;
using UnityEngine.Events;

namespace Abstractions.Components
{
    public abstract class JumpComponent : MonoBehaviour
    {
        [Header("Ground check")]
        [SerializeField] private LayerMask _groundMask = -1;
        [SerializeField] private Vector2 _groundCheckOffset;
        [SerializeField] private Vector2 _groundCheckSize;

        [Header("Jump base")]
        [SerializeField] protected float JumpHeight = 4f;
        [SerializeField] protected float JumpTimeToApex = 0.4f;
        [SerializeField] protected int AdditionalJumps = 0;
        [SerializeField] protected bool CanJumpByStart = true;

        [Header("Gravity modifiers")]
        [SerializeField] protected float FallGravityMultiplier = 1.5f;
        [SerializeField] protected float MaxFallSpeed = 25f;
        [SerializeField] protected float JumpCutGravity = 2.5f;
        [Range(0f, 1f)] [SerializeField] protected float JumpHangGravityMultiplier = 0.5f;
        [SerializeField] protected float JumpHangTimeThreshold = 1f;

        [Header("Input assists")]
        [Range(0.01f, 0.5f)] [SerializeField] protected float CoyoteTime = 0.2f;
        [Range(0.01f, 0.5f)] [SerializeField] protected float JumpInputBufferTime = 0.2f;

        public float GravityStrength => -(2f * JumpHeight) / (JumpTimeToApex * JumpTimeToApex);
        public float GravityScale => GravityStrength / Physics2D.gravity.y;
        public float JumpForce => Mathf.Abs(GravityStrength) * JumpTimeToApex;

        public bool InputRequest { get; protected set; }
        public bool HandleLongJumps { get; protected set; }
        public bool IsActiveCoyoteTime { get; protected set; }
        public bool CanJump { get; protected set; }

        protected int AdditionalJumpsAvailable;

        private float _coyoteTimer;
        private float _inputBufferTimer;
        private bool _wasGrounded;
        private bool _hasJumpedThisAirTime;

        public UnityAction JumpStarted;
        public UnityAction Landed;

        protected virtual void Awake()
        {
            AdditionalJumpsAvailable = AdditionalJumps;

            if (CanJumpByStart)
                EnableJumping();
        }

        protected virtual void Update()
        {
            UpdateTimers(Time.deltaTime);
            HandleGroundState();
        }

        protected virtual void FixedUpdate()
        {
            TryJump();
        }

        public void OnJumpPressed()
        {
            InputRequest = true;
            HandleLongJumps = true;
            _inputBufferTimer = JumpInputBufferTime;
        }

        public void OnJumpReleased()
        {
            HandleLongJumps = false;
        }

        public virtual void EnableJumping()
        {
            CanJump = true;
        }

        public virtual void DisableJumping()
        {
            CanJump = false;
        }

        private void UpdateTimers(float delta)
        {
            if (_inputBufferTimer > 0)
                _inputBufferTimer -= delta;
            else
                InputRequest = false;

            if (_coyoteTimer > 0)
                _coyoteTimer -= delta;
        }

        private void HandleGroundState()
        {
            bool grounded = IsGrounded();

            if (grounded)
            {
                if (!_wasGrounded)
                    Landed?.Invoke();

                AdditionalJumpsAvailable = AdditionalJumps;
                _coyoteTimer = CoyoteTime;
                _hasJumpedThisAirTime = false;
            }

            IsActiveCoyoteTime = _coyoteTimer > 0;
            _wasGrounded = grounded;
        }

        private void TryJump()
        {
            if (!InputRequest)
                return;

            if (!CanJump)
                return;

            bool canUseCoyote = IsActiveCoyoteTime && !_hasJumpedThisAirTime;

            if (IsGrounded() || canUseCoyote || AdditionalJumpsAvailable > 0)
            {
                PerformJump();
                InputRequest = false;
                _hasJumpedThisAirTime = true;

                if (!IsGrounded() && !canUseCoyote)
                    AdditionalJumpsAvailable--;
            }
        }

        protected abstract void PerformJump();

        protected void InvokeJumpStarted() => JumpStarted?.Invoke();

        public bool IsGrounded()
        {
            Vector2 origin = (Vector2)transform.position + _groundCheckOffset;
            return Physics2D.OverlapBox(origin, _groundCheckSize, 0f, _groundMask);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector2 origin = (Vector2)transform.position + _groundCheckOffset;
            Gizmos.DrawWireCube(origin, _groundCheckSize);
        }
#endif
    }
}