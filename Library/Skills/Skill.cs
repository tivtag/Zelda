// <copyright file="Skill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Skill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills
{
    using System;
    using System.Diagnostics.Contracts;
    using Atom;
    using Atom.Xna;
    using Zelda.Status;

    /// <summary>
    /// A <see cref="Skill"/> is an action controlled by the Player.
    /// </summary>
    public abstract class Skill : IDescriptionProvider
    {
        #region [ Events ]

        /// <summary>
        /// Raised when this Skill has been succesfully fired.
        /// </summary>
        public event SimpleEventHandler<Skill> Fired;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the localized name of this <see cref="Skill"/>.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return this.localizedName;
            }
        }

        /// <summary>
        /// Gets the localized description of this <see cref="Skill"/>.
        /// </summary>
        public abstract string Description
        {
            get;
        }

        /// <summary>
        /// Gets the symbol of this <see cref="Skill"/>.
        /// </summary>
        public Sprite Symbol
        {
            get
            {
                return this.symbol;
            }
        }

        /// <summary>
        /// Gets the cooldown of this <see cref="Skill"/>.
        /// </summary>
        public Cooldown Cooldown
        {
            get
            {
                return this.cooldown;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Skill"/> is useable 
        /// depending on the current state of the user.
        /// </summary>
        /// <remarks>
        /// The cooldown is not taken into account.
        /// </remarks>
        public abstract bool IsUseable
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Skill"/> is only limited by its own cooldown
        /// and not such things as location/mana cost/etc.
        /// </summary>
        /// <value>The default value is false.</value>
        public virtual bool IsOnlyLimitedByCooldown
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Skill"/> is currently inactive;
        /// and as such unuseable.
        /// </summary>
        /// <value>The default value is false.</value>
        public virtual bool IsInactive
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the IManaCost of this Skill.
        /// </summary>
        public Zelda.Status.Cost.IManaCost Cost
        {
            get
            {
                return this.manaCost;
            }

            protected set
            {
                Contract.Requires<ArgumentNullException>( value != null );

                this.manaCost = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the owner of this Skill has
        /// enought mana to use it.
        /// </summary>
        protected bool HasRequiredMana
        {
            get
            {
                return this.Cost.Has( this.statable );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Skill"/> class.
        /// </summary>
        /// <param name="localizedName">
        /// The localized name of the new Skill.
        /// </param>
        /// <param name="cooldown">
        /// The cooldown on the new Skill.
        /// </param>
        /// <param name="symbol">
        /// The symbol displayed for the new Skill.
        /// </param>
        /// <param name="statable">
        /// The statable component of the entity that wants to own the new Skill.
        /// </param>
        protected Skill( string localizedName, Cooldown cooldown, Sprite symbol, Statable statable )
        {
            this.symbol        = symbol;
            this.cooldown      = cooldown;
            this.localizedName = localizedName;
            this.statable = statable;
        }

        /// <summary>
        /// Initializes this Skill.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Uninitializes this Skill.
        /// </summary>
        public virtual void Uninitialize()
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Uses this <see cref="Skill"/>.
        /// </summary>
        /// <returns>
        /// true if this Skill has been used;
        /// otherwise false.
        /// </returns>
        public bool Use()
        {
            if( this.CanUse() )
            {
                if( this.Fire() )
                {
                    this.OnFiredCore();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Fires this Skill.
        /// </summary>
        /// <remarks>
        /// At this points the Skill has been checked for beeing
        /// useable and that the owner of the skill fulfills the mana cost.
        /// </remarks>
        /// <returns>
        /// true if this Skill has been fired;
        /// -or- otherwise false.
        /// </returns>
        protected abstract bool Fire();

        /// <summary>
        /// Called when this Skill has been succesfully fired.
        /// </summary>
        private void OnFiredCore()
        {
            this.Cost.Apply( this.statable );
            this.OnFired();
            this.Fired.Raise( this );
        }

        /// <summary>
        /// Called when this Skill has been succesfully fired.
        /// Must not be called when overriden.
        /// </summary>
        protected virtual void OnFired()
        {
        }

        /// <summary>
        /// Gets a value indicating whether this Skill can currently be used.
        /// </summary>
        /// <returns>
        /// true if it can be used;
        /// otherwise false.
        /// </returns>
        private bool CanUse()
        {
            if( this.IsUseable && this.HasRequiredMana )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> 
        /// Refreshes the data from talents that modify this <see cref="Skill"/>'s power. 
        /// </summary>
        public abstract void RefreshDataFromTalents();

        /// <summary>
        /// Updates this <see cref="Skill"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
            this.Cooldown.Update( updateContext.FrameTime );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The cost of this Skill.
        /// </summary>
        private Zelda.Status.Cost.IManaCost manaCost = ManaCost.None;

        /// <summary>
        /// The symbol that is displayed in the Skill Bar for this Skill.
        /// </summary>
        private readonly Sprite symbol;

        /// <summary>
        /// The cooldown on this Skill.
        /// </summary>
        private readonly Cooldown cooldown;

        /// <summary>
        /// The localized name of this Skill.
        /// </summary>
        private readonly string localizedName;

        /// <summary>
        /// The statable component of the entity that owns this Skill.
        /// </summary>
        private readonly Status.Statable statable;

        #endregion
    }
}
