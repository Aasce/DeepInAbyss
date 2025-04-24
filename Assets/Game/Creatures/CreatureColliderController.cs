using Asce.Managers.Utils;
using System;
using UnityEngine;

namespace Asce.Game.Entities
{
    public class CreatureColliderController : MonoBehaviour
    {
        [SerializeField] protected Collider2D _body;
        [SerializeField] protected Rigidbody2D _rb;

        [Header("Ground")]
        [SerializeField] protected Transform _groundCheck;
        [SerializeField] protected float _groundCheckRadius = 0.2f;
        [SerializeField] protected LayerMask _groundLayer;

        

        public Collider2D Body => _body;
        public Rigidbody2D Rigidbody => _rb;


        public bool IsGrounded { get; protected set; }


        protected virtual void Update()
        {
            IsGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
        }


        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {

        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {

        }
    }
}
