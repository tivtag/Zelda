// <copyright file="TimedStatusProcEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Procs.TimedStatusProcEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Procs
{
    using System.ComponentModel;
    using System.Globalization;
    
    /// <summary>
    /// Represents a <see cref="TimedStatusProcEffect"/> that applies
    /// </summary>
    public sealed class TimedStatusProcEffect : ProcEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the StatusEffect that gets temporarily applies by this TimedStatusProcEffect.
        /// </summary>
        [Editor( typeof( Zelda.Status.Design.StatusEffectEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public StatusEffect Effect
        {
            get 
            {
                return this.aura.Effects.Count == 0 ? null : this.aura.Effects[0];
            }
            
            set
            {
                this.aura.ClearEffects();
                this.aura.Effects.Add( value );
            }
        }

        /// <summary>
        /// Gets the list of StatusEffects that gets temporarily applied by this TimedStatusProcEffect.
        /// </summary>
        [Editor( "Zelda.Status.Design.StatusEffectListEditor, Library.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        public System.Collections.Generic.List<StatusEffect> Effects
        {
            get
            {
                return this.aura.Effects;
            }
        }

        /// <summary>
        /// Gets or sets the duration this TimedStatusProcEffect lasts.
        /// </summary>
        public float Duration
        {
            get
            {
                return this.aura.Duration;
            }

            set
            {
                this.aura.Duration = value;
            }
        }

        /// <summary>
        /// Gets or sets the symbol that shows up in the Buff Bar
        /// when this TimedStatusEffect procs.
        /// </summary>
        [Editor( typeof( Zelda.Graphics.Design.SpriteEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Atom.Xna.Sprite Symbol
        {
            get
            {
                return this.aura.Symbol;
            }

            set
            {
                this.aura.Symbol = value;
            }
        }

        /// <summary>
        /// Gets or sets the Color the <see cref="Symbol"/> is tinted in.
        /// </summary>
        public Microsoft.Xna.Framework.Color SymbolColor
        {
            get
            {
                return this.aura.SymbolColor;
            }

            set
            {
                this.aura.SymbolColor = value;
            }
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="StatusEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Statable statable )
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "Equip: Chance on {0} to gain\n  {1}\n  for {2} seconds.",
                this.GetProcChanceDescription( statable ),
                this.GetEffectDescription( statable ),
                this.Duration.ToString( CultureInfo.CurrentCulture )
            );
        }

        /// <summary>
        /// Gets a localized string that descripes the effect of the proc effect of this TimedStatusProcEffect.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        /// <returns>
        /// The localized effect description.
        /// </returns>
        private string GetEffectDescription( Statable statable )
        {
            return this.aura.GetEffectDescription( statable );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Modifies the power of this StatusEffect by the given factor.
        /// </summary>
        /// <param name="factor">
        /// The factor to change this StatusEffect by.
        /// </param>
        public override void ModifyPowerBy( float factor )
        {
            this.aura.ModifyPowerBy( factor );
        }

        /// <summary>
        /// Called when this TimedStatusProcEffect has actually procced.
        /// </summary>
        /// <param name="invoker">
        /// The Statable component of the entity that has invoked this TimedProcEffect.
        /// </param>
        protected override void OnProcced( Statable invoker )
        {
            this.aura.ResetDuration();

            if( !this.aura.IsEnabled )
            {
                invoker.AuraList.Add( this.aura );
            }
        }

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this TimedStatusProcEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new TimedStatusProcEffect();

            this.SetupClone( clone );

            return clone;
        }
        
        /// <summary>
        /// Setups the given TimedStatusProcEffect to be a clone of this TimedStatusProcEffect.
        /// </summary>
        /// <param name="clone">
        /// The StatusEffect to setup as a clone of this StatusEffect.
        /// </param>
        private void SetupClone( TimedStatusProcEffect clone )
        {
            this.aura.SetupClone( clone.aura );                  
            base.SetupClone( clone );
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <remarks>
        /// This method should be called first when overriding this method
        /// in a sub-class. It writes the global header of the StatusEffect.
        /// </remarks>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            this.aura.Serialize( context );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <remarks>
        /// This method should be called first when overriding this method
        /// in a sub-class. It reads the global header/data of the StatusEffect.
        /// </remarks>
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

            this.aura.Deserialize( context );
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The TimedAura that applies the StatusEffect of this TimedStatusProcEffect.
        /// </summary>
        private readonly TimedAura aura = new TimedAura() { IsVisible = true };

        #endregion
    }
}