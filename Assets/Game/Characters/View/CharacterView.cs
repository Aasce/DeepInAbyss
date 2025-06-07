using System.Linq;
using TMPro;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CharacterView : CreatureView, IHasOwner<Character>
    {
        #region - FIELDS -
        [SerializeField] protected SO_CharacterView _view;

        [Space]
        [SerializeField] protected Renderer _hatRenderer;
        [SerializeField] protected Renderer _hairRenderer;
        [SerializeField] protected Renderer _hairClippedRenderer;
        [SerializeField] protected Renderer _eyeRenderer;
        [SerializeField] protected Renderer _eyeClosedRenderer;
        [SerializeField] protected Renderer _eyeBaseRenderer;
        [SerializeField] protected Renderer _facewearRenderer;
        [SerializeField] protected Renderer _clothRenderer;
        [SerializeField] protected Renderer _skirtRenderer;
        [SerializeField] protected Renderer _pantsRenderer;
        [SerializeField] protected Renderer _socksRenderer;
        [SerializeField] protected Renderer _shoesRenderer;
        [SerializeField] protected Renderer _shoesFrontRenderer;
        [SerializeField] protected Renderer _backRenderer;
        [SerializeField] protected Renderer _expressionRenderer;
        [SerializeField] protected Renderer _bodyRenderer;

        [Space]
        [SerializeField] protected Transform _rigHead;
        [SerializeField] protected Transform _rigNeck;
        [SerializeField] protected Transform _rigPelvis;
        [SerializeField] protected Transform _rigSpine1;
        [SerializeField] protected Transform _rigSpine2;
        [SerializeField] protected Transform _rigUpperArmL;
        [SerializeField] protected Transform _rigHandL;
        [SerializeField] protected Transform _rigUpperArmR;
        [SerializeField] protected Transform _rigHandR;
        [SerializeField] protected Transform _rigWeapon;

        [Space]
        [SerializeField] protected Transform _weaponSlot;

        [Space]
        [SerializeField] private Vector2 _rootMotionVelocity;

        [Space]
        [SerializeField] private bool _clipHair;
        [SerializeField] private bool _hideHair;
        private MaterialPropertyBlock _mpbHair;
        private Texture _hairRampTexture;

        [Space]
        [SerializeField] private ExpressionType _expression = ExpressionType.Normal;

        [Space]
        [SerializeField] private bool _isEyeCloed;
        [SerializeField] private Vector2 _blinkInterval = new Vector2(0.5f, 5.0f);
        [SerializeField] private float _blinkTimer;

        [SerializeField] private bool _isShoesFrontPants;


        private readonly Vector2 _headRotationRange = new(-15.0f, 20.0f);
        private readonly Vector2 _neckRotationRange = new(-15.0f, 15.0f);
        private readonly Vector2 _spine2RotationRange = new(-15.0f, 15.0f);
        private readonly Vector2 _spine1RotationRange = new(-15.0f, 15.0f);

        private readonly float _neckRotationPercent = 0.5f;
        private readonly float _spine2RotationPercent = 0.3f;
        private readonly float _RotationPercent = 0.3f;
        private readonly float _lookAtTargetLerpSpeed = 7.5f;

        private float _lookAtTargetPercent;

        private Vector3 _spine1Direction;
        private float _targetSpine1Rotation;

        private Vector3 _spine2Direction;
        private float _targetSpine2Rotation;

        private Vector3 _neckDirection;
        private float _targetNeckRotation;

        private Vector3 headDirection;
        private float _targetHeadRotation;

        private Vector3 _rotation;
        private float _power;

        private float _spine1Rotation;
        private float _spine2Rotation;

        // Point at Target
        private readonly float _pointAtTargetLerpSpeed = 7.5f;
        private readonly Vector2 _armRotationRange = new(-80.0f, 70.0f);

        private bool _isPointingAtTarget;

        private float _targetArmRotation;

        private float _pointAtTargetPercent; 
        
        // current move blend, for blending idle, walk, run animation, lerps to target move blend on frame update
        protected float _moveBlend;

        #endregion

        #region - PROPERTIES -
        public new Character Owner
        {
            get => base.Owner as Character;
            set => base.Owner = value;
        }

        public SO_CharacterView View => _view;

        // Rig Transforms
        public Transform RigHead => _rigHead;
        public Transform RigNeck => _rigNeck;
        public Transform RigPelvis => _rigPelvis;
        public Transform RigSpine1 => _rigSpine1;
        public Transform RigSpine2 => _rigSpine2;
        public Transform RigUpperArmL => _rigUpperArmL;
        public Transform RigHandL => _rigHandL;
        public Transform RigUpperArmR => _rigUpperArmR;
        public Transform RigHandR => _rigHandR;
        public Transform RigWeapon => _rigWeapon;

        // Should the look at target also affect the spine rotation
        // Only disabled when crawling
        private bool EnableSpineRot => !IsCrawlAnimation();

        // Hair
        public bool ClipHair
        {
            get => _clipHair;
            set
            {
                _clipHair = value;
                UpdateHairVisibility();
            }
        }
        public bool HideHair
        {
            get => _hideHair;
            set
            {
                _hideHair = value;
                UpdateHairVisibility();
            }
        }

        private MaterialPropertyBlock MPBHair => _mpbHair ??= new MaterialPropertyBlock();
        public Texture HairRampTexture
        {
            get => _hairRampTexture;
            set
            {
                _hairRampTexture = value;
                MPBHair.SetTexture("_RampTex", _hairRampTexture);

                _hairRenderer.SetPropertyBlock(MPBHair);
                _hairClippedRenderer.SetPropertyBlock(MPBHair);
            }
        }


        // Face
        public ExpressionType Expression
        {
            get => _expression;
            set
            {
                _expression = value;
                Animator.SetInteger("Expression", (int)_expression);
            }
        }
        public bool IsEyeCloed
        {
            get { return _isEyeCloed; }
            set
            {
                if (_isEyeCloed == value) return;
                _isEyeCloed = value;

                Animator.SetBool("IsEyeClosed", _isEyeCloed);
            }
        }

        public override bool IsLookingAtTarget
        {
            get => base.IsLookingAtTarget;
            set
            {
                if (_isLookingAtTarget == value) return;
                _isLookingAtTarget = value;

                if (_isLookingAtTarget == true)
                {
                    _targetNeckRotation = RigNeck.localRotation.eulerAngles.z;
                    _targetHeadRotation = RigHead.localRotation.eulerAngles.z;
                }
            }
        }

        public bool IsPointingAtTarget
        {
            get => _isPointingAtTarget;
            set
            {
                if (_isPointingAtTarget == value) return;
                _isPointingAtTarget = value;

                _targetArmRotation = 0.0f;
            }
        }

        // Shoes
        public bool IsShoesFrontPants
        {
            get => _isShoesFrontPants;
            set
            {
                _isShoesFrontPants = value;

                if (_isShoesFrontPants) _shoesFrontRenderer.enabled = true;
                else _shoesFrontRenderer.enabled = false;
            }
        }

        // Move Blend
        public float MoveBlend
        {
            get => _moveBlend;
            protected set => _moveBlend = value;
        }

        #endregion

        #region - UNITY METHODS -

        protected override void Awake()
        {
            base.Awake();
            this.ResetRendererList();
        }

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;

            _blinkTimer = Random.Range(_blinkInterval.x, _blinkInterval.y);
            if (_blinkTimer < 0.1f) _blinkTimer = 0.1f;

            Animator.SetFloat("CycleOffset", Random.value);
        }

        protected override void Update()
        {
            base.Update();
            this.BlinkUpdate();
            this.UpdateMoveBlend();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            this.PointAtTarget();
        }

        #endregion

        #region - METHODS -

        private void BlinkUpdate()
        {
            if (Owner.Status.IsDead) return;

            _blinkTimer -= Time.deltaTime;
            if (_blinkTimer <= 0.0f)
            {
                // Random blink time, but min time is 0.1f
                _blinkTimer = Mathf.Max(0.1f, Random.Range(_blinkInterval.x, _blinkInterval.y));

                if (Expression == ExpressionType.Normal || Expression == ExpressionType.Shy)
                    Animator.SetTrigger("Blink");
            }
        }

        public override void LookAtTarget()
        {
            base .LookAtTarget();
            Vector2 targetPosition = Owner.Action.TargetPosition;

            //check should turn on looking at target or not
            bool shouldLookAtTarget = false;
            if (Owner.Action.IsLooking) shouldLookAtTarget = true;
            if (Owner.Status.IsDead) shouldLookAtTarget = false;

            IsLookingAtTarget = shouldLookAtTarget;

            _lookAtTargetPercent = Mathf.Lerp(_lookAtTargetPercent, _isLookingAtTarget ? 1.0f : 0.0f, _lookAtTargetLerpSpeed * Time.deltaTime);

            if (EnableSpineRot)
            {
                //spine1
                //spine1 also use neck dir
                _spine1Direction = RigNeck.InverseTransformPoint(targetPosition) - RigNeck.localPosition;
                _spine1Direction.z = 0.0f;
                _targetSpine1Rotation = Vector2.Angle(Vector2.right, _spine1Direction) - 90.0f;
                _power = Mathf.Abs(_targetSpine1Rotation / 45.0f);
                _targetSpine1Rotation = Mathf.Clamp(_targetSpine1Rotation, _spine1RotationRange.x, _spine1RotationRange.y);

                _rotation = RigSpine1.localRotation.eulerAngles;
                _spine1Rotation = Mathf.LerpAngle(_rotation.z, _targetSpine1Rotation, _power * _lookAtTargetPercent * _RotationPercent);
                _rotation.z = _spine1Rotation;
                RigSpine1.localRotation = Quaternion.Euler(_rotation);

                //spine2
                //spine2 also use neck dir
                _spine2Direction = RigNeck.InverseTransformPoint(targetPosition) - RigNeck.localPosition;
                _spine2Direction.z = 0.0f;
                _targetSpine2Rotation = Vector2.Angle(Vector2.right, _spine2Direction) - 90.0f;
                _power = Mathf.Abs(_targetSpine2Rotation / 45.0f);
                _targetSpine2Rotation = Mathf.Clamp(_targetSpine2Rotation, _spine2RotationRange.x, _spine2RotationRange.y);

                _rotation = RigSpine2.localRotation.eulerAngles;
                _spine2Rotation = Mathf.LerpAngle(_rotation.z, _targetSpine2Rotation, _power * _lookAtTargetPercent * _spine2RotationPercent);
                _rotation.z = _spine2Rotation;
                RigSpine2.localRotation = Quaternion.Euler(_rotation);
            }

            //neck
            _neckDirection = RigNeck.InverseTransformPoint(targetPosition) - RigNeck.localPosition;
            _neckDirection.z = 0.0f;
            _targetNeckRotation = Vector2.Angle(Vector2.right, _neckDirection) - 90.0f;
            _targetNeckRotation = Mathf.Clamp(_targetNeckRotation, _neckRotationRange.x, _neckRotationRange.y);

            _rotation = RigNeck.localRotation.eulerAngles;
            _rotation.z = Mathf.LerpAngle(_rotation.z, _targetNeckRotation, _lookAtTargetPercent * _neckRotationPercent);
            RigNeck.localRotation = Quaternion.Euler(_rotation);

            //head
            headDirection = RigHead.InverseTransformPoint(targetPosition) - RigHead.localPosition;
            headDirection.z = 0.0f;
            _targetHeadRotation = Vector2.Angle(Vector2.right, headDirection) - 90.0f;
            _targetHeadRotation = Mathf.Clamp(_targetHeadRotation, _headRotationRange.x, _headRotationRange.y);

            _rotation = RigHead.localRotation.eulerAngles;
            _rotation.z = Mathf.LerpAngle(_rotation.z, _targetHeadRotation, _lookAtTargetPercent);
            RigHead.localRotation = Quaternion.Euler(_rotation);
        }
        public virtual void PointAtTarget()
        {
            bool shouldPointAtTarget = false;
            if (Owner.Action.IsCrawling) shouldPointAtTarget = false;
            if (Owner.Action.IsDodging) shouldPointAtTarget = false;
            if (Owner.Status.IsDead) shouldPointAtTarget = false;

            float attackSpeedMul = 1.0f;
            IsPointingAtTarget = shouldPointAtTarget;

            _pointAtTargetPercent = Mathf.Lerp(_pointAtTargetPercent, IsPointingAtTarget ? 1.0f : 0.0f, _pointAtTargetLerpSpeed * Time.deltaTime * attackSpeedMul);

            Vector2 pointAtTargetDirection = (Owner.Action.TargetPosition - (Vector2)RigUpperArmR.position).normalized;
            _targetArmRotation = Vector2.Angle(Vector2.up, pointAtTargetDirection) - 90.0f;

            _targetArmRotation = Mathf.Clamp(_targetArmRotation, _armRotationRange.x, _armRotationRange.y);
            _targetArmRotation = _targetArmRotation + _spine1Rotation + _spine2Rotation + RigPelvis.transform.localRotation.eulerAngles.z;

            _rotation = RigUpperArmR.rotation.eulerAngles;
            _rotation.z += Mathf.LerpAngle(0.0f, _targetArmRotation, _pointAtTargetPercent);
            RigUpperArmR.rotation = Quaternion.Euler(_rotation);
        }

        public bool IsCrawlAnimation() => Animator.GetBool("IsCrawling");
        public void InjuredFront() => Animator.SetTrigger("InjuredFront");
        public void InjuredBack() => Animator.SetTrigger("InjuredBack");
        
        public void SetIsDodgingAnimation(bool state, int dir)
        {
            Animator.SetBool("IsDodging", state);
            Animator.SetInteger("DodgeDir", dir);
        }
        public void TriggerGetDownPlatformAnimation() => Animator.SetTrigger("GetDownPlatform");
        public void SetIsGroundedAnimation(bool state) => Animator.SetBool("IsGrounded", state);
        public void SetIsCrouchingAnimation(bool state) => Animator.SetBool("IsCrouching", state);
        public void SetIsCrawlingAnimation(bool state) => Animator.SetBool("IsCrawling", state);
        public void SetCrawlSpeedMultiplyAnimation(float value) => Animator.SetFloat("CrawlSpeedMul", value);

        public void SetIsMovingAnimation(bool state) => Animator.SetBool("IsMoving", state);
        public void SetIsRunningAnimation(bool state) => Animator.SetBool("IsRunning", state);
        public void SetIsDashingAnimation(bool state) => Animator.SetBool("IsDashing", state);
        public void SetVelocityAnimation(Vector2 value)
        {
            Animator.SetFloat("VelocityX", Mathf.Abs(value.x));
            Animator.SetFloat("VelocityY", value.y);
        }
        public void SetMoveDirectionAnimation(float value) => Animator.SetFloat("MoveDir", value);
        public void SetMoveBlendAnimation(float value) => Animator.SetFloat("MoveBlend", value);

        public void SetIsClimbingLadderAnimation(bool state) => Animator.SetBool("IsClimbingLadder", state);
        public void SetClimbingSpeedMultiplyAnimation(float value) => Animator.SetFloat("ClimbingSpeedMul", value);
        public void SetIsClimbingLedgeAnimation(bool state) => Animator.SetBool("IsClimbingLedge", state);
        public void SetLedgeHeightAnimation(float value) => Animator.SetFloat("LedgeHeight", value);

        public void SetIsEnteringLadderAnimation(bool state) => Animator.SetBool("IsEnteringLadder", state);
        public void SetIsExitingLadderAnimation(bool state) => Animator.SetBool("IsExitingLadder", state);
        public void SetLadderEnterHeightAnimation(float value) => Animator.SetFloat("LadderEnterHeight", value);
        public void SetLadderExitHeightAnimation(float value) => Animator.SetFloat("LadderExitHeight", value);

        protected override void UpdateAnimator()
        {
            if (Owner.Status.IsDead) return;
            base.UpdateAnimator();

            SetVelocityAnimation(Owner.PhysicController.currentVelocity);
            SetIsMovingAnimation(Owner.Action.IsMoving);
            SetIsRunningAnimation(Owner.Action.IsRunning);
            SetIsDashingAnimation(Owner.Action.IsDashing);
            SetMoveBlendAnimation(MoveBlend);
            SetMoveDirectionAnimation(Owner.Action.HorizontalMoveDirection);

            SetIsGroundedAnimation(Owner.PhysicController.IsGrounded);
            SetIsCrouchingAnimation(Owner.Action.IsCrouching);
            SetIsCrawlingAnimation(Owner.Action.IsCrawling);
            SetIsClimbingLadderAnimation(Owner.Action.IsClimbingLadder);
            SetClimbingSpeedMultiplyAnimation(Owner.Action.ClimbingSpeedMultiply);

            SetIsClimbingLedgeAnimation(Owner.Action.IsClimbingLedge);
            if (Owner.Action.IsClimbingLedge) SetLedgeHeightAnimation(Owner.Action.LedgeHeight);

            SetIsEnteringLadderAnimation(Owner.Action.IsEnteringLadder);
            if (Owner.Action.IsEnteringLadder) SetLadderEnterHeightAnimation(Owner.Action.LadderEnterHeight);

            SetIsExitingLadderAnimation(Owner.Action.IsExitingLadder);
            if (Owner.Action.IsExitingLadder) SetLadderExitHeightAnimation(Owner.Action.LadderExitHeight);

        }
        private void UpdateHairVisibility()
        {
            if (_hairRenderer == null) return;
            if (_hairClippedRenderer == null) return;

            if (HideHair)
            {
                _hairRenderer.enabled = false;
                _hairClippedRenderer.enabled = false;
            }
            else
            {
                _hairRenderer.enabled = !_clipHair;
                _hairClippedRenderer.enabled = _clipHair;
            }
        }
        protected virtual void UpdateMoveBlend()
        {
            float targetMoveBlend = 0.0f;
            if (Owner.Action.IsMoving)
            {
                if (Owner.Action.IsRunning) targetMoveBlend = 3.0f;
                else targetMoveBlend = 1.0f;
            }

            MoveBlend = Mathf.Lerp(MoveBlend, targetMoveBlend, 7.0f * Time.deltaTime);
        }
        
        protected override void ResetRendererList()
        {
            base.ResetRendererList();
            Renderers.AddRange(new[]
            {
                _hatRenderer,
                _hairRenderer,
                _hairClippedRenderer,
                _eyeRenderer,
                _eyeClosedRenderer,
                _eyeBaseRenderer,
                _facewearRenderer,
                _clothRenderer,
                _skirtRenderer,
                _pantsRenderer,
                _socksRenderer,
                _shoesRenderer,
                _shoesFrontRenderer,
                _backRenderer,
                _expressionRenderer,
                _bodyRenderer
            }.Where(renderer => renderer != null));
        }

        protected override void Status_OnFacingChanged(object sender, FacingType facing)
        {
            base.Status_OnFacingChanged(sender, facing);
            if (_weaponSlot != null) _weaponSlot.transform.localScale = new Vector3(1.0f, 1.0f, (int)facing);
        }

        #endregion
    }
}