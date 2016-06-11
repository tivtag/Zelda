// <copyright file="StatusSetBonus.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Sets.StatusSetBonus class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Sets
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Zelda.Status;

    /// <summary>
    /// Represents an <see cref="ISetBonus"/> that provides an arabitary list
    /// of <see cref="StatusEffect"/>s.
    /// </summary>
    public sealed class StatusSetBonus : IStatusSetBonus
    {
        /// <summary>
        /// Gets a value indicating whether this ISetBonus is currently applied.
        /// </summary>
        [Browsable(false)]
        public bool IsApplied
        {
            get
            {
                return this.aura.IsEnabled;
            }
        }

        /// <summary>
        /// Gets the list of <see cref="StatusEffect"/> this StatusSetBonus provides.
        /// </summary>
        [Editor( "Zelda.Status.Design.StatusEffectListEditor, Library.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        public IList<Zelda.Status.StatusEffect> Effects
        {
            get 
            {
                return this.aura.Effects;
            }
        }

        /// <summary>
        /// Applies this ISetBonus to the specified ExtendedStatable component.
        /// </summary>
        /// <param name="statable">
        /// The statable component the bonus should be enabled for.
        /// </param>
        public void Enable( Zelda.Status.ExtendedStatable statable )
        {
            if( this.IsApplied )
                throw new InvalidOperationException( "The ISetBonus is already applied." );

            statable.AuraList.Add( this.aura );
        }

        /// <summary>
        /// Removes this ISetBonus from the ExtendedStatable 
        /// it was previously enabled for.
        /// </summary>
        public void Disable()
        {
            if( !this.IsApplied )
                throw new InvalidOperationException( "The ISetBonus has not been applied." );

            this.aura.AuraList.Remove( this.aura );
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            this.aura.Serialize( context );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.aura.Deserialize( context );
        }

        /// <summary>
        /// The PermanentAura that holds the StatusEffects that are applied
        /// by this StatusSetBonus.
        /// </summary>
        private readonly PermanentAura aura = new PermanentAura();
    }
}
