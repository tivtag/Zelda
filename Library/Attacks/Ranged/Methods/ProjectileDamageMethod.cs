using Zelda.Entities;
using Zelda.Entities.Projectiles;

namespace Zelda.Attacks.Ranged
{
    /// <summary>
    /// Represents an <see cref="AttackDamageMethod"/> that provides
    /// access to the <see cref="Projectile"/> currently processed in the AttackDamageMethod.
    /// </summary>
    public abstract class ProjectileDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Gets the <see cref="Projectile"/> currently processed by this ProjectileDamageMethod.
        /// </summary>
        protected Projectile CurrentProjectile
        {
            get
            {
                return this.projectile;
            }
        }

        /// <summary>
        /// Gets called just before this AttackDamageMethod is used by
        /// a new calling object.
        /// </summary>
        /// <param name="caller">
        /// The object which is going to call this AttackDamageMethod.
        /// </param>
        protected override void OnCallerChanged( object caller )
        {
            var attack = caller as ProjectileMeleeAttack;
            
            if( attack != null )
            {
                this.projectile = attack.Projectile;
            }
            else
            {
                this.projectile = null;
            }
        }

        /// <summary>
        /// Identifies the Projectile currently processed by this ProjectileDamageMethod.
        /// </summary>
        private Projectile projectile;
    }
}
