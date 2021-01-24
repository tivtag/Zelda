// <copyright file="RemoveEntityEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Events.RemoveEntityEvent class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Events
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using Zelda.Entities;
    using Zelda.Entities.Design;
    
    /// <summary>
    /// Represents an Event that when triggered removes
    /// a specific <see cref="ZeldaEntity"/> from the Scene.
    /// </summary>
    public sealed class RemoveEntityEvent : ZeldaEvent
    {
        /// <summary>
        /// Gets or sets the <see cref="ZeldaEntity"/> this RemoveEntityEvent
        /// removes from the scene when triggered.
        /// </summary>
        [Editor( typeof( EntitySelectionEditor ), typeof( UITypeEditor ) )]
        public ZeldaEntity Entity
        {
            get;
            set;
        }        

        /// <summary>
        /// Gets or sets a value indicating whether the persistance of
        /// the <see cref="ZeldaEntity"/> should be removed.
        /// See <see cref="IPersistentEntity"/> for more information.
        /// </summary>
        [DefaultValue(false)]
        public bool IsRemovingPersistance
        {
            get;
            set;
        }

        /// <summary>
        /// Triggers this RemoveEntityEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this RemoveEntityEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            if( this.Entity != null )
            {
                this.Entity.RemoveFromScene();
                this.HandlePersistanceChange();
            }
        }

        /// <summary>
        /// Helper method that removes the persistance of the Entity if <see cref="IsRemovingPersistance"/> is true.
        /// </summary>
        private void HandlePersistanceChange()
        {
            if( this.IsRemovingPersistance )
            {
                var persistentEntity = this.Entity as IPersistentEntity;

                if( persistentEntity != null )
                {
                    this.Scene.Status.RemovePersistantEntity( persistentEntity );
                }
            }
        }

        /// <summary>
        /// Serializes this RemoveEntityEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            // Header
            const int CurrentVersion = 2;
            context.Write( CurrentVersion );

            // Data
            if( this.Entity != null )
                context.Write( this.Entity.Name ?? string.Empty );
            else
                context.Write( string.Empty );

            context.Write( this.IsRemovingPersistance );
        }

        /// <summary>
        /// Deserializes this RemoveEntityEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            // Header
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

            // Data
            string entityName = context.ReadString();

            if( entityName.Length > 0 )
                this.Entity = this.Scene.GetEntity( entityName );
            else
                this.Entity = null;

            if( version >= 2 )
            {
                this.IsRemovingPersistance = context.ReadBoolean();
            }
        }
    }
}
