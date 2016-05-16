
namespace Zelda.Attacks
{
    using Atom.Math;
    using Atom.Xna;
    using Zelda.Attacks.Melee;
    using Zelda.Entities;
    using Zelda.Entities.Components;
    using Zelda.Entities.Drawing;
    using Zelda.Items;

    /// <summary>
    /// Tracks the current 'normal' melee attack of the player
    /// -----------
    /// | | = frame
    /// x   = hit
    /// o   = no hit
    /// -   = stopped tracking
    /// -----------
    /// |x|-|-|-|-|
    /// |o|x|-|-|-|
    /// |o|o|x|-|-|
    /// |o|o|o|x|-|
    /// |o|o|o|o|x|
    /// |o|o|o|o|o|
    /// </summary>
    internal sealed class PlayerMultiFrameAttackTracker : ZeldaComponent
    {
        public override void InitializeBindings()
        {
            this.player = (PlayerEntity)this.Owner;
            player.Attackable.AttackFiring += OnAttackFiring;
            player.Attackable.AttackHit += OnAttackHit;
        }

        public override void Update(Atom.IUpdateContext updateContext)
        {
            if(activeMeleeAttack == null)
                return;

            PlayerDrawDataAndStrategy dds = player.DrawDataAndStrategy;

            if (dds.SpecialAnimation == PlayerSpecialAnimation.AttackMelee)
            {
                // Ignore the first frame, because damage is already calculated for it
                SpriteAnimation animation = dds.CurrentAnimation;
                if (animation == null || animation.FrameIndex == 0)
                {
                    return;
                }

                WeaponInstance weaponInstance = player.Equipment.WeaponHand;
                if (weaponInstance != null)
                {
                    AnimatedSpriteFrame frame = dds.GetActiveEquipmentFrame( weaponInstance.Weapon );
                    if (frame != null && frame.Sprite != null)
                    {
                        Vector2 position = dds.DrawPosition + frame.Offset;
                        Point2 attackSize = frame.Sprite.Size;
                        RectangleF attackArea = new RectangleF( position, attackSize );

                        activeMeleeAttack.HandleAttack(ref attackArea);
                    }
                }
            }
        }

        private void OnAttackFiring(object sender, Attack attack)
        {
            this.activeMeleeAttack = attack as PlayerMeleeAttack;
        }

        private void OnAttackHit(object sender, AttackEventArgs e)
        {
            activeMeleeAttack = null;
        }

        private PlayerEntity player;
        private PlayerMeleeAttack activeMeleeAttack;
    }
}
