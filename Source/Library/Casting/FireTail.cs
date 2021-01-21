
namespace Zelda.Casting
{
    using Atom.Math;
    using Zelda.Attacks;
    using Zelda.Casting.Spells;
    using Zelda.Entities;
    using Zelda.Entities.Behaviours;
    using Zelda.Entities.Drawing;
    using Zelda.Entities.Spawning;
    using Zelda.Status;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a tail of fire bombs that explore in a row.
    /// </summary>
    public sealed class FireTail : ZeldaEntity
    {
        private const float TimeBetweenPillars = 0.5f;
        private const float SpellDuration = 15.0f;
        private const int ExplosionIndex = 5;

        /// <summary>
        /// Gets or sets the color tint of this FireTail. 
        /// </summary>
        public Xna.Color Color
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new <see cref="FireTail"/> instance.
        /// </summary>
        public FireTail( 
            ZeldaEntity target, 
            ZeldaEntity owner, 
            AttackDamageMethod method,
            AttackDamageMethod explosionMethod,
            IZeldaServiceProvider serviceProvider )
        {
            this.Color = Xna.Color.DimGray;
            this.target = target;
            this.target.Removed += this.OnTargetRemoved;
            this.explosionMethod = explosionMethod;

            method.Setup( serviceProvider );
            explosionMethod.Setup( serviceProvider );

            this.spell = new FirePillarSpell(
                owner,
                owner.Components.Find<Statable>(),
                0.0f,
                method,
                serviceProvider,
                "FirePillar_B"
            );

            spell.AllowedAnimationIndexRange = new IntegerRange( 8, 22 );
            spell.Setup( serviceProvider );

            bombAudio = new BombAudio( serviceProvider.AudioSystem, serviceProvider.Rand );
        }

        private void SpawnPillar()
        {
            Vector2 position = target.Transform.Position + new Vector2( 1.0f, 6.0f );
            DamageEffectEntity pillar = spell.SpawnFirePillarAt( position, target.FloorNumber, target.Scene );

            var removeBehaviour = new RemoveAfterAnimationEndedNtimesBehaviour( pillar ) {
                TimesAnimationHasToEnded = 1
            };

            var explodeBehaviour = new LambdaBehaviour<DamageEffectEntity>( pillar, this.OnPillarUpdated );
            pillar.Behaveable.Behaviour = new MultiBehaviour( pillar, new IEntityBehaviour[] { explodeBehaviour, removeBehaviour } );

            var tinted = (TintedDrawDataAndStrategy)pillar.DrawDataAndStrategy;
            tinted.BaseColor = this.Color;

            SpawnHelper.AddBlendInColorTint( pillar );
        }

        /// <summary>
        /// Updates this FireTail.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            timeUntilNextPillar -= updateContext.FrameTime;
            timeLeft -= updateContext.FrameTime;

            if( timeLeft <= 0.0f )
            {
                this.Despawn();
                return;
            }

            if( timeUntilNextPillar <= 0.0f )
            {
                timeUntilNextPillar = TimeBetweenPillars;
                this.SpawnPillar();
            }
        }

        private void OnTargetRemoved( object sender, ZeldaScene e )
        {
            Despawn();
        }

        private void Despawn()
        {
            target.Removed -= OnTargetRemoved;
            this.RemoveFromScene();
        }

        private void OnPillarUpdated( LambdaBehaviour<DamageEffectEntity> sender, ZeldaUpdateContext updateContext )
        {
            var dds = (IAnimatedDrawDataAndStrategy)sender.Owner.DrawDataAndStrategy;

            if( dds.CurrentAnimation.FrameIndex == ExplosionIndex )
            {
                const string Id = "boom";

                if( !sender.Data.Contains( Id ) )
                {
                    this.OnPillarExploded( sender.Owner );
                    sender.Data.Add( Id );
                }
            }
        }

        private void OnPillarExploded( DamageEffectEntity pillar )
        {
            const float ExplosionRadius = 35.0f;
            const float PushingPower = 100.0f;

            Bomb bomb = Bomb.Spawn( 
                spell.OwnerStatable, 
                pillar.Collision.Center,
                pillar.FloorNumber,
                this.explosionMethod,
                null,
                ExplosionRadius,
                PushingPower,
                null,
                null,
                Vector2.Zero,
                bombAudio.GetRandomSound()
            );

            bomb.Explode( target.Scene );
        }

        private float timeUntilNextPillar = TimeBetweenPillars;
        private float timeLeft = SpellDuration;

        private readonly ZeldaEntity target;
        private readonly FirePillarSpell spell;
        private readonly BombAudio bombAudio;
        private readonly AttackDamageMethod explosionMethod;
    }
}
