// <copyright file="BlockTrigger.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.BlockTrigger class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using System;
    using Atom.Events;
    using Atom.Math;
    using Zelda.Entities.Drawing;
    using Zelda.Saving;

    /// <summary>
    /// Represents a trigger that executes an event when it is triggered.
    /// The player can trigger the trigger by attacking or using it.
    /// </summary>
    public class BlockTrigger : ZeldaEntity, IUseable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this BlockTrigger is currently enabled.
        /// </summary>
        public bool IsEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Event"/> that controls the
        /// actions when this BlockTrigger gets triggered.
        /// </summary>
        public Event Event
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the BlockTrigger class.
        /// </summary>
        public BlockTrigger()
            : base( 4 )
        {
            // Set Properties.
            this.IsEnabled = true;

            // Because the TileSize is 16 the size is set to be a bit larger.
            this.Collision.Set( new Vector2( -1.0f, -1.0f ), new Vector2( 18.0f, 18.0f ) );

            // Create & Add Components.
            this.attackable = new Zelda.Entities.Components.Attackable();
            this.Components.Add( attackable );

            // Hook Events.
            this.attackable.Attacked += OnAttacked;
        }

        /// <summary>
        /// Updates this BlockTrigger.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( timeLeftUntriggerable > 0.0f )
                this.timeLeftUntriggerable -= updateContext.FrameTime;

            base.Update( updateContext );
        }

        /// <summary>
        /// Uses this BlockTrigger, and as such triggering it.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wants to use this BlockTrigger.
        /// </param>
        /// <returns>
        /// True if the BlockTrigger has been triggered;
        /// otherwise false.
        /// </returns>
        public bool Use( PlayerEntity user )
        {
            if( !this.Collision.IntersectsUnstrict( user.Collision, 2.0f ) )
                return false;

            // The player must face the Trigger:
            var delta = user.Transform.Position - this.Transform.Position;
            var direction = delta.ToDirection4();
            if( direction.Invert() != user.Transform.Direction )
                return false;

            return this.Trigger( user );
        }

        /// <summary>
        /// Triggers this BlockTrigger.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this BlockTrigger.
        /// </param>
        /// <returns>
        /// True if the BlockTrigger has been triggered;
        /// otherwise false.
        /// </returns>
        private bool Trigger( object obj )
        {
            if( !this.IsEnabled || this.Event == null )
                return false;
            if( this.timeLeftUntriggerable > 0.0f )
                return false;

            if( this.Event.CanBeTriggeredBy( obj ) )
            {
                this.Event.Trigger( obj );
                this.OnTriggered( obj );

                this.timeLeftUntriggerable = TimeNotTriggerable;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when this BlockTrigger has been triggered.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this BlockTrigger.
        /// </param>
        protected virtual void OnTriggered( object obj )
        {
        }

        /// <summary>
        /// Gets called when this BlockTrigger has been attacked.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="Zelda.Entities.Components.AttackEventArgs"/> that contains
        /// the event data.
        /// </param>
        private void OnAttacked( object sender, Zelda.Entities.Components.AttackEventArgs e )
        {
            Trigger( e.Attacker );
        }

        /// <summary>
        /// Returns a clone of this BlockTrigger.
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Identifies the Components.Attackable component of this BlockTrigger.
        /// </summary>
        private readonly Components.Attackable attackable;

        /// <summary>
        /// The time left until this BlockTrigger becomes triggerable again.
        /// </summary>
        private float timeLeftUntriggerable;

        /// <summary>
        /// The minimum time in seconds between triggering a BlockTrigger again.
        /// </summary>
        private const float TimeNotTriggerable = 0.5f;

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="BlockTrigger"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<BlockTrigger>
        {
            /// <summary>
            /// Initializes a new instance of the ReaderWriter class.
            /// </summary>
            /// <param name="serviceProvider">
            /// Provides fast access to game-related services.
            /// </param>
            public ReaderWriter( IZeldaServiceProvider serviceProvider )
                : base( serviceProvider )
            {
            }

            /// <summary>
            /// Serializes the given object using the given <see cref="System.IO.BinaryWriter"/>.
            /// </summary>
            /// <param name="entity">
            /// The entity to serialize.
            /// </param>
            /// <param name="context">
            /// The context under which the serialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Serialize( BlockTrigger entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                context.WriteDefaultHeader();

                // Write Name:
                context.Write( entity.Name );

                // Write Position
                context.Write( entity.Transform.Position );

                // Write Collision component
                entity.Collision.Serialize( context );

                context.WriteDrawStrategy( entity.DrawDataAndStrategy );
                context.Write( entity.Event != null ? entity.Event.Name : string.Empty );
            }

            /// <summary>
            /// Deserializes the data in the given <see cref="System.IO.BinaryWriter"/> to initialize
            /// the given ZeldaEntity.
            /// </summary>
            /// <param name="entity">
            /// The ZeldaEntity to initialize.
            /// </param>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Deserialize( BlockTrigger entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                context.ReadDefaultHeader( typeof( BlockTrigger ) );

                // Read Name
                entity.Name = context.ReadString();

                // Read Position
                entity.Transform.Position = context.ReadVector2();

                // Read Collision component
                entity.Collision.Deserialize( context );

                entity.DrawDataAndStrategy = context.ReadDrawStrategy( entity );
                string eventName = context.ReadString();

                // \o/.
                var eventManager = entity.Scene.EventManager;
                entity.Event = eventManager.GetEvent( eventName );
            }
        }
    }
}
