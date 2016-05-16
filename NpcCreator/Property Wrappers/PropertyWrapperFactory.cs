// <copyright file="PropertyWrapperFactory.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.NpcCreator.PropertyWrapperFactory class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.NpcCreator
{
    /// <summary>
    /// Defines the ObjectPropertyWrapperFactory used in the NpcCreator tool.
    /// </summary>
    internal sealed class PropertyWrapperFactory : Atom.Design.ObjectPropertyWrapperFactory
    {
        /// <summary>
        /// Loads the default IObjectPropertyWrappers into this <see cref="PropertyWrapperManager"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void LoadDefaults( IZeldaServiceProvider serviceProvider )
        {
            this.RegisterWrapper( new EnemyPropertyWrapper( serviceProvider ) );
            this.RegisterWrapper( new PlantPropertyWrapper( serviceProvider ) );
            this.RegisterWrapper( new FriendlyNpcPropertyWrapper( serviceProvider ) );
            this.RegisterWrapper( new MerchantPropertyWrapper( serviceProvider ) );
        }
    }
}
