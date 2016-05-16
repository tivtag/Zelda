// <copyright file="ColorTintList.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Tinting.ColorTintList class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Graphics.Tinting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Atom.Math;
    using Atom.Xna;
    using Xna = Microsoft.Xna.Framework;
    
    /// <summary>
    /// Represents a list of <see cref="IColorTint"/> instances that
    /// are combined to tint a Xna.Color.
    /// This class can't be inherited.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public sealed class ColorTintList : IZeldaUpdateable
    {
        /// <summary>
        /// Applies the combined color tinting of this ColorTintList to
        /// the given <paramref name="baseColor"/>.
        /// </summary>
        /// <param name="baseColor">
        /// The Xna.Color the tinting should be based on.
        /// </param>
        /// <returns>
        /// The tinted Xna.Color.
        /// </returns>
        public Xna.Color ApplyTinting( Xna.Color baseColor )
        {
            if( this.list.Count == 0 )
                return baseColor;

            Vector4 tintedColor = baseColor.ToVector4().ToAtom();

            for( int i = 0; i < this.list.Count; ++i )
            {
                tintedColor = this.list[i].Apply( tintedColor );
            }

            return new Xna.Color( tintedColor.ToXna() );
        }

        /// <summary>
        /// Updates this ColorTintList.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            for( int i = 0; i < this.list.Count; i++ )
            {
                this.list[i].Update( updateContext );
            }
        }

        /// <summary>
        /// Adds the given <see cref="IColorTint"/> instance to this ColorTintList.
        /// </summary>
        /// <param name="colorTint">
        /// The IColorTint instance to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="colorTint"/> is null.
        /// </exception>
        public void Add( IColorTint colorTint )
        {
            if( colorTint == null )
                throw new ArgumentNullException( "colorTint" );
            
            this.list.Add( colorTint );
        }

        /// <summary>
        /// Tries to remove the given <see cref="IColorTint"/> instance from this ColorTintList.
        /// </summary>
        /// <param name="colorTint">
        /// The IColorTint instance to remove.
        /// </param>
        /// <returns>
        /// Whether the given <see cref="IColorTint"/> instance has been removed.
        /// </returns>
        public bool Remove( IColorTint colorTint )
        {
            return this.list.Remove( colorTint );
        }

        /// <summary>
        /// Removes all IColorTints from this ColorTintList.
        /// </summary>
        internal void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// The internal list that keeps track of the IColorTint instances
        /// this ColorTintList consists of.
        /// </summary>
        private readonly List<IColorTint> list = new List<IColorTint>( 0 );
    }
}
