using System;
using UnityEngine;

namespace Asce.Managers.Utils
{
    /// <summary>
    ///     Represents a countdown timer used for cooldowns.
    /// </summary>
    [Serializable]
    public class Cooldown
    {
        [SerializeField, Min(0f)] private float _baseTime;
        [SerializeField, Min(0f)] private float _currentTime;
        private bool _completeTrigger = false; // Prevents multiple OnCompleted invocations when CurrentTime <= 0.

        /// <summary>
        ///     Invoked when <see cref="BaseTime"/> is changed.
        ///     <br/>
        ///     The sender is this instance of <see cref="Cooldown"/>, 
        ///     and the argument is the new base time in seconds.
        /// </summary>
        public event Action<object, float> OnBaseTimeChanged;

        /// <summary>
        ///     Invoked when <see cref="CurrentTime"/> is changed.
        ///     <br/>
        ///     The sender is this instance of <see cref="Cooldown"/>, 
        ///     and the argument is the new current time in seconds.
        /// </summary>
        public event Action<object, float> OnCurrentTimeChanged;

        /// <summary>
        ///     Invoked when cooldown is reset using <see cref="Reset"/>.
        /// </summary>
        public event Action<object> OnTimeReset;

        /// <summary>
        ///     Invoked once when cooldown completes (CurrentTime <= 0).
        /// </summary>
        public event Action<object> OnCompleted;

        /// <summary>
        ///     The base cooldown time (in seconds).
        /// </summary>
        public float BaseTime
        {
            get => _baseTime;
            set
            {
                _baseTime = Mathf.Max(0f, value);
                OnBaseTimeChanged?.Invoke(this, _baseTime);
            }
        }

        /// <summary>
        ///     The current cooldown time (in seconds).
        /// </summary>
        public float CurrentTime
        {
            get => _currentTime;
            set 
            {
                _currentTime = Mathf.Max(0f, value);
                OnCurrentTimeChanged?.Invoke(this, _currentTime);
                this.HandleComplete();
            }
        }

        /// <summary>
        ///     A value indicating whether the cooldown has completed.
        /// </summary>
        public bool IsComplete => CurrentTime <= 0f;

        /// <summary>
        ///     Gets the ratio of remaining time to base time.
        ///     <br/>
        ///     Returns 0 if the base time is zero or less.
        /// </summary>
        public float Ratio => BaseTime <= 0f ? 0f : CurrentTime / BaseTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Cooldown"/> class 
        ///     with base time and current time is zero.
        /// </summary>
        public Cooldown() : this (0f, 0f)
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Cooldown"/> class 
        ///     with a specified base time.
        ///     <br/>
        ///     The current time is also initialized to the base time.
        /// </summary>
        /// <param name="baseTime"> The base time in seconds. </param>
        public Cooldown(float baseTime) : this (baseTime, baseTime)
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Cooldown"/> class 
        ///     with specified base and current time.
        /// </summary>
        /// <param name="baseTime"> The base time in seconds. </param>
        /// <param name="currentTime"> The current remaining time in seconds. </param>
        public Cooldown(float baseTime, float currentTime) 
        { 
            BaseTime = baseTime;
            CurrentTime = currentTime;
        }

        /// <summary>
        ///     Decreases the current cooldown time by the specified delta time.
        /// </summary>
        /// <param name="deltaTime"> Time to subtract, typically <see cref="Time.deltaTime"/> or <see cref="Time.fixedDeltaTime"/>. </param>
        public void Update(float deltaTime)
        {
            CurrentTime -= deltaTime;
        }


        /// <summary>
        ///     Set base time to <paramref name="baseTime"/> value
        ///     and <see cref="Reset"/> cooldown time.
        /// </summary>
        /// <param name="baseTime"> Value will be set to Base Time. </param>
        /// <param name="isReset"> (Optional) If true, reset cooldown. </param>
        public void SetBaseTime(float baseTime, bool isReset = true)
        {
            BaseTime = baseTime;
            if (isReset) this.Reset();
        }

        /// <summary>
        ///     Set current time to the ratio value of base time.
        /// </summary>
        /// <param name="ratio"></param>
        public void SetCurrentByRatio(float ratio)
        {
            CurrentTime = BaseTime * ratio;
        }

        /// <summary>
        ///     Immediately completes the cooldown by setting current time to zero.
        /// </summary>
        public void ToComplete() => CurrentTime = 0f;

        /// <summary>
        ///     Resets the current cooldown time to the base time.
        /// </summary>
        public void Reset()
        {
            CurrentTime = BaseTime;
            OnTimeReset?.Invoke(this);
        }

        /// <summary>
        ///     Internal logic to trigger OnCompleted only once when cooldown finishes.
        /// </summary>
        private void HandleComplete()
        {
            // If the cooldown is complete and wasn't already flagged, invoke the event
            if (IsComplete) 
            {
                if (_completeTrigger)
                {
                    OnCompleted?.Invoke(this);
                    _completeTrigger = false; // Prevent firing again until reset
                }
            }
            else _completeTrigger = true; // Cooldown is running again, re-enable the trigger
        }
    }
}
