// <copyright file="ChanceToBeContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Containers.ChanceToBeContainer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Containers
{
    using System;
    
    /// <summary>
    /// Encapsulates various status properties with a chance;
    /// such as chance-to-be parry or chance-to-be hit.
    /// </summary>
    public class ChanceToBeContainer : Zelda.Saving.ISaveable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the base extra chance for the Statable that own this ChanceToBeContainer
        /// to be the receiver of a critical attack.
        /// </summary>
        public float BaseCrit
        {
            get
            {
                return this.baseCrit;
            }

            set
            {
                this.baseCrit = value;
                this.crit = value;
            }
        }

        /// <summary>
        /// Gets or sets the base extra chance for the Statable that owns this ChanceToBeContainer
        /// to be the receiver of an attack.
        /// </summary>
        public float BaseHit
        {
            get
            {
                return this.baseHit;
            }

            set
            {
                this.baseHit = value;
                this.hit = value;
            }
        }

        /// <summary>
        /// Gets or sets the extra chance for the Statable that own this ChanceToBeContainer
        /// to be the receiver of a critical attack.
        /// </summary>
        public float Crit
        {
            get
            {
                return this.crit;
            }

            protected set
            {
                this.crit = value;
            }
        }

        /// <summary>
        /// Gets or sets the extra chance for the Statable that owns this ChanceToBeContainer
        /// to be the receiver of an attack.
        /// </summary>
        public float Hit
        {
            get
            {
                return this.hit;
            }

            protected set
            {
                this.hit = value;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the ChanceToBeContainer class.
        /// </summary>
        internal ChanceToBeContainer()
        {
        }

        /// <summary>
        /// Initializes this ChanceToBeContainer.
        /// </summary>
        /// <param name="statable">
        /// The statable that owns this ChanceToBeContainer.
        /// </param>
        public virtual void Initialize( Statable statable )
        {
            this.statable = statable;
        }
        
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the chance-to-be value for the specified ChanceToStatus.
        /// </summary>
        /// <param name="statusType">
        /// The ChanceToStatus to refresh.
        /// </param>
        internal void Refresh( ChanceToStatus statusType )
        {
            switch( statusType )
            {
                case ChanceToStatus.Crit:
                    this.RefreshCrit();
                    break;

                case ChanceToStatus.Miss:
                    this.RefreshHit();
                    break;

                default:
                    throw new NotImplementedException( statusType.ToString() );
            }
        }

        /// <summary>
        /// Refreshes the extra chance to be hit by an enemy attack.
        /// </summary>
        internal virtual void RefreshHit()
        {
            float fixedValue, percentValue, ratingValue;
            this.statable.AuraList.GetEffectValues(
                ChanceToBeStatusEffect.IdentifierToBeHit,
                out fixedValue,
                out percentValue,
                out ratingValue
            );

            this.Hit = StatusCalc.GetChanceToBeHit( this.baseHit + fixedValue, percentValue, ratingValue, this.statable.Level );
        }

        /// <summary>
        /// Refreshes the extra chance to be crit by an enemy attack.
        /// </summary>
        internal virtual void RefreshCrit()
        {
            float fixedValue = this.statable.AuraList.GetFixedEffectValue(
                ChanceToBeStatusEffect.IdentifierToBeCrit
            );

            this.Crit = this.baseCrit + fixedValue;
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

            context.Write( this.baseCrit );
            context.Write( this.baseHit );
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

            this.baseCrit = context.ReadSingle();
            this.baseHit = context.ReadSingle();
            this.SetFinalToBase();
        }

        /// <summary>
        /// Sets the final values to the base values.
        /// </summary>
        private void SetFinalToBase()
        {
            this.hit = this.baseHit;
            this.crit = this.baseCrit;
        }

        /// <summary>
        /// Setups the given ChanceToBeContainer to be a clone of this ChanceToBeContainer.
        /// </summary>
        /// <param name="clone">
        /// The ChanceToBeContainer to setup as a clone of this ChanceToBeContainer
        /// </param>
        internal void SetupClone( ChanceToBeContainer clone )
        {
            clone.hit = this.hit;
            clone.crit = this.crit;
            clone.baseHit = this.baseHit;
            clone.baseCrit = this.baseCrit;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The extra chance for the owner of this ChanceToBeContainer to be
        /// the receiver of a critical attack. Might be a negative value.
        /// </summary>
        private float crit, baseCrit;

        /// <summary>
        /// The extra chance for the owner of this ChanceToBeContainer to be
        /// the receiver of an attack. Might be a negative value.
        /// </summary>
        private float hit, baseHit;

        /// <summary>
        /// Identifies the t component that owns this ChanceToBeContainer.
        /// </summary>
        private Statable statable;

        #endregion
    }
}
