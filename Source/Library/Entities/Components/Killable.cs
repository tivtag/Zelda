// <copyright file="Killable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Components.Killable class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Components
{
    using System;
    using System.Diagnostics;
    using Atom;
    using Atom.Components;
    using Zelda.Status;

    /// <summary>
    /// Defines the <see cref="Component"/> that makes a <see cref="ZeldaEntity"/> killable (by the player).
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// The entities that attack the killable ZeldaEntity
    /// may want to implement the <see cref="INotifyKilledEntity"/> if they want to be informed.
    /// </remarks>
    public sealed class Killable : ZeldaComponent
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the ZeldaEntity has been killed.
        /// </summary>
        public event SimpleEventHandler<Killable> Killed;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the experience the ZeldaEntity gives when killed.
        /// </summary>
        public int Experience
        {
            get;
            set;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Called when an IComponent has been removed or added to the <see cref="IEntity"/> that owns this IComponent.
        /// </summary>
        public override void InitializeBindings()
        {
            this.statable = this.Owner.Components.Find<Statable>();
            this.attackable = this.Owner.Components.Find<Attackable>();

            this.attackable.Attacked += OnAttacked;
        }

        #endregion

        #region [ Methods ]
                
        /// <summary>
        /// Gets called when the killable ZeldaEntity has been attacked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The AttackedEventArgs that contains the event data.</param>
        private void OnAttacked( object sender, AttackEventArgs e )
        {
            Debug.Assert( e.Target == this.Owner );
            if( this.statable.IsInvincible )
                return;

            if( this.statable.IsDead )
            {
                if( this.Killed != null )
                {
                    this.Killed( this );
                }

                var notifier = e.Attacker as INotifyKilledEntity;
                if( notifier != null )
                    notifier.NotifyKilled( this );

                if( this.Owner.Scene != null )
                    this.Owner.RemoveFromScene();
            }
        }

        /// <summary>
        /// Setups the given <see cref="Killable"/> component to be a clone of this <see cref="Killable"/> component.
        /// </summary>
        /// <param name="clone">
        /// The <see cref="Killable"/> component to setup as a clone of this <see cref="Killable"/> component.
        /// </param>
        public void SetupClone( Killable clone )
        {
            clone.Experience = this.Experience;
        }

        #region > Storage <
        
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

            context.Write( this.Experience );
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

            this.Experience = context.ReadInt32();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the <see cref="Statable"/> component.
        /// </summary>
        private Statable statable;

        /// <summary>
        /// Identifies the <see cref="Attackable"/> component.
        /// </summary>
        private Attackable attackable;

        #endregion
    }
}
