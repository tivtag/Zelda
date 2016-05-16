// <copyright file="WarpPlayerEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Events.WarpPlayerEvent class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Events
{
    using System;
    using Atom.Events;
    using Zelda.Entities;

    /// <summary>
    /// Represents a <see cref="SceneChangeEvent"/> that applies additional StatusEffects
    /// before actually changing the scene.
    /// </summary>
    public sealed class WarpPlayerEvent : SceneChangeEvent, IZeldaSetupable
    {
        /// <summary>
        /// Triggers this WarpPlayerEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that wants to trigger this WarpPlayerEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            if( !this.CanBeTriggeredBy( obj ) )
                return;

            this.player = (PlayerEntity)obj;
            CreateTurnEvent( this.player.Scene.EventManager );

            this.blendingStarted = false;
            this.player.Moveable.CanMove = false;
            this.turnPlayerEvent.Trigger( player );
            PlayWarpSample();
        }

        /// <summary>
        /// Gets a value indicating whether this WarpPlayerEvent
        /// can be triggered by the specified <see cref="Object"/>.
        /// </summary>
        /// <param name="obj">
        /// The object that wishes to trigger this SceneChangeEvent.
        /// </param>
        /// <returns>
        /// true if the specified Object can trigger this SceneChangeEvent;
        /// otherwise false.
        /// </returns>
        public override bool CanBeTriggeredBy( object obj )
        {
            return this.player == null && obj is PlayerEntity;
        }

        /// <summary>
        /// Creates the TurnEntityEvent used by this WarpPlayerEvent.
        /// </summary>
        /// <param name="eventManager">
        /// The ZeldaEventManager to use.
        /// </param>
        private void CreateTurnEvent( ZeldaEventManager eventManager )
        {
            this.turnPlayerEvent = new TurnEntityEvent() {
                Name = this.Name + "_TurnPlayer",
                Entity = player,

                TurnCount = 25,
                TimeBetweenDirectionChanges = 0.2f,
                TurnSpeedIncreasePerTurn = 0.20f
            };

            this.turnPlayerEvent.Stopped += this.OnTurnPlayerEventStopped;
            this.turnPlayerEvent.TurnCompleted += this.OnTurnPlayerEventTurnCompleted;

            eventManager.Add( this.turnPlayerEvent );
        }

        /// <summary>
        /// Called when the TurnEntityEvent has completed a 360° turn of the player.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnTurnPlayerEventTurnCompleted( TurnEntityEvent sender )
        {
            if( this.blendingStarted )
                return;

            if( this.turnPlayerEvent.TurnsCompleted == 5 )
            {
                PlayWarpSample();
                StartBlendingOut();
            }
        }

        private void StartBlendingOut()
        {
            var blender = this.player.Scene.UserInterface.BlendElement;
            blender.StartBlending( 2.5f, false, false );
            this.blendingStarted = true;
        }

        private void PlayWarpSample()
        {
            Atom.Fmod.Sound sample = audioSystem.GetSample( "WorldWarp.ogg" );
            if( sample != null )
            {
                sample.LoadAsSample();
                sample.Play( 0.4f );
            }
        }

        /// <summary>
        /// Called when the TurnEntityEvent has stopped.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnTurnPlayerEventStopped( LongTermEvent sender )
        {
            this.DestroyTurnEvent();

            this.ChangeScene( this.player );
            this.player.Moveable.CanMove = true;
            this.player.Scene.UserInterface.BlendElement.StartBlending( 5.5f, true, true );
            this.player = null;
        }

        /// <summary>
        /// Destroys the TurnEntityEvent.
        /// </summary>
        private void DestroyTurnEvent()
        {
            this.turnPlayerEvent.EventManager.RemoveEvent( this.turnPlayerEvent.Name );
            this.turnPlayerEvent.Stopped -= this.OnTurnPlayerEventStopped;
            this.turnPlayerEvent.TurnCompleted -= this.OnTurnPlayerEventTurnCompleted;

            this.turnPlayerEvent = null;
        }

        /// <summary>
        /// Setups this WarpPlayerEvent.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.audioSystem = serviceProvider.AudioSystem;
        }

        /// <summary>
        /// Serializes this WarpPlayerEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            const int CurrentVersion = 1;
            context.Write( CurrentVersion );
        }

        /// <summary>
        /// Deserializes this WarpPlayerEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );
        }

        /// <summary>
        /// States whether the blending-out of the scene has started.
        /// </summary>
        private bool blendingStarted;

        /// <summary>
        /// The TurnEntityEvent used to turn the player just before teleporting him.
        /// </summary>
        private TurnEntityEvent turnPlayerEvent;

        /// <summary>
        /// The PlayerEntity that has triggered this WarpPlayerEvent.
        /// </summary>
        private PlayerEntity player;

        /// <summary>
        /// Allows playing of the warp sound sample.
        /// </summary>
        private Atom.Fmod.AudioSystem audioSystem;
    }
}
