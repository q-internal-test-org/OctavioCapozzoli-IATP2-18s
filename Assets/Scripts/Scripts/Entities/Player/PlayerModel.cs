﻿using _Main.Scripts.FSM_SO_VERSION;
using _Main.Scripts.Steering_Behaviours.Steering_Behaviours;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Main.Scripts.Entities.Player
{
    public class PlayerModel : EntityModel
    {

        [SerializeField] private StateData[] fsmStates;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float jumpForce;
        [SerializeField] private float maxLife = 100;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] float groundCheckLength;
        [SerializeField] ProjectileScript projectile;
        [SerializeField] Transform projectilePosition;
        [SerializeField] GameObject guitarPrefab;


        private PlayerView _view;
        PlayerController _controller;
        private HealthController _healthController;
        bool _isGrounded;

        Rigidbody _rigidbody;
        Transform _transform;

        public bool IsGrounded { get => _isGrounded; set => _isGrounded = value; }
        public PlayerView View { get => _view; set => _view = value; }
        public PlayerController Controller { get => _controller; set => _controller = value; }

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _view = GetComponent<PlayerView>();
            _controller = GetComponent<PlayerController>();
            _healthController = new HealthController(maxLife);
            _healthController.OnDie += Die;
        }



        public void CheckGround()
        {
            _isGrounded = Physics.CheckSphere(transform.position, groundCheckLength, groundMask);
            //View.PlayerGroundedAnimation(_isGrounded);
            //View.PlayerFallingAnimation(!_isGrounded);
        }

        public override void Move(Vector3 direction)
        {
            _view.PlayRunAnimation(true);
            direction.y = 0;
            _rigidbody.velocity = direction * maxSpeed;
            if (direction.x != 0 || direction.z != 0)
                transform.forward = direction;

        }
        public void Jump()
        {
            _rigidbody.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        }
        public override void LookDir(Vector3 dir)
        {
            if (dir == Vector3.zero) return;
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * rotSpeed);
        }
        public override void GetDamage(int damage)
        {
            _healthController.TakeDamage(damage);
        }

        public override void Heal(int healingPoint)
        {
            _healthController.Heal(healingPoint);
        }

        public void InstantiateFireball()
        {
            Instantiate(projectile, projectilePosition);
        }

        public void EnableGuitar()
        {
            guitarPrefab.SetActive(true);
        }

        public void DisableGuitar()
        {
            guitarPrefab.SetActive(false);
        }
        public override bool IsEntityDead()
        {
            return _healthController.CurrentHealth <= 0;
        }

        public override void Die()
        {
            SceneManager.LoadScene("Game Over");
        }

        public override EntityModel GetModel() => this;
        public override StateData[] GetStates() => fsmStates;
        public override Vector3 GetFoward() => transform.forward;
        public override float GetSpeed() => _rigidbody.velocity.magnitude;
        public override Rigidbody GetRigidbody() => _rigidbody;
    }
}