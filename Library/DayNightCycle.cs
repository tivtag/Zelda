// <copyright file="DayNightCycle.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.DayNightCycle class, the DayNightState enumeration and
//     the DayNightEvent enumeration.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using System;
    using Atom;
    using Atom.Math;

    /// <summary>
    /// Enumerates the various <see cref="DayNightCycle"/> related events.
    /// </summary>
    public enum DayNightState
    {
        /// <summary>
        /// Represents no specific state.
        /// </summary>
        None,

        /// <summary>
        /// Day goes from 6:00 to 18:00.
        /// </summary>
        Day,

        /// <summary>
        /// Evening goes from 18:00 to 23:00.
        /// </summary>
        Evening,

        /// <summary>
        /// Night goes from 23:00 to 6:00.
        /// </summary>
        Night
    }
    
    /// <summary>
    /// Enumerates the various <see cref="DayNightCycle"/> related events.
    /// </summary>
    public enum DayNightEvent
    {
        /// <summary>
        /// Represents no specific event.
        /// </summary>
        None,

        /// <summary>
        /// The event fired when the day begins. Day goes from 6:00 to 18:00.
        /// </summary>
        DayBegan,

        /// <summary>
        /// The event fired when the day ends. Day goes from 6:00 to 18:00.
        /// </summary>
        DayEnded,

        /// <summary>
        /// The event fired when the evening begins. Evening goes from 18:00 to 23:00.
        /// </summary>
        EveningBegan,

        /// <summary>
        /// The event fired when the evening ends. Evening goes from 18:00 to 23:00.
        /// </summary>
        EveningEnded,

        /// <summary>
        /// The event fired when the night begins. Night goes from 23:00 to 6:00.
        /// </summary>
        NightBegan,

        /// <summary>
        /// The event fired when the night ends. Night goes from 23:00 to 6:00.
        /// </summary>
        NightEnded
    }

    /// <summary> 
    /// Defines the day/night cycle controller.  
    /// </summary>
    public sealed class DayNightCycle
    {
        #region [ Events ]

        /// <summary>
        /// Raised when a <see cref="DayNightEvent"/> has occurred.
        /// </summary>
        public event RelaxedEventHandler<DayNightEvent> Event;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether  this <see cref="DayNightCycle"/> is activated.
        /// </summary>
        public bool IsEnabled
        {
            get 
            {
                return this.isActivated;
            }

            set
            {
                this.isActivated = value;
            }
        }

        /// <summary>
        /// Gets the current alpha value.
        /// </summary>
        public float Alpha
        {
            get
            {
                return this.lightRatio;
            }
        }

        /// <summary>
        /// Gets the current <see cref="DayNightState"/>.
        /// </summary>
        public DayNightState State
        {
            get
            {
                return this.state;
            }
        }

        /// <summary>
        /// Gets or sets the current <see cref="IngameDateTime"/>.
        /// </summary>
        public IngameDateTime IngameDateTime
        {
            get 
            {
                return this.ingameDateTime;
            }

            set
            {
                this.ingameDateTime = value;
                this.Update();
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="DayNightCycle"/> class.
        /// </summary>
        public DayNightCycle()
            : this( null )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DayNightCycle"/> class.
        /// </summary>
        /// <param name="currentDateTime">
        /// A reference to the <see cref="IngameDateTime"/> object
        /// which stores to current time ingame.
        /// </param>
        public DayNightCycle( IngameDateTime currentDateTime )
        {
            this.ingameDateTime = currentDateTime;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this DayNightCycle based on the current date/time.
        /// </summary>
        public void Update()
        {
            if( this.ingameDateTime == null )
                return;

            this.UpdateEvents();
            this.UpdateLightRatio();
        }

        /// <summary>
        /// Updates the raising of Day/Night - change events.
        /// </summary>
        private void UpdateEvents()
        {
            DayNightState currentState = GetState( this.ingameDateTime.Current );

            if( currentState != this.state )
            {
                this.LeaveState();
                this.EnterState( currentState );
            }
        }

        /// <summary>
        /// Leaves the current DayNightState, raising the Event.
        /// </summary>
        private void LeaveState()
        {
            DayNightEvent e = GetEndEvent( this.state );

            if( e != DayNightEvent.None )
            {
                this.Event.Raise( this, e );
            }
        }

        /// <summary>
        /// Enters the given DayNightState, raising the Event.
        /// </summary>
        /// <param name="newState">
        /// The DayNightState to enter.
        /// </param>
        private void EnterState( DayNightState newState )
        {
            DayNightEvent e = GetBeginEvent( newState );
            this.state = newState;   

            if( e != DayNightEvent.None )
            {
                this.Event.Raise( this, e );
            }
        }

        /// <summary>
        /// Gets the begin <see cref="DayNightEvent"/> that is associated
        /// with the given DayNightState.
        /// </summary>
        /// <param name="state">
        /// The input DayNightState.
        /// </param>
        /// <returns>
        /// The related DayNightEvent.
        /// </returns>
        public static DayNightEvent GetBeginEvent( DayNightState state )
        {
            switch( state )
            {
                case DayNightState.Day:
                    return DayNightEvent.DayBegan;

                case DayNightState.Evening:
                    return DayNightEvent.EveningBegan;

                case DayNightState.Night:
                    return DayNightEvent.NightBegan;

                default:
                    return DayNightEvent.None;
            }
        }

        /// <summary>
        /// Gets the end <see cref="DayNightEvent"/> that is associated
        /// with the given DayNightState.
        /// </summary>
        /// <param name="state">
        /// The input DayNightState.
        /// </param>
        /// <returns>
        /// The related DayNightEvent.
        /// </returns>
        public static DayNightEvent GetEndEvent( DayNightState state )
        {
            switch( state )
            {
                case DayNightState.Day:
                    return DayNightEvent.DayEnded;

                case DayNightState.Evening:
                    return DayNightEvent.EveningEnded;

                case DayNightState.Night:
                    return DayNightEvent.NightEnded;

                default:
                    return DayNightEvent.None;
            }
        }
        
        /// <summary>
        /// Gets the DayNightState for the given <see cref="DateTime"/>. 
        /// </summary>
        /// <param name="dateTime">
        /// The input DateTime.
        /// </param>
        /// <returns>
        /// The output DayNightState.
        /// </returns>
        public static DayNightState GetState( DateTime dateTime )
        {
            int hour = dateTime.Hour;

            if( hour > 6 && hour <= 18 )
            {
                return DayNightState.Day;
            }

            else if( hour > 18 && hour <= 23 )
            {
                return DayNightState.Evening;
            }

            else
            {
                return DayNightState.Night;
            }
        }

        /// <summary>
        /// Updates the current ambient Light Ratio.
        /// </summary>
        private void UpdateLightRatio()
        {
            if( this.isActivated == false )
                return;

            this.lightRatio = LightRatios.Get( this.ingameDateTime.Current );
        }

        public void AddOnceEvent( Func<DayNightEvent, bool> action )
        {
            RelaxedEventHandler<DayNightEvent> handler = null;

            handler = ( s, e ) => {
                if( action( e ) )
                {
                    this.Event -= handler;
                }
            };
            
            this.Event += handler;
        }
        
        #endregion

        #region [ Fields ]

        /// <summary>
        /// THe current state of the DayNightCycle.
        /// </summary>
        private DayNightState state;

        /// <summary>
        /// Stores the current global alpha value.
        /// </summary>
        private float lightRatio;

        /// <summary>
        /// Stores the current date/time.
        /// </summary>
        private IngameDateTime ingameDateTime;

        /// <summary>
        /// Specifies whether the DayNightCycle is activated. 
        /// </summary>
        private bool isActivated = true;

        #endregion

        #region [ Classes ]

        /// <summary>
        /// Provides a mechanism to receive the
        /// </summary>
        private static class LightRatios
        {
            /// <summary>
            /// Calculates and returns the current light ratio value based on the current date and time.
            /// </summary>
            /// <param name="dateTime">
            /// The current ingame DateTime.
            /// </param>
            /// <returns>
            /// The light ratio value.
            /// </returns>
            public static float Get( DateTime dateTime )
            {
                int month = dateTime.Month - 1;
                int hour  = dateTime.Hour;
                int nextHour = hour == 23 ? 0 : hour + 1;

                float currentLightRatio = lightRatios[month][hour];
                float nextLightRatio    = lightRatios[month][nextHour];

                float ratioBetweenHours = (float)(dateTime.Minute) / 60.0f;

                float lightRatio = MathUtilities.Lerp( currentLightRatio, nextLightRatio, ratioBetweenHours );
                return lightRatio.Clamp( 0.0f, 1.0f );
            }

            /// <summary>
            /// Creates and setups the array of light ratios.
            /// </summary>
            /// <returns>
            /// The 2D array of light ratios; indexed by [month][hour].
            /// </returns>
            private static float[][] CreateAndSetupLightRatiosArray()
            {
                var ratios = new float[12][];

                for( int i = 0; i < ratios.Length; ++i )
                    ratios[i] = new float[24];

                SetupEuropean( ratios );

                return ratios;
            }

            /// <summary>
            /// Setups the specified light <paramref name="ratios"/> array
            /// to roughtly represent middle european light.
            /// </summary>
            /// <param name="ratios">
            /// The array to initialize.
            /// </param>
            private static void SetupEuropean( float[][] ratios )
            {
                // January
                int month = 0;

                ratios[month][0] = 0.0f; ratios[month][1] = 0.0f; ratios[month][2] = 0.0f;
                ratios[month][3] = 0.0f; ratios[month][4] = 0.0f; ratios[month][5] = 0.05f;
                ratios[month][6] = 0.125f; ratios[month][7] = 0.15f; ratios[month][8] = 0.22f;
                ratios[month][9] = 0.37f; ratios[month][10] = 0.55f; ratios[month][11] = 0.77f;
                ratios[month][12] = 1.00f; ratios[month][13] = 0.97f; ratios[month][14] = 0.87f;
                ratios[month][15] = 0.76f; ratios[month][16] = 0.67f; ratios[month][17] = 0.47f;
                ratios[month][18] = 0.33f; ratios[month][19] = 0.22f; ratios[month][20] = 0.11f;
                ratios[month][21] = 0.05f; ratios[month][22] = 0.0f; ratios[month][23] = 0.0f;

                // February
                month = 1;

                ratios[month][0] = 0.0f; ratios[month][1] = 0.0f; ratios[month][2] = 0.0f;
                ratios[month][3] = 0.0f; ratios[month][4] = 0.0f; ratios[month][5] = 0.05f;
                ratios[month][6] = 0.135f; ratios[month][7] = 0.17f; ratios[month][8] = 0.27f;
                ratios[month][9] = 0.39f; ratios[month][10] = 0.56f; ratios[month][11] = 0.79f;
                ratios[month][12] = 1.00f; ratios[month][13] = 0.97f; ratios[month][14] = 0.87f;
                ratios[month][15] = 0.76f; ratios[month][16] = 0.67f; ratios[month][17] = 0.47f;
                ratios[month][18] = 0.33f; ratios[month][19] = 0.22f; ratios[month][20] = 0.11f;
                ratios[month][21] = 0.05f; ratios[month][22] = 0.0f; ratios[month][23] = 0.0f;

                // March
                month = 2;

                ratios[month][0] = 0.0f; ratios[month][1] = 0.0f; ratios[month][2] = 0.0f;
                ratios[month][3] = 0.0f; ratios[month][4] = 0.0f; ratios[month][5] = 0.05f;
                ratios[month][6] = 0.12f; ratios[month][7] = 0.19f; ratios[month][8] = 0.30f;
                ratios[month][9] = 0.42f; ratios[month][10] = 0.65f; ratios[month][11] = 0.81f;
                ratios[month][12] = 1.00f; ratios[month][13] = 0.98f; ratios[month][14] = 0.89f;
                ratios[month][15] = 0.76f; ratios[month][16] = 0.69f; ratios[month][17] = 0.52f;
                ratios[month][18] = 0.39f; ratios[month][19] = 0.22f; ratios[month][20] = 0.18f;
                ratios[month][21] = 0.1f; ratios[month][22] = 0.0f; ratios[month][23] = 0.0f;

                // April
                month = 3;

                ratios[month][0] = 0.0f; ratios[month][1] = 0.0f; ratios[month][2] = 0.0f;
                ratios[month][3] = 0.0f; ratios[month][4] = 0.0f; ratios[month][5] = 0.05f;
                ratios[month][6] = 0.14f; ratios[month][7] = 0.23f; ratios[month][8] = 0.33f;
                ratios[month][9] = 0.45f; ratios[month][10] = 0.72f; ratios[month][11] = 0.85f;
                ratios[month][12] = 1.00f; ratios[month][13] = 0.99f; ratios[month][14] = 0.85f;
                ratios[month][15] = 0.85f; ratios[month][16] = 0.80f; ratios[month][17] = 0.70f;
                ratios[month][18] = 0.65f; ratios[month][19] = 0.42f; ratios[month][20] = 0.32f;
                ratios[month][21] = 0.12f; ratios[month][22] = 0.05f; ratios[month][23] = 0.0f;

                // May
                month = 4;

                ratios[month][0] = 0.0f; ratios[month][1] = 0.0f; ratios[month][2] = 0.0f;
                ratios[month][3] = 0.0f; ratios[month][4] = 0.05f; ratios[month][5] = 0.12f;
                ratios[month][6] = 0.24f; ratios[month][7] = 0.28f; ratios[month][8] = 0.38f;
                ratios[month][9] = 0.49f; ratios[month][10] = 0.75f; ratios[month][11] = 0.90f;
                ratios[month][12] = 1.00f; ratios[month][13] = 0.99f; ratios[month][14] = 0.91f;
                ratios[month][15] = 0.85f; ratios[month][16] = 0.80f; ratios[month][17] = 0.70f;
                ratios[month][18] = 0.65f; ratios[month][19] = 0.52f; ratios[month][20] = 0.37f;
                ratios[month][21] = 0.22f; ratios[month][22] = 0.10f; ratios[month][23] = 0.01f;

                // June
                month = 5;

                ratios[month][0] = 0.0f; ratios[month][1] = 0.0f; ratios[month][2] = 0.0f;
                ratios[month][3] = 0.05f; ratios[month][4] = 0.11f; ratios[month][5] = 0.22f;
                ratios[month][6] = 0.40f; ratios[month][7] = 0.45f; ratios[month][8] = 0.52f;
                ratios[month][9] = 0.68f; ratios[month][10] = 0.79f; ratios[month][11] = 0.93f;
                ratios[month][12] = 1.00f; ratios[month][13] = 1.00f; ratios[month][14] = 0.98f;
                ratios[month][15] = 0.91f; ratios[month][16] = 0.86f; ratios[month][17] = 0.80f;
                ratios[month][18] = 0.72f; ratios[month][19] = 0.60f; ratios[month][20] = 0.50f;
                ratios[month][21] = 0.33f; ratios[month][22] = 0.20f; ratios[month][23] = 0.02f;

                // July
                month = 6;

                ratios[month][0] = 0.0f; ratios[month][1]  = 0.0f; ratios[month][2] = 0.0f;
                ratios[month][3] = 0.05f; ratios[month][4]  = 0.19f; ratios[month][5] = 0.29f;
                ratios[month][6] = 0.60f; ratios[month][7]  = 0.64f; ratios[month][8] = 0.78f;
                ratios[month][9] = 0.81f; ratios[month][10] = 0.85f; ratios[month][11] = 0.97f;
                ratios[month][12] = 1.00f; ratios[month][13] = 1.00f; ratios[month][14] = 0.99f;
                ratios[month][15] = 0.92f; ratios[month][16] = 0.88f; ratios[month][17] = 0.79f;
                ratios[month][18] = 0.72f; ratios[month][19] = 0.66f; ratios[month][20] = 0.58f;
                ratios[month][21] = 0.43f; ratios[month][22] = 0.30f; ratios[month][23] = 0.08f;
                
                // Do others to avoid strange behaviour
                // This has not been completed!
                for( month = 7; month < 12; ++month )
                {
                    ratios[month][0] = 0.0f; ratios[month][1]  = 0.0f; ratios[month][2] = 0.0f;
                    ratios[month][3] = 0.05f; ratios[month][4]  = 0.19f; ratios[month][5] = 0.29f;
                    ratios[month][6] = 0.60f; ratios[month][7]  = 0.64f; ratios[month][8] = 0.78f;
                    ratios[month][9] = 0.81f; ratios[month][10] = 0.85f; ratios[month][11] = 0.97f;
                    ratios[month][12] = 1.00f; ratios[month][13] = 1.00f; ratios[month][14] = 0.99f;
                    ratios[month][15] = 0.92f; ratios[month][16] = 0.88f; ratios[month][17] = 0.79f;
                    ratios[month][18] = 0.72f; ratios[month][19] = 0.66f; ratios[month][20] = 0.58f;
                    ratios[month][21] = 0.43f; ratios[month][22] = 0.30f; ratios[month][23] = 0.08f;
                }
            }

            /// <summary> 
            /// Stores the light ratio sorted by month|hour;
            /// </summary>
            private static readonly float[][] lightRatios = CreateAndSetupLightRatiosArray();
        }

        #endregion
    }
}