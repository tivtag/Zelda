// <copyright file="ExecuteActionEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.ExecuteActionEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Items.UseEffects
{
    using System.ComponentModel;
    using Atom;
    using Zelda.Actions;
    using Zelda.Saving;

    /// <summary>
    /// Represents an <see cref="ItemUseEffect"/> that when used executes an <see cref="IAction"/>.
    /// </summary>
    public class ExecuteActionEffect : ItemUseEffect
    {
        /// <summary>
        /// Gets or sets the IAction this is executed when this ExecuteActionEffect is used.
        /// </summary>
        [Browsable(false)]
        public IAction Action
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the IAction this is executed when this ExecuteActionEffect is used.
        /// </summary>
        /// <remarks>
        /// The silly M$ property grid does not work on interface properties. This is our workaround.
        /// </remarks>
        [DisplayName( "Action" )]
        [Editor( typeof( Zelda.Actions.Design.ActionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        [EditorBrowsable( EditorBrowsableState.Never )]
        public BaseAction ActionToExecute
        {
            get
            {
                return (BaseAction)this.Action;
            }

            set
            {
                this.Action = (IAction)value;
            }
        }

        /// <summary>
        /// Gets a value that represents how much "Item Points" this <see cref="ItemUseEffect"/> uses.
        /// </summary>
        public override double ItemBudgetUsed
        {
            get
            {
                return 10.0;
            }
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
        public override string GetDescription( Status.Statable statable )
        {
            return Resources.OnUse + this.Action.GetDescription();
        }

        /// <summary>
        /// Uses this ExecuteActionEffect.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wishes to use this ItemUseEffect.
        /// </param>
        /// <returns>
        /// true if this ItemUseEffect has been used;
        /// otherwise false.
        /// </returns>
        public override bool Use( Entities.PlayerEntity user )
        {
            if( !this.IsFulfilledBy(user) )
                return false;

            this.Action.Execute();
            return true;
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
            return base.IsFulfilledBy( user ) && this.Action.CanExecute();
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            context.WriteDefaultHeader();
            context.WriteStoreObject( this.Action );
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

            context.ReadDefaultHeader( this.GetType() );
            this.Action = context.ReadStoreObject<IAction>();
        }
    }
}
