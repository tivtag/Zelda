// <copyright file="TeleportationEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.TeleportationEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Items.UseEffects
{
    using System.ComponentModel;
    using Atom.Events;
    using Zelda.Core.Requirements;
    using Zelda.Events;
    using Zelda.Saving;
    
    /// <summary>
    /// Represents an ItemUseEffect that when used teleports
    /// the player to some location.
    /// </summary>
    /// <seealso cref="TeleportPlayerEvent"/>
    public sealed class TeleportationEffect : ItemUseEffect
    {
        /// <summary>
        /// Gets the <see cref="TeleportPlayerEvent"/> that is executed
        /// when this TeleportationEffect is used.
        /// </summary>
        public TeleportPlayerEvent TeleportEvent
        {
            get
            {
                return this.teleportEvent;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IRequirement"/> that
        /// is additionaly used to decide whether this TeleportationEffect
        /// can currently be used.
        /// </summary>
        [Editor( typeof( Zelda.Core.Requirements.Design.RequirementEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IRequirement AllowedRequirement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the description of this TeleportationEffect
        /// should hide the location it is teleporting to.
        /// </summary>
        public bool HideTeleporationLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="ItemUseEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this ItemUseEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Zelda.Status.Statable statable )
        {
            return string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                ItemResources.IUE_TeleportToX,
                this.GetLocalizationDescription()
            );
        }

        /// <summary>
        /// Gets the localized description of the location this TeleportationEffect
        /// is teleporting to.
        /// </summary>
        /// <returns></returns>
        private string GetLocalizationDescription()
        {
            if( this.HideTeleporationLocation )
            {
                return "?";
            }
            else
            {
                return ZeldaScene.GetLocalizedName( this.teleportEvent.SceneName );
            }
        }

        /// <summary>
        /// Gets a value that represents how much "Item Points" this <see cref="ItemUseEffect"/> uses.
        /// </summary>
        public override double ItemBudgetUsed
        {
            get 
            {
                return 0.0f;
            }
        }

        /// <summary>
        /// Uses this <see cref="ItemUseEffect"/>.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wishes to use this ItemUseEffect.
        /// </param>
        /// <returns>
        /// true if this ItemUseEffect has been used;
        /// otherwise false.
        /// </returns>
        public override bool Use( Zelda.Entities.PlayerEntity user )
        {
            if( this.IsFulfilledBy( user ) )
            {
                this.teleportEvent.Trigger( user );
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a value that indicates whether the given PlayerEntity
        /// can use this <see cref="ItemUseEffect"/> at this moment.
        /// </summary>
        /// <param name="user">The PlayerEntity that wishes to use this ItemUseEffect.</param>
        /// <returns>
        /// true if this ItemUseEffect can been used;
        /// otherwise false.
        /// </returns>
        public override bool IsFulfilledBy( Zelda.Entities.PlayerEntity user )
        {
            if( base.IsFulfilledBy( user ) )
            {
                if( this.AllowedRequirement != null )
                {
                    if( !this.AllowedRequirement.IsFulfilledBy( user ) )
                    {
                        return false;
                    }
                }

                return this.teleportEvent.CanBeTriggeredBy( user );
            }

            return false;
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

            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.HideTeleporationLocation );
            context.WriteObject( this.AllowedRequirement );
            this.teleportEvent.Serialize( new EventSerializationContext( null, context.Writer ) );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( IZeldaDeserializationContext context )
        {
            base.Deserialize( context );
            
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.HideTeleporationLocation = context.ReadBoolean();
            this.AllowedRequirement = context.ReadObject<IRequirement>();

            string eventTypeName = context.ReadString();
            string eventName = context.ReadString();
            this.teleportEvent.Deserialize( new EventDeserializationContext( null, context.Reader ) );
        }

        /// <summary>
        /// The event that is executed when this TeleportationEffect is used.
        /// </summary>
        private readonly TeleportPlayerEvent teleportEvent = new TeleportPlayerEvent();
    }
}
