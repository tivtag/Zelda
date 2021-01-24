// <copyright file="Behaveable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Components.Behaveable class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Components
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Atom.Components;
    using Zelda.Entities.Behaviours;

    /// <summary>
    /// Defines a <see cref="Component"/> that allows the Entity that owns 
    /// the <see cref="Behaveable"/> component to be controlled by an <see cref="IEntityBehaviour"/>.
    /// This class can't be inherited.
    /// </summary> 
    public sealed class Behaveable : ZeldaComponent
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="IEntityBehaviour"/> that controls how the Entity acts in the world.
        /// </summary>
        public IEntityBehaviour Behaviour
        {
            get
            { 
                return this.behaviour;
            }

            set
            {
                if( value == this.behaviour )
                    return;

                if( this.behaviour != null && this.behaviour.IsActive )
                    this.behaviour.Leave();

                this.behaviour = value;

                if( this.behaviour != null && !this.behaviour.IsActive )
                    this.behaviour.Enter();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="Behaveable"/> component.
        /// </summary>
        /// <param name="updateContext">
        /// The current Atom.IUpdateContext.
        /// </param>
        public override void Update( Atom.IUpdateContext updateContext )
        {
            if( behaviour != null )
                behaviour.Update( (ZeldaUpdateContext)updateContext );
        }

        /// <summary>
        /// Tries to get the <see cref="IEntityBehaviour"/> or sub <see cref="IEntityBehaviour"/>
        /// of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the IEntityBehaviour to get.
        /// </param>
        /// <returns>
        /// The requested IEntityBehaviour; or null.
        /// </returns>
        public IEntityBehaviour GetBehaviour( Type type )
        {
            if( this.behaviour == null )
                return null;

            // Try direct
            if( this.behaviour.GetType() == type )
                return this.behaviour;

            // Try sub behaviour
            var subBehaviourContainer = this.behaviour as ISubEntityBehaviourContainer;
            if( subBehaviourContainer != null )
                return subBehaviourContainer.GetSubBehaviour( type );

            return null;
        }

        /// <summary>
        /// Resets the current <see cref="Behaviour"/> of this Behaveable component.
        /// </summary>
        internal void Reset()
        {
            if( this.behaviour != null )
            {
                this.behaviour.Reset();
            }
        }

        /// <summary>
        /// Enters the current <see cref="Behaviour"/> of this Behaveable component.
        /// </summary>
        internal void Enter()
        {
            if( this.behaviour != null )
            {
                this.behaviour.Enter();
            }
        }

        /// <summary>
        /// Setups the given <see cref="Behaveable"/> component to be a clone of this <see cref="Behaveable"/>.
        /// </summary>
        /// <param name="clone">
        /// The <see cref="Behaveable"/> component to setup as a clone of this <see cref="Behaveable"/>.
        /// </param>
        public void SetupClone( Behaveable clone )
        {
            Contract.Requires<ArgumentNullException>( clone != null );

            clone.Behaviour = this.behaviour != null ? this.behaviour.Clone( (ZeldaEntity)clone.Owner ) : null;
        }

        #region > Storage <

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

            if( this.behaviour == null )
            {
                context.Write( string.Empty );
            }
            else
            {
                context.Write( BehaviourManager.GetName( this.behaviour ) );
                this.behaviour.Serialize( context );
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

            string behaviourName = context.ReadString();
            
            if( behaviourName.Length == 0 )
            {
                this.Behaviour = null;
            }
            else
            {
                var behaviourManager = context.ServiceProvider.BehaviourManager;
                IEntityBehaviour behaviour = behaviourManager.GetBehaviourClone( behaviourName, this.Owner );
                behaviour.Deserialize( context );

                this.Behaviour = behaviour;                
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Internal storage the <see cref="Behaviour"/> property.
        /// </summary>
        private IEntityBehaviour behaviour;

        #endregion
    }
}
