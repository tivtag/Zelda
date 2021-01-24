// <copyright file="SchottlanderEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.SchottlanderEffect class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.UseEffects
{
    using Zelda.Status;

    /// <summary>
    /// The Schottlander effect consists of two phases.
    /// When used an StatusEffect is applied; and when
    /// that StatusEffect ends another StatusEffect is applied.
    /// </summary>
    public sealed class SchottlanderEffect : ItemUseEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the initial effect of this SchottlanderEffect.
        /// </summary>
        public TimedAura InitialEffect
        {
            get
            {
                return this.initialEffect;
            }
        }

        /// <summary>
        /// Gets the effect that follows the <see cref="InitialEffect"/> of this SchottlanderEffect.
        /// </summary>
        public TimedAura FollowUpEffect
        {
            get
            {
                return this.followUpEffect;
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
        public override string GetDescription( Statable statable )
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            return string.Format(
                culture,
                ItemResources.IUE_Schottlander,
                this.initialEffect.GetEffectDescription( statable ),
                this.initialEffect.Duration.ToString( culture ),
                this.followUpEffect.GetEffectDescription( statable ),
                this.followUpEffect.Duration.ToString( culture )
            );
        }

        /// <summary>
        /// Gets a value that represents how much "Item Points" this <see cref="ItemUseEffect"/> uses.
        /// </summary>
        public override double ItemBudgetUsed
        {
            get { return 0.0f; }
        }
        
        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the SchottlanderEffect class.
        /// </summary>
        public SchottlanderEffect()
        {
            this.initialEffect.Disabled += this.OnEffectEnded;
        }
        
        #endregion

        #region [ Methods ]

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
                this.initialEffect.ResetDuration();
                user.Statable.AuraList.Add( this.initialEffect );
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when the first effect of this Schottlander has ended.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="user">the statable component that had the effect.</param>
        private void OnEffectEnded( object sender, Statable user )
        {
            this.followUpEffect.ResetDuration();
            user.AuraList.Add( this.followUpEffect );
        }

        #region > Storage <

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

            this.initialEffect.Serialize( context );
            this.followUpEffect.Serialize( context );
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
            
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.initialEffect.Deserialize( context );
            this.followUpEffect.Deserialize( context );
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The effect that gets activated when this SchottlanderEffect ItemUseEffect gets used.
        /// </summary>
        private readonly TimedAura initialEffect = new TimedAura();
        
        /// <summary>
        /// The effect that gets activated when the <see cref="initialEffect"/> has ended.
        /// </summary>
        private readonly TimedAura followUpEffect = new TimedAura();

        #endregion
    }
}