// <copyright file="IncreaseSizeAndStrengthOnPolarityChangeBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.Bosses.HellChicken.IncreaseSizeAndStrengthOnPolarityChangeBehaviour class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours.Bosses.HellChicken
{
    using System;
    using Atom.Math;

    /// <summary>
    /// Defines the behaviour that increases the size of the Chicken Hell
    /// boss when he changes polarity.
    /// </summary>
    internal sealed class IncreaseSizeAndStrengthOnPolarityChangeBehaviour
    {
        /// <summary>
        /// The amount the scaling factor increases when the polarity changes.
        /// </summary>
        private static readonly Vector2 ScaleIncrease = new Vector2( 0.05f, 0.05f );

        /// <summary>
        /// The amount of damage the boss does additionaly when his polarity changes.
        /// </summary>
        private const int DamageIncrease = 3;

        /// <summary>
        /// Initializes a new instance of the IncreaseSizeAndStrengthOnPolarityChangeBehaviour class.
        /// </summary>
        /// <param name="polarityBehaviour">
        /// The PolarityBehaviour to hook onto.
        /// </param>
        /// <param name="boss">
        /// The boss that is controlled by this IncreaseSizeAndStrengthOnPolarityChangeBehaviour.
        /// </param>
        public IncreaseSizeAndStrengthOnPolarityChangeBehaviour( PolarityBehaviour polarityBehaviour, Enemy boss )
        {
            this.boss = boss;
            this.initialDamage = new IntegerRange( this.boss.Statable.DamageMeleeMin, this.boss.Statable.DamageMeleeMax );

            // Hook events.
            polarityBehaviour.PolarityChanged += this.OnPolarityChanged;
        }

        /// <summary>
        /// Called when the polarity of the boss has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contain the event data.</param>
        private void OnPolarityChanged( object sender, EventArgs e )
        {
            if( this.ShouldStillIncreaseSize() )
            {
                this.boss.Transform.Scale += ScaleIncrease;
                this.boss.Statable.DamageMeleeMin += DamageIncrease;
                this.boss.Statable.DamageMeleeMax += DamageIncrease;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the boss should still increase in size.
        /// </summary>
        /// <returns>
        /// true if the boss hasn't reached the size limit yet; 
        /// otherwise false.
        /// </returns>
        private bool ShouldStillIncreaseSize()
        {
            return boss.Transform.Scale.X <= 3.5f;
        }

        /// <summary>
        /// Resets the increased strength and size of the boss.
        /// </summary>
        internal void Reset()
        {
            this.boss.Statable.DamageMeleeMin = initialDamage.Minimum;
            this.boss.Statable.DamageMeleeMax = initialDamage.Maximum;
            this.boss.Transform.Scale = Vector2.One;
        }

        /// <summary>
        /// Stores the initial damage of the boss.
        /// </summary>
        private readonly IntegerRange initialDamage;

        /// <summary>
        /// The boss that is controlled by this SpawnAddsOnPolarityChangeBehaviour.
        /// </summary>
        private readonly Enemy boss;
    }
}
