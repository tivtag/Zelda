// <copyright file="BossPolarity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.Bosses.HellChicken.BossPolarity class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours.Bosses.HellChicken
{
    using Atom.Math;
    using Zelda.Status;
    
    /// <summary>
    /// Represents a polarity of the Chicken Hell boss.
    /// </summary>
    /// <remarks>
    /// If the boss changes to a specific polarity then
    /// he takes extra damage towards that polarity.
    /// </remarks>
    internal class BossPolarity : IZeldaUpdateable
    {
        /// <summary>
        /// Gets the color of this BossPolarity.
        /// </summary>
        /// <remarks>Used for tinting the boss.</remarks>
        public Vector4 Color
        {
            get
            {
                return this.color;
            }
        }

        /// <summary>
        /// Gets the hell-chicken boss object that owns this BossPolarity.
        /// </summary>
        protected Enemy Boss
        {
            get
            {
                return this.boss;
            }
        }

        /// <summary>
        /// Initializes a new instance of the BossPolarity class.
        /// </summary>
        /// <param name="color">
        /// The color of the new BossPolarity.
        /// </param>
        /// <param name="aura">
        /// The effect the new BossPolarity has on the boss.
        /// </param>
        /// <param name="boss">
        /// Identifies the hell-chicken boss object that owns the new BossPolarity.
        /// </param>
        public BossPolarity( Vector4 color, PermanentAura aura, Enemy boss )
        {
            this.color = color;
            this.aura = aura;
            this.boss = boss;
        }

        /// <summary>
        /// Enables this BossPolarity.
        /// </summary>
        public virtual void Enable()
        {
            this.boss.Statable.AuraList.Add( this.aura );
        }

        /// <summary>
        /// Disables this BossPolarity.
        /// </summary>
        public virtual void Disable()
        {
            this.boss.Statable.AuraList.Remove( this.aura );
        }

        /// <summary>
        /// Updates this BossPolarity.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
            // no op.
        }

        /// <summary>
        /// The color of this BossPolarity.
        /// </summary>
        private readonly Vector4 color;

        /// <summary>
        /// Identifies the hell-chicken boss object that owns this BossPolarity.
        /// </summary>
        private readonly Enemy boss;

        /// <summary>
        /// The effect the BossPolarity has on the boss.
        /// </summary>
        private readonly PermanentAura aura;
    }
}
