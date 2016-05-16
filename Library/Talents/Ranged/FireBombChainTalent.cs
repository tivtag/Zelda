
namespace Zelda.Talents.Ranged
{
    /// <summary>
    /// The Bomb Chain talent allows the player to place more than one FireBomb before the cooldown is triggered.
    /// </summary>
    public sealed class FireBombChainTalent : Talent
    {
        /// <summary>
        /// The minimum time in seconds between bombs.
        /// </summary>
        public const float GlobalCooldown = 0.4f;

        /// <summary>
        /// The number of extra bombs this FireBombChainTalent provides per level to the FireBombSkill.
        /// </summary>
        private const int ExtraBombsPerLevel = 2;

        /// <summary>
        /// The number of seconds added to the cooldown for each talent level.
        /// </summary>
        private const float ExtraCooldownPerLevel = 1.25f;
                
        /// <summary>
        /// Gets the number of extra bombs this FireBombChainTalent provides to the FireBombSkill.
        /// </summary>
        public int ExtraBombs
        {
            get
            {
                return this.Level * ExtraBombsPerLevel;
            }
        }

        /// <summary>
        /// The number of seconds added to the cooldown added to the FireBombSkill.
        /// </summary>
        public float ExtraCooldown
        {
            get
            {
                return this.Level * ExtraCooldownPerLevel;
            }
        }

        /// <summary>
        /// Gets the reduction of mana needed per point of base mana for each bomb.
        /// </summary>
        public float ManaCostReductionPoBM
        {
            get
            {
                return this.Level > 0 ? 0.1f : 0.0f;
            }
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
        protected override string GetDescription( int level )
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            return string.Format( 
                culture,
                TalentResources.TD_FireBombChain,
                (level * ExtraBombsPerLevel).ToString( culture ),
                (level * ExtraCooldownPerLevel).ToString( culture )
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FireBombChainTalent"/> class.
        /// </summary>
        /// <param name="tree">
        /// The TalentTree that owns the new FireBombChainTalent.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal FireBombChainTalent( TalentTree tree, IZeldaServiceProvider serviceProvider )
            : base(
                TalentResources.TN_FireBombChain,
                serviceProvider.SpriteLoader.LoadSprite( "Symbol_FireBombChain" ),
                3,
                tree, 
                serviceProvider
            )
        {
        }

        /// <summary>
        /// Setups the connections of this FireBombChainTalent with the other Talents of the TalentTree.
        /// </summary>
        internal override void SetupNetwork()
        {
            TalentRequirement[] requirements = new TalentRequirement[1] {
                new TalentRequirement( this.Tree.GetTalent( typeof( FireBombTalent ) ), 1 )
            };

            Talent[] following = null;
            this.SetTreeStructure( requirements, following );
        }

        /// <summary>
        /// Initializes this FireBombChainTalent.
        /// </summary>
        protected override void Initialize()
        {
            // no op.
        }

        /// <summary>
        /// Uninitializes this FireBombChainTalent.
        /// </summary>
        protected override void Uninitialize()
        {
            // no op.
        }

        /// <summary>
        /// Refreshes the strength of this FireBombChainTalent.
        /// </summary>
        protected override void Refresh()
        {
            var fireBombTalent = this.Tree.GetTalent<FireBombTalent>();
            fireBombTalent.Skill.RefreshDataFromTalents();
        }
    }
}
