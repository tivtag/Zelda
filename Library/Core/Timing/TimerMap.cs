// <copyright file="TimerMap.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Timing.TimerMap class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Timing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Atom;

    /// <summary>
    /// Represents a dictionary that maps <see cref="ITimer"/>s onto (unique)
    /// identifier strings.
    /// </summary>
    public sealed class TimerMap : IZeldaUpdateable, Zelda.Saving.ISaveable
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the TimerMap class.
        /// </summary>
        public TimerMap()
        {
            this.timers = this.map.Values;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tries to add the given ITimer to this TimerMap by
        /// associating it with the given identifier.
        /// </summary>
        /// <param name="identifier">
        /// An (unique) string that identifies the given ITtimer.
        /// </param>
        /// <param name="timer">
        /// The ITtimer to add.
        /// </param>
        /// <returns>
        /// Returns true if the ITimer has been added;
        /// otherwise false.
        /// </returns>
        public bool Add( string identifier, ITimer timer )
        {
            if( this.map.ContainsKey( identifier ) )
                return false;

            this.map.Add( identifier, new WrappedTimer( identifier, timer, this ) );
            return true;
        }

        /// <summary>
        /// Tries to get the ITimer that is associated with the given
        /// identifier string.
        /// </summary>
        /// <param name="identifier">
        /// The string that (uniquely) identifies the the ITimer to get.
        /// </param>
        /// <returns>
        /// The requested timer;
        /// or null.
        /// </returns>
        public ITimer TryGet( string identifier )
        {
            WrappedTimer wrapper;

            if( this.map.TryGetValue( identifier, out wrapper ) )
            {
                return wrapper.Timer;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Updates this TimerMap.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            foreach( var timer in this.timers )
            {
                timer.Update( updateContext );
            }

            this.RemoveTimersThatEnded();
        }

        /// <summary>
        /// Removes all timers from the map that have been ended.
        /// </summary>
        private void RemoveTimersThatEnded()
        {
            int count = this.pendingRemoves.Count;

            if( count > 0 )
            {
                for( int i = 0; i < count; ++i )
                {
                    WrappedTimer wrapper = this.pendingRemoves[i];
                    this.map.Remove( wrapper.Identifier );
                }

                this.pendingRemoves.Clear();
            }
        }

        /// <summary>
        /// Adds the given WrappedTimer to the list of timers that
        /// have been ended; and should be removed from the map soon.
        /// </summary>
        /// <param name="timer">
        /// The WrappedTimer that should be removed.
        /// </param>
        private void AddToPendingRemove( WrappedTimer timer )
        {
            this.pendingRemoves.Add( timer );
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            var timersToSerialize = this.GetTimersToSerialize();
            context.Write( timersToSerialize.Count );

            foreach( var timer in timersToSerialize )
            {
                SerializeTimer( timer, context );
            }
        }

        /// <summary>
        /// Gets the collection of WrappedTimers that should be serialized.
        /// </summary>
        /// <returns>
        /// A new collection of timers.
        /// </returns>
        private ICollection<WrappedTimer> GetTimersToSerialize()
        {
            var query = 
                from entry in this.map
                    where ShouldSerializeTimer( entry.Value )
                    select entry.Value;

            return query.ToArray();
        }
        
        /// <summary>
        /// Gets a value indicating whether the given WrappedTimer should be
        /// serialized.
        /// </summary>
        /// <param name="wrappedTimer">
        /// The wrapped timer that should be checked.
        /// </param>
        /// <returns>
        /// true if the timer should be serialized;
        /// otherwise false.
        /// </returns>
        private static bool ShouldSerializeTimer( WrappedTimer wrappedTimer )
        {
            var maybeSaved = wrappedTimer.Timer as Zelda.Saving.IMaybeSaved;

            if( maybeSaved != null )
            {
                return maybeSaved.ShouldSerialize();
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Serializes the data required to descripe the specified map entry of this TimerMap.
        /// </summary>
        /// <param name="wrappedTimer">
        /// The timer to serialize.
        /// </param>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private static void SerializeTimer( WrappedTimer wrappedTimer, Zelda.Saving.IZeldaSerializationContext context )
        {
            string identifier = wrappedTimer.Identifier;
            ITimer timer = wrappedTimer.Timer;
            
            context.Write( identifier );
            context.Write( timer.GetType().GetTypeName() );
            timer.Serialize( context );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            int count = context.ReadInt32();
            
            for( int i = 0; i < count; ++i )
            {
                this.DeserializeAndAddEntry( context );
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe one entry in this TimerMap.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private void DeserializeAndAddEntry( Zelda.Saving.IZeldaDeserializationContext context )
        {
            string identifier = context.ReadString();

            string timerTypeName = context.ReadString();
            Type type = Type.GetType( timerTypeName );
            ITimer timer = (ITimer)Activator.CreateInstance( type );
            timer.Deserialize( context );

            this.Add( identifier, timer );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The dictionary that maps an (unique) identifier string onto an <see cref="ITimer"/>.
        /// </summary>
        private readonly Dictionary<string, WrappedTimer> map = new Dictionary<string, WrappedTimer>();

        /// <summary>
        /// The (cached) collection of mapped timers; for direct access.
        /// </summary>
        private readonly Dictionary<string, WrappedTimer>.ValueCollection timers;

        /// <summary>
        /// The list of ITimers that want to be removed from this TimerMap.
        /// </summary>
        private readonly List<WrappedTimer> pendingRemoves = new List<WrappedTimer>();
        
        #endregion

        #region [ Classes ]

        /// <summary>
        /// Decorates an existing <see cref="ITimer"/>
        /// with the identifier string that is associated with it.
        /// </summary>
        private sealed class WrappedTimer : ITimer
        {
            /// <summary>
            /// Raised when the wrapped ITimer has ended.
            /// </summary>
            public event SimpleEventHandler<ITimer> Ended
            {
                add
                {
                    this.timer.Ended += value;
                }

                remove
                {
                    this.timer.Ended -= value;
                }
            }

            /// <summary>
            /// Gets the (unique) identifier string that is associated with this WrappedTimer.
            /// </summary>
            public string Identifier
            {
                get
                {
                    return this.identifier;
                }
            }

            /// <summary>
            /// Gets the <see cref="ITimer"/> that gets wrapped by this WrappedTimer.
            /// </summary>
            public ITimer Timer
            {
                get
                {
                    return this.timer;
                }
            }

            /// <summary>
            /// Initializes a new instance of the WrappedTimer class.
            /// </summary>
            /// <param name="identifier">
            /// The identifier that is associated with the <paramref name="timer"/>.
            /// </param>
            /// <param name="timer">
            /// The ITimer that is wrapped by the new WrappedTimer.
            /// </param>
            /// <param name="map">
            /// The TimerMap that owns the new WrappedTimer.
            /// </param>
            public WrappedTimer( string identifier, ITimer timer, TimerMap map )
            {
                this.identifier = identifier;
                this.timer = timer;
                this.map = map;

                this.timer.Ended += this.OnTimerEnded;
            }

            /// <summary>
            /// Called when the timer this WrappedTimer wraps around
            /// has ended.
            /// </summary>
            /// <param name="sender">
            /// The sender of the event.
            /// </param>
            private void OnTimerEnded( ITimer sender )
            {
                this.timer.Ended -= this.OnTimerEnded;
                this.map.AddToPendingRemove( this );
            }

            /// <summary>
            /// Updates this IZeldaUpdateable.
            /// </summary>
            /// <param name="updateContext">
            /// The current ZeldaUpdateContext.
            /// </param>
            public void Update( ZeldaUpdateContext updateContext )
            {
                this.timer.Update( updateContext );
            }

            /// <summary>
            /// Serializes the data required to descripe this ISaveable.
            /// </summary>
            /// <param name="context">
            /// The context under which the serialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
            {
                this.timer.Serialize( context );
            }

            /// <summary>
            /// Deserializes the data required to descripe this ISaveable.
            /// </summary>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
            {
                this.timer.Deserialize( context );
            }

            /// <summary>
            /// The identifier that is associated with the wrapped ITimer.
            /// </summary>
            private readonly string identifier;

            /// <summary>
            /// The ITimer that is wrapped by this WrappedTimer.
            /// </summary>
            private readonly ITimer timer;

            /// <summary>
            /// The TimerMap this WrappedTimer is associated with.
            /// </summary>
            private readonly TimerMap map;
        }

        #endregion
    }
}