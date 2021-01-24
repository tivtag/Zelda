// <copyright file="ProjectileHitSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Projectiles.ProjectileHitSettings class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Projectiles
{
    using System.ComponentModel;
    using Atom;
    using Zelda.Attacks;
    using Zelda.Audio;
    using Zelda.Saving;

    /// <summary>
    /// Encapsulates the Projectile settings that relate to
    /// the projectile hitting a target or getting destroyed.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public sealed class ProjectileHitSettings : IZeldaSetupable, ISaveable
    {
        /// <summary>
        /// Gets or sets the <see cref="IAttackHitEffect"/> applied
        /// when the Projectile hits a target.
        /// </summary>
        public IAttackHitEffect AttackHitEffect
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the SoundSampleSettings that encapsulates what sound 
        /// is played when the Projectile gets destroyed.
        /// </summary>
        public SoundSampleSettings SoundSample
        {
            get
            {
                return this.soundSample;
            }
        }

        /// <summary>
        /// Setups this ProjectileHitSettings.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-reladed services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.soundSample.Setup( serviceProvider );
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

            context.Write( this.AttackHitEffect != null ? this.AttackHitEffect.GetType().GetTypeName() : string.Empty );
            this.soundSample.Serialize( context );
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
            ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            string attackHitEffectTypeName = context.ReadString();
            if( attackHitEffectTypeName.Length > 0 )
            {
            }

            this.soundSample.Deserialize( context );
        }

        /// <summary>
        /// Encapsulates what sound is played when the Projectile gets destroyed.
        /// </summary>
        private readonly SoundSampleSettings soundSample = new SoundSampleSettings();
    }
}
