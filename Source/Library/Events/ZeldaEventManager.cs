// <copyright file="ZeldaEventManager.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.ZeldaEventManager class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Events
{
    using System.Collections.Generic;
    using Atom.Events;

    /// <summary>
    /// Defines the <see cref="EventManager"/> used by the Zelda game.
    /// </summary>
    public sealed class ZeldaEventManager : TileEventManager
    {
        /// <summary>
        /// The time in seconds between event checks.
        /// </summary>
        private const float TickTime = 0.125f;

        /// <summary>
        /// Gets the <see cref="ZeldaScene"/> this ZeldaEventManager is part of.
        /// </summary>
        public ZeldaScene Scene
        {
            get
            {
                return this.scene;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeldaEventManager"/> class.
        /// </summary>
        /// <param name="scene">
        /// The ZeldaScene the new ZeldaEventManager is part of.
        /// </param>
        internal ZeldaEventManager( ZeldaScene scene )
            : base(
                scene.Map,
                0,
                0,
                new System.Predicate<Event>( ShouldSaveEvent ),
                new System.Predicate<EventTrigger>( ShouldSaveTrigger )
            )
        {
            this.scene = scene;
        }

        /// <summary>
        /// Registers the Events and EventTriggers of the game at the <see cref="EventTypeRegister"/>.
        /// </summary>
        public static void RegisterEvents()
        {
            EventTypeRegister.RegisterCommon();

            // Events
            EventTypeRegister.RegisterEvent( typeof( ScriptedEvent ) );
            EventTypeRegister.RegisterEvent( typeof( ActionEvent ) );
            EventTypeRegister.RegisterEvent( typeof( ChangeBackgroundMusicEvent ) );
            EventTypeRegister.RegisterEvent( typeof( ChangeToRandomBackgroundMusicEvent ) );
            EventTypeRegister.RegisterEvent( typeof( DoubleFullTileChangeEvent ) );
            EventTypeRegister.RegisterEvent( typeof( TileAreaClearEvent ) );
            EventTypeRegister.RegisterEvent( typeof( PlayAudioSampleEvent ) );
            EventTypeRegister.RegisterEvent( typeof( RemoveEntityEvent ) );
            EventTypeRegister.RegisterEvent( typeof( SceneAmbientChangeEvent ) );
            EventTypeRegister.RegisterEvent( typeof( SceneChangeEvent ) );
            EventTypeRegister.RegisterEvent( typeof( SpawnEntityEvent ) );
            EventTypeRegister.RegisterEvent( typeof( ShowDialogTextEvent ) );
            EventTypeRegister.RegisterEvent( typeof( SwitchPatternGameLogic ) );
            EventTypeRegister.RegisterEvent( typeof( SwitchEntityEvent ) );
            EventTypeRegister.RegisterEvent( typeof( ToggleTileMapLayerVisabilityEvent ) );
            EventTypeRegister.RegisterEvent( typeof( WarpPlayerEvent ) );
            EventTypeRegister.RegisterEvent( typeof( SetWeatherEvent ) );

            // Triggers
            EventTypeRegister.RegisterTrigger( typeof( SongTileAreaEventTrigger ) );
        }

        /// <summary>
        /// Triggers all related TileAreaEventTriggers for the given Zelda.Entities.PlayerEntity.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        /// <param name="player">
        /// The related Zelda.Entities.PlayerEntity.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext, Zelda.Entities.PlayerEntity player )
        {
            if( !player.Statable.IsDead )
            {
                this.TriggerEvents( player, player.FloorNumber, (Atom.Math.Rectangle)player.Collision.Rectangle );
            }

            base.Update( updateContext );
        }

        /// <summary>
        /// Helper-method that gets an unique string that can be used
        /// to extend an <see cref="Event"/>'s name.
        /// </summary>
        /// <returns>
        /// A new unique string.
        /// </returns>
        internal static string GetEventNameExtension()
        {
            return (++eventIdCreator).ToString( System.Globalization.CultureInfo.InvariantCulture );
        }

        /// <summary>
        /// Helper-method that gets an unique string that can be used
        /// to extend an <see cref="EventTrigger"/>'s name.
        /// </summary>
        /// <returns>
        /// A new unique string.
        /// </returns>
        internal static string GetTriggerNameExtension()
        {
            return (++triggerIdCreator).ToString( System.Globalization.CultureInfo.InvariantCulture );
        }

        /// <summary>
        /// Gets all related TileAreaEventTriggers of type <typeparamref name="T"/> for the given Object.
        /// </summary>
        /// <typeparam name="T">The type of triggers to query.</typeparam>
        /// <param name="player">
        /// The related object.
        /// </param>
        /// <param name="source">
        /// The source of execution.
        /// </param>
        /// <returns>
        /// The triggers that would trigger.
        /// </returns>
        internal IEnumerable<T> GetTriggers<T>( Zelda.Entities.PlayerEntity player, object source = null )
            where T : TileAreaEventTrigger
        {
            return GetTriggers<T>( new TriggerContext( source, player ), player.FloorNumber, (Atom.Math.Rectangle)player.Collision.Rectangle );
        }

        /// <summary>
        /// Setups this ZeldaEventManager and all events/triggers it contains.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal void Setup( IZeldaServiceProvider serviceProvider )
        {
            foreach( var e in this.Events )
            {
                var setupable = e as IZeldaSetupable;
                if( setupable != null )
                    setupable.Setup( serviceProvider );
            }

            foreach( var trigger in this.Triggers )
            {
                var setupable = trigger as IZeldaSetupable;
                if( setupable != null )
                    setupable.Setup( serviceProvider );
            }
        }

        /// <summary>
        /// Receives a value that indicates whether the given Event should be saved.
        /// </summary>
        /// <param name="e">The Event to save.</param>
        /// <returns>true if the Event should be saved; otherwise false.</returns>
        private static bool ShouldSaveEvent( Event e )
        {
            if( e == null )
                return false;

            var saveableSate = e as Zelda.Saving.ISavedState;
            if( saveableSate != null )
                return saveableSate.IsSaved;

            return true;
        }

        /// <summary>
        /// Receives a value that indicates whether the given EventTrigger should be saved.
        /// </summary>
        /// <param name="trigger">The EventTrigger to save.</param>
        /// <returns>true if the EventTrigger should be saved; otherwise false.</returns>
        private static bool ShouldSaveTrigger( EventTrigger trigger )
        {
            if( trigger == null )
            {
                return false;
            }

            var saveableSate = trigger as Zelda.Saving.ISavedState;
            if( saveableSate != null )
            {
                return saveableSate.IsSaved;
            }

            return true;
        }

        /// <summary>
        /// The scene this ZeldaEventManager is part of.
        /// </summary>
        private readonly ZeldaScene scene;

        /// <summary>
        /// This field is used to create unique Event name extensions.
        /// </summary>
        private static ulong eventIdCreator;

        /// <summary>
        /// This field is used to create unique EventTrigger name extensions.
        /// </summary>
        private static ulong triggerIdCreator;
    }
}
