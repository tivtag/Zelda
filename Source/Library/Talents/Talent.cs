// <copyright file="Talent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.Talent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Talents
{ 
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Atom;
    using Atom.Math;
    
    /// <summary>
    /// A talent is an improvement to the player's character.
    /// The player can aquire them by investing talent points,
    /// which he gets each level-up.
    /// <para>
    /// Every level 1 extra talent point,
    /// but on every 5th level 2 extra points
    /// and on every 10th level 3 extra points.s
    /// </para>
    /// <para>
    /// Talents come in two forms;
    /// passive Talents which are always active and
    /// active Talents which the Player has to active.
    /// These active Talents are also called Skills.
    /// </para>
    /// </summary>
    public abstract class Talent : IDescriptionProvider
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the <see cref="Level"/> of this Talent has changed.
        /// </summary>
        public event EventHandler LevelChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the talent that follow this talent.
        /// </summary>
        public IEnumerable<Talent> Following
        {
            get
            {
                return this.followingTalents ?? Enumerable.Empty<Talent>();
            }
        }

        /// <summary>
        /// Gets or sets the number of Talent Points the Player
        /// has invested into the Talent.
        /// </summary>
        public int Level
        {
            get
            {
                return this.level;
            }

            protected set
            {
                if( value == this.level )
                    return;

                this.level = value;
                this.LevelChanged.Raise( this );
            }
        }

        /// <summary>
        /// Gets the maximum possible number of Talent Points the Player
        /// can invest into the Talent.
        /// </summary>
        public int MaximumLevel
        {
            get
            {
                return this.maximumLevel;
            }
        }

        /// <summary>
        /// Gets the localized name of this <see cref="Talent"/>.
        /// </summary>
        public string LocalizedName
        {
            get
            { 
                return this.localizedName;
            }
        }

        /// <summary>
        /// Gets the (localized) description of this <see cref="Talent"/>.
        /// </summary>
        public string Description
        {
            get
            {

                int level = this.Level;
                if( level == 0 )
                    level = 1;

                return this.GetDescription( level );
            }
        }
        
        /// <summary>
        /// Gets the symbol that is displayed for the Talent.
        /// </summary>
        public Atom.Xna.Sprite Symbol
        {
            get
            {
                return this.symbol;
            }
        }

        /// <summary>
        /// Gets the TalentTree that 'owns' the <see cref="Talent"/>.
        /// </summary>
        public TalentTree Tree
        {
            get
            {
                return this.tree;
            }
        }

        /// <summary>
        /// Gets the owner of this <see cref="Talent"/>.
        /// </summary>
        public Entities.PlayerEntity Owner
        {
            get 
            { 
                return this.tree.Owner; 
            }
        }

        /// <summary>
        /// Gets the main category/tree under which this Talent is grouped.
        /// </summary>
        public TalentCategory Category
        {
            get
            {
                return this.category;
            }
        }

        /// <summary>
        /// Gets the type of this Talent.
        /// </summary>
        public TalentType Type
        {
            get
            {
                return (this is SkillTalent) ? TalentType.Active : TalentType.Passive;
            }
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Status.ExtendedStatable"/> component
        /// of the PlayerEntity that owns this Talent.
        /// </summary>
        protected Status.ExtendedStatable Statable
        {
            get
            { 
                return this.tree.Owner.Statable; 
            }
        }

        /// <summary>
        /// Gets the <see cref="Zelda.Status.AuraList"/>
        /// of the PlayerEntity that owns this Talent.
        /// </summary>
        protected Status.AuraList AuraList
        {
            get 
            {
                return this.tree.Owner.Statable.AuraList;
            }
        }

        /// <summary>
        /// Gets the <see cref="IZeldaServiceProvider"/> object
        /// which provides type-safe access to game services.
        /// </summary>
        protected IZeldaServiceProvider ServiceProvider
        {
            get 
            {
                return this.serviceProvider;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Talent"/> class.
        /// </summary>
        /// <param name="localizedName">
        /// The localized name of the talent.
        /// </param>
        /// <param name="symbol">
        /// The symbol of the Talent.
        /// </param>
        /// <param name="maximumLevel">
        /// The maximum number of TalentPoints the Player can invest into the talent.
        /// </param>
        /// <param name="tree">
        /// The TalentTree that 'owns' the new Talent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides type-safe access to game services.
        /// </param>
        protected internal Talent( 
            string localizedName,
            Atom.Xna.Sprite symbol, 
            int maximumLevel, 
            TalentTree tree, 
            IZeldaServiceProvider serviceProvider )
        {
            if( localizedName == null )
                throw new ArgumentNullException( "localizedName" );
            if( symbol == null )
                throw new ArgumentNullException( "symbol" );
            if( maximumLevel <= 0 )
                throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsZeroOrNegative, "maximumLevel" );
            if( tree == null )
                throw new ArgumentNullException( "tree" );

            this.localizedName   = localizedName;
            this.symbol          = symbol;
            this.maximumLevel    = maximumLevel;
            this.tree            = tree;
            this.serviceProvider = serviceProvider;
            this.category = Talent.GetCategory( this.GetType() );
        }
        
        /// <summary>
        /// Setups the Talent's TalentRequirements, following Talents and any additional components.
        /// </summary>
        /// <remarks>
        /// SetTreeStructure has to be used within the method
        /// to actually set the data.
        /// </remarks>
        internal abstract void SetupNetwork();

        /// <summary>
        /// Initializes this Talent.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Uninitializes this Talent.
        /// </summary>
        protected abstract void Uninitialize();

        /// <summary>
        /// Calls <see cref="Initialize"/> if this Talent
        /// has not yet been initialized.
        /// </summary>
        private void InitializeCore()
        {
            if( !this.isInitialized )
            {
                this.Initialize();
                this.isInitialized = true;
            }
        }

        /// <summary>
        /// Calls <see cref="Uninitialize"/> if this Talent
        /// has not yet been uninitialized.
        /// </summary>
        private void UninitializeCore()
        {
            if( this.isInitialized )
            {
                this.Uninitialize();
                this.isInitialized = false;
            }
        }

        /// <summary>
        /// Sets the fields that makeup the tree structure of this Talent.
        /// </summary>
        /// <param name="requirements">
        /// Descripes the talents this Talent requires.
        /// </param>
        /// <param name="followingTalents">
        /// The talents that follow this Talent (and also require this Talent).
        /// </param>
        protected void SetTreeStructure( TalentRequirement[] requirements, Talent[] followingTalents )
        {
            this.requirements = requirements;
            this.followingTalents = followingTalents;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Resets this Talent.
        /// </summary>
        public void Reset()
        {
            this.SetLevel( 0 );
        }

        /// <summary>
        /// Refreshes the strength of this Talent
        /// based on the current level.
        /// </summary>
        /// <remarks>
        /// Iniitialize is ensured to be called before Refresh.
        /// </remarks>
        protected abstract void Refresh();

        /// <summary>
        /// Sets the level of this Talent.
        /// </summary>
        /// <param name="newLevel">
        /// The new level.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="newLevel"/> is invalid.
        /// </exception>
        internal void SetLevel( int newLevel )
        {
            if( newLevel < 0 || newLevel > this.MaximumLevel )
                throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, "newLevel" );

            this.level = newLevel;

            if( this.level == 0 )
            {
                this.UninitializeCore();
            }
            else
            {
                this.InitializeCore();
                this.Refresh();
            }
        }

        /// <summary>
        /// Increases the level of this Talent by one.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// If the Talent already has reached it maximum possible level.
        /// </exception>
        public void GainLevel()
        {
            if( this.level == maximumLevel )
                throw new System.InvalidOperationException();

            SetLevel( this.level + 1 );
        }

        /// <summary>
        /// Gets a value indicating whether the requirments
        /// needed to invest into this Talent have bene fulfilled.
        /// </summary>
        /// <returns>
        /// Returns <see langword="true"/> if the player fulfills
        /// the requirements; otherwise <see langword="false"/>.
        /// </returns>
        public bool FulfillsRequirements()
        {
            if( requirements == null )
                return true;

            foreach( TalentRequirement requirement in requirements )
            {
                if( requirement.IsFulfilled == false )
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the requirments
        /// needed to invest into this Talent have bene fulfilled; if the given talent is changed to the given level.
        /// </summary>
        /// <param name="changedTalent">
        /// The talent that is beeing changed.
        /// </param>
        /// <param name="changedTalentLevel">
        /// The level the talent is beeing changed to.
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> if the player fulfills
        /// the requirements; otherwise <see langword="false"/>.
        /// </returns>
        public bool FulfillsRequirementsWith( Talent changedTalent, int changedTalentLevel )
        {
            if( requirements == null )
                return true;

            foreach( TalentRequirement requirement in requirements )
            {
                if( requirement.RequiredTalent == changedTalent )
                {
                    if( !requirement.IsFulfilledAt( changedTalentLevel ) )
                        return false;
                }
                else
                {
                    if( !requirement.IsFulfilled )
                        return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Gets the following talent at the given zero-based index.
        /// </summary>
        /// <param name="index">The index of the talent to get.</param>
        /// <returns>
        /// The talent at the given index, or null if at that inde position is no Talent.
        /// </returns>
        public Talent GetFollowing( int index )
        {
            if( followingTalents == null )
                return null;

            if( index < 0 || index >= followingTalents.Length )
                return null;

            return followingTalents[index];
        }

        /// <summary>
        /// Gets the TalentRequirement at the given zero-based index.
        /// </summary>
        /// <param name="index">The index of the talent to get.</param>
        /// <returns>
        /// The TalentRequirement at the given index, or null if at that index position is no TalentRequirement.
        /// </returns>
        public TalentRequirement GetRequirement( int index )
        {
            if( requirements == null )
                return null;

            if( index < 0 || index >= requirements.Length )
                return null;

            return requirements[index];
        }
        
        /// <summary>
        /// Gets the TalentRequirement of the Talent that requires the given <paramref name="requiredTalent"/>.
        /// </summary>
        /// <param name="requiredTalent">The talent that is required.</param>
        /// <returns>
        /// The TalentRequirement that corresponds to the given requiredTalent, or null if there is no such requirement.
        /// </returns>
        public TalentRequirement GetRequirement( Talent requiredTalent )
        {
            if( requirements == null )
                return null;

            foreach( TalentRequirement req in requirements )
            {
                if( req.RequiredTalent == requiredTalent )
                    return req;
            }

            return null;
        }

        /// <summary>
        /// Gets the description of this Talent for
        /// the specified talent level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent to get the description for.
        /// Is safely clamped into a valid value.
        /// </param>
        /// <returns>
        /// The localized description of this Talent for the specified talent level.
        /// </returns>
        public string GetDescriptionSafe( int level )
        {
            level = MathUtilities.Clamp( level, 0, this.MaximumLevel );
            return this.GetDescription( level );
        }

        /// <summary>
        /// Gets the description of this Talent for
        /// the specified talent level.
        /// </summary>
        /// <param name="level">
        /// The level of the talent to get the description for.
        /// </param>
        /// <returns>
        /// The localized description of this Talent for the specified talent level.
        /// </returns>
        protected abstract string GetDescription( int level );
        
        private static TalentCategory GetCategory( Type type )
        {
            string str = type.Namespace.Split( '.' ).Last();

            TalentCategory category;
            Enum.TryParse<TalentCategory>( str, out category );
            return category;
        }

        #endregion

        #region [ Fields ]

        /// <summary> 
        /// The number of points the player invested into this Talent. 
        /// </summary>
        private int level;

        /// <summary>
        /// The maximum number of points the player can invest into this Talent.
        /// </summary>
        private readonly int maximumLevel;

        /// <summary>
        /// The localized name of this <see cref="Talent"/>.
        /// </summary>
        private readonly string localizedName;

        /// <summary>
        /// The symbol that is displayed for this Talent.
        /// </summary>
        private readonly Atom.Xna.Sprite symbol;

        /// <summary>
        /// The requirements the Player must fullfil to be able to invest into this <see cref="Talent"/>.
        /// There can be up to three TalentRequirements per Talent.
        /// </summary>
        private TalentRequirement[] requirements;

        /// <summary>
        /// The requirements the Player must fullfil to be able to invest into this <see cref="Talent"/>.
        /// There can be up to three TalentRequirements per Talent.
        /// </summary>
        private Talent[] followingTalents;

        /// <summary>
        /// States whether this Talent is currently initialized.
        /// </summary>
        private bool isInitialized;

        /// <summary>
        /// The main category/tree under which this Talent is grouped.
        /// </summary>
        private readonly TalentCategory category;

        /// <summary>
        /// The TalentTree that 'owns' this <see cref="Talent"/>.
        /// </summary>
        private readonly TalentTree tree;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion
    }
}
