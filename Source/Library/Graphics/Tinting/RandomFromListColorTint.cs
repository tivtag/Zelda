// <copyright file="RandomFromListColorTint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Tinting.RandomFromListColorTint class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Tinting
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Atom.Math;
    using Zelda.Saving;

    /// <summary>
    /// Implements an IColorTint that completly replaces the input color
    /// by randomly picking from a list of colors.
    /// </summary>
    public sealed class RandomFromListColorTint : IColorTint, IZeldaSetupable
    {
        /// <summary>
        /// Gets the list of colors from which randomly a color is selected.
        /// </summary>
        [Editor( "Zelda.Graphics.Tinting.Design.ColorTintListEditor, Libary.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        public List<IColorTint> Colors
        {
            get
            {
                return this.colorTints;
            }
        }

        /// <summary>
        /// Setups this RandomFromListColorReplacementTint.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.rand = serviceProvider.Rand;
        }  

        /// <summary>
        /// Applies this IColorTint to the given color.
        /// </summary>
        /// <param name="color">
        /// The input color.
        /// </param>
        /// <returns>
        /// The output color.
        /// </returns>
        public Vector4 Apply( Vector4 color )
        {
            if( this.colorTints.Count == 0 )
                return color;

            int index = this.rand.RandomRange( 0, this.colorTints.Count - 1 );
            IColorTint tint = this.colorTints[index];

            return tint.Apply( color );
        }

        /// <summary>
        /// Updates this IZeldaUpdateable.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        void IZeldaUpdateable.Update( ZeldaUpdateContext updateContext )
        {
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

            context.Write( this.colorTints.Count );            
            for( int i = 0; i < this.colorTints.Count; ++i )
            {
                context.WriteObject( this.colorTints[i] );
            }
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

            int colorCount = context.ReadInt32();
            this.colorTints.Capacity = colorCount;
            
            for( int i = 0; i < colorCount; ++i )
            {
                this.colorTints.Add( context.ReadObject<IColorTint>() );
            }
        }

        /// <summary>
        /// The list of colors.
        /// </summary>
        private readonly List<IColorTint> colorTints = new List<IColorTint>();

        /// <summary>
        /// A random number generator.
        /// </summary>
        private RandMT rand;
    }
}
