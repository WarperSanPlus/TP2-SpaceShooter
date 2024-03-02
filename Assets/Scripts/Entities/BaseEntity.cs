using Interfaces;
using System.Collections;
using UnityEngine;

namespace Entities
{
    public class BaseEntity : MonoBehaviour, IEnterActivation, IResetable
    {
        #region MonoBehaviour

        /// <inheritdoc/>
        private void FixedUpdate()
        {
            if (this.iframes != 0)
                this.iframes--;

            this.OnUpdate(Time.fixedDeltaTime);
        }

        /// <inheritdoc/>
        private void OnTriggerEnter2D(Collider2D collision) => this.ManageCollision(collision);

        /// <inheritdoc/>
        private void OnTriggerStay2D(Collider2D collision) => this.ManageCollision(collision);

        /// <inheritdoc/>
        private void OnDisable()
        {
            if (this.hitBlinkCoroutine != null)
                this.StopCoroutine(this.hitBlinkCoroutine);
        }

        #endregion MonoBehaviour

        #region IActivatable

        /// <inheritdoc/>
        public void SetActive(bool isActive) => this.isInvicible = !isActive;

        #endregion

        #region IResetable

        /// <inheritdoc/>
        public virtual void OnReset()
        {
            this.iframes = 0;
            this.hitBlinkTime = 0;
            this.health = this.MaxHealth;
        }

        #endregion

        [Header("Base Entity")]

        #region I-frames

        [SerializeField, Min(0)]
        [Tooltip("Number of seconds where the entity cannot be damaged after a hit")]
        private float invincibilityTime = 0;
        private int iframes = 0;

        #endregion I-frames

        #region Health

        [SerializeField, Tooltip("Current health of the entity")]
        private float health;

        [SerializeField, Tooltip("Maximum health reachable by the entity")]
        protected float MaxHealth;

        [Tooltip("Determines if this entity can take damage")]
        public bool isInvicible = true;

        private void ManageCollision(Collider2D collider)
        {
            if (this.ShouldCollide(collider))
                this.Damage(1);
        }

        private void Damage(float amount)
        {
            // If the entity is on invincibility
            if (this.iframes != 0 || !this.enabled || this.isInvicible)
                return;

            // Remember the previous health
            var oldHealth = this.health;

            // Calculate the damage amount
            amount = this.OnDamageModifier(amount);

            // Don't do anything if no damage
            if (amount == 0)
                return;

            // Reduce health
            this.health -= amount;

            this.OnHealthChanged(
                this.health,
                oldHealth,
                this.MaxHealth
                );

            // If the entity will die
            if (this.health <= 0f)
                return;

            this.iframes = Mathf.FloorToInt(this.invincibilityTime / Time.fixedDeltaTime);

            this.TriggerHitBlink();
        }

        #endregion Health

        #region Hit Blink

        private const float HIT_BLINK_DURATION = 0.15f;
        private const float HIT_BLINK_INTERVAL = 0.05f;

        [SerializeField, Tooltip("Sprite to apply the blink effect on")]
        private SpriteRenderer hitBlinkSprite;

        [SerializeField, Tooltip("Color of the blink effect")]
        private Color hitBlinkColor;

        private float hitBlinkTime = 0f;
        private Coroutine hitBlinkCoroutine = null;

        private void TriggerHitBlink()
        {
            if (this.hitBlinkCoroutine != null)
                this.hitBlinkTime = 0f;
            else
                this.hitBlinkCoroutine = this.StartCoroutine(this.FlashSprite());
        }

        // Taken from: https://forum.unity.com/threads/make-a-sprite-flash.224086/#post-1837621
        private IEnumerator FlashSprite()
        {
            Color originalColor = this.hitBlinkSprite.color;

            Color[] colors = { originalColor, this.hitBlinkColor };

            this.hitBlinkTime = 0f;
            var index = 0;
            while (this.hitBlinkTime < HIT_BLINK_DURATION)
            {
                this.hitBlinkSprite.color = colors[index % 2];

                this.hitBlinkTime += HIT_BLINK_INTERVAL;
                index++;
                yield return new WaitForSeconds(HIT_BLINK_INTERVAL);
            }

            this.hitBlinkSprite.color = originalColor;

            // Clear coroutine
            this.hitBlinkCoroutine = null;
        }

        #endregion Hit Blink

        #region Virtual

        /// <summary>
        /// Called when the entity's health changed
        /// </summary>
        /// <param name="newHealth">New value of <see cref="health"/></param>
        /// <param name="oldHealth">Previous value of <see cref="health"/></param>
        /// <param name="maxHealth">Value of <see cref="MaxHealth"/></param>
        protected virtual void OnHealthChanged(float newHealth, float oldHealth, float maxHealth) { }

        /// <param name="amount">Current amount of damage</param>
        /// <returns>How much damage does the attack do?</returns>
        protected virtual float OnDamageModifier(float amount) => amount;

        /// <summary>
        /// Called when <see cref="FixedUpdate"/> is called
        /// </summary>
        /// <param name="elapsed">Time elapsed since the last call</param>
        protected virtual void OnUpdate(float elapsed) { }

        /// <returns>Should the collision be managed?</returns>
        protected virtual bool ShouldCollide(Collider2D collision) => true;

        #endregion Virtual
    }
}