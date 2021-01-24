// <copyright file="TimedAura.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.TimedAura class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// A <see cref="TimedAura"/> is an <see cref="Aura"/> that stays active until for a given <see cref="Cooldown"/>.
    /// </summary>
    public class TimedAura : Aura
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the cooldown of this <see cref="TimedAura"/>.
        /// </summary>
        public Cooldown Cooldown
        {
            get
            { 
                return this.cooldown; 
            }
        }

        /// <summary>
        /// Gets or sets the total duration this TimedAura lasts.
        /// </summary>
        public float Duration
        {
            get
            {
                return this.cooldown.TotalTime;
            }

            set
            {
                this.cooldown.TotalTime = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="TimedAura"/> is still active.
        /// </summary>
        [Browsable( false )]
        public bool IsActive
        {
            get 
            { 
                return !this.cooldown.IsReady;
            }
        }       

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedAura"/> class.
        /// </summary>
        public TimedAura()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedAura"/> class.
        /// </summary>
        /// <param name="time">
        /// The time the new TimedAura lasts.
        /// </param>
        public TimedAura( float time )
            : base()
        {
            this.cooldown.TotalTime = time;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedAura"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException"> 
        /// If <paramref name="effect"/> is null.
        /// </exception>
        /// <param name="time">
        /// The time the new TimedAura lasts.
        /// </param>
        /// <param name="effect">
        /// The StatusEffect that gets applied by the new TimedAura.
        /// </param>
        public TimedAura( float time, StatusEffect effect )
            : this( time, new StatusEffect[1] { effect } )
        {
            if( effect == null )
                throw new ArgumentNullException( "effect" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedAura"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException"> 
        /// If <paramref name="effects"/> is null.
        /// </exception>
        /// <param name="time">
        /// The time the new TimedAura lasts.
        /// </param>
        /// <param name="effects">
        /// The list of <see cref="StatusEffect"/>s of the new TimedAura.
        /// </param>
        public TimedAura( float time, StatusEffect[] effects )
            : base( effects )
        {
            this.cooldown.TotalTime = time;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="TimedAura"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.cooldown.Update( updateContext.FrameTime );
        }

        /// <summary>
        /// Resets the time this <see cref="TimedAura"/> lasts.
        /// </summary>
        public void ResetDuration()
        {
            this.cooldown.Reset();
        }

        /// <summary>
        /// Gets a localized string that descripes the effect(s) of this TimedAura.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        /// <returns>
        /// The localized effect description.
        /// </returns>
        public string GetEffectDescription( Statable statable )
        {
            switch( this.Effects.Count )
            {
                case 0:
                    return string.Empty;

                case 1:
                    return this.Effects[0].GetDescription( statable );

                default:
                    return this.GetEffectsDescription( statable );
            }
        }

        /// <summary>
        /// Gets a localized string that descripes the effect of the proc effect of this TimedStatusProcEffect;
        /// taking into account that the effect consists of multiple other StatusEffects.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        /// <returns>
        /// The localized effect description.
        /// </returns>
        private string GetEffectsDescription( Statable statable )
        {
            var sb = new System.Text.StringBuilder();
            sb.Append( this.GetEffectDescription( 0, statable ) );

            for( int effectIndex = 1; effectIndex < this.Effects.Count; ++effectIndex )
            {
                this.AppendSeperator( effectIndex, sb );
                this.AppendEffectDescription( effectIndex, sb, statable );
            }

            return sb.ToString();
        }

        /// <summary>
        /// Appends the description of the StatusEffect at the given index to the given StringBuilder.
        /// </summary>
        /// <param name="effectIndex">
        /// The index of the StatusEffect.
        /// </param>
        /// <param name="sb">
        /// The StringBuilder that should be used.
        /// </param>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        private void AppendEffectDescription( int effectIndex, System.Text.StringBuilder sb, Statable statable )
        {
            string description = this.GetEffectDescription( effectIndex, statable );
            sb.Append( description );
        }

        /// <summary>
        /// Appends a seperator between two effect description elements.
        /// </summary>
        /// <param name="effectIndex">
        /// The index of the StatusEffect.
        /// </param>
        /// <param name="sb">
        /// The StringBuilder that should be used.
        /// </param>
        private void AppendSeperator( int effectIndex, System.Text.StringBuilder sb )
        {
            if( (effectIndex + 1) >= this.Effects.Count )
            {
                sb.AppendLine( Resources.ListEndSeperator );
            }
            else
            {
                sb.AppendLine( Resources.ListSeperator );
            }

            sb.Append( "  " );
        }

        /// <summary>
        /// Gets the description of the StatusEffect at the given index.
        /// </summary>
        /// <param name="effectIndex">
        /// The index of the StatusEffect.
        /// </param>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        /// <returns>
        /// The description of the effect.
        /// </returns>
        private string GetEffectDescription( int effectIndex, Statable statable )
        {
            StatusEffect effect = this.Effects[effectIndex];
            return effect.GetDescription( statable );
        }

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this <see cref="TimedAura"/>.
        /// </summary>
        /// <returns>The cloned Aura.</returns>
        public override Aura Clone()
        {
            var clone = new TimedAura();

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given TimedAura to be a clone of this TimedAura.
        /// </summary>
        /// <param name="clone">
        /// The TimedAura to setup as a clone of this TimedAura.
        /// </param>
        public void SetupClone( TimedAura clone )
        {
            clone.Duration = this.Duration;
            
            base.SetupClone( clone );
        }

        #endregion

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

            // Write Cooldown Information:
            context.Write( this.cooldown.TotalTime );
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

            // Read Cooldown Information:
            this.cooldown.TotalTime = context.ReadSingle();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the time this <see cref="TimedAura"/> lasts.
        /// </summary>
        private readonly Cooldown cooldown = new Cooldown();

        #endregion
    }
}