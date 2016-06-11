// <copyright file="RangedEnemyAttackSettings.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.Ranged.RangedEnemyAttackSettings class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks.Ranged
{
    using System.ComponentModel;
    using Atom;
    using Atom.Math;
    using Zelda.Entities.Projectiles;
    using Zelda.Entities.Projectiles.Drawing;

    /// <summary>
    /// Encapsulates the attack settings of a Ranged Enemy, controlled by a RangedEnemyBehaviour.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public sealed class RangedEnemyAttackSettings
    {
        /// <summary>
        /// Gets or sets the extra time that is added to the attack cooldown
        /// before the Ranged Enemy fires a Projectile attack again.
        /// </summary>
        public FloatRange ExtraTimeBetweenAttacks
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time the Ranged Enemy is unable to move for
        /// after firing a Projectile.
        /// </summary>
        public float TimeUnmoveableAfterAttack
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the AttackDamageMethod that is used to calculate the damage
        /// done by the Projectiles fired by the Ranged Enemy.
        /// </summary>
        public Zelda.Attacks.AttackDamageMethod DamageMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Sprite Group that is used to load the Projectile Sprites
        /// used by the Ranged Enemy.
        /// </summary>
        public string ProjectileSpriteGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the speed of the Projectiles fired by the Ranged Enemy.
        /// </summary>
        public IntegerRange ProjectileSpeed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="ProjectileHitSettings"/> of the Ranged Enemy.
        /// </summary>
        public ProjectileHitSettings HitSettings
        {
            get { return this.hitSettings; }
        }

        /// <summary>
        /// Gets the IProjectileSprites used to visualize the Projectiles
        /// fired by the Ranged Enemy.
        /// </summary>
        [Browsable(false)]
        public IProjectileSprites ProjectileSprites
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the RangedEnemyAttackSettings class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related settings.
        /// </param>
        internal RangedEnemyAttackSettings( IZeldaServiceProvider serviceProvider )
        {
            this.SetDefaults( serviceProvider );
            this.hitSettings.Setup( serviceProvider );
        }

        /// <summary>
        /// Setups this RangedAttackSettings to default values.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related settings.
        /// </param>
        private void SetDefaults( IZeldaServiceProvider serviceProvider )
        {
            this.ProjectileSpeed = new IntegerRange( 75, 90 );
            this.ExtraTimeBetweenAttacks = new FloatRange( 2.0f, 10.0f );

            this.DamageMethod = new Zelda.Attacks.Ranged.RangedDamageMethod();
            this.DamageMethod.Setup( serviceProvider );

            this.ProjectileSpriteGroup = "Octorok_Stone";
            this.TimeUnmoveableAfterAttack = 1.0f;
        }

        /// <summary>
        /// Tries to load the <see cref="ProjectileSprites"/> using the current <see cref="ProjectileSpriteGroup"/>.
        /// </summary>
        /// <param name="spriteLoader">
        /// Provides a mechanism that loads ISprite assets.
        /// </param>
        public void LoadProjectileSprites( Atom.Xna.ISpriteLoader spriteLoader )
        {
            this.ProjectileSprites = ProjectileSpritesHelper.Load( this.ProjectileSpriteGroup, spriteLoader );
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
            const int CurrentVersion = 4;
            context.Write( CurrentVersion );

            context.Write( this.ExtraTimeBetweenAttacks );
            context.Write( this.ProjectileSpeed );

            context.Write( this.TimeUnmoveableAfterAttack );
            context.Write( this.ProjectileSpriteGroup ?? string.Empty );
            context.Write( this.DamageMethod.GetType().GetTypeName() );

            this.hitSettings.Serialize( context );
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
            const int CurrentVersion = 4;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.ExtraTimeBetweenAttacks = context.ReadFloatRange();
            this.ProjectileSpeed = context.ReadIntegerRange();

            this.TimeUnmoveableAfterAttack = context.ReadSingle();
            this.ProjectileSpriteGroup = context.ReadString();

            // Add loading of DamageMethod --
            string damageMethodTypeName = context.ReadString();

            this.hitSettings.Deserialize( context );
        }

        /// <summary>
        /// The ProjectileHitSettings used by the Ranged Attack.
        /// </summary>
        private readonly ProjectileHitSettings hitSettings = new ProjectileHitSettings();
    }
}