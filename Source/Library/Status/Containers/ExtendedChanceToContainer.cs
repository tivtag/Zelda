// <copyright file="ExtendedChanceToContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Containers.ExtendedChanceToContainer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Containers
{
    using System;
    
    /// <summary>
    /// Encapsulates various status properties with a chance;
    /// such as chance to parry, chance to miss or even chance to pierce.
    /// </summary>
    public sealed class ExtendedChanceToContainer : ChanceToContainer
    {
        /// <summary>
        /// Initializes a new instance of the ExtendedChanceToContainer class.
        /// </summary>
        internal ExtendedChanceToContainer()
        {
        }

        /// <summary>
        /// Initializes this ExtendedChanceToContainer to be used
        /// for the specified Statable.
        /// </summary>
        /// <param name="statable">
        /// The Statable component that will own the ExtendedChanceToContainer.
        /// </param>
        public override void Initialize( Statable statable )
        {
            this.statable = (ExtendedStatable)statable;
            base.Initialize( statable );
        }

        /// <summary>
        /// Refreshes all chance-to properties of this ExtendedChanceToContainer.
        /// </summary>
        internal void Refresh()
        {
            this.RefreshBlock();
            this.RefreshCrit();
            this.RefreshDodge();
            this.RefreshMiss();
            this.RefreshParry();
            this.RefreshPierce();
        }
        
        /// <summary>
        /// Refreshes the chance for the specified ChanceToStatus.
        /// </summary>
        /// <param name="statusType">
        /// The ChanceToStatus to refresh.
        /// </param>
        internal void Refresh( ChanceToStatus statusType )
        {
            switch( statusType )
            {
                case ChanceToStatus.Block:
                    this.RefreshBlock();
                    break;

                case ChanceToStatus.CritBlock:
                    this.RefreshCritBlock();
                    break;
                    
                case ChanceToStatus.Crit:
                    this.RefreshCrit();
                    break;

                case ChanceToStatus.CritHeal:
                    this.RefreshCritHeal();
                    break;

                case ChanceToStatus.Dodge:
                    this.RefreshDodge();
                    break;

                case ChanceToStatus.Miss:
                    this.RefreshMiss();
                    break;

                case ChanceToStatus.Parry:
                    this.RefreshParry();
                    break;

                case ChanceToStatus.Pierce:
                    this.RefreshPierce();
                    break;

                case ChanceToStatus.None:
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Refreshes the chance to get a critical attack.
        /// </summary>
        internal void RefreshCrit()
        {
            float fixedValue, mulValue, ratingValue;
            this.AuraList.GetEffectValues( ChanceToStatusEffect.IdentifierCrit, out fixedValue, out mulValue, out ratingValue );

            this.Crit = StatusCalc.GetChanceToCrit(
                this.statable.Luck,
                fixedValue,
                mulValue, 
                ratingValue,
                this.statable.Level 
            );

            this.RefreshCritHeal();
            this.RefreshCritBlock();
        }

        /// <summary>
        /// Refreshes the chance to dodge an incomming attack.
        /// </summary>
        internal void RefreshDodge()
        {
            float fixedValue, mulValue, ratingValue;
            this.AuraList.GetEffectValues( ChanceToStatusEffect.IdentifierDodge, out fixedValue, out mulValue, out ratingValue );

            this.Dodge = StatusCalc.GetChanceToDodge( 
                this.statable.Agility, 
                fixedValue,
                mulValue,
                ratingValue,
                this.statable.Level
            );
        }

        /// <summary>
        /// Refreshes the chance to parry an incomming attack.
        /// </summary>
        internal void RefreshParry()
        {
            float fixedValue, mulValue, ratingValue;
            this.AuraList.GetEffectValues( ChanceToStatusEffect.IdentifierParry, out fixedValue, out mulValue, out ratingValue );

            this.Parry = StatusCalc.GetChanceToParry(
                this.statable.Strength,
                fixedValue,
                mulValue, 
                ratingValue,
                this.statable.Level 
            );
        }

        /// <summary>
        /// Refreshes the chance to miss with an outgoing attack.
        /// </summary>
        internal void RefreshMiss()
        {
            float fixedValue, percentalValue, ratingValue;
            this.AuraList.GetEffectValues( ChanceToStatusEffect.IdentifierMiss, out fixedValue, out percentalValue, out ratingValue );

            this.Miss = StatusCalc.GetChanceToMiss( 
                this.statable.Dexterity,
                fixedValue, 
                percentalValue,
                ratingValue,
                this.statable.Level
            );
        }

        /// <summary>
        /// Refreshes the chance to get a critical heal.
        /// </summary>
        internal void RefreshCritHeal()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( ChanceToStatusEffect.IdentifierCritHeal, out addValue, out mulValue );

            this.CritHeal = ((this.Crit / 2.0f) + addValue) * mulValue;
        }

        /// <summary>
        /// Refreshes the chance to block an incomming attack.
        /// </summary>
        internal void RefreshBlock()
        {
            float fixedValue, percentValue, ratingValue;
            this.AuraList.GetEffectValues( ChanceToStatusEffect.IdentifierBlock, out fixedValue, out percentValue, out ratingValue );

            this.Block = StatusCalc.GetChanceToBlock( 
                fixedValue,
                percentValue,
                ratingValue,
                this.statable.Level
            );
        }

        /// <summary>
        /// Refreshes the chance to get a critical block.
        /// </summary>
        internal void RefreshCritBlock()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( ChanceToStatusEffect.IdentifierCritBlock, out addValue, out mulValue );

            this.CritBlock = ((this.Crit / 2.0f) + addValue) * mulValue;
        }

        /// <summary>
        /// Refreshes the chance to pierce with a ranged Projectile.
        /// </summary>
        internal void RefreshPierce()
        {
            float fixedValue, percentalValue, ratingValue;
            this.AuraList.GetEffectValues( ChanceToStatusEffect.IdentifierPierce, out fixedValue, out percentalValue, out ratingValue );

            this.Pierce = StatusCalc.GetChanceToPierce( fixedValue, percentalValue, ratingValue, this.statable.Level );
        }

        /// <summary>
        /// Identifies the ExtendedStatable component that owns this ExtendedChanceToContainer.
        /// </summary>
        private ExtendedStatable statable;
    }
}