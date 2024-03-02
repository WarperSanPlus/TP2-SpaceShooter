using Controllers;
using Interfaces;
using Singletons;
using UnityEngine;

namespace Entities
{
    public class EnemyEntity : BaseEntity, ILifespan
    {
        [Header("Enemy Entity")]
        [SerializeField]
        private GameObject explodePrefab;

        [SerializeField, Tooltip("Determines if, on death, its' bullets will be destroyed")]
        private bool destroyBulletsOnDeath = false;

        private void KillSelf(bool fromPlayer)
        {
            if (fromPlayer)
            {
                // Explode
                GameObject obj = ObjectPool.Instance.GetPooledObject(this.explodePrefab.name, "Explosions");
                obj.transform.position = this.transform.position;
                obj.SetActive(true);

                if (this.destroyBulletsOnDeath)
                {
                    foreach (BaseController item in this.GetComponents<BaseController>())
                        item.DestroyBullets();
                }
            }

            // Destroy enemy
            this.gameObject.SetActive(false);
        }

        #region BaseEntity

        /// <inheritdoc/>
        protected override bool ShouldCollide(Collider2D collision) => collision.gameObject.CompareTag("Bullet");

        /// <inheritdoc/>
        protected override void OnHealthChanged(float newHealth, float oldHealth, float maxHealth)
        {
            if (newHealth > 0f)
                return;

            this.KillSelf(true);
        }

        #endregion BaseEntity

        #region ILifespan

        /// <inheritdoc/>
        public void OnLifeEnd() => this.KillSelf(false);

        #endregion ILifespan
    }
}
