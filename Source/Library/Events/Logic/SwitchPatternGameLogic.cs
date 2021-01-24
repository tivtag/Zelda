// <copyright file="SwitchPatternGameLogic.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.SwitchPatternGameLogic class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Events
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using Atom;
    using Zelda.Entities.Design;
    
    /// <summary>
    /// Represents a N-way boolean logic circuit that matches
    /// N-input objects of type <see cref="ISwitchable"/> with
    /// an expected state.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The SwitchPatternGameLogic matches when all entries match against their respective expected states.
    /// </para>
    /// <para>
    /// An example usage for the SwitchPatternGameLogic class would be
    /// an ingame puzzle that requires the player to enable FirePlaces
    /// in a correct pattern.
    /// </para>
    /// </remarks>
    public sealed class SwitchPatternGameLogic : GameLogicEvent
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the IsMatch property has changed.
        /// </summary>
        public event EventHandler IsMatchChanged;

        /// <summary>
        /// Gets or sets the <see cref="Atom.Events.Event"/> that gets triggered
        /// when the SwitchPatternLogic matches.
        /// </summary>
        [Editor( typeof( Atom.Events.Design.EventCreationEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Atom.Events.Event OnMatchEvent
        {
            get;
            set;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the list of entiries of this SwitchPatternLogic.
        /// </summary>
        public List<Entry> Entries
        {
            get
            {
                return this.entries;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the switchables
        /// connectected to this SwitchPatternLogic are 'disabled'
        /// (made unswitchable) when the pattern matches.
        /// </summary>
        /// <value>The default value is false.</value>
        public bool DisableSwitchablesOnMatch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the SwitchPatternLogic matches.
        /// </summary>
        public bool IsMatch
        {
            get 
            {
                return this._isMatch;
            }

            private set
            {
                if( value == this.IsMatch )
                    return;

                this._isMatch = value;
                this.IsMatchChanged.Raise( this );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the SwitchPatternGameLogic class.
        /// </summary>
        public SwitchPatternGameLogic()
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes this SwitchPatternGameLogic, 
        /// checking the current match state.
        /// </summary>
        private void Refresh()
        {
            bool isMatch = true;

            for( int i = 0; i < entries.Count; ++i )
            {
                var entry = entries[i];

                if( !entry.IsMatch )
                {
                    isMatch = false;
                    break;
                }
            }

            if( isMatch )
            {
                if( this.OnMatchEvent != null )
                    this.OnMatchEvent.Trigger( this );

                if( this.DisableSwitchablesOnMatch )
                    DisableSwitchables();
            }

            this.IsMatch = isMatch;
        }

        /// <summary>
        /// Disables the entries of this SwitchPatternGameLogic so
        /// that they can't be switched on or off again.
        /// </summary>
        private void DisableSwitchables()
        {
            foreach( var entry in this.entries )
            {
                if( entry.Switchable != null )
                {
                    entry.Switchable.IsSwitchable = false;
                }
            }
        }

        #region > Events <

        /// <summary>
        /// Called when the switch state of one of the switchables has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contain the event data.</param>
        private void OnSwitchableStateChanged( object sender, EventArgs e )
        {
            Refresh();
        }

        /// <summary>
        /// Hooks the OnSwitchableStateChanged delegate to listen
        /// to the switch state of all entries.
        /// </summary>
        private void HookEvents()
        {
            foreach( var entry in this.entries )
            {
                Debug.Assert( entry != null );
                var switchable = entry.Switchable as ISwitchable;

                if( switchable != null )
                {
                    switchable.IsSwitchedChanged += OnSwitchableStateChanged;
                }                
            }
        }

        /// <summary>
        /// Unhooks the OnSwitchableStateChanged delegate that was listening
        /// to the switch state of all entries.
        /// </summary>
        private void UnhookEvents()
        {
            foreach( var entry in this.entries )
            {
                Debug.Assert( entry != null );
                var switchable = (ISwitchable)entry.Switchable;

                if( switchable != null )
                {
                    switchable.IsSwitchedChanged -= OnSwitchableStateChanged;
                }
            }
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes this SwitchPatternGameLogic event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            // Write header.
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            // Write entries.
            context.Write( this.entries.Count );
            foreach( var entry in this.entries )
            {
                context.Write( entry.SwitchableEntity != null ? entry.SwitchableEntity.Name ?? string.Empty : string.Empty );
                context.Write( entry.ExpectedSwitchState );
            }

            // Write other.
            context.Write( this.DisableSwitchablesOnMatch );
            context.Write( this.OnMatchEvent != null ? this.OnMatchEvent.Name : string.Empty );
        }

        /// <summary>
        /// Deserializes this SwitchPatternGameLogic event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            // Read base.
            base.Deserialize( context );

            // Read header:
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            // Read entries.
            int entryCount = context.ReadInt32();
            
            this.entries.Clear();
            this.entries.Capacity = entryCount;
            
            var eventManager = (ZeldaEventManager)context.EventManager;
            var scene = eventManager.Scene;            

            for( int i = 0; i < entryCount; ++i )
            {
                string entityName  = context.ReadString();
                bool expectedState = context.ReadBoolean();

                var entity = scene.GetEntity( entityName );
                var entry  = new Entry() { SwitchableEntity = entity, ExpectedSwitchState = expectedState };

                this.entries.Add( entry );
            }

            // Read other.
            this.DisableSwitchablesOnMatch = context.ReadBoolean();
            string eventName = context.ReadString();

            if( eventName.Length > 0 )
                this.OnMatchEvent = eventManager.GetEvent( eventName );
            else
                this.OnMatchEvent = null;
            
            // Don't hook the events if we are running in the editor.
            if( ZeldaScene.EditorMode )
                return;
            this.HookEvents();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Represents the storage field of the <see cref="IsMatch"/> property.
        /// </summary>
        private bool _isMatch;

        /// <summary>
        /// The list of SwitchPatternLogic entries.
        /// </summary>
        private readonly List<Entry> entries = new List<Entry>();

        #endregion

        #region [ class Entry ]

        /// <summary>
        /// Represents a single entry in the SwitchPatternLogic.
        /// This class can't be inherited.
        /// </summary>
        [TypeConverter( typeof( ExpandableObjectConverter ) )]
        public sealed class Entry
        {
            /// <summary>
            /// Gets or sets the <see cref="ISwitchable"/> ZeldaEntity that is hooked into this <see cref="SwitchPatternGameLogic.Entry"/>.
            /// </summary>
            [DisplayName( "Switchable" )]
            [Editor( typeof( SwitchableEntitySelectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
            public Zelda.Entities.ZeldaEntity SwitchableEntity
            {
                get 
                { 
                    return this.switchableEntity; 
                }

                set
                {
                    if( value != null )
                    {
                        if( !value.GetType().Implements( typeof( ISwitchable ) ) )
                        {
                            throw new ArgumentException(
                                string.Format(
                                    System.Globalization.CultureInfo.CurrentCulture,
                                    Zelda.Resources.Error_EntityIsRequiredToImplementInterfaceX,
                                    "ISwitchable"
                                ),
                                "value"
                            );
                        }
                    }

                    this.switchableEntity = value;
                }
            }

            /// <summary>
            /// Gets the <see cref="ISwitchable"/> that is hooked into this <see cref="SwitchPatternGameLogic.Entry"/>.
            /// </summary>
            [Browsable(false)]
            public ISwitchable Switchable
            {
                get { return this.switchableEntity as ISwitchable; }
            }

            /// <summary>
            /// Gets or sets a value indicating the state the <see cref="ISwitchable.IsSwitched"/>
            /// property of the set <see cref="Switchable"/> must evulate to.
            /// </summary>
            public bool ExpectedSwitchState
            {
                get;
                set;
            }

            /// <summary>
            /// Gets a value indicating whether the boolean switch state of the <see cref="Switchable"/>
            /// is equal to the <see cref="ExpectedSwitchState"/>.
            /// </summary>
            public bool IsMatch
            {
                get
                {
                    return this.Switchable != null && (this.Switchable.IsSwitched == this.ExpectedSwitchState);
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Entry"/> class.
            /// </summary>
            public Entry()
            {
            }

            /// <summary>
            /// The <see cref="ISwitchable"/> ZeldaEntity that is hooked into this <see cref="SwitchPatternGameLogic.Entry"/>.
            /// </summary>
            private Zelda.Entities.ZeldaEntity switchableEntity;
        }

        #endregion
    }
}
