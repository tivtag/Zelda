// <copyright file="ExtendedChanceToBeContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Containers.ExtendedChanceToBeContainer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Containers
{
    using System;

    /// <summary>
    /// Encapsulates various status properties with a chance;
    /// such as chance-to-be parry or chance-to-be hit.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ExtendedChanceToBeContainer : ChanceToBeContainer
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the extra chance that attacks of the owner of this ExtendedChanceToBeContainer
        /// are resisted.
        /// </summary>
        /// <seealso cref="SpellPenetrationEffect"/>
        public float Resisted
        {
            get
            {
                return this.resisted;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the ExtendedChanceToBeContainer class.
        /// </summary>
        internal ExtendedChanceToBeContainer()
        {
        }
        
        /// <summary>
        /// Initializes this ExtendedChanceToBeContainer to be used
        /// for the specified Statable.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable component that will own the ExtendedChanceToBeContainer.
        /// </param>
        public override void Initialize( Statable statable )
        {
            this.statable = (ExtendedStatable)statable;
            base.Initialize( statable );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes all chance-to-be values stored in this ExtendedChanceToBeContainer.
        /// </summary>
        internal void Refresh()
        {
            this.RefreshCrit();
            this.RefreshHit();
            this.RefreshResisted();
        }

        /// <summary>
        /// Refreshes the extra chance to be hit by an enemy attack.
        /// </summary>
        internal override void RefreshHit()
        {
            float fixedValue, percentValue, ratingValue;
            this.statable.AuraList.GetEffectValues( 
                ChanceToBeStatusEffect.IdentifierToBeHit, 
                out fixedValue,
                out percentValue,
                out ratingValue
            );

            this.Hit = StatusCalc.GetChanceToBeHit( fixedValue, percentValue, ratingValue, this.statable.Level );
        }

        /// <summary>
        /// Refreshes the extra chance to be crit by an enemy attack.
        /// </summary>
        internal override void RefreshCrit()
        {
            float fixedValue = this.statable.AuraList.GetFixedEffectValue( 
                ChanceToBeStatusEffect.IdentifierToBeCrit 
            );

            this.Crit = StatusCalc.GetChanceToBeCrit( this.statable.Vitality, fixedValue );
        }

        /// <summary>
        /// Refreshes the extra chance for (magical) attacks to be resisted.
        /// </summary>
        /// <seealso cref="SpellPenetrationEffect"/>
        internal void RefreshResisted()
        {
            float fixedValue, percentalValue, ratingValue;            
            this.statable.AuraList.GetEffectValues(
                 SpellPenetrationEffect.IdentifierString,
                 out fixedValue,
                 out percentalValue,
                 out ratingValue
            );

            this.resisted = StatusCalc.GetChanceToBeResisted( fixedValue, ratingValue, percentalValue, this.statable.Level );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The extra chance that attacks of the owner of this ExtendedChanceToBeContainer are resisted.
        /// </summary>
        private float resisted;

        /// <summary>
        /// Identifies the ExtendedStatable component that owns this ExtendedChanceToBeContainer.
        /// </summary>
        private ExtendedStatable statable;

        #endregion
    }
}
