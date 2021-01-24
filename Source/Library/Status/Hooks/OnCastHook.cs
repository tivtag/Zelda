// <copyright file="OnCastHook.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Hooks.OnCastHook class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Hooks
{ 
    using System;
    using Zelda.Casting;
    using Zelda.Entities;

    /// <summary>
    /// Defines an <see cref="IStatusHook"/> that hooks up with
    /// the Castable.Casted event.
    /// </summary>
    /// <remarks>
    /// No entity should be hooked/unhooked until the OnCastHook
    /// has been fully initialized.
    /// </remarks>
    public sealed class OnCastHook : StatusHook
    {
        /// <summary>
        /// Enumerates the different types of casting
        /// the OnCastHook can hook up with.
        /// </summary>
        public enum CastMode
        {
            /// <summary>
            /// The OnCastHook hooks up with the Castbar.Started event.
            /// </summary>
            CastStarted,

            /// <summary>
            /// The OnCastHook hooks up with the Castbar.Finished event.
            /// </summary>
            CastFinished
        }

        /// <summary>
        /// Gets or sets a value indicating what kinda of casting
        /// this OnCastHook should hook up with.
        /// </summary>
        public CastMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Hooks this StatusHook up with the given Statable.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity to hook up with.
        /// </param>
        public override void Hook( Statable statable )
        {
            var castbar = GetCastbar( statable );

            if( this.ShouldHookWithCastStarted() )
            {
                castbar.Started += this.OnCasted;
            }

            if( this.ShouldHookWithCastFinished() )
            {
                castbar.Finished += this.OnCasted;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this StatusHook
        /// should hook up with the Castbar.Started event.
        /// </summary>
        /// <returns>
        /// true if the events should be hooked-up;
        /// otherwise false.
        /// </returns>
        private bool ShouldHookWithCastStarted()
        {
            return this.Mode == CastMode.CastStarted;
        }

        /// <summary>
        /// Gets a value indicating whether this StatusHook
        /// should hook up with the Castbar.Finished event.
        /// </summary>
        /// <returns>
        /// true if the events should be hooked-up;
        /// otherwise false.
        /// </returns>
        private bool ShouldHookWithCastFinished()
        {
            return this.Mode == CastMode.CastFinished;
        }

        /// <summary>
        /// Called when a cast has either completed or started.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="spell">The spell that has been casted/is casting.</param>
        private void OnCasted( CastBar sender, Spell spell )
        {
            this.OnInvoked( sender.Statable );
        }

        /// <summary>
        /// Unhooks this StatusHook from the given Statable.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity to unhook from.
        /// </param>
        public override void Unhook( Statable statable )
        {
            var castbar = GetCastbar( statable );

            if( this.ShouldHookWithCastStarted() )
            {
                castbar.Started -= this.OnCasted;
            }

            if( this.ShouldHookWithCastFinished() )
            {
                castbar.Finished -= this.OnCasted;
            }
        }

        /// <summary>
        /// Gets the CastBar associated with the given
        /// Statable component.
        /// </summary>
        /// <param name="statable">
        /// The Statable component of the ZeldaEntity that hooks up with this IStatusHook.
        /// </param>
        /// <returns>
        /// The CastBar of the given statable enetity..
        /// </returns>
        private static CastBar GetCastbar( Statable statable )
        {
            var owner = (PlayerEntity)statable.Owner;
            var castable = owner.Castable;
            return castable.CastBar;
        }

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
            switch( this.Mode )
            {
                case CastMode.CastStarted:
                    return Resources.SpellCast;

                case CastMode.CastFinished:
                    return Resources.FinishedSpellCast;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns a clone of this StatusHook.
        /// </summary>
        /// <returns>
        /// The cloned IStatusHook.
        /// </returns>
        public override IStatusHook Clone()
        {
            return new OnCastHook() {
                Mode = this.Mode
            };
        }

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

            context.Write( (byte)this.Mode );
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

            this.Mode = (CastMode)context.ReadByte();
        }
    }
}
