// <copyright file="EntitySpawn.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Spawning.EntitySpawn class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Spawning
{
    using System;
    using System.Diagnostics;
    using Atom;
    using Atom.Math;
    using Zelda.Entities.Modifiers;
    using Zelda.Saving;

    /// <summary>
    /// Defines a ZeldaEntity that spawn a single other <see cref="ZeldaEntity"/>
    /// based on an entity template loaden from the <see cref="EntityTemplateManager"/>.
    /// </summary>
    public class EntitySpawn : ZeldaEntity, IZeldaSetupable, IEditModeDrawable, IActivatable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether this EntitySpawn
        /// is currently active, and should spawn the entity.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                this.isActive = value;

                if( this.IsActive )
                {
                    this.SpawnEntity();
                }
                else
                {
                    this.DespawnEntity();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the EntitySpawn should
        /// clone an template entity to create the entity;
        /// or directly load it from the hard-disc.
        /// </summary>
        /// <value>
        /// The default value is false.
        /// </value>
        public bool UseTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the ZeldaEntity template
        /// whose instance gets spawned by this EntitySpawn.
        /// </summary>
        public string TemplateName
        {
            get 
            { 
                return this.templateName;
            }

            set
            {
                this.templateName = value;
                this.CreateEntity();
            }
        }

        /// <summary>
        /// Gets or sets the ZeldaEntity that gets spawned by this EntitySpawn.
        /// </summary>
        public virtual ZeldaEntity Entity
        {
            get 
            { 
                return this.entity;
            }

            protected set
            {
                // Remove previous
                if( this.entity != null )
                {
                    this.UnregisterEntity( this.entity );
                    this.DespawnEntity();
                }

                // Set current
                this.entity = value;

                if( this.entity != null )
                {
                    this.RegisterEntity( this.entity );
                    this.SpawnEntity();
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IEntityModifier"/> that is applied to the entity
        /// spawned by this EntitySpawn.
        /// </summary>
        public IEntityModifier EntityModifier
        {
            get;
            set;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the EntitySpawn class.
        /// </summary>
        public EntitySpawn()
        {
            this.Collision.IsSolid  = false;
            this.Transform.Position = new Vector2( 16.0f * 3, 16.0f * 3 );

            // Register events related to this
            this.FloorNumberChanged         += This_FloorNumberChanged;
            this.Transform.PositionChanged  += This_PositionChanged;
            this.Transform.DirectionChanged += This_DirectionChanged;

            if( !ZeldaScene.EditorMode )
            {
                // Make entity spawns invisible in the normal game. 
                // This improves runtime perfomance.
                this.IsVisible = false;
            }
            else
            {
                this.Collision.Size = new Vector2( 16.0f, 16.0f );
                this.Collision.Offset = new Vector2( -8.0f, -8.0f );
            }
        }

        /// <summary>
        /// Setups this EntitySpawn.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public virtual void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Creates the <see cref="Entity"/> by using the <see cref="TemplateName"/>.
        /// </summary>
        private void CreateEntity()
        {
            if( string.IsNullOrEmpty( this.templateName ) )
            {
                this.Entity = null;
            }
            else
            {
                var entityManager = serviceProvider.EntityTemplateManager;

                if( this.UseTemplate )
                {
                    this.Entity = entityManager.GetEntity( templateName );
                }
                else
                {
                    this.Entity = entityManager.LoadEntity( templateName );
                }

                if( this.EntityModifier != null )
                {
                    this.EntityModifier.Apply( this.Entity );
                }
            }
        }

        /// <summary>
        /// Gets called when this EntitySpawn
        /// gets added to the given scene.
        /// </summary>
        /// <param name="scene">
        /// The related scene.
        /// </param>
        public override void AddToScene( ZeldaScene scene )
        {
            base.AddToScene( scene );

            this.SpawnEntity();
        }

        /// <summary>
        /// Gets called when this EntitySpawn
        /// gets removed from the scene.
        /// </summary>
        public override void RemoveFromScene()
        {
            base.RemoveFromScene();

            this.DespawnEntity();
        }

        /// <summary>
        /// Spawns the entity - if required and allowed.
        /// </summary>
        private void SpawnEntity()
        {
            if( this.IsActive )
            {
                if( this.entity != null && this.Scene != null )
                {
                    this.entity.AddToScene( this.Scene );
                }
            }
        }
        
        /// <summary>
        /// Despawns the entity - if required and allowed.
        /// </summary>
        private void DespawnEntity()
        {
            if( this.entity != null && this.entity.Scene != null )
            {
                this.entity.RemoveFromScene();
            }
        }

        /// <summary>
        /// Setups the given ZeldaEntity for usage in this EntitySpawn.
        /// </summary>
        /// <param name="entity">
        /// The related ZeldaEntity.
        /// </param>
        protected virtual void RegisterEntity( ZeldaEntity entity )
        {
            entity.IsSaved             = false;
            entity.IsEditable          = false;

            entity.FloorNumber         = this.FloorNumber;
            entity.Transform.Position  = this.Transform.Position;
            entity.Transform.Direction = this.Transform.Direction;

            entity.FloorNumberChanged         += this.Entity_FloorNumberChanged;
            entity.Transform.PositionChanged  += this.Entity_PositionChanged;
            entity.Transform.DirectionChanged += this.Entity_DirectionChanged;

            this.Collision.Size = entity.Collision.Size;
        }

        /// <summary>
        /// Unregisters the given ZeldaEntity from usage in this EntitySpawn.
        /// </summary>
        /// <param name="entity">
        /// The related ZeldaEntity.
        /// </param>
        protected virtual void UnregisterEntity( ZeldaEntity entity )
        {
            entity.FloorNumberChanged         -= this.Entity_FloorNumberChanged;
            entity.Transform.PositionChanged  -= this.Entity_PositionChanged;
            entity.Transform.DirectionChanged -= this.Entity_DirectionChanged;
        }

        #region > Drawing <

        /// <summary>
        /// Draws this <see cref="EntitySpawn"/> in edit-mode.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public virtual void DrawEditMode( ZeldaDrawContext drawContext )
        {
            if( !this.IsVisible )
                return;

            drawContext.Batch.DrawRect(
                new Rectangle(
                    (int)this.Transform.X - 8,
                    (int)this.Transform.Y - 8,
                    16,
                    16
                ),
                new Microsoft.Xna.Framework.Color( 150, 0, 0, 150 )
            );
        }

        #endregion

        #region > Events <

        #region - This -

        /// <summary>
        /// Fired when the position of this EntitySpawn has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The Atom.ChangedValue{Atom.Math.Vector2} that contains the event data.
        /// </param>
        private void This_PositionChanged( object sender, Atom.ChangedValue<Atom.Math.Vector2> e )
        {
            if( this.entity != null )
            {
                this.entity.Transform.Position = e.NewValue;
            }
        }

        /// <summary>
        /// Fired when the direction of this EntitySpawn has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The Atom.ChangedValue{Atom.Math.Direction4} that contains the event data.
        /// </param>
        private void This_DirectionChanged( object sender, Atom.ChangedValue<Atom.Math.Direction4> e )
        {
            if( this.entity != null )
            {
                this.entity.Transform.Direction = e.NewValue;
            }
        }

        /// <summary>
        /// Fired when the Floor Number of this EntitySpawn has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The Atom.ChangedValue{Int32} that contains the event data.
        /// </param>
        private void This_FloorNumberChanged( object sender, Atom.ChangedValue<int> e )
        {
            if( this.entity != null )
            {
                this.entity.FloorNumber = e.NewValue;
            }
        }

        #endregion

        #region - Entity -

        /// <summary>
        /// Fired when the position property of the ZeldaEntity has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The Atom.ChangedValue{Vector2} that contains the event data.
        /// </param>
        private void Entity_PositionChanged( object sender, Atom.ChangedValue<Vector2> e )
        {
            Debug.Assert( sender == entity );
            this.Transform.Position = e.NewValue;
        }

        /// <summary>
        /// Fired when the direction property of the ZeldaEntity has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The Atom.ChangedValue{Vector2} that contains the event data.
        /// </param>
        private void Entity_DirectionChanged( object sender, Atom.ChangedValue<Direction4> e )
        {
            Debug.Assert( sender == entity );
            this.Transform.Direction = e.NewValue;
        }

        /// <summary>
        /// Fired when the FloorNumber property of the ZeldaEntity has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The Atom.ChangedValue{Vector2} that contains the event data.
        /// </param>
        private void Entity_FloorNumberChanged( object sender, Atom.ChangedValue<int> e )
        {
            Debug.Assert( sender == entity );
            this.FloorNumber = e.NewValue;
        }

        #endregion

        #endregion

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this EntitySpawn.
        /// </summary>
        /// <returns>
        /// The cloned ZeldaEntity.
        /// </returns>
        public override ZeldaEntity Clone()
        {
            var clone = new EntitySpawn();
            clone.Setup( serviceProvider );

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given EntitySpawn to be a clone
        /// of this EntitySpawn.
        /// </summary>
        /// <param name="clone">
        /// The EntitySpawn to setup as a clone
        /// of this EntitySpawn.
        /// </param>
        protected void SetupClone( EntitySpawn clone )
        {
            base.SetupClone( clone );

            clone.isActive = this.IsActive;
            clone.TemplateName = this.TemplateName;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Represents the storage field of the IsActive property.
        /// </summary>
        private bool isActive = true;

        /// <summary>
        /// The name of the ZeldaEntity-template 
        /// whose instance gets spawned by this EntitySpawn.
        /// </summary>
        private string templateName;

        /// <summary>
        /// Identifies the ZeldaEntity that was/is spawned by this EntitySpawn.
        /// </summary>
        private ZeldaEntity entity;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        protected IZeldaServiceProvider serviceProvider;

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="EntitySpawn"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<EntitySpawn>
        {
            /// <summary>
            /// Initializes a new instance of the ReaderWriter class.
            /// </summary>   
            /// <exception cref="ArgumentNullException">
            /// If <paramref name="serviceProvider"/> is null.
            /// </exception>
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
            public override void Serialize( EntitySpawn entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                // Header:
                const int CurrentVersion = 3;
                context.Write( CurrentVersion );

                // Data:
                context.Write( entity.Name );
                context.Write( entity.IsActive );
                context.WriteObject( entity.EntityModifier );

                context.Write( entity.UseTemplate );
                context.Write( entity.templateName ?? string.Empty );

                context.Write( entity.Transform.Position );
                context.Write( (int)entity.Transform.Direction );
                context.Write( entity.FloorNumber );
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
            public override void Deserialize( EntitySpawn entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                entity.Setup( serviceProvider );

                // Header
                const int CurrentVersion = 3;
                int version = context.ReadInt32();
                ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

                // Read
                entity.Name = context.ReadString();

                if( version >= 3 )
                {
                    entity.isActive = context.ReadBoolean();
                    entity.EntityModifier = context.ReadObject<IEntityModifier>();
                }

                entity.UseTemplate = context.ReadBoolean();
                entity.TemplateName = context.ReadString();

                // Advanced properties
                entity.Transform.Position  = context.ReadVector2();
                entity.Transform.Direction = (Direction4)context.ReadInt32();
                entity.FloorNumber = context.ReadInt32();
            }
        }

        #endregion
    }
}
