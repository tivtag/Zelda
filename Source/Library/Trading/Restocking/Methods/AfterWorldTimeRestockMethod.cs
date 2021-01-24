// <copyright file="AfterWorldTimeRestockMethod.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Trading.Restocking.AfterWorldTimeRestockMethod class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Trading.Restocking
{
    using System;
    using System.Diagnostics;
    using Atom;
    using Zelda.Saving;
    using Zelda.Timing;

    /// <summary>
    /// Defines an <see cref="IRestockMethod"/> that restocks the MerchantItem
    /// after a fixed amount of world-wide gametime.
    /// </summary>
    public sealed class AfterWorldTimeRestockMethod : RestockMethod
    {
        #region [ Enums ]

        /// <summary>
        /// Enumerates the different ways this AfterWorldTimeRestockMethod
        /// actually triggers.
        /// </summary>
        public enum RestockTriggerMode
        {
            /// <summary>
            /// States that the ITimer should be started
            /// when the complete stock of the MerchantItem 
            /// has been sold.
            /// </summary>
            WhenStockIsEmpty,

            /// <summary>
            /// States that the ITimer should be started
            /// Deferredly after an item has been sold.
            /// </summary>
            OnItemSold
        }

        /// <summary>
        /// Enumerates the different kind of timers this
        /// AfterWorldTimeRestockMethod supports.
        /// </summary>
        public enum RestockTimerType
        {
            /// <summary>
            /// A normal ITimer is used.
            /// </summary>
            Normal,

            /// <summary>
            /// The ITimer is not saved to the WorldStatus.
            /// </summary>
            Unsaved
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the time in game seconds that must pass
        /// after this IRestockMethod has been triggered until
        /// the MerchantTime gets restocked.
        /// </summary>
        public float Time
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time in game minutes that must pass
        /// after this IRestockMethod has been triggered until
        /// the MerchantTime gets restocked.
        /// </summary>
        public float TimeInMinutes
        {
            get
            {
                return this.Time / 60.0f;
            }

            set
            {
                this.Time = value * 60.0f;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RestockTriggerMode"/> used by this AfterWorldTimeRestockMethod.
        /// </summary>
        public RestockTriggerMode TriggerMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="RestockTimerType"/> used by this AfterWorldTimeRestockMethod.
        /// </summary>
        public RestockTimerType TimerType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether an timer is currently
        /// active.
        /// </summary>
        private bool IsTimerActive
        {
            get
            {
                var storage = this.GetCreateStateStorage();
                return storage.IsTimerActive;
            }

            set
            {
                var storage = this.GetCreateStateStorage();
                storage.IsTimerActive = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this RestockMethod has been hooked up with the given MerchantItem.
        /// </summary>
        /// <param name="merchantItem">
        /// The related MerchantItem.
        /// </param>
        protected override void OnHooked( MerchantItem merchantItem )
        {
            merchantItem.Sold += this.OnItemSold;

            merchantItem.Merchant.Added += (sender, e) => {
                this.RestockIfTimerHasPreviouslyRunOut();
            };
        }

        /// <summary>
        /// Restocks the MerchantItem if the Timer has run out while
        /// the Merchant wasn't actually loaded-up.
        /// </summary>
        private void RestockIfTimerHasPreviouslyRunOut()
        {
            if( this.HasTimerPreviouslyRunOut() )
            {
                this.IsTimerActive = false;
                this.Restock();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Timer has run out while
        /// the Merchant wasn't actually loaded-up.
        /// </summary>
        /// <returns></returns>
        private bool HasTimerPreviouslyRunOut()
        {
            if( this.timer == null && (this.TimerType != RestockTimerType.Unsaved) )
            {
                return this.IsTimerActive;
            }

            return false;
        }

        /// <summary>
        /// Called when this RestockMethod has been unhooked from the given MerchantItem.
        /// </summary>
        /// <param name="merchantItem">
        /// The related MerchantItem.
        /// </param>
        protected override void OnUnhooked( MerchantItem merchantItem )
        {
            merchantItem.Sold -= this.OnItemSold;
        }
        
        /// <summary>
        /// Gets called when an item has been sold.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnItemSold( object sender, EventArgs e )
        {
            if( this.ShouldStartTimerOnItemSold() )
            {
                this.StartTimer();
            }
        }

        /// <summary>
        /// Gets a value indicating whether an ITimer should
        /// be started after an item has been sold.
        /// </summary>
        /// <returns>
        /// true if the timer should be started;
        /// otherwise false.
        /// </returns>
        private bool ShouldStartTimerOnItemSold()
        {
            if( this.timer == null )
            {
                switch( this.TriggerMode )
                {
                    case RestockTriggerMode.OnItemSold:
                        return true;

                    case RestockTriggerMode.WhenStockIsEmpty:
                        return this.MerchantItem.StockCount == 0;

                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Stats a new ITimer that when ended restocks the MerchantItem.
        /// </summary>
        private void StartTimer()
        {
            Debug.Assert( this.timer == null );
            var timerMap = this.GetWorldTimerMap();
            if( timerMap == null )
                return;
            
            string identifier = this.GetTimerIdentifier();
            ITimer newTimer   = this.CreateTimer();
           
            if( timerMap.Add( identifier, newTimer ) )
            {
                newTimer.Ended += this.OnTimerEnded;

                this.timer = newTimer;
                this.IsTimerActive = true;
            }
        }

        /// <summary>
        /// Creates a new Timer for this AfterWorldTimeRestockMethod.
        /// </summary>
        /// <returns>
        /// A new ITimer.
        /// </returns>
        private ITimer CreateTimer()
        {
            var timer = new Timer() { Time = this.Time };

            switch( this.TimerType )
            {
                case RestockTimerType.Normal:
                    return timer;

                case RestockTimerType.Unsaved:
                    return new UnsavedTimer( timer );

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Called when the ITimer has ended.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnTimerEnded( ITimer sender )
        {
            Debug.Assert( sender == this.timer );
            
            this.timer.Ended -= this.OnTimerEnded;
            this.timer = null;
            this.IsTimerActive = false;

            this.Restock();
        }

        /// <summary>
        /// Tries to get the TimerMap that holds all ITimers
        /// that are updated world-wide.
        /// </summary>
        /// <returns>
        /// The requested TimerMap; or null.
        /// </returns>
        private TimerMap GetWorldTimerMap()
        {
            if( this.worldStatusProvider == null )
                return null;

            var worldStatus = this.worldStatusProvider.WorldStatus;
            if( worldStatus == null )
                return null;

            return worldStatus.WorldWideTimers;
        }

        /// <summary>
        /// Gets the string that uniquely identifies the timer for
        /// this AfterWorldTimeRestockMethod.
        /// </summary>
        /// <returns>
        /// Gets the string that uniquely identifies the Timer 
        /// in the SaveFile used by this AfterWorldTimeRestockMethod.
        /// </returns>
        private string GetTimerIdentifier()
        {
            return this.MerchantItem.Identifier + ".RestockTimer";
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            const int CurrentVersion = 2;
            context.Write( CurrentVersion );

            context.Write( (int)this.TriggerMode );
            context.Write( (int)this.TimerType );
            context.Write( this.Time );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

            // Read.
            this.TriggerMode = (RestockTriggerMode)context.ReadInt32();

            if( version >= 2 )
            {
                this.TimerType = (RestockTimerType)context.ReadInt32();
            }

            this.Time = context.ReadSingle();
            
            // Finish up.
            this.worldStatusProvider = context.ServiceProvider.GetService<IWorldStatusProvider>();
            this.TryGetExistingTimer();
        }

        /// <summary>
        /// Tries to get the ITimer that has been previously created 
        /// and has not ended just yet.
        /// </summary>
        private void TryGetExistingTimer()
        {
            var timerMap = this.GetWorldTimerMap();
            if( timerMap == null )
                return;

           this.timer = timerMap.TryGet( this.GetTimerIdentifier() );

           if( this.timer != null )
           {
               this.timer.Ended += this.OnTimerEnded;
           }
        }

        /// <summary>
        /// Gets (and creates) the <see cref="RestockStateStorage"/> of this AfterWorldTimeRestockMethod.
        /// </summary>
        /// <returns>
        /// The RestockStateStorage that should be used by this AfterWorldTimeRestockMethod
        /// to store data that is persisted into the SaveFile.
        /// </returns>
        private RestockStateStorage GetCreateStateStorage()
        {
            var storage = this.MerchantItem.RestockDataStorage as RestockStateStorage;

            if( storage == null )
            {
                storage = new RestockStateStorage();
                this.MerchantItem.RestockDataStorage = storage;
            }

            return storage;            
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the ITimer that when ended restocks the MerchantItem.
        /// </summary>
        private ITimer timer;

        /// <summary>
        /// Provides a mechanism that allows to receive the current <see cref="WorldStatus"/>.
        /// </summary>
        private IWorldStatusProvider worldStatusProvider;

        #endregion

        #region [ Classes ]

        /// <summary>
        /// Defines the IStorage in which data about the state of this AferWorldTimeRestockMethod
        /// is saved.
        /// </summary>
        private sealed class RestockStateStorage : Zelda.Saving.Storage.IStorage
        {
            /// <summary>
            /// Gets or sets a value indicating whether the timer
            /// was active while saving this DataStorage.
            /// </summary>
            public bool IsTimerActive
            {
                get;
                set;
            }

            /// <summary>
            /// Serializes the data required to descripe this IStorage.
            /// </summary>
            /// <param name="context">
            /// The context under which the serialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public void SerializeStorage( IZeldaSerializationContext context )
            {
                const int CurrentVersion = 1;
                context.Write( CurrentVersion );

                context.Write( this.IsTimerActive );
            }

            /// <summary>
            /// Deserializes the data required to descripe this IStorage.
            /// </summary>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public void DeserializeStorage( IZeldaDeserializationContext context )
            {
                const int CurrentVersion = 1;
                int version = context.ReadInt32();
                Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

                this.IsTimerActive = context.ReadBoolean();
            }
        }

        #endregion
    }
}