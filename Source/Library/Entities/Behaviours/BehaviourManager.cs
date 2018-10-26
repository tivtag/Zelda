// <copyright file="BehaviourManager.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.BehaviourManager class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours
{
    using System;
    using System.Collections.Generic;
    using Atom.Diagnostics.Contracts;

    /// <summary>
    /// The <see cref="BehaviourManager"/> manages the creation of <see cref="IEntityBehaviour"/>s.
    /// </summary>
    public sealed class BehaviourManager
    {
        #region [ Properties ]

        /// <summary>
        /// Gets a collection that contains the names of all known <see cref="IEntityBehaviour"/>s.
        /// </summary>
        public ICollection<string> KnownBehaviourNames
        {
            get 
            {
                return this.dict.Keys;
            }
        }

        /// <summary>
        /// Gets a collection that contains the types of all known <see cref="IEntityBehaviour"/>s.
        /// </summary>
        public ICollection<Type> KnownBehaviours
        {
            get
            {
                var types = new List<Type>( this.dict.Count );

                foreach( var entry in this.dict )
                {
                    types.Add( entry.Value.GetType() );
                }

                return types;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviourManager"/> class.
        /// </summary>
        public BehaviourManager()
        {
            this.dict = new Dictionary<string, IEntityBehaviour>( 7 );
        }

        /// <summary>
        /// Loads the default IEntityBehaviours into this BehaviourManager.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceProvider"/> is null.
        /// </exception>
        public void LoadDefaults( IZeldaServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( serviceProvider != null );

            this.Register( new MeleeEnemyBehaviour( serviceProvider ) );
            this.Register( new RangedEnemyBehaviour( serviceProvider ) );
            this.Register( new RazorMovementBehaviour( serviceProvider ) );

            this.Register( new RandomMovementBehaviour( serviceProvider ) );
            this.Register( new RandomEnemyMovementBehaviour( serviceProvider ) );
            this.Register( new RandomSinusMovementBehaviour( serviceProvider ) );

            this.Register( new TurnToPlayerOnSightBehaviour( serviceProvider.Rand ) );
            this.Register( new TravellingSalesmanBehaviour( serviceProvider ) );    

            this.Register( new DespawnAfterBehaviour( serviceProvider.Rand ) );
            this.Register( new MultiBehaviour() );
            this.Register( new ResizeToCurrentSpriteBehaviour() );                

            this.Register( new Bosses.StoneTombsBossBehaviour( serviceProvider ) );
            this.Register( new Bosses.HellChicken.HellChickenBossBehaviour( serviceProvider ) );            
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets the IEntityBehaviour template with the given name.
        /// </summary>
        /// <param name="name">
        /// The name of the behaviour.
        /// </param>
        /// <returns>
        /// The requested IEntityBehaviour.
        /// </returns>
        internal IEntityBehaviour GetBehaviourTemplate( string name )
        {
            return this.dict[name];
        }

        /// <summary>
        /// Receives a clone of the <see cref="IEntityBehaviour"/> that has the given <paramref name="name"/>
        /// for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="IEntityBehaviour"/> to receive.
        /// </param>
        /// <param name="newOwner">
        /// The entity that should get controlled by the cloned IEntityBehaviour.
        /// </param>
        /// <returns>
        /// The newly cloned the <see cref="IEntityBehaviour"/>.
        /// </returns>
        public IEntityBehaviour GetBehaviourClone( string name, Atom.Components.Entity newOwner )
        {
            return this.GetBehaviourClone( name, (ZeldaEntity)newOwner );
        }

        /// <summary>
        /// Receives a clone of the <see cref="IEntityBehaviour"/> that has the given <paramref name="name"/>
        /// for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="IEntityBehaviour"/> to receive.
        /// </param>
        /// <param name="newOwner">
        /// The entity that should get controlled by the cloned IEntityBehaviour.
        /// </param>
        /// <returns>
        /// The newly cloned the <see cref="IEntityBehaviour"/>.
        /// </returns>
        public IEntityBehaviour GetBehaviourClone( string name, ZeldaEntity newOwner )
        {
            return this.dict[name].Clone( newOwner );
        }

        /// <summary>
        /// Receives a clone of the <see cref="IEntityBehaviour"/> of the given <paramref name="type"/>
        /// for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the <see cref="IEntityBehaviour"/> to receive.
        /// </param>
        /// <param name="newOwner">
        /// The entity that should get controlled by the cloned IEntityBehaviour.
        /// </param>
        /// <returns>
        /// The newly cloned the <see cref="IEntityBehaviour"/>.
        /// </returns>
        public IEntityBehaviour GetBehaviourClone( Type type, ZeldaEntity newOwner )
        {
            return this.dict[GetName(type)].Clone( newOwner );
        }

        /// <summary>
        /// Utility method that returns the name that identifies the given <see cref="IEntityBehaviour"/>.
        /// </summary>
        /// <param name="entityBehaviour">The IEntityBehaviour to receive the name for.</param>
        /// <returns>The name that identifies the IEntityBehaviour.</returns>
        public static string GetName( IEntityBehaviour entityBehaviour )
        {
            Contract.Requires<ArgumentNullException>( entityBehaviour != null );

            return entityBehaviour.GetType().FullName;
        }

        /// <summary>
        /// Utility method that returns the name that identifies the <see cref="IEntityBehaviour"/> of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of the IEntityBehaviour to receive the name for.</param>
        /// <returns>The name that identifies the IEntityBehaviour.</returns>
        public static string GetName( Type type )
        {
            Contract.Requires<ArgumentNullException>( type != null );

            return type.FullName;
        }

        /// <summary>
        /// Utility method that registers the given IEntityBehaviour.
        /// </summary>
        /// <param name="behaviour">
        /// The IEntityBehaviour to register.
        /// </param>
        private void Register( IEntityBehaviour behaviour )
        {
            this.dict.Add( GetName( behaviour ), behaviour );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The dictionary that stores the known <see cref="IEntityBehaviour"/> implementations, sorted by name.
        /// </summary>
        private readonly Dictionary<string, IEntityBehaviour> dict;
         
        #endregion
    }
}
