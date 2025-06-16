using UnityEngine;

namespace Asce.Game
{
    /// <summary>
    ///     Simulates second-order dynamics (spring-damper systems) for smooth interpolation.
    ///     <br/>
    ///     Based on the paper "Critically Damped Ease-In/Ease-Out Smoothing" by Ian Qvist (GDC 2019).
    /// </summary>
    [System.Serializable]
    public struct SecondOrderDynamics
    {
        [Tooltip("[f] Natural frequency (Hz)")]
        [SerializeField] private float _frequency;

        [Tooltip("[d] Damping ratio (0 = no damping, 1 = critically damped)")]
        [SerializeField] private float _damping;

        [Tooltip("[r] Response factor (affects how input velocity influences output)")]
        [SerializeField] private float _response;

        private Vector3 _previousInput;              // Previous input value
        private Vector3 _position;                   // Output position (y)
        private Vector3 _velocity;                   // Output velocity (dy/dt)
        private Vector3 _inputVelocity;              // Estimated input velocity (dx/dt)

        // System constants derived from input parameters
        private float _dampingCoefficient;           // k1 = d / (pi * f)
        private float _springiness;                  // k2 = 1 / (2 * pi * f)^2
        private float _inputVelocityInfluence;       // k3 = r * d / (2 * pi * f)
        private float _stableSpringiness;            // Stable spring constant for small deltaTimes

        /// <summary>
        ///     Gets or sets the system's natural frequency.
        /// </summary>
        public float Frequency
        {
            get => _frequency;
            set
            {
                _frequency = Mathf.Max(value, 0.01f); // Prevent division by zero
                UpdateSystemConstants();
            }
        }

        /// <summary>
        ///     Gets or sets the damping ratio (0 = undamped, 1 = critically damped).
        /// </summary>
        public float Damping
        {
            get => _damping;
            set
            {
                _damping = Mathf.Max(value, 0.01f); // Prevent division by zero
                UpdateSystemConstants();
            }
        }

        /// <summary>
        ///     Gets or sets the response factor, which controls input velocity influence.
        /// </summary>
        public float Response
        {
            get => _response;
            set
            {
                _response = value;
                UpdateSystemConstants();
            }
        }

        /// <summary>
        ///     Gets the current output position.
        /// </summary>
        public Vector3 Position => _position;

        /// <summary>
        ///     Gets the current output velocity.
        /// </summary>
        public Vector3 Velocity => _velocity;

        /// <summary>
        ///     Creates a new instance of <see cref="SecondOrderDynamics"/> with the given parameters.
        /// </summary>
        /// <param name="frequency"> Natural frequency of the system (Hz). </param>
        /// <param name="damping"> Damping ratio (0 to 1). </param>
        /// <param name="response"> Response factor to input velocity. </param>
        public SecondOrderDynamics(float frequency, float damping, float response)
        {
            _frequency = 1f;
            _damping = 0f;
            _response = 0f;

            _previousInput = Vector3.zero;
            _position = Vector3.zero;
            _velocity = Vector3.zero;
            _inputVelocity = Vector3.zero;

            _dampingCoefficient = _springiness = _inputVelocityInfluence = _stableSpringiness = 0f;

            Reset(frequency, damping, response, Vector3.zero);
        }

        /// <summary>
        ///     Resets the simulation with new parameters and an initial position.
        /// </summary>
        public void Reset(float frequency, float damping, float response, Vector3 initialPosition)
        {
            _frequency = frequency;
            _damping = damping;
            _response = response;

            _previousInput = initialPosition;
            _position = initialPosition;
            _velocity = Vector3.zero;
            _inputVelocity = Vector3.zero;

            UpdateSystemConstants();
        }

        /// <summary>
        ///     Resets using current parameters and a 3D initial position.
        /// </summary>
        public void Reset(Vector3 initialPosition) =>
            Reset(_frequency, _damping, _response, initialPosition);

        /// <summary>
        ///     Resets using a 2D initial position (Z = 0).
        /// </summary>
        public void Reset(Vector2 initialPosition) =>
            Reset(new Vector3(initialPosition.x, initialPosition.y, 0f));

        /// <summary>
        ///     Resets using a scalar initial position for all axes.
        /// </summary>
        public void Reset(float initialValue) =>
            Reset(new Vector3(initialValue, initialValue, initialValue));

        /// <summary>
        ///     Updates the system state based on new input and elapsed time.
        /// </summary>
        /// <param name="input"> Target input position.</param>
        /// <param name="deltaTime"> Elapsed time since last update (in seconds). </param>
        /// <returns> New output position. </returns>
        public Vector3 Update(Vector3 input, float deltaTime)
        {
            if (deltaTime < Mathf.Epsilon)
                return _position;

            // Estimate input velocity using finite difference
            _inputVelocity = (input - _previousInput) / deltaTime;
            _previousInput = input;

            // Stabilize springiness to prevent instability at small deltaTime
            // This prevents jitter when frame rate is very high
            _stableSpringiness = Mathf.Max(
                _springiness,
                1.1f * (0.25f * deltaTime * deltaTime + 0.5f * deltaTime * _dampingCoefficient)
            );

            // Integrate using semi-implicit Euler integration
            _position += _velocity * deltaTime;
            _velocity += deltaTime * (
                input + _inputVelocityInfluence * _inputVelocity - _position - _dampingCoefficient * _velocity
            ) / _stableSpringiness;

            return _position;
        }

        /// <summary>
        ///     Overload for 2D input.
        /// </summary>
        public Vector2 Update(Vector2 input, float deltaTime)
        {
            Vector3 result = Update(new Vector3(input.x, input.y, 0f), deltaTime);
            return new Vector2(result.x, result.y);
        }

        /// <summary>
        ///     Overload for scalar input.
        /// </summary>
        public float Update(float input, float deltaTime)
        {
            return Update(new Vector3(input, input, input), deltaTime).x;
        }

        /// <summary>
        ///     Recalculates internal constants based on frequency, damping, and response.
        /// </summary>
        private void UpdateSystemConstants()
        {
            // omega = 2 * pi * f
            float piFrequency = Mathf.PI * _frequency;

            // Derived from the second-order differential equation:
            // y'' + (2 * zeta * omega) y' + omega^2 y = omega^2 x + (2 * zeta * omega * r * x')
            _dampingCoefficient = _damping / piFrequency;                        // k1 = d / pi * f
            _springiness = 1f / ((2f * piFrequency) * (2f * piFrequency));       // k2 = 1 / (2 * pi * f)^2
            _inputVelocityInfluence = _response * _damping / (2f * piFrequency); // k3 = r * d / 2 * pi * f
        }
    }
}
