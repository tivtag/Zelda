// <copyright file="MultiBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.MultiBehaviour class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Represents an <see cref="IEntityBehaviour"/> that consists of multiple other sub <see cref="IEntityBehaviour"/>.
    /// </summary>
    public class MultiBehaviour : IEntityBehaviour, ISubEntityBehaviourContainer
    {
        /// <summary>
        /// Gets the list of sub behaviours that make up this MultiBehaviour.
        /// </summary>
        public IList<IEntityBehaviour> Behaviours
        {
            get
            {
                return this.list;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this MultiBehaviour is currently active.
        /// </summary>
        public bool IsActive
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the MultiBehaviour class.
        /// </summary>
        internal MultiBehaviour()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MultiBehaviour class.
        /// </summary>
        /// <param name="owner">
        /// The ZeldaEntity that gets controlled by the new MultiBehaviour.
        /// </param>
        public MultiBehaviour( ZeldaEntity owner )
        {
            this.owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the MultiBehaviour class.
        /// </summary>
        /// <param name="owner">
        /// The ZeldaEntity that gets controlled by the new MultiBehaviour.
        /// </param>
        /// <param name="behaviours">
        /// The behaviours to initially add.
        /// </param>
        public MultiBehaviour( ZeldaEntity owner, IEnumerable<IEntityBehaviour> behaviours )
        {
            this.owner = owner;
            this.list.AddRange( behaviours );
        }

        /// <summary>
        /// Updates this MultiBehaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
            foreach( var behaviour in this.list )
            {
                behaviour.Update( updateContext );
            }
        }

        /// <summary>
        /// Enters this MultiBehaviour.
        /// </summary>
        public virtual void Enter()
        {
            if( this.IsActive )
                return;

            foreach( var behaviour in this.list )
            {
                behaviour.Enter();
            }

            this.IsActive = true;
        }

        /// <summary>
        /// Leaves this MultiBehaviour.
        /// </summary>
        public virtual void Leave()
        {
            if( !this.IsActive )
                return;

            foreach( var behaviour in this.list )
            {
                behaviour.Leave();
            }

            this.IsActive = false;
        }

        /// <summary>
        /// Resets this MultiBehaviour.
        /// </summary>
        public virtual void Reset()
        {
            foreach( var behaviour in this.list )
            {
                behaviour.Reset();
            }
        }

        /// <summary>
        /// Returns a clone of this MultiBehaviour.
        /// </summary>
        /// <param name="newOwner">
        /// The ZeldaEntity that should own the newly owned MultiBehaviour.
        /// </param>
        /// <returns>
        /// The cloned IEntityBehaviour.
        /// </returns>
        public virtual IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            var clone = new MultiBehaviour( newOwner );

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the specified MultiBehaviour to be a clone of this MultiBehaviour.
        /// </summary>
        /// <param name="clone">
        /// The MultiBehaviour to setup as a clone of this MultiBehaviour.
        /// </param>
        protected void SetupClone( MultiBehaviour clone )
        {
            clone.list.Capacity = this.list.Count;
           
            for( int i = 0; i < this.list.Count; ++i )
            {
                clone.list.Add( this.list[i].Clone( clone.owner ) );
            }
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

            // Data
            context.Write( this.list.Count );

            for( int i = 0; i < list.Count; ++i )
            {
                var behaviour = list[i];

                context.Write( BehaviourManager.GetName( behaviour ) );
                behaviour.Serialize( context );
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

            int count = context.ReadInt32();
            list.Capacity = count;

            var behaviourManager = context.ServiceProvider.BehaviourManager;

            for( int i = 0; i < count; ++i )
            {
                string behaviourName = context.ReadString();
                var behaviour = behaviourManager.GetBehaviourClone( behaviourName, this.owner );

                this.list.Add( behaviour );
            }
        }

        /// <summary>
        /// Adds the specified IEntityBehaviour to the list of sub IEntityBehaviour of this MultiBehaviour.
        /// </summary>
        /// <param name="behaviour">
        /// The IEntityBehaviour to add.
        /// </param>
        internal void Add( IEntityBehaviour behaviour )
        {
            Debug.Assert( behaviour != null );

            this.Behaviours.Add( behaviour );
        }

        /// <summary>
        /// Tries to get the sub <see cref="IEntityBehaviour"/>
        /// of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the IEntityBehaviour to get.
        /// </param>
        /// <returns>
        /// The requested IEntityBehaviour;
        /// or null if there exists no IEntityBehaviour of the given <paramref name="type"/>.
        /// </returns>
        public IEntityBehaviour GetSubBehaviour( System.Type type )
        {
            foreach( var behaviour in this.list )
            {
                if( behaviour.GetType() == type )
                {
                    return behaviour;
                }
            }

            return null;
        }       

        /// <summary>
        /// The list of sub behaviours that make up this MultiBehaviour.
        /// </summary>
        private readonly List<IEntityBehaviour> list = new List<IEntityBehaviour>();

        /// <summary>
        /// Identifies the ZeldaEntity that owns this MultiBehaviour;
        /// </summary>
        private readonly ZeldaEntity owner;
    }
}