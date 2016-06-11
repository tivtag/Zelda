// <copyright file="ChanceToContainer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Containers.ChanceToContainer class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Containers
{
    using System;

    /// <summary>
    /// Encapsulates various status properties with a chance;
    /// such as chance to parry, chance to miss or even chance to pierce.
    /// </summary>
    public class ChanceToContainer : Zelda.Saving.ISaveable
    {
        #region [ Constants ]

        /// <summary>
        /// The default chance to miss with an outgoing attack.
        /// </summary>
        private const float DefaultMiss = 5.0f;

        /// <summary>
        /// The default chance to crit with an outgoing attack.
        /// </summary>
        private const float DefaultCrit = 5.0f;

        /// <summary>
        /// The default chance to crit with a life restoring effect and blocking.
        /// </summary>
        private const float DefaultCritHealBlock = 2.5f;

        /// <summary>
        /// The default chance to dodge an incomming attack.
        /// </summary>
        private const float DefaultDodge = 3.5f;

        /// <summary>
        /// The default chance to parry an incomming attack.
        /// </summary>
        private const float DefaultParry = 2.5f;

        /// <summary>
        /// The default chance for a ranged attack to pierce.
        /// </summary>
        private const float DefaultPierce = 5.0f;

        /// <summary>
        /// The default chance to block an incomming attack.
        /// </summary>
        private const float DefaultBlock = 25.0f;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the chance to get a critical attack in %.
        /// </summary>
        public float Crit
        {
            get 
            { 
                return this.crit; 
            }

            protected internal set
            {
                this.crit = value;
            }
        }

        /// <summary>
        /// Gets or sets the chance to miss with an attack in %.
        /// </summary>
        public float Miss
        {
            get
            {
                return this.miss;
            }

            protected internal set
            {
                this.miss = value;
            }
        }

        /// <summary>
        /// Gets or sets the chance to get a critical heal in %.
        /// </summary>
        public float CritHeal
        {
            get
            {
                return this.critHeal;
            }

            protected internal set
            {
                this.critHeal = value;
            }
        }

        /// <summary>
        /// Gets or sets the chance to get a critical block in %.
        /// </summary>
        public float CritBlock
        {
            get
            {
                return this.critBlock;
            }

            protected internal set
            {
                this.critBlock = value;
            }
        }

        /// <summary>
        /// Gets or sets the chance to dodge an incomming attack.
        /// </summary>
        public float Dodge
        {
            get
            {
                return this.dodge;
            }

            protected internal set
            {
                this.dodge = value;
            }
        }

        /// <summary>
        /// Gets or sets the chance to parry an incomming attack.
        /// </summary>
        /// <remarks>
        /// Parried attacks still knockback.
        /// </remarks>
        public float Parry
        {
            get
            {
                return this.parry;
            }

            protected internal set
            {
                this.parry = value;
            }
        }

        /// <summary>
        /// Gets or sets the chance to block an incomming attack.
        /// </summary>
        public float Block
        {
            get
            {
                return this.block;
            }

            protected internal set
            {
                this.block = value;
            }
        }

        /// <summary>
        /// Gets or sets the chance that a ranged Projectile pierces through a target it hit.
        /// </summary>
        public float Pierce
        {
            get
            {
                return this.pierce;
            }

            protected internal set
            {
                this.pierce = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="AuraList"/> of the Statable that owns this ChanceToContainer.
        /// </summary>
        protected AuraList AuraList
        {
            get
            {
                return this.statable.AuraList;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the ChanceToContainer class.
        /// </summary>
        internal ChanceToContainer()
        {
        }

        /// <summary>
        /// Initializes this ChanceToContainer to be used
        /// for the specified Statable.
        /// </summary>
        /// <param name="statable">
        /// The Statable component that will own the ChanceToContainer.
        /// </param>
        public virtual void Initialize( Statable statable )
        {
            this.statable = statable;
        }

        #endregion

        #region [ Methods ]
        
        /// <summary>
        /// Gets the base chance for the specified ChanceToStatus.
        /// </summary>
        /// <param name="statusType">
        /// The ChanceToStatus modifier to get.
        /// </param>
        /// <returns>
        /// The base chance in percent.
        /// </returns>
        public float GetBase( ChanceToStatus statusType )
        {
            switch( statusType )
            {
                case ChanceToStatus.Block:
                    return this.baseBlock;

                case ChanceToStatus.Crit:
                    return this.baseCrit;

                case ChanceToStatus.Dodge:
                    return this.baseDodge;

                case ChanceToStatus.Miss:
                    return this.baseMiss;

                case ChanceToStatus.Parry:
                    return this.baseParry;

                case ChanceToStatus.Pierce:
                    return this.basePierce;

                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// Sets the base chance for the specified <see cref="ChanceToStatus"/>.
        /// </summary>
        /// <param name="statusType">
        /// The ChanceToStatus to set.
        /// </param>
        /// <param name="value">
        /// The value to set.
        /// </param>
        public void SetBase( ChanceToStatus statusType, float value )
        {
            switch( statusType )
            {
                case ChanceToStatus.Crit:
                    this.baseCrit = value;
                    break;

                case ChanceToStatus.Dodge:
                    this.baseDodge = value;
                    break;

                case ChanceToStatus.Parry:
                    this.baseParry = value;
                    break;

                case ChanceToStatus.Miss:
                    this.baseMiss = value;
                    break;

                case ChanceToStatus.Block:
                    this.baseBlock = value;
                    break;

                case ChanceToStatus.Pierce:
                    this.basePierce = value;
                    break;

                default:
                    throw new ArgumentException( statusType.ToString(), "statusType" );
            }
        }

        /// <summary>
        /// Sets the chance to dodge, parry and block to the specified value.
        /// </summary>
        /// <param name="value">
        /// The value to set.
        /// </param>
        internal void SetAvoidance( float value )
        {
            this.baseDodge = value;
            this.baseParry = value;
            this.baseBlock = value;

            this.dodge = value;
            this.parry = value;
            this.block = value;
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

            context.Write( this.baseBlock );
            context.Write( this.baseCrit );
            context.Write( this.baseDodge );
            context.Write( this.baseMiss );
            context.Write( this.baseParry );
            context.Write( this.basePierce );
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

            this.baseBlock = context.ReadSingle();
            this.baseCrit = context.ReadSingle();
            this.baseDodge = context.ReadSingle();
            this.baseMiss = context.ReadSingle();
            this.baseParry = context.ReadSingle();
            this.basePierce = context.ReadSingle();

            this.SetFinalToBase();
        }

        /// <summary>
        /// Sets the chance-to values to the base chance-to values.
        /// </summary>
        public void SetFinalToBase()
        {
            this.block = this.baseBlock;
            this.crit = this.baseCrit;
            this.dodge = this.baseDodge;
            this.miss = this.baseMiss;
            this.parry = this.baseParry;
            this.pierce = this.basePierce;
        }

        /// <summary>
        /// Setups the given ChanceToContainer to be a clone of this
        /// ChanceToContainer.
        /// </summary>
        /// <param name="clone">
        /// The ChanceToContainer to setup as a clone of this ChanceToContainer.
        /// </param>
        internal void SetupClone( ChanceToContainer clone )
        {
            clone.baseBlock = this.baseBlock;
            clone.baseCrit = this.baseCrit;
            clone.baseDodge = this.baseDodge;
            clone.baseMiss = this.baseMiss;
            clone.baseParry = this.baseParry;
            clone.basePierce = this.basePierce;

            clone.SetFinalToBase();
        }

        #endregion

        #region [ Fields ]

        /// <summary> 
        /// The chance to miss with an outgoing attack.
        /// </summary>
        private float baseMiss = DefaultMiss, miss = DefaultMiss;

        /// <summary> 
        /// The chance to crit with an outgoing attack.
        /// </summary>
        private float baseCrit = DefaultCrit, crit = DefaultCrit;

        /// <summary>
        /// The chance to crit with a life-restoring effect.
        /// </summary>
        private float critHeal = DefaultCritHealBlock;

        /// <summary>
        /// The chance to crit with a block.
        /// </summary>
        private float critBlock = DefaultCritHealBlock;
                
        /// <summary> 
        /// The chance to dodge an incomming attack.
        /// </summary>
        private float baseDodge = DefaultDodge, dodge = DefaultDodge;
                
        /// <summary> 
        /// The chance to parry an incomming attack.
        /// </summary>
        private float baseParry = DefaultParry, parry = DefaultParry;

        /// <summary>
        /// The chance to block and incomming attack.
        /// </summary>
        private float baseBlock = DefaultBlock, block = DefaultBlock;

        /// <summary>
        /// The chance to pierce with a ranged attack.
        /// </summary>
        private float basePierce = DefaultBlock, pierce = DefaultBlock;
        
        /// <summary>
        /// Identifies the Statable component that owns the new ChanceToContainer.
        /// </summary>
        private Statable statable;

        #endregion
    }
}
