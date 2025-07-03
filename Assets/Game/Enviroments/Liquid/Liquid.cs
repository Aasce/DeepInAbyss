using Asce.Game.Utils;
using Asce.Managers.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Asce.Game.Enviroments
{
    /// <summary>
    ///     Represents a dynamic 2D water surface using a spring-based simulation and a procedural mesh.
    ///     <br/>
    ///     This component generates a mesh and simulates ripples and waves using a simplified physics system.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(BoxCollider2D))]
    public class Liquid : MonoBehaviour, IEnviromentComponent, IOptimizedComponent
    {
        public const int NUM_OF_Y_VERTICES = 2;

        // Reference
        [SerializeField, HideInInspector] protected BoxCollider2D _collider;
        [SerializeField, HideInInspector] protected MeshRenderer _meshRenderer;
        [SerializeField, HideInInspector] protected MeshFilter _meshFilter;

        [Header("Mesh Generator")]
        [Tooltip("Number of horizontal vertices used to generate the mesh.")]
        [SerializeField] protected int _numOfHorizontalVertices = 70;

        [Tooltip("Width of the liquid surface.")]
        [SerializeField] protected float _width = 10f;

        [Tooltip("Height of the liquid surface.")]
        [SerializeField] protected float _height = 4f;

        [SerializeField, HideInInspector] protected Mesh _mesh;
        [SerializeField, HideInInspector] protected Vector3[] _vertices;
        [SerializeField, HideInInspector] protected int[] _topVerticesIndex;


        [Header("Springs")]
        [Tooltip("Spring constant used in the simulation (stiffness).")]
        [SerializeField] protected float _springConstant = 1.4f;

        [Tooltip("Damping value to reduce oscillations.")]
        [SerializeField] protected float _damping = 1.1f;

        [Tooltip("How much adjacent points influence each other.")]
        [SerializeField] protected float _spread = 6.5f;

        [Tooltip("Number of times wave propagation logic runs per update.")]
        [SerializeField, Range(1, 10)] protected int _wavePropagationInteractions = 8;

        [Tooltip("Multiplier for the simulation speed.")]
        [SerializeField, Range(0f, 20f)] protected float _speedMultiplier = 5.5f;


        [Header("Force")]
        [Tooltip("Multiplier applied to incoming forces (e.g., splash).")]
        [SerializeField, Range(0f, 5f)] protected float _forceMultiplier = 0.2f;

        [Tooltip("Maximum allowable force per splash interaction.")]
        [SerializeField] protected float _maxForce = 0.5f;


        [Header("Collision")]
        [Tooltip("Scale for the collision radius used to determine splash influence.")]
        [SerializeField, Range(1f, 10f)] protected float _collisionRadiusMultiply = 4.15f;

        // Simulated water surface points
        protected readonly List<LiquidSurfacePoint> _surfacePoints = new();


        public BoxCollider2D Collider => _collider;
        public MeshRenderer MeshRenderer => _meshRenderer;
        public MeshFilter MeshFilter => _meshFilter;

        bool IOptimizedComponent.IsActive => this.gameObject.activeSelf;
        OptimizeBehavior IOptimizedComponent.OptimizeBehavior => OptimizeBehavior.DeactivateOutsideView;

        public int NumOfHorizontalVertices => _numOfHorizontalVertices;
        public float Width
        {
            get => _width;
            set => _width = value;
        }
        public float Height
        {
            get => _height;
            set => _height = value;
        }

        public float SpringConstant => _springConstant;
        public float Damping => _damping;
        public float Spread => _spread;
        public float SpeedMultiplier => _speedMultiplier;

        public float ForceMultiplier => _forceMultiplier;
        public float MaxForce => _maxForce;
        public float CollisionRadiusMultiply => _collisionRadiusMultiply;


        protected virtual void Reset()
        {
            if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
            if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
            if (_collider == null) _collider = GetComponent<BoxCollider2D>();

            this.GenerateMesh();
            this.ResetCollider();
        }

        protected virtual void Start()
        {
            this.CreateWaterPoint();
        }

        protected virtual void FixedUpdate()
        {
            this.UpdateWaterPoints(Time.fixedDeltaTime);
            this.HandleWavePropagation(Time.fixedDeltaTime);

            _mesh.vertices = _vertices; // Update mesh with new surface heights
        }

        /// <summary>
        ///     Generates the initial flat mesh for the water surface.
        /// </summary>
        public virtual void GenerateMesh()
        {
            _vertices = new Vector3[NumOfHorizontalVertices * NUM_OF_Y_VERTICES];
            _topVerticesIndex = new int[NumOfHorizontalVertices];

            for (int y = 0; y < NUM_OF_Y_VERTICES; y++)
            {
                for (int x = 0; x < NumOfHorizontalVertices; x++)
                {
                    // Compute vertex position in mesh space
                    float xPos = x / (float)(NumOfHorizontalVertices - 1) * _width - _width * 0.5f;
                    float yPos = y / (float)(NUM_OF_Y_VERTICES - 1) * _height - _height * 0.5f;
                    _vertices[y * NumOfHorizontalVertices + x] = new Vector3(xPos, yPos, 0f);

                    // Cache the top row of vertices for later water simulation
                    if (y == NUM_OF_Y_VERTICES - 1)
                    {
                        _topVerticesIndex[x] = y * NumOfHorizontalVertices + x;
                    }
                }
            }

            // Contruct the triangles
            int[] triangles = MeshUtils.ConstructTriangles(NumOfHorizontalVertices, NUM_OF_Y_VERTICES);

            // Contruct the UVs
            Vector2[] uvs = MeshUtils.ConstructUVs(_vertices, _width, _height);

            _mesh = new Mesh { name = "Water Mesh" };
            MeshUtils.SetMeshData(_mesh, _vertices, triangles, uvs);

            if (MeshFilter != null) MeshFilter.mesh = _mesh;
        }

        /// <summary>
        ///     Resets the collider to match the current size of the water mesh.
        /// </summary>
        public virtual void ResetCollider()
        {
            if (Collider == null) return;

            Collider.isTrigger = true;
            Collider.offset = Vector2.zero;
            Collider.size = new Vector2(Width, Height);
        }

        /// <summary>
        ///     Applies a splash force to the water at a given world position.
        /// </summary>
        /// <param name="collisionPosition"> World position of the impact. </param>
        /// <param name="collisionRadius"> Effective radius of the impact. </param>
        /// <param name="force"> Force value to apply. </param>
        public virtual void Splash(Vector2 collisionPosition, float collisionRadius, float force)
        {
            float radius = collisionRadius * CollisionRadiusMultiply;

            for (int i = 0; i < _surfacePoints.Count; i++)
            {
                Vector2 vertexWorldPoint = transform.TransformPoint(_vertices[_topVerticesIndex[i]]);
                if (Vector2Utils.IsPointInsideCircle(collisionPosition, radius, vertexWorldPoint))
                {
                    _surfacePoints[i].Velocity = force;
                }
            }
        }

        /// <summary>
        ///     Initializes water points from top row of mesh for spring simulation.
        /// </summary>
        protected virtual void CreateWaterPoint()
        {
            if (_topVerticesIndex == null) return;

            _surfacePoints.Clear();
            for (int i = 0; i < _topVerticesIndex.Length; i++)
            {
                LiquidSurfacePoint waterPoint = new LiquidSurfacePoint
                {
                    CurrentHeight = _vertices[_topVerticesIndex[i]].y,
                    RestHeight = _vertices[_topVerticesIndex[i]].y
                };
                _surfacePoints.Add(waterPoint);
            }
        }

        /// <summary>
        ///     Updates spring simulation for each water point.
        /// </summary>
        /// <param name="deltaTime"> Delta time for the physics step. </param>
        protected virtual void UpdateWaterPoints(float deltaTime)
        {
            float speed = SpeedMultiplier * deltaTime;

            for (int i = 1; i < _surfacePoints.Count - 1; i++)
            {
                LiquidSurfacePoint waterPoint = _surfacePoints[i];

                float deltaY = waterPoint.CurrentHeight - waterPoint.RestHeight; // Calculate the difference (spring displacement)

                // Calculate the acceleration using Hooke's law with damping: 
                //      a = -k * x - d * v (spring force + damping force)
                float acceleration = -SpringConstant * deltaY - Damping * waterPoint.Velocity;

                // Apply the updated height to the corresponding vertex
                waterPoint.CurrentHeight += waterPoint.Velocity * speed; 
                _vertices[_topVerticesIndex[i]].y = waterPoint.CurrentHeight;

                waterPoint.Velocity += acceleration * speed;
            }
        }

        /// <summary>
        ///     Simulates wave spreading between neighboring points to create ripple effects.
        /// </summary>
        /// <param name="deltaTime"> Delta time for the physics step. </param>
        protected virtual void HandleWavePropagation(float deltaTime)
        {
            float speed = SpeedMultiplier * deltaTime;

            for (int index = 0; index < _wavePropagationInteractions; index++)
            {
                for (int i = 1; i < _surfacePoints.Count - 1; i++)
                {
                    LiquidSurfacePoint currentPoint = _surfacePoints[i];

                    // Calculate how much wave energy should be passed to the left neighbor point
                    LiquidSurfacePoint leftPoint = _surfacePoints[i - 1];
                    float leftDelta = Spread * (currentPoint.CurrentHeight - leftPoint.CurrentHeight) * speed;
                    leftPoint.Velocity += leftDelta;

                    // Calculate how much wave energy should be passed to the right neighbor point
                    LiquidSurfacePoint rightPoint = _surfacePoints[i + 1];
                    float rightDelta = Spread * (currentPoint.CurrentHeight - rightPoint.CurrentHeight) * speed;
                    rightPoint.Velocity += rightDelta;
                }
            }
        }

        void IOptimizedComponent.SetActivate(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}