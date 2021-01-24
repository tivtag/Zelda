// <copyright file="ShowDialogTextEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.ShowDialogTextEvent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Events
{
    using Zelda.UI;
    using Atom;

    /// <summary>
    /// Represents an Event that when triggered shows
    /// a <see cref="LocalizableText"/> to the player by displaying
    /// the text using a <see cref="Dialog"/> box.
    /// </summary>
    public class ShowDialogTextEvent : ZeldaEvent
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="LocalizableText"/> that is shown to the
        /// player using a <see cref="Dialog"/> when this ShowDialogTextEvent gets triggered.
        /// </summary>
        public LocalizableText Text
        {
            get { return this.localizeableText; }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Triggers this ShowDialogTextEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this ShowDialogTextEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            if( !this.CanBeTriggeredBy( obj ) )
                return;

            Dialog dialog = this.Scene.UserInterface.Dialog;
            dialog.Show( this.localizeableText.LocalizedText );
        }

        /// <summary>
        /// Gets a value indicating whether the given Object can
        /// currently trigger this ShowDialogTextEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that wants to trigger this ShowDialogTextEvent.
        /// </param>
        /// <returns>
        /// True if the event can be triggered;
        /// otherwise false.
        /// </returns>
        public override bool CanBeTriggeredBy( object obj )
        {
            if( this.Scene != null )
            {
                Dialog dialog = this.Scene.UserInterface.Dialog;
                return !dialog.IsVisible;
            }

            return false;
        }

        /// <summary>
        /// Serializes this ShowDialogTextEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            // Header
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            // Data
            context.Write( this.localizeableText.Id );
        }

        /// <summary>
        /// Deserializes this ShowDialogTextEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            // Header
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.localizeableText.Id = context.ReadString();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the text shown by this DialogEvent.
        /// </summary>
        private readonly LocalizableText localizeableText = new LocalizableText();

        #endregion
    }
}