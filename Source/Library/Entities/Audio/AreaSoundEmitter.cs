// <copyright file="AreaSoundEmitter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.AreaSoundEmitter class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using System;
    using Atom.Fmod;
    using Atom.Math;
    using Atom.Xna;
    using Zelda.Saving;

    /// <summary>
    /// Represents a <see cref="SoundEmitter"/> that plays a <see cref="Sound"/>
    /// that changes its volumne depending on the position of the current audio listener.
    /// The area of sound is rectangular.
    /// </summary>
    public class AreaSoundEmitter : SoundEmitter, IEditModeDrawable, IZeldaSetupable
    {
        /// <summary>
        /// Gets or sets the the minimum area size that the sound emitter
        /// will cease to continue growing louder at (as it approaches the listener).
        /// </summary>
        public Vector2 MinimumDistance
        {
            get
            {
                return this.minimumDistance;
            }

            set
            {
                this.minimumDistance = value;
                this.RefreshAreas();
            }
        }

        /// <summary>
        /// Gets or sets the maximum area size the sound stops attenuating at. 
        /// Beyond this point it will stay at the volume it would be at MaximumDistance units from the listener 
        /// and will not attenuate any more.
        /// </summary>
        public Vector2 MaximumDistance
        {
            get
            {
                return this.maximumDistance;
            }

            set
            {
                this.maximumDistance = value;
                this.RefreshAreas();
            }
        }

        /// <summary>
        /// Gets or sets the offset for the outer (maximum) area.
        /// </summary>
        public Point2 MaximumAreaOffset
        {
            get
            {
                return this.maximumAreaOffset;
            }

            set
            {
                this.maximumAreaOffset = value;
                this.RefreshAreas();
            }
        }

        /// <summary>
        /// Gets the area in which the sound emitter will cease to continue growing louder at (as it approaches the listener).
        /// </summary>
        public Rectangle MinimumArea
        {
            get
            {
                return minimumArea;
            }
        }

        /// <summary>
        /// Gets the maximum area the sound stops attenuating at. 
        /// Beyond this point it will stay at the volume it would be at MaximumDistance units from the listener 
        /// and will not attenuate any more.
        /// </summary>
        public Rectangle MaximumArea
        {
            get
            {
                return maximumArea;
            }
        }

        /// <summary>
        /// Initializes a new instance of the AreaSoundEmitter class.
        /// </summary>
        public AreaSoundEmitter()
        {
            this.FloorRelativity = EntityFloorRelativity.IsAbove;

            this.Collision.Size = new Atom.Math.Vector2( 16.0f, 16.0f );
            this.Collision.Offset = new Atom.Math.Vector2( -8.0f, -8.0f );
            this.Collision.IsSolid = false;

            // Make SoundEmitter completly invisible in normal gameplay
            // to improve perfomance.
            this.IsVisible = ZeldaScene.EditorMode;

            // Hook events.
            this.Transform.PositionChanged += this.OnPositionChanged;
        }

        /// <summary>
        /// Setups this AreaSoundEmitter.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.audioSystem = serviceProvider.AudioSystem;
        }

        /// <summary>
        /// Loads the given Sound object, by default as a simple music file.
        /// </summary>
        /// <param name="sound">
        /// The sound object. Is never null.
        /// </param>
        protected override void LoadSound( Sound sound )
        {
            var mode =
                Atom.Fmod.Native.MODE.LOOP_NORMAL;

            sound.LoadAsSample( mode );
        }

        /// <summary>
        /// Creates a new Channel object of the given Sound object.
        /// </summary>
        /// <param name="sound">
        /// The sound object. Is never null.
        /// </param>
        /// <returns>
        /// The new channel.
        /// </returns>
        protected override Channel CreateChannel( Sound sound )
        {
            // Start playing the sound paused so
            // we have enough time to initialize the channel.
            return sound.Play( true );
        }

        /// <summary>
        /// Setups the given channel for playback.
        /// </summary>
        /// <param name="channel">
        /// The channel object. Is never null.
        /// </param>
        protected override void SetupChannel( Channel channel )
        {
            base.SetupChannel( channel );

            channel.Volume = 0.0f;
            channel.Unpause();
        }

        protected override void OnVolumeChanged( float value )
        {
            // Don't call base so that the channel volume isn't changed
        }

        /// <summary>
        /// Called when the position of this SoundEntity has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The Atom.ChangedValue{Atom.Math.Vector2} that contains the event data.</param>
        private void OnPositionChanged( object sender, Atom.ChangedValue<Atom.Math.Vector2> e )
        {
            RefreshAreas();
        }

        /// <summary>
        /// Refreshes the cached minimum and maximum areas.
        /// </summary>
        private void RefreshAreas()
        {
            minimumArea = new Rectangle( (Point2)(this.Transform.Position - minimumDistance), (Point2)(minimumDistance * 2.0f) );
            maximumArea = new Rectangle( (Point2)(this.Transform.Position + maximumAreaOffset - maximumDistance), (Point2)(maximumDistance * 2.0f) );
        }

        /// <summary>
        /// Updates the volume of the emitter.
        /// </summary>
        /// <param name="updateContext">
        /// The current update context.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            base.Update( updateContext );

            if( this.Channel != null )
            {
                this.Channel.Volume = this.CalculateVolume();
            }
        }

        private float CalculateVolume()
        {
            Point2 listenerPosition = (Point2)audioSystem.ListenerPosition2D;

            if( this.MaximumArea.Contains( listenerPosition ) )
            {
                if( this.MinimumArea.Contains( listenerPosition ) )
                {
                    return this.Volume;
                }

                Point2 projectedPoint, projectedOuterPoint;

                float distance = this.MinimumArea.DistanceTo( listenerPosition, out projectedPoint );
                float outerDistance = this.MaximumArea.DistanceTo( listenerPosition, out projectedOuterPoint );
                float totalDistance = distance + outerDistance;

                float factor = 1.0f - distance / totalDistance;
                return factor * this.Volume;
            }

            return 0.0f;
        }

        /// <summary>
        /// Draws this <see cref="AreaSoundEmitter"/> in edit-mode.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void DrawEditMode( ZeldaDrawContext drawContext )
        {
            if( !this.IsVisible )
                return;

            drawContext.Batch.DrawRect(
                this.Collision.Rectangle,
                new Microsoft.Xna.Framework.Color( 150, 100, 0, 125 )
            );

            drawContext.Batch.DrawLineRect(
                MinimumArea,
                Microsoft.Xna.Framework.Color.Red,
                1
            );

            drawContext.Batch.DrawLineRect(
                MaximumArea,
                Microsoft.Xna.Framework.Color.LightGreen,
                1
            );

            Point2 listenerPosition = (Point2)audioSystem.ListenerPosition2D;

            if( this.MaximumArea.Contains( listenerPosition ) )
            {
                if( !this.MinimumArea.Contains( listenerPosition ) )
                {
                    // Line from position to minimum area
                    Point2 projectedPoint;
                    float distance = this.MinimumArea.DistanceTo( listenerPosition, out projectedPoint );
                    drawContext.Batch.DrawLine( listenerPosition, projectedPoint, Microsoft.Xna.Framework.Color.Red, 1 );

                    // Line from position to maximum area
                    Point2 projectedOuterPoint;
                    float outerDistance = this.MaximumArea.DistanceTo( listenerPosition, out projectedOuterPoint );
                    drawContext.Batch.DrawLine( listenerPosition, projectedOuterPoint, Microsoft.Xna.Framework.Color.Yellow, 1 );
                }
            }
        }

        /// <summary>
        /// The storage field of the <see cref="MinimumDistance"/> property.
        /// </summary>
        private Vector2 minimumDistance = new Vector2( 32.0f, 32.0f );

        /// <summary>
        /// The storage field of the <see cref="MaximumDistance"/> property.
        /// </summary>
        private Vector2 maximumDistance = new Vector2( 100.0f, 100.0f );

        /// <summary>
        /// The storage field of the <see cref="MaximumOffset"/> property.
        /// </summary>
        private Point2 maximumAreaOffset;

        /// <summary>
        /// Cached sound areas.
        /// </summary>
        private Rectangle minimumArea, maximumArea;

        /// <summary>
        /// Used to get the current audio listener position.
        /// </summary>
        private AudioSystem audioSystem;

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="AreaSoundEmitter"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<AreaSoundEmitter>
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
            public override void Serialize( AreaSoundEmitter entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                context.WriteHeader( 2 );

                // Write Name:
                context.Write( entity.Name );

                // Write settings.
                context.Write( entity.Volume );
                context.Write( entity.MinimumDistance );
                context.Write( entity.MaximumDistance );
                context.Write( entity.MaximumAreaOffset ); // New in V2

                // Write transform.
                context.Write( entity.Transform.Position );

                // Write sound-info:
                if( entity.Sound != null )
                {
                    string fullPath = entity.Sound.FullPath;
                    string relativePath = fullPath.Replace( System.AppDomain.CurrentDomain.BaseDirectory, string.Empty );

                    context.Write( relativePath );
                }
                else
                {
                    context.Write( string.Empty );
                }
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
            public override void Deserialize( AreaSoundEmitter entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                int version = context.ReadHeader( 2, this.GetType() );

                // Read Name
                entity.Name = context.ReadString();

                // Read settings.
                entity.Volume = context.ReadSingle();
                entity.minimumDistance = context.ReadVector2();
                entity.maximumDistance = context.ReadVector2();

                if( version >= 2 )
                {
                    entity.maximumAreaOffset = context.ReadPoint2();
                }

                // Read transform.
                entity.Transform.Position = context.ReadVector2();

                // Read sound:
                string soundPath = context.ReadString();

                if( soundPath.Length > 0 )
                {
                    string assetName = System.IO.Path.GetFileName( soundPath );
                    string directory = System.IO.Path.GetDirectoryName( soundPath );

                    entity.Sound = serviceProvider.AudioSystem.Get( assetName, directory );
                }
            }
        }
    }
}
