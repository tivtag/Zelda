// <copyright file="FirePlace.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.FirePlace class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using System;
    using Atom;
    using Atom.Math;

    /// <summary>
    /// Encapsulates the functionality of a fire-place entity.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// The player can unlit or lit fire places by simply using them.
    /// </remarks>
    public sealed class FirePlace : ZeldaEntity, IUseable, Atom.ISwitchable
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the FirePlace has been lit or unlit.
        /// </summary>
        public event EventHandler IsSwitchedChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether the FirePlace
        /// is currently lit.
        /// </summary>
        /// <value>
        /// The FirePlace is lit if the value is true;
        /// and unlit if false. The default value is true.
        /// </value>
        /// <remarks>
        /// This property doesn't take into account the <see cref="IsSwitchable"/> property
        /// to allow change using simple code.
        /// </remarks>
        public bool IsSwitched
        {
            get
            {
                return this.isSwitched;
            }

            set
            {
                if( value == this.IsSwitched )
                    return;

                this.isSwitched = value;

                if( this.light != null )
                    this.light.IsVisible = value;

                if( this.soundEmitter != null )
                    this.soundEmitter.IsMuted = !value;

                this.IsSwitchedChanged.Raise( this );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this FirePlace
        /// can be lit / unlit.
        /// </summary>
        /// <value>The default value is true.</value>
        public bool IsSwitchable
        {
            get;
            set;
        }

        #region > Light <

        /// <summary>
        /// Gets or sets a value indicating whether this FirePlace 
        /// has a <see cref="Light"/> entity associated with it.
        /// </summary>
        /// <value>The default value is false.</value>
        public bool HasLight
        {
            get 
            {
                return this.light != null;
            }

            set 
            {
                if( value == this.HasLight )
                    return;

                if( value )
                {
                    this.Light = new Light() {
                        Name = this.Name + "_Light"
                    };

                    light.AddToScene( this.Scene );
                }
                else
                {
                    light.RemoveFromScene();
                    light = null;
                }
            }
        }

        /// <summary>
        /// Gets the Light entity that is associated with this FirePlace.
        /// </summary>
        public Light Light
        {
            get { return this.light; }
            private set
            {
                this.light = value;

                // Setup:
                if( light != null )
                {
                    light.IsRemoveable = false;
                }
            }
        }

        #endregion

        #region > Sound <

        /// <summary>
        /// Gets or sets a value indicating whether this FirePlace 
        /// has a <see cref="PositionalSoundEmitter"/> entity associated with it.
        /// </summary>
        /// <value>The default value is false.</value>
        public bool HasSoundEmitter
        {
            get
            {
                return this.soundEmitter != null;
            }

            set
            {
                if( value == this.HasSoundEmitter )
                    return;

                if( value )
                {
                    this.SoundEmitter = new PositionalSoundEmitter() {
                        Name = this.Name + "_SoundEmitter"
                    };

                    soundEmitter.AddToScene( this.Scene );
                }
                else
                {
                    soundEmitter.RemoveFromScene();
                    soundEmitter = null;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="PositionalSoundEmitter"/> entity that is associated with this FirePlace.
        /// </summary>
        public PositionalSoundEmitter SoundEmitter
        {
            get { return this.soundEmitter; }
            private set
            {
                this.soundEmitter = value;

                // Setup:
                if( soundEmitter != null )
                {
                    soundEmitter.IsMuted      = !this.IsSwitched;
                    soundEmitter.IsRemoveable = false;
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="Zelda.Entities.Drawing.DualSwitchableDrawDataAndStrategy"/> of this <see cref="FirePlace"/>.
        /// </summary>
        public new Zelda.Entities.Drawing.DualSwitchableDrawDataAndStrategy DrawDataAndStrategy
        {
            get { return (Zelda.Entities.Drawing.DualSwitchableDrawDataAndStrategy)base.DrawDataAndStrategy; }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="FirePlace"/> class.
        /// </summary>
        public FirePlace()
        {
            this.IsSwitchable = true;
            this.Collision.Size = new Atom.Math.Vector2( 16.0f, 16.0f );
            base.DrawDataAndStrategy = new Zelda.Entities.Drawing.DualSwitchableDrawDataAndStrategy( this );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Uses this FirePlace, and as such litting or unlitting it.
        /// </summary>
        /// <param name="user">The player that wants to lit/unlit this FirePlace.</param>
        /// <returns>
        /// Returns true if the FirePlace has been lit or unlit; 
        /// or false if nothing has happened.
        /// </returns>
        public bool Use( PlayerEntity user )
        {
            if( !this.IsSwitchable )
                return false;

            if( !this.Collision.IntersectsUnstrict( user.Collision ) )
                return false;
            if( !this.Transform.IsFacing( user.Transform ) )
                return false;

            // And lit/unlit it! :)
            this.IsSwitched = !this.IsSwitched;
            return true;
        }
        
        /// <summary>
        /// Adds this FirePlace and it's associated Light to the given <paramref name="scene"/>.
        /// </summary>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        public override void AddToScene( ZeldaScene scene )
        {
            base.AddToScene( scene );

            if( light != null )
                light.AddToScene( scene );

            if( soundEmitter != null )
                soundEmitter.AddToScene( scene );            
        }

        /// <summary>
        /// Removes this FirePlace and it's associated Light
        /// from the <see cref="ZeldaScene"/> they are in.
        /// </summary>
        public override void RemoveFromScene()
        {
            base.RemoveFromScene();

            if( light != null )
                light.RemoveFromScene();

            if( soundEmitter != null )
                soundEmitter.RemoveFromScene();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Represents the storage field of the <see cref="IsSwitched"/> property.
        /// </summary>
        private bool isSwitched = true;

        /// <summary>
        /// The <see cref="Light"/> entity that is associated with this FirePlace.
        /// </summary>
        private Light light;

        /// <summary>
        /// The <see cref="PositionalSoundEmitter"/> entity that is associated with this FirePlace.
        /// </summary>
        private PositionalSoundEmitter soundEmitter;

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="FirePlace"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<FirePlace>
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
            public override void Serialize( FirePlace entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                // First Write the header:
                const int Version = 2;
                context.Write( Version );

                // Write Name:
                context.Write( entity.Name );

                // Write settings.
                context.Write( entity.IsSwitched );
                context.Write( entity.IsSwitchable );
                context.Write( entity.FloorNumber );
                context.Write( (int)entity.FloorRelativity );

                // Write transform.
                context.Write( entity.Transform.Position );

                // Write DrawData.
                entity.DrawDataAndStrategy.Serialize( context );

                // Write LightName:
                if( entity.HasLight )
                    context.Write( entity.Light.Name );
                else
                    context.Write( string.Empty );

                // Write SoundEmitterName:
                if( entity.HasSoundEmitter )
                    context.Write( entity.SoundEmitter.Name );
                else
                    context.Write( string.Empty );
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
            public override void Deserialize( FirePlace entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                // Header
                const int CurrentVersion = 2;
                int version = context.ReadInt32();
                ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.EntityType );

                // Read Name
                entity.Name = context.ReadString();

                // Read settings.
                entity.isSwitched   = context.ReadBoolean();
                entity.IsSwitchable = context.ReadBoolean();
                entity.FloorNumber     = context.ReadInt32();
                entity.FloorRelativity = (EntityFloorRelativity)context.ReadInt32();

                // Read transform.
                entity.Transform.Position = context.ReadVector2();

                // Read DrawData.
                entity.DrawDataAndStrategy.Deserialize( context );
                entity.DrawDataAndStrategy.Load( serviceProvider );

                // Read LightName:
                string lightName = context.ReadString();

                if( lightName.Length > 0 )
                {
                    entity.Light = entity.Scene.GetEntity( lightName ) as Light;
                }
                else
                {
                    entity.Light = null;
                }

                if( version >= 2 )
                {
                    // Read SoundEmitterName:
                    string soundEmitterName = context.ReadString();

                    if( soundEmitterName.Length > 0 )
                    {
                        entity.SoundEmitter = entity.Scene.GetEntity( soundEmitterName ) as PositionalSoundEmitter;
                    }
                    else
                    {
                        entity.SoundEmitter = null;
                    }
                }
            }
        }

        #endregion
    }
}
