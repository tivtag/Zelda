// <copyright file="AttackDamageMethod.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.AttackDamageMethod class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Attacks
{
    using System.ComponentModel;
    using Zelda.Status;
    using Atom.Math;

    /// <summary>
    /// Specifies the interface of a method that is used to calculate
    /// the damage of a specific attack-way.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public abstract class AttackDamageMethod : IZeldaSetupable, Zelda.Saving.ISaveable
    {
        /// <summary>
        /// Calculates the damage done by the <paramref name="user"/> on the <paramref name="target"/>
        /// using this <see cref="AttackDamageMethod"/>.
        /// </summary>
        /// <param name="user">The user of the attack.</param>
        /// <param name="target">The target of the attack.</param>
        /// <returns>The calculated result.</returns>
        public abstract AttackDamageResult GetDamageDone( Statable user, Statable target );

        /// <summary>
        /// Notifies this AttackDamageMethod that it is going to be used 
        /// by an object.
        /// </summary>
        /// <param name="caller">
        /// The object which is going to call this AttackDamageMethod.
        /// </param>
        public void NotifyCallerChanged( object caller )
        {
            this.OnCallerChanged( caller );
        }

        /// <summary>
        /// Gets called just before this AttackDamageMethod is used by
        /// a new calling object.
        /// </summary>
        /// <param name="caller">
        /// The object which is going to call this AttackDamageMethod.
        /// </param>
        protected virtual void OnCallerChanged( object caller )
        {
        }

        /// <summary>
        /// Setups this <see cref="AttackDamageMethod"/>.
        /// </summary>
        /// <remarks>
        /// Called after the <see cref="AttackDamageMethod"/> has been created.
        /// </remarks>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public virtual void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.rand = serviceProvider.Rand;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.Write( Atom.ReflectionExtensions.GetTypeName( this.GetType() ) );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
        }

        /// <summary>
        /// A random number generator.
        /// </summary>
        protected RandMT rand;
    }
}
