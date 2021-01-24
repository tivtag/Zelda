// <copyright file="UseBottleEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.UseBottleEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Items.UseEffects
{
    using System.Collections.Generic;
    using Zelda.Saving;

    /// <summary>
    /// 
    /// </summary>
    public sealed class UseBottleEffect :
        ItemUseEffect
    {
        /// <summary>
        /// Gets the list of <see cref="UseBottleEffectPart"/>s this UseBottleEffect consists of.
        /// </summary>
        public List<UseBottleEffectPart> Parts
        {
            get
            {
                return this.parts;
            }
        }

        /// <summary>
        /// Initializes a new instance of the UseBottleEffect class.
        /// </summary>
        public UseBottleEffect()
        {
            this.DestroyItemOnUse = true;
        }

        /// <summary>
        /// Gets a short localised description of this UseBottleEffect.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this ItemUseEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Status.Statable statable )
        {
            return ItemResources.IUE_UseBottle;
        }

        /// <summary>
        /// Uses this UseBottleEffect.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wishes to use this UseBottleEffect.
        /// </param>
        /// <returns>
        /// true if this UseBottleEffect has been used;
        /// otherwise false.
        /// </returns>
        public override bool Use( Entities.PlayerEntity user )
        {
            foreach( var part in this.parts )
            {
                if( part.Use( user ) )
                {
                    return true;
                }
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
        public override void Serialize( Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            context.WriteList( this.parts );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            context.ReadListInto( this.parts );
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly List<UseBottleEffectPart> parts = new List<UseBottleEffectPart>();
    }
}
