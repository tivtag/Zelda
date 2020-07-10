// <copyright file="RedBlueBlockTrigger.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.RedBlueBlockTrigger class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using System;
    using Atom.Events;
    using Atom.Fmod;
    using Atom.Fmod.Native;
    using Atom.Math;
    using Atom.Xna;
    
    /// <summary>
    /// Represents a trigger that can be in two different states.
    /// The player can trigger the trigger by attacking it.
    /// </summary>
    public sealed class RedBlueBlockTrigger : ZeldaEntity, IUseable, IZeldaSetupable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether this RedBlueBlockTrigger is currently enabled.
        /// </summary>
        public bool IsEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="DualSwitchEvent"/> that controls the
        /// actions when this RedBlueBlockTrigger gets triggered.
        /// </summary>
        public DualSwitchEvent SwitchEvent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Sprite that is shown when this RedBlueBlockTrigger is switched on.
        /// </summary>
        public Sprite SpriteOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Sprite that is shown when this RedBlueBlockTrigger is switched off.
        /// </summary>
        public Sprite SpriteOff
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the trigger is switch on or off.
        /// </summary>
        public bool IsSwitched
        {
            get
            {
                if( this.SwitchEvent == null )
                    return false;

                return this.SwitchEvent.IsSwitched;
            }
            set
            {
                if( this.SwitchEvent == null )
                    throw new InvalidOperationException();

                this.SwitchEvent.IsSwitched = value;
            }
        }

        #endregion

        #region [ Initiialization ]

        /// <summary>
        /// Initializes a new instance of the RedBlueBlockTrigger class.
        /// </summary>
        public RedBlueBlockTrigger()
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
        /// Setups this RedBlueBlockTrigger.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services. </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            // Load Default Sprites:
            if( this.SpriteOn == null )
                this.SpriteOn = serviceProvider.SpriteLoader.LoadSprite( "Trigger_Round_Red" );

            if( this.SpriteOff == null )
                this.SpriteOff = serviceProvider.SpriteLoader.LoadSprite( "Trigger_Round_Blue" );

            this.audio = serviceProvider.AudioSystem.GetSample( "TrapDoor.wav" );
            audio.LoadAsSample( MODE._3D | MODE.CREATESAMPLE );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this RedBlueBlockTrigger.
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
        /// Draws this RedBlueBlockTrigger.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public override void Draw( ZeldaDrawContext drawContext )
        {
            if( this.SwitchEvent != null && this.SwitchEvent.IsSwitched )
            {
                if( this.SpriteOn != null )
                    this.SpriteOn.Draw( this.Transform.Position, drawContext.Batch );
            }
            else
            {
                if( this.SpriteOff != null )
                    this.SpriteOff.Draw( this.Transform.Position, drawContext.Batch );
            }
        }
        
        /// <summary>
        /// Uses this RedBlueBlockTrigger, and as such triggering it.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wants to use this RedBlueBlockTrigger.
        /// </param>
        /// <returns>
        /// True if the RedBlueBlockTrigger has been triggered;
        /// otherwise false.
        /// </returns>
        public bool Use( PlayerEntity user )
        {
            if( !this.Collision.IntersectsUnstrict( user.Collision, 2.0f ) )
                return false;

            // The player must face the Trigger:
            var delta     = user.Transform.Position - this.Transform.Position;
            var direction = delta.ToDirection4();
            if( direction.Invert() != user.Transform.Direction )
                return false;

            return this.Trigger( user );
        }

        /// <summary>
        /// Triggers this RedBlueBlockTrigger.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this RedBlueBlockTrigger.
        /// </param>
        /// <returns>
        /// True if the RedBlueBlockTrigger has been triggered;
        /// otherwise false.
        /// </returns>
        private bool Trigger( object obj )
        {
            if( !this.IsEnabled || this.SwitchEvent == null )
                return false;
            if( this.timeLeftUntriggerable > 0.0f )
                return false;

            if( this.SwitchEvent.CanBeTriggeredBy( obj ) )
            {
                this.SwitchEvent.Trigger( obj );
                this.SwitchEvent.Toggle();

                this.PlaySoundEffect();                
                this.timeLeftUntriggerable = TimeNotTriggerable;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Plays 
        /// </summary>
        private void PlaySoundEffect()
        {
            Atom.Fmod.Channel channel = this.audio.Play( true );
            channel.Set3DAttributes( this.Collision.Center.X, this.Collision.Center.Y );
            channel.Set3DMinMaxDistance( 16, 56 );
            channel.Unpause();
        }

        /// <summary>
        /// Gets called when this RedBlueBlockTrigger has been attacked.
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
        /// Returns a clone of this RedBlueBlockTrigger.
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The audio sample that is played when triggered.
        /// </summary>
        private Atom.Fmod.Sound audio;

        /// <summary>
        /// Identifies the Components.Attackable component of this RedBlueBlockTrigger.
        /// </summary>
        private readonly Components.Attackable attackable;

        /// <summary>
        /// The time left until this RedBlueBlockTrigger becomes triggerable again.
        /// </summary>
        private float timeLeftUntriggerable;

        /// <summary>
        /// The minimum time in seconds between triggering a RedBlueBlockTrigger again.
        /// </summary>
        private const float TimeNotTriggerable = 0.5f;

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="RedBlueBlockTrigger"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<RedBlueBlockTrigger>
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
            public override void Serialize( RedBlueBlockTrigger entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                // First Write the header:
                const int CurrentVersion = 2;
                context.Write( CurrentVersion );

                // Write Name:
                context.Write( entity.Name );

                // Write Position
                context.Write( entity.Transform.Position );

                // Write Collision component
                entity.Collision.Serialize( context );

                context.Write( entity.SpriteOn    != null ? entity.SpriteOn.Name    : string.Empty );
                context.Write( entity.SpriteOff   != null ? entity.SpriteOff.Name   : string.Empty );   
                context.Write( entity.SwitchEvent != null ? entity.SwitchEvent.Name : string.Empty );
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
            public override void Deserialize( RedBlueBlockTrigger entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                const int CurrentVersion = 2;
                int version = context.ReadInt32();
                Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

                // Read Name
                entity.Name = context.ReadString();

                // Read Position
                entity.Transform.Position = context.ReadVector2();

                // Read Collision component
                entity.Collision.Deserialize( context );

                string spriteOnName    = context.ReadString();
                string spriteOffName   = context.ReadString();
                string switchEventName = context.ReadString();                
                
                // Whee.
                var eventManager = entity.Scene.EventManager;

                // And by the magic of hyrule .. ;)
                entity.SwitchEvent = eventManager.GetEvent( switchEventName ) as DualSwitchEvent;
            }
        }

        #endregion
    }
}
