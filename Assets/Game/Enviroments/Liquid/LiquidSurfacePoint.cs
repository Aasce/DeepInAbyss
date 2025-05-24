using System;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    /// <summary>
    ///     Represents a single point on a liquid surface used for simulating wave or fluid dynamics.
    ///     Stores physical properties such as height, velocity, and acceleration.
    /// </summary>
    [Serializable]
    public class LiquidSurfacePoint
    {
        [SerializeField] private float _currentHeight;
        [SerializeField] private float _restHeight;

        [SerializeField] private float _velocity;
        [SerializeField] private float _acceleration;

        /// <summary>
        ///     Gets or sets the current vertical height of the surface point.
        /// </summary>
        public float CurrentHeight
        {
            get => _currentHeight;
            set => _currentHeight = value;
        }

        /// <summary>
        ///     Gets or sets the rest height, representing the stable or equilibrium position of the surface point.
        /// </summary>
        public float RestHeight
        {
            get => _restHeight;
            set => _restHeight = value;
        }

        /// <summary>
        ///     Gets or sets the velocity of the surface point's vertical movement.
        /// </summary>
        public float Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        /// <summary>
        ///     Gets or sets the acceleration applied to the surface point.
        /// </summary>
        public float Acceleration
        {
            get => _acceleration;
            set => _acceleration = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LiquidSurfacePoint"/> class with default velocity and acceleration.
        /// </summary>
        public LiquidSurfacePoint()
        {
            Velocity = 0f;
            Acceleration = 0f;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LiquidSurfacePoint"/> class with specified values.
        /// </summary>
        /// <param name="current"> The current height of the surface point. </param>
        /// <param name="rest"> The rest height (equilibrium position) of the surface point. </param>
        /// <param name="velocity"> The initial velocity of the surface point. </param>
        /// <param name="acceleration"> The initial acceleration of the surface point. </param>
        public LiquidSurfacePoint(float current, float rest, float velocity = 0f, float acceleration = 0f)
        {
            CurrentHeight = current;
            RestHeight = rest;
            Velocity = velocity;
            Acceleration = acceleration;
        }
    }
}