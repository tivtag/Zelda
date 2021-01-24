// <copyright file="PositionalSoundEmitter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.PositionalSoundEmitter class.
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
    /// that changes its volumne depending on the position of the current ZeldaCamera.
    /// </summary>
    public class PositionalShotSoundEmitter : ShotSoundEmitter, IEditModeDrawable
    {
        /// <summary>
        /// The tag under which sounds used by the PositionalSoundEmitter are created. 
        /// </summary>
        public const string SoundTag = "_3D";

        /// <summary>
        /// Gets or sets the the minimum distance that the sound emitter
        /// will cease to continue growing louder at (as it approaches the listener).
        /// </summary>
        public float MinimumDistance
        {
            get { return this.minimumDistance; }
            set { this.minimumDistance = value; }
        }

        /// <summary>
        /// Gets or sets the maximum distance the sound stops attenuating at. 
        /// Beyond this point it will stay at the volume it would be at MaximumDistance units from the listener 
        /// and will not attenuate any more.
        /// </summary>
        public float MaximumDistance
        {
            get { return this.maximumDistance; }
            set { this.maximumDistance = value; }
        }

        /// <summary>
        /// Gets or sets how much the 3d engine has an effect on the sound emitter, versus that set by Channel::setPan, Channel::setSpeakerMix, Channel::setSpeakerLevels
        /// </summary>
        /// <value>
        /// 1 = Sound pans and attenuates according to 3d position. 0 = Attenuation is ignored and pan/speaker levels are defined by Channel::setPan, Channel::setSpeakerMix, Channel::setSpeakerLevels.
        /// Default = 1 (all by 3D position). 
        /// </value>
        public float PanLevel3D
        {
            get { return this.panLevel3D; }
            set { this.panLevel3D = value; }
        }

        /// <summary>
        /// Initializes a new instance of the PositionalShotSoundEmitter class.
        /// </summary>
        public PositionalShotSoundEmitter()
        {
            this.FloorRelativity = EntityFloorRelativity.IsAbove;

            this.Collision.Size = new Atom.Math.Vector2( 16.0f, 16.0f );
            this.Collision.Offset = new Atom.Math.Vector2( -8.0f, -8.0f );
            this.Collision.IsSolid = false;

            // Make SoundEmitter completly invisible in normal gameplay
            // to improve perfomance.
            this.IsVisible = ZeldaScene.EditorMode;
        }

        /// <summary>
        /// Loads the given Sound object.
        /// </summary>
        /// <param name="sound">
        /// The sound object. Is never null.
        /// </param>
        protected override void LoadSound( Sound sound )
        {
            var mode =
                Atom.Fmod.Native.MODE._3D_LINEARROLLOFF;
            sound.LoadAsSample( mode );
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

            channel.Set3DAttributes( this.Transform.X, this.Transform.Y );
            channel.Set3DMinMaxDistance( this.minimumDistance, this.maximumDistance );

            // TODO
            //// channel.PanLevel3D = this.PanLevel3D;
        }
        
        /// <summary>
        /// Draws this <see cref="PositionalSoundEmitter"/> in edit-mode.
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
                new Microsoft.Xna.Framework.Color( 150, 50, 50, 125 )
            );

            drawContext.Batch.DrawLineRect(
                new Rectangle( (Point2)(this.Transform.Position - minimumDistance), new Point2( (int)(2 * minimumDistance), (int)(2 * minimumDistance) ) ),
                Microsoft.Xna.Framework.Color.Yellow,
                1
            );

            drawContext.Batch.DrawLineRect(
                new Rectangle( (Point2)(this.Transform.Position - maximumDistance), new Point2( (int)(2 * maximumDistance), (int)(2 * maximumDistance) ) ),
                Microsoft.Xna.Framework.Color.LightBlue,
                1
            );
        }

        /// <summary>
        /// The storage field of the <see cref="MinimumDistance"/> property.
        /// </summary>
        private float minimumDistance = 2.0f;

        /// <summary>
        /// The storage field of the <see cref="MaximumDistance"/> property.
        /// </summary>
        private float maximumDistance = 100.0f;

        /// <summary>
        /// The storage field of the <see cref="PanLevel3D"/> property.
        /// </summary>
        private float panLevel3D = 1.0f;

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="PositionalShotSoundEmitter"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<PositionalShotSoundEmitter>
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
            public override void Serialize( PositionalShotSoundEmitter entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                context.WriteDefaultHeader();

                // Write Name:
                context.Write( entity.Name );

                // Write settings.
                context.Write( entity.Volume );
                context.Write( entity.MinimumDistance );
                context.Write( entity.MaximumDistance );
                context.Write( entity.PanLevel3D );
                context.Write( entity.TriggerPeriod );

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
            public override void Deserialize( PositionalShotSoundEmitter entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                entity.Setup( context.ServiceProvider );
                context.ReadDefaultHeader( this.GetType() );

                // Read Name
                entity.Name = context.ReadString();

                // Read settings.
                entity.Volume = context.ReadFloatRange();
                entity.minimumDistance = context.ReadSingle();
                entity.maximumDistance = context.ReadSingle();
                entity.panLevel3D = context.ReadSingle();
                entity.TriggerPeriod = context.ReadFloatRange();

                // Read transform.
                entity.Transform.Position = context.ReadVector2();

                // Read sound:
                string soundPath = context.ReadString();

                if( soundPath.Length > 0 )
                {
                    string assetName = System.IO.Path.GetFileName( soundPath );
                    string directory = System.IO.Path.GetDirectoryName( soundPath );
                    entity.Sound = serviceProvider.AudioSystem.Get( assetName, directory, SoundTag );
                }
            }
        }
    }
}
