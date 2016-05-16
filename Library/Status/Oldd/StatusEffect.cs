using System.ComponentModel;

namespace Zelda.Status
{
    /// <summary>
    /// Defines the abstract base class of all effects that manipulate a <see cref="Statable"/> ZeldaEntity.
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
        public abstract string Manipulates { get; }

        /// <summary>
        /// Gets a short localised description of this <see cref="StatusEffect"/>.
        /// </summary>
        public abstract string Description { get; }

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

        #region > Storage <

        /// <summary>
        /// Writes/Serializes the StatusEffect to the System.IO.BinaryWriter.
        /// </summary>
        /// <remarks>
        /// This method should be called first when overriding this method
        /// in a sub-class. It writes the global header of the StatusEffect.
        /// </remarks>
        /// <param name="writer">The System.IO.BinaryWriter to write the StatusEffect to.</param>
        public virtual void Serialize( System.IO.BinaryWriter writer )
        {
            // Write Global-Header:
            writer.Write( Acid.ReflectionUtilities.GetTypeName( this.GetType() ) );

            // Write Data:
            writer.Write( (int)this.DebuffFlags );
        }

        /// <summary>
        /// Reads/Deserializes the System.IO.BinaryReader into the StatusEffect.
        /// Assumes that the global header has been readen.
        /// </summary>
        /// <remarks>
        /// This method should be called first when overriding this method
        /// in a sub-class. It reads the global header/data of the StatusEffect.
        /// </remarks>
        /// <param name="reader">The System.IO.BinaryReader to read the StatusEffect's data from.</param>
        public virtual void Deserialize( System.IO.BinaryReader reader )
        {
            // Global Header has been readen!

            // Read Data:
            this.DebuffFlags = (DebuffFlags)reader.ReadInt32();
        }

        #endregion

        #endregion

        #region [ Fields ]

        #endregion
    }
}
