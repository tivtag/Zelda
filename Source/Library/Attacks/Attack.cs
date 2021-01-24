// <copyright file="Attack.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Attack class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Attacks
{
    using Zelda.Attacks.Limiter;
    using Zelda.Entities;
    using Zelda.Entities.Components;
    using Zelda.Status;

    /// <summary>
    /// Represents the base-class of all <see cref="Attack"/>s.
    /// </summary>
    public abstract class Attack : IZeldaSetupable, IEntityOwned
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="ZeldaEntity"/> that owns this <see cref="Attack"/>.
        /// </summary>
        /// <exception cref="Atom.Components.ComponentNotFoundException">
        /// Set: If the given ZeldaEntity doesn't own the Statable component.
        /// </exception>
        public ZeldaEntity Owner
        {
            get 
            {
                return this.owner;
            }

            set
            {
                if( value != null )
                {
                    var statable = value.Components.Find<Statable>();
                    if( statable == null )
                        throw new Atom.Components.ComponentNotFoundException( typeof( Statable ) );

                    var attackable = value.Components.Get<Attackable>();
                    if( attackable == null )
                        throw new Atom.Components.ComponentNotFoundException( typeof( Attackable ) );

                    this.owner = value;
                    this.statable = statable;
                    this.attackable = attackable;
                    this.transform = value.Transform;
                }
                else
                {
                    this.owner = null;
                    this.statable = null;
                    this.transform = null;
                    this.attackable = null;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Attackable"/> component of the Owner of this Attack.
        /// </summary>
        public Attackable OwnerAttackable
        {
            get
            {
                return this.attackable;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="AttackDamageMethod"/> which is used
        /// to calculate the damage done by this <see cref="Attack"/>.
        /// </summary>
        public AttackDamageMethod DamageMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IAttackLimiter"/> that is used to limit this Attack.
        /// </summary>
        public IAttackLimiter Limiter
        {
            get
            {
                return this.limiter;
            }

            set
            {
                this.limiter = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Attack"/> the attack is currently in use.
        /// </summary>
        public bool IsAttacking 
        { 
            get 
            {
                return !this.IsReady;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Attack"/> can be used;
        /// regarding the state of its cooldown.
        /// </summary>
        public bool IsReady
        {
            get
            {
                return this.Limiter.IsAllowed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Attack"/> is useable depending on the state of its owner.
        /// E.g. one usually can't use an attack while swimming, or if there is not enough mana to use it.
        /// </summary>
        public virtual bool IsUseable
        {
            get 
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the <see cref="Statable"/> component of the <see cref="ZeldaEntity"/>
        /// that owns this <see cref="Attack"/>.
        /// </summary>
        protected Statable Statable
        {
            get { return this.statable; }
        }

        /// <summary>
        /// Gets the <see cref="ZeldaTransform"/> component of the <see cref="ZeldaEntity"/>
        /// that owns this <see cref="Attack"/>.
        /// </summary>
        protected ZeldaTransform Transform
        {
            get { return this.transform; }
        }

        /// <summary>
        /// Gets the object that provides fast access to game-related services.
        /// </summary>
        protected IZeldaServiceProvider ServiceProvider
        {
            get
            {
                return this.serviceProvider;
            }
        }

        /// <summary>
        /// Gets a random number generator.
        /// </summary>
        protected Atom.Math.RandMT Rand
        {
            get
            {
                return serviceProvider.Rand;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Attack"/> class.
        /// </summary>
        /// <param name="owner">
        /// The entity that owns the new ZeldaEntity.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod that calculates the damage the new Attack does. 
        /// </param>
        protected Attack( ZeldaEntity owner, AttackDamageMethod method )
        {
            this.Owner        = owner;
            this.DamageMethod = method;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Fires the attack at the given <paramref name="target"/>,
        /// if possible.
        /// </summary>
        /// <param name="target">
        /// The target of the attack.
        /// </param>
        /// <returns>
        /// true if the Attack was executed, otherwise false.
        /// </returns>
        public abstract bool Fire( Attackable target );

        /// <summary>
        /// Updates this <see cref="Attack"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
            this.Limiter.Update( updateContext );
        }

        /// <summary>
        /// Setups this <see cref="Attack"/>
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        public virtual void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Called when this Attack has been fired against the specified target.
        /// </summary>
        /// <param name="target">
        /// The traget of the attack.
        /// </param>
        protected virtual void OnFiredAgainst( Attackable target )
        {
            this.OnFired();
            this.limiter.OnAttackHit( target.Statable );
        }

        /// <summary>
        /// Called when this Attack ist just firing; e.g. before all possible targets have been checked.
        /// </summary>
        protected virtual void OnFiring()
        {
            this.attackable.NotifyFiring(this);
        }


        /// <summary>
        /// Called when this Attack has been fired; e.g. after all possible targets have been checked.
        /// </summary>
        protected virtual void OnFired()
        {
            this.attackable.NotifyFired(this);
            this.Limiter.OnAttackFired();
        }

        /// <summary>
        /// Gets a value indicating whether this Attack should fire
        /// against the specified target.
        /// </summary>
        /// <param name="target">
        /// The traget of the attack.
        /// </param>
        /// <returns>
        /// true if it can and should fire;
        /// otherwise false.
        /// </returns>
        protected virtual bool ShouldFireAgainst( Attackable target )
        {
            if( this.ShouldFire() )
            {
                return this.limiter.IsAllowedOn( target.Statable );
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this Attack should fire.
        /// </summary>
        /// <returns>
        /// true if it can and should fire;
        /// otherwise false.
        /// </returns>
        protected virtual bool ShouldFire()
        {
            return this.Limiter.IsAllowed;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// the <see cref="IAttackLimiter"/> that is used to limit this Attack.
        /// </summary>
        private IAttackLimiter limiter = StubAttackLimiter.Instance;

        /// <summary>
        /// The ZeldaEntity that owns this <see cref="Attack"/>.
        /// </summary>
        private ZeldaEntity owner;

        /// <summary>
        /// Identifies the <see cref="Statable"/> component of the <see cref="ZeldaEntity"/>
        /// that owns this <see cref="Attack"/>.
        /// </summary>
        private Statable statable;

        /// <summary>
        /// Identifies the <see cref="ZeldaTransform"/> component of the <see cref="ZeldaEntity"/>
        /// that owns this <see cref="Attack"/>.
        /// </summary>
        private ZeldaTransform transform;

        /// <summary>
        /// Idenitfies the Attackable component of the Owner of this Attack.
        /// </summary>
        private Attackable attackable;

        /// <summary>
        /// Provides fast-access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;              

        #endregion
    }
}