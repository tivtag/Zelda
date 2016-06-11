// <copyright file="GemViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ItemCreator.GemViewModel class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.ItemCreator
{
    using System;
    using System.ComponentModel;
    using Zelda.Items;

    /// <summary>
    /// Defines the view-model/property wrapper around the <see cref="Gem"/> class.
    /// </summary>
    internal sealed class GemViewModel : ItemViewModel
    {
        #region [ Wrapped Properties ]

        [LocalizedCategory( "PropCate_Gem" )]
        [LocalizedDisplayName( "PropDisp_GemEffects" )]
        [LocalizedDescription( "PropDesc_GemEffects" )]
        public Zelda.Status.PermanentAura GemEffects
        {
            get { return this.WrappedGem.EffectAura; }
        }

        [LocalizedCategory( "PropCate_Gem" )]
        [LocalizedDisplayName( "PropDisp_GemColor" )]
        [LocalizedDescription( "PropDesc_GemColor" )]
        public Zelda.Status.ElementalSchool GemColor
        {
            get { return this.WrappedGem.GemColor; }
            set { this.WrappedGem.GemColor = value; }
        }

        [LocalizedCategory( "PropCate_Gem" )]
        [LocalizedDisplayName( "PropDisp_RequiredLevel" )]
        [LocalizedDescription( "PropDesc_RequiredLevel" )]
        public int RequiredLevel
        {
            get { return this.WrappedGem.RequiredLevel; }
            set { this.WrappedGem.RequiredLevel = value; }
        }

        #endregion

        #region [ Wrapper ]

        /// <summary>
        /// Gets the Gem this <see cref="EquipmentViewModel"/> wraps around.
        /// </summary>
        [Browsable( false )]
        public Gem WrappedGem
        {
            get
            {
                return (Gem)this.WrappedObject;
            }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> this <see cref="IObjectPropertyWrapper"/> wraps around.
        /// </summary>
        public override Type WrappedType
        {
            get
            {
                return typeof( Gem );
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GemViewModel"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        internal GemViewModel( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
        }

        /// <summary>
        /// Creates a clone of this GemViewModel.
        /// </summary>
        /// <returns>
        /// The cloned IObjectPropertyWrapper.
        /// </returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new GemViewModel( this.ServiceProvider );
        }

        #endregion
    }
}
