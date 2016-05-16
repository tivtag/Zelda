// <copyright file="StatusEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.StatusEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    using System.ComponentModel;

    /// <summary>
    /// Defines the abstract base class of all StatusEffects that manipulate a <see cref="Statable"/> ZeldaEntity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// StatusEffects aren't bound to a specific entity, but are globally shared among all <see cref="Statable"/> entities.
    /// We use this system because StatusEffects are designed to be modified only during design time.
    /// </para>
    /// <para>
    /// StatusEffect are applied to a statable Entity by adding 
    /// them within an <see cref="Aura"/> to the <see cref="AuraList"/> of the statable entity.
    /// </para>
    /// </remarks>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public abstract class StatusEffect
    {
        #region [ Properties ]

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="StatusEffect"/> manipulates.
        /// </summary>
        /// <remarks>
        /// The Manipulates value is looked at when an <see cref="AuraList"/> 
        /// chooses to find all StatusEffects that apply to one specific kind of status.
        /// Examples would be "Strength", "Crit" or "AtkSpd".
        /// </remarks>
        [Browsable( false )]
        public abstract string Identifier { get; }

        /// <summary>
        /// Gets a short localised description of this <see cref="StatusEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public abstract string GetDescription( Statable statable );

        /// <summary>
        /// Gets or sets the <see cref="DebuffFlags"/> of this <see cref="StatusEffect"/>.
        /// </summary>
        public DebuffFlags DebuffFlags
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="StatusEffect"/> is a debuff.
        /// </summary>
        [Browsable( false )]
        public bool IsDebuff
        {
            get { return this.DebuffFlags != DebuffFlags.None; }
        }

        /// <summary>
        /// Gets a value indicating whether this StatusEffect is 'bad' for the statable ZeldaEntity.
        /// </summary>
        /// <value>The default value is false.</value>
        public virtual bool IsBad
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether this StatusEffect has no actual effect.
        /// </summary>
        public virtual bool IsUseless
        {
            get { return false; }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusEffect"/> class.
        /// </summary>
        protected StatusEffect()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusEffect"/> class.
        /// </summary>
        /// <param name="debuffFlags">
        /// Descripes what debuffs the new <see cref="StatusEffect"/> applies.
        /// </param>
        protected StatusEffect( DebuffFlags debuffFlags )
        {
            this.DebuffFlags = debuffFlags;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public abstract void OnEnable( Statable user );

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public abstract void OnDisable( Statable user );

        /// <summary>
        /// Gets a value indicating whether the given StatusEffect is 'equal' to this StatusEffect.
        /// </summary>
        /// <param name="effect">
        /// The StatusEffect to compare with this.
        /// </param>
        /// <returns>
        /// Returns true if they capture the same 'concept';
        /// otherwise false.
        /// </returns>
        public abstract bool Equals( StatusEffect effect );
        
        /// <summary>
        /// Modifies the power of this StatusEffect by the given factor.
        /// </summary>
        /// <param name="factor">
        /// The factor to change this StatusEffect by.
        /// </param>
        public abstract void ModifyPowerBy( float factor );

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this StatusEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public abstract StatusEffect Clone();

        /// <summary>
        /// Setups the given StatusEffect to be a clone of this StatusEffect.
        /// </summary>
        /// <param name="clone">
        /// The StatusEffect to setup as a clone of this StatusEffect.
        /// </param>
        protected void SetupClone( StatusEffect clone )
        {
            clone.DebuffFlags = this.DebuffFlags;
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
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            // Write Global-Header:
            context.Write( Atom.ReflectionExtensions.GetTypeName( this.GetType() ) );

            // Write Data:
            context.Write( (int)this.DebuffFlags );
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
        public virtual void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            // Global Header has been readen!

            // Read Data:
            this.DebuffFlags = (DebuffFlags)context.ReadInt32();
        }

        #endregion

        #endregion

        #region [ Fields ]

        #endregion
    }
}
