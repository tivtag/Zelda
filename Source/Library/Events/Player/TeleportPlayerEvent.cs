// <copyright file="TeleportPlayerEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.TeleportPlayerEvent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Events
{
    using System;
    using Atom.Events;
    using Zelda.Entities;
    using Atom;

    /// <summary>
    /// Represents a <see cref="SceneChangeEvent"/> that instantly teleports
    /// the player after blending out the scene.
    /// </summary>
    public sealed class TeleportPlayerEvent : SceneChangeEvent
    {
        #region [ Methods ]

        /// <summary>
        /// Triggers this TeleportPlayerEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that wants to trigger this TeleportPlayerEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            if( !this.CanBeTriggeredBy( obj ) )
                return;

            this.player = (PlayerEntity)obj;

            var blender = this.player.Scene.UserInterface.BlendElement;
            blender.StartBlending( 2.5f, false, false, this.OnBlendOutEndedOrReplaced );
        }

        /// <summary>
        /// Gets a value indicating whether this TeleportPlayerEvent
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
            var triggerPlayer = obj as PlayerEntity;
            return this.player == null && triggerPlayer != null && triggerPlayer.Moveable.CanMove;
        }

        /// <summary>
        /// Called when the blending operation has completed or has been replaced.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnBlendOutEndedOrReplaced( object sender, EventArgs e )
        {
            this.ChangeScene( this.player );

            this.player.Scene.UserInterface.BlendElement.StartBlending( 5.5f, true, true );
            this.player = null;
        }

        #region > Storage <

        /// <summary>
        /// Serializes this TeleportPlayerEvent event.
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
        /// Deserializes this TeleportPlayerEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The PlayerEntity that has triggered this WarpPlayerEvent.
        /// </summary>
        private PlayerEntity player;

        #endregion
    }
}