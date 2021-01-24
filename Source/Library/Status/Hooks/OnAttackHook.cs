// <copyright file="OnAttackHook.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Hooks.OnAttackHook class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Hooks
{
    using System;
    using Zelda.Attacks;

    /// <summary>
    /// Defines an <see cref="IStatusHook"/> that hooks up with 
    /// <see cref="Statable.MeleeHit"/>, <see cref="Statable.RangedHit"/> and so on...
    /// </summary>
    /// <remarks>
    /// No entity should be hooked/unhooked until the OnAttackHook
    /// has been fully initialized.
    /// </remarks>
    public sealed class OnAttackHook : StatusHook
    {
        /// <summary>
        /// Gets or sets what kind of attacks this OnAttackHook 
        /// should be hooked up with.
        /// </summary>
        public AttackType AttackType
        {
            get 
            {
                return this.attackType;
            }

            set
            {
                if( value == AttackType.Spell )
                {
                    throw new ArgumentException( "It is not possible to hook up with Spell attacks.", "value" );
                }

                this.attackType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this OnAttackHook should
        /// hook up with crits, hits or both.
        /// </summary>
        public HitCritHookMode HookMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this OnAttackHook hooks up with hits.
        /// </summary>
        /// <returns>
        /// Returns true if it hooks up with attacks that hit;
        /// otherwise false.
        /// </returns>
        private bool HooksUpWithHits()
        {
            return this.HookMode != HitCritHookMode.OnlyCrit;
        }
        
        /// <summary>
        /// Gets a value indicating whether this OnAttackHook hooks up with crits.
        /// </summary>
        /// <returns>
        /// Returns true if it hooks up with attacks that crit;
        /// otherwise false.
        /// </returns>
        private bool HooksUpWithCrits()
        {
            return this.HookMode != HitCritHookMode.OnlyHit;
        }

        #region > Hook <

        /// <summary>
        /// Hooks this OnAttackHook up with the given Statable.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity to hook up with.
        /// </param>
        public override void Hook( Statable statable )
        {
            switch( this.AttackType )
            {
                case AttackType.Melee:
                    this.HookMelee( statable );
                    break;

                case AttackType.Ranged:
                    this.HookRanged( statable );
                    break;

                case AttackType.All:
                    this.HookMelee( statable );
                    this.HookRanged( statable );
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Hooks this OnAttackHook up with the melee hit/crit.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity to hook up with.
        /// </param>
        private void HookMelee( Statable statable )
        {
            if( this.HooksUpWithHits() )
            {
                statable.MeleeHit += this.OnAttack;
                statable.DefaultMeleeHit += this.OnAttackWithArgs;
            }

            if( this.HooksUpWithCrits() )
            {
                statable.MeleeCrit += this.OnAttack;
            }
        }

        /// <summary>
        /// Hooks this OnAttackHook up with the ranged hit/crit.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity to hook up with.
        /// </param>
        private void HookRanged( Statable statable )
        {
            if( this.HooksUpWithHits() )
            {
                statable.RangedHit += this.OnAttack;
                statable.DefaultRangedHit += this.OnAttackWithArgs;
            }

            if( this.HooksUpWithCrits() )
            {
                statable.RangedCrit += this.OnAttack;
            }
        }

        #endregion

        #region > Unhook <

        /// <summary>
        /// Unhooks this OnAttackHook from the given Statable.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity to unhook from.
        /// </param>
        public override void Unhook( Statable statable )
        {
            switch( this.AttackType )
            {
                case AttackType.Melee:
                    this.UnhookMelee( statable );
                    break;

                case AttackType.Ranged:
                    this.UnhookRanged( statable );
                    break;

                case AttackType.All:
                    this.UnhookMelee( statable );
                    this.UnhookRanged( statable );
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Unhook this OnAttackHook up with the melee hit/crit.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity to hook up with.
        /// </param>
        private void UnhookMelee( Statable statable )
        {
            if( this.HooksUpWithHits() )
            {
                statable.MeleeHit -= this.OnAttack;
                statable.DefaultMeleeHit -= this.OnAttackWithArgs;
            }

            if( this.HooksUpWithCrits() )
            {
                statable.MeleeCrit -= this.OnAttack;
            }
        }

        /// <summary>
        /// Unhook this OnAttackHook up with the ranged hit/crit.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity to hook up with.
        /// </param>
        private void UnhookRanged( Statable statable )
        {
            if( this.HooksUpWithHits() )
            {
                statable.RangedHit -= this.OnAttack;
                statable.DefaultRangedHit -= this.OnAttackWithArgs;
            }

            if( this.HooksUpWithCrits() )
            {
                statable.RangedCrit -= this.OnAttack;
            }
        }

        #endregion

        #region > Events <

        /// <summary>
        /// Called when damage has been received by a statable entity 
        /// that has been hooked up with this OnAttackHook.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnAttack( Statable sender )
        {
            this.OnInvoked( sender );
        }

        /// <summary>
        /// Called when damage has been received by a statable entity 
        /// that has been hooked up with this OnAttackHook.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The CombatEventArgs that contain the event data.
        /// </param>
        private void OnAttackWithArgs( Statable sender, CombatEventArgs e )
        {
            this.OnInvoked( sender );
        }

        #endregion

        #region > Description <

        /// <summary>
        /// Gets a short description of this IStatusHook.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity that hooks up with this IStatusHook.
        /// </param>
        /// <returns>
        /// A short string that is descriping with what this IStatusHook is hooking up.
        /// </returns>
        public override string GetShortDescription( Statable statable )
        {
            switch( this.AttackType )
            {
                case AttackType.Melee:
                    return this.GetShortMeleeDescription();
                    
                case AttackType.Ranged:
                    return this.GetShortRangedDescription();

                case AttackType.All:
                    return this.GetShortDescriptionForAll();

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets a short description of this IStatusHook; in the case of AttackType.Melee.
        /// </summary>
        /// <returns>
        /// A localized string that is displayed to the player.
        /// </returns>
        private string GetShortMeleeDescription()
        {
            switch( this.HookMode )
            {
                case HitCritHookMode.OnlyHit:
                    return Resources.MeleeHit;

                case HitCritHookMode.OnlyCrit:
                    return Resources.MeleeCrit;

                default:
                case HitCritHookMode.HitAndCrit:
                    return Resources.MeleeStrike;
            }
        }

        /// <summary>
        /// Gets a short description of this IStatusHook; in the case of a AttackType.Ranged.
        /// </summary>
        /// <returns>
        /// A localized string that is displayed to the player.
        /// </returns>
        private string GetShortRangedDescription()
        {
            switch( this.HookMode )
            {
                case HitCritHookMode.OnlyHit:
                    return Resources.RangedHit;

                case HitCritHookMode.OnlyCrit:
                    return Resources.RangedCrit;

                default:
                case HitCritHookMode.HitAndCrit:
                    return Resources.RangedStrike;
            }
        }
        
        /// <summary>
        /// Gets a short description of this IStatusHook; in the case of AttackType.All.
        /// </summary>
        /// <returns>
        /// A localized string that is displayed to the player.
        /// </returns>
        private string GetShortDescriptionForAll()
        {
            switch( this.HookMode )
            {
                case HitCritHookMode.OnlyHit:
                    return Resources.MeleeRangedHit;

                case HitCritHookMode.OnlyCrit:
                    return Resources.MeleeRangedCrit;

                default:
                case HitCritHookMode.HitAndCrit:
                    return Resources.MeleeRangedStrike;
            }
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( (byte)this.attackType );
            context.Write( (byte)this.HookMode );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.attackType = (AttackType)context.ReadByte();
            this.HookMode = (HitCritHookMode)context.ReadByte();
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this OnAttackHook.
        /// </summary>
        /// <returns>
        /// The cloned IStatusHook.
        /// </returns>
        public override IStatusHook Clone()
        {
            return new OnAttackHook() {
                 AttackType = this.AttackType,
                 HookMode = this.HookMode
            };
        }

        #endregion

        /// <summary>
        /// The storage field of the <see cref="AttackType"/> property.
        /// </summary>
        private AttackType attackType = AttackType.Melee;
    }
}