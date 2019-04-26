﻿using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    [AddComponentMenu("TopDown Engine/Weapons/Melee Weapon")]
    /// <summary>
    /// A basic melee weapon class, that will activate a "hurt zone" when the weapon is used
    /// </summary>
    public class MeleeWeapon : Weapon
    {
        /// the possible shapes for the melee weapon's damage area
        public enum MeleeDamageAreaShapes { Rectangle, Circle, Box, Sphere }

        [Header("Damage Area")]
        /// the shape of the damage area (rectangle or circle)
        public MeleeDamageAreaShapes DamageAreaShape = MeleeDamageAreaShapes.Rectangle;
        /// the size of the damage area
        public Vector3 AreaSize = new Vector3(1, 1);
        /// the offset to apply to the damage area (from the weapon's attachment position
        public Vector3 AreaOffset = new Vector3(1, 0);

        [Header("Damage Area Timing")]
        /// the initial delay to apply before triggering the damage area
        public float InitialDelay = 0f;
        /// the duration during which the damage area is active
        public float ActiveDuration = 1f;

        [Header("Damage Caused")]
        // the layers that will be damaged by this object
        public LayerMask TargetLayerMask;
        /// The amount of health to remove from the player's health
        public int DamageCaused = 10;
        /// the kind of knockback to apply
        public DamageOnTouch.KnockbackStyles Knockback;
        /// The force to apply to the object that gets damaged
        public Vector2 KnockbackForce = new Vector2(10, 2);
        /// The duration of the invincibility frames after the hit (in seconds)
        public float InvincibilityDuration = 0.5f;

        protected Collider _damageAreaCollider;
        protected Collider2D _damageAreaCollider2D;
        protected bool _attackInProgress = false;
        protected Color _gizmosColor;
        protected Vector3 _gizmoSize;
        protected CircleCollider2D _circleCollider2D;
        protected BoxCollider2D _boxCollider2D;
        protected BoxCollider _boxCollider;
        protected SphereCollider _sphereCollider;
        protected Vector3 _gizmoOffset;
        protected DamageOnTouch _damageOnTouch;
        protected GameObject _damageArea;

        /// <summary>
        /// Initialization
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();
            if (_damageArea == null)
            {
                CreateDamageArea();
                DisableDamageArea();
            }
            if (Owner != null)
            {
                _damageOnTouch.Owner = Owner.gameObject;
            }            
        }

        /// <summary>
        /// Creates the damage area.
        /// </summary>
        protected virtual void CreateDamageArea()
        {
            _damageArea = new GameObject();
            _damageArea.name = this.name + "DamageArea";
            _damageArea.transform.position = this.transform.position;
            _damageArea.transform.rotation = this.transform.rotation;
            _damageArea.transform.SetParent(this.transform);
            _damageArea.layer = this.gameObject.layer;

            _damageOnTouch = _damageArea.AddComponent<DamageOnTouch>();

            if (DamageAreaShape == MeleeDamageAreaShapes.Rectangle)
            {
                _boxCollider2D = _damageArea.AddComponent<BoxCollider2D>();
                _boxCollider2D.offset = AreaOffset;
                _boxCollider2D.size = AreaSize;
                _damageAreaCollider2D = _boxCollider2D;
                _damageAreaCollider2D.isTrigger = true;
            }
            if (DamageAreaShape == MeleeDamageAreaShapes.Circle)
            {
                _circleCollider2D = _damageArea.AddComponent<CircleCollider2D>();
                _circleCollider2D.transform.position = this.transform.position + this.transform.rotation * AreaOffset;
                _circleCollider2D.radius = AreaSize.x / 2;
                _damageAreaCollider2D = _circleCollider2D;
                _damageAreaCollider2D.isTrigger = true;
            }

            if ((DamageAreaShape == MeleeDamageAreaShapes.Rectangle) || (DamageAreaShape == MeleeDamageAreaShapes.Circle))
            {
                Rigidbody2D rigidBody = _damageArea.AddComponent<Rigidbody2D>();
                rigidBody.isKinematic = true;
            }            

            if (DamageAreaShape == MeleeDamageAreaShapes.Box)
            {
                _boxCollider = _damageArea.AddComponent<BoxCollider>();
                _boxCollider.center = AreaOffset;
                _boxCollider.size = AreaSize;
                _damageAreaCollider = _boxCollider;
                _damageAreaCollider.isTrigger = true;
                _damageOnTouch.SetGizmoSize(AreaSize);
                _damageOnTouch.SetGizmoOffset(AreaOffset);
            }
            if (DamageAreaShape == MeleeDamageAreaShapes.Sphere)
            {
                _sphereCollider = _damageArea.AddComponent<SphereCollider>();
                _sphereCollider.transform.position = this.transform.position + this.transform.rotation * AreaOffset;
                _sphereCollider.radius = AreaSize.x / 2;
                _damageAreaCollider = _sphereCollider;
                _damageAreaCollider.isTrigger = true;
            }

            if ((DamageAreaShape == MeleeDamageAreaShapes.Box) || (DamageAreaShape == MeleeDamageAreaShapes.Sphere))
            {
                Rigidbody rigidBody = _damageArea.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
            }
            
            _damageOnTouch.TargetLayerMask = TargetLayerMask;
            _damageOnTouch.DamageCaused = DamageCaused;
            _damageOnTouch.DamageCausedKnockbackType = Knockback;
            _damageOnTouch.DamageCausedKnockbackForce = KnockbackForce;
            _damageOnTouch.InvincibilityDuration = InvincibilityDuration;
        }

        /// <summary>
        /// When the weapon is used, we trigger our attack routine
        /// </summary>
        protected override void WeaponUse()
        {
            base.WeaponUse();
            StartCoroutine(MeleeWeaponAttack());
        }

        /// <summary>
        /// Triggers an attack, turning the damage area on and then off
        /// </summary>
        /// <returns>The weapon attack.</returns>
        protected virtual IEnumerator MeleeWeaponAttack()
        {
            if (_attackInProgress) { yield break; }

            _attackInProgress = true;
            yield return new WaitForSeconds(InitialDelay);
            EnableDamageArea();
            yield return new WaitForSeconds(ActiveDuration);
            DisableDamageArea();
            _attackInProgress = false;
        }

        /// <summary>
        /// Enables the damage area.
        /// </summary>
        protected virtual void EnableDamageArea()
        {
            if (_damageAreaCollider2D != null)
            {
                _damageAreaCollider2D.enabled = true;
            }
            if (_damageAreaCollider != null)
            {
                _damageAreaCollider.enabled = true;
            }
        }


        /// <summary>
        /// Disables the damage area.
        /// </summary>
        protected virtual void DisableDamageArea()
        {
            if (_damageAreaCollider2D != null)
            {
                _damageAreaCollider2D.enabled = false;
            }
            if (_damageAreaCollider != null)
            {
                _damageAreaCollider.enabled = false;
            }
        }

        protected virtual void OnDrawGizmos()
        {
            DrawGizmos();
        }

        /// <summary>
        /// When selected, we draw a bunch of gizmos
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }

        protected virtual void DrawGizmos()
        {
            if (Owner == null) { return; }

            if (_damageAreaCollider2D != null)
            {
                float flipped = 1f;
                if (Owner.Orientation2D != null)
                {
                    flipped = Owner.Orientation2D.IsFacingRight ? 1f : -1f;
                }

                _gizmoOffset = AreaOffset;
                _gizmoOffset.x *= flipped;

                Gizmos.color = Color.white;
                if (DamageAreaShape == MeleeDamageAreaShapes.Circle)
                {
                    Gizmos.DrawWireSphere(this.transform.position + _gizmoOffset, AreaSize.x / 2);
                }
                if (DamageAreaShape == MeleeDamageAreaShapes.Rectangle)
                {
                    MMDebug.DrawGizmoRectangle(this.transform.position + _gizmoOffset, AreaSize, Color.red);
                }
            }

            if (_damageAreaCollider != null)
            {
                _gizmoOffset = AreaOffset;

                Gizmos.color = Color.white;
                if (DamageAreaShape == MeleeDamageAreaShapes.Sphere)
                {
                    Gizmos.DrawWireSphere(this.transform.position + _gizmoOffset, AreaSize.x / 2);
                }
                if (DamageAreaShape == MeleeDamageAreaShapes.Box)
                {
                    _gizmosColor = Color.red;
                    _gizmosColor.a = _damageAreaCollider.enabled ? 1f : 0.5f;
                    Gizmos.color = _gizmosColor;
                    Gizmos.matrix = Matrix4x4.TRS(this.transform.TransformPoint(_gizmoOffset), this.transform.rotation, this.transform.lossyScale);
                    Gizmos.DrawWireCube(Vector3.zero, AreaSize);
                }
            }
        }
    }
}