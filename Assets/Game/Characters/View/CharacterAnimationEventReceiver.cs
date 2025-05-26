using UnityEngine;

namespace Asce.Game.Entities
{
    public class CharacterAnimationEventReceiver : MonoBehaviour, IHasOwner<Character>
    {
        [SerializeField] private Character _owner;

        public Character Owner 
        { 
            get => _owner; 
            set => _owner = value; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        ///     Called by <see cref="Animator" /> when the animation event is triggered.
        /// </remarks>
        /// <param name="evt"></param>
        public void OnFootstep(AnimationEvent evt)
        {
            Owner.PhysicController.OnFootstep(evt);
        }


        #region - ATTACK EVENTS

        //the animatorStateInfo.speed here is used to distinguish
        //Attack Action - Arm - speed: 1.0
        //Attack Action - Body - speed: 0.98
        //events from the body layer is not send to the controller to prevent it form being fired twice

        //when attack action starts
        public void OnAttackStart(AnimationEvent evt)
        {
            if (evt.animatorStateInfo.speed < 0.99f) return;

            // controller.AttackStart();
        }

        //when the attack action is supposed to hit the target
        //for continuous attack action like [Point] and [Summon], this event is fired at the moment the attack animtion enters a looping state
        public void OnAttackHit(AnimationEvent evt)
        {
            if (evt.animatorStateInfo.speed < 0.99f) return;

            // controller.AttackHit();
        }

        //when the attack action is supposed to cast a projectile
        //used only in [Cast] attack action
        public void OnAttackCast(AnimationEvent evt)
        {
            if (evt.animatorStateInfo.speed < 0.99f) return;

            // controller.AttackCast();
        }

        //when the attack action ends
        public void OnAttackEnd(AnimationEvent evt)
        {
            if (evt.animatorStateInfo.speed < 0.99f) return;

            // controller.AttackEnd();
        }

        //when the attack action is supposed to throw out the holding weapon
        //used only in [Throw] attack action
        public void OnThrow()
        {
            // controller.Throw();
        }

        #endregion

        #region - ARCHERY EVENTS -

        public void OnArrowDraw()
        {
            // if (controller) controller.ArrowDraw();
        }

        public void OnArrowNock()
        {
            // if (controller) controller.ArrowNock();
        }

        public void OnArrowReady()
        {
            // if (controller) controller.ArrowReady();
        }

        public void OnArrowPutBack()
        {
            // if (controller) controller.ArrowPutBack();
        }

        #endregion

        #region - LEDGE CLIMB EVENTS - 


        /// <summary>
        ///     When ledge climb animation passes the time this event defines
        ///     it cannot be cancelled by releasing forward key
        /// </summary>
        /// <remarks>
        ///     Called by <see cref="Animator" /> when the animation event is triggered.
        /// </remarks>
        public void OnLedgeClimbLocked()
        {
            if (Owner) Owner.Action.OnLedgeClimbLocked();
        }

        /// <summary>
        /// 
        /// </summary
        /// <remarks>
        ///     Called by <see cref="Animator" /> when the animation event is triggered.
        /// </remarks>
        public void OnLedgeClimbFinised()
        {
            if (Owner) Owner.Action.OnLedgeClimbFinised();
        }

        #endregion

        #region - LADDER CLIMB EVENTS -

        /// <summary>
        /// 
        /// </summary
        /// <remarks>
        ///     Called by <see cref="Animator" /> when the animation event is triggered.
        /// </remarks>
        public void OnLadderEntered()
        {
            if (Owner) Owner.Action.OnLadderEntered();
        }

        /// <summary>
        /// 
        /// </summary
        /// <remarks>
        ///     Called by <see cref="Animator" /> when the animation event is triggered.
        /// </remarks>
        public void OnLadderExited()
        {
            if (Owner) Owner.Action.OnLadderExited();
        }

        #endregion

        #region - CRAWL EVENTS -

        /// <summary>
        /// 
        /// </summary
        /// <remarks>
        ///     Called by <see cref="Animator" /> when the animation event is triggered.
        /// </remarks>
        public void OnCrawlEnter()
        {
            if (Owner) Owner.Action.OnCrawlEnter();
        }

        /// <summary>
        /// 
        /// </summary
        /// <remarks>
        ///     Called by <see cref="Animator" /> when the animation event is triggered.
        /// </remarks>
        public void OnCrawlEntered()
        {
            if (Owner) Owner.Action.OnCrawlEntered();
        }

        /// <summary>
        /// 
        /// </summary
        /// <remarks>
        ///     Called by <see cref="Animator" /> when the animation event is triggered.
        /// </remarks>
        public void OnCrawlExit()
        {
            if (Owner) Owner.Action.OnCrawlExit();
        }

        /// <summary>
        /// 
        /// </summary
        /// <remarks>
        ///     Called by <see cref="Animator" /> when the animation event is triggered.
        /// </remarks>
        public void OnCrawlExited()
        {
            if (Owner) Owner.Action.OnCrawlExited();
        }

        #endregion

        #region - DODGE EVENTS -

        /// <summary>
        /// 
        /// </summary
        /// <remarks>
        ///     Called by <see cref="Animator" /> when the animation event is triggered.
        /// </remarks>
        public void OnDodgeStart()
        {
            if (Owner) Owner.Action.DodgeStart();
        }

        /// <summary>
        /// 
        /// </summary
        /// <remarks>
        ///     Called by <see cref="Animator" /> when the animation event is triggered.
        /// </remarks>
        public void OnDodgeEnd()
        {
            if (Owner) Owner.Action.DodgeEnd();
        }

        #endregion
    }
}