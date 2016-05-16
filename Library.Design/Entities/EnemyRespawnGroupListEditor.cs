// <copyright file="EnemyRespawnGroupListEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Spawning.Design.EnemyRespawnGroupListEditor class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Spawning.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    
    /// <summary>
    /// Implements a <see cref="System.ComponentModel.Design.CollectionEditor"/> 
    /// for <see cref="List&lt;IEnemyRespawnGroup&gt;"/> objects.
    /// This class can't be inherited.
    /// </summary>
    public sealed class EnemyRespawnGroupListEditor : CollectionEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyRespawnGroupListEditor"/> class.
        /// </summary>
        public EnemyRespawnGroupListEditor()
            : base( typeof( List<IEnemyRespawnGroup> ) )
        {
        }

        /// <summary>
        /// Receives the list of types this CollectionEditor can create.
        /// </summary>
        /// <returns>
        /// The list of types the CollectionEditor can create.
        /// </returns>
        protected override Type[] CreateNewItemTypes()
        {
            return types;
        }

        /// <summary>
        /// The list of types this EnemyRespawnGroupListEditor creates.
        /// </summary>
        private static readonly Type[] types = new Type[2] {
            typeof( EnemyRespawnGroup ),
            typeof( DayNightSpecificEnemyRespawnGroup )
        };
    }
}
