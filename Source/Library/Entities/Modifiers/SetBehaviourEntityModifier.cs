// <copyright file="SetBehaviourEntityModifier.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.SetBehaviourEntityModifier class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Modifiers
{
    using System.ComponentModel;
    using Zelda.Entities.Behaviours;
    using Zelda.Entities.Components;
    using Zelda.Saving;

    /// <summary>
    /// Implements an <see cref="IEntityModifier"/> that sets the <see cref="IEntityBehaviour"/> of
    /// a <see cref="Behaveable"/> ZeldaEntity.
    /// </summary>
    public sealed class SetBehaviourEntityModifier : IEntityModifier
    {
        /// <summary>
        /// Gets or sets the template IEntityBehaviour that is cloned and then
        /// injected into behaveable entities to which this IEntityModifier is applied.
        /// </summary>
        [Editor( typeof( Zelda.Entities.Behaviours.Design.BehaviourEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IEntityBehaviour TemplateBehaviour
        {
            get;
            set;
        }

        /// <summary>
        /// Applies this IEntityModifier on the specified IZeldaEntity.
        /// </summary>
        /// <param name="entity">
        /// The entity to modify.
        /// </param>
        public void Apply( ZeldaEntity entity )
        {
            var behaveable = entity.Components.Get<Behaveable>();

            if( behaveable == null )
            {
                return;
            }

            behaveable.Behaviour = this.GetBehaviourFor( entity );
        }

        /// <summary>
        /// Gets the <see cref="IEntityBehaviour"/> for the specified ZeldaEntity.
        /// </summary>
        /// <param name="entity">
        /// The entity for which the new behaviour is for.
        /// </param>
        /// <returns>
        /// The newly created IEntityBehaviour.
        /// </returns>
        private IEntityBehaviour GetBehaviourFor( ZeldaEntity entity )
        {
            if( this.TemplateBehaviour != null )
            {
                return this.TemplateBehaviour.Clone( entity );
            }
            else
            {
                return null;
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
            context.WriteHeader( 2 );

            if( this.TemplateBehaviour != null  )
            {
                context.Write( BehaviourManager.GetName( this.TemplateBehaviour ) );
                this.TemplateBehaviour.Serialize( context );
            }
            else
            {
                context.Write( string.Empty );
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
            int version = context.ReadHeader( 2, this.GetType() );

            string behaviourName = context.ReadString();

            if( behaviourName.Length > 0 )
            {
                var manager = context.ServiceProvider.BehaviourManager;
                this.TemplateBehaviour = manager.GetBehaviourClone( behaviourName, null );

                if( version >= 2 )
                {
                    this.TemplateBehaviour.Deserialize( context );
                }
            }
        }
    }
}
