
namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    using Zelda.Saving;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Holds the sprites responsible for drawing the main character: Link!
    /// </summary>
    public sealed class LinkSprites
    {
        /// <summary>         
        /// Enumerates the default colors used by the Link sprites.
        /// These are tintable.
        /// </summary>
        public struct ColorDefaults
        {
            public static readonly Xna.Color ClothMain = new Xna.Color( 80, 144, 16 );
            public static readonly Xna.Color ClothSecondary = new Xna.Color( 68, 133, 11 );
            public static readonly Xna.Color ClothHighlight = new Xna.Color( 120, 184, 32 );

            public static readonly Xna.Color HairMain = new Xna.Color( 176, 120, 24 );
            public static readonly Xna.Color HairHighlight = new Xna.Color( 216, 168, 0 );
        }

        /// <summary>
        /// The Sprites that show Link standing around.
        /// </summary>
        public readonly Sprite StandLeft, StandRight, StandDown, StandUp;

        /// <summary>
        /// The SpriteAnimations that shown Link swimming (but not moving) in deep water.
        /// </summary>
        public readonly SpriteAnimation SwimStandLeft, SwimStandRight, SwimStandDown, SwimStandUp;

        /// <summary>
        /// The SpriteAnimations that show Link moving around.
        /// </summary>
        public readonly SpriteAnimation MoveLeft, MoveRight, MoveDown, MoveUp;

        /// <summary>
        /// The SpriteAnimations that shown Link swimming in deep water.
        /// </summary>
        public readonly SpriteAnimation SwimLeft, SwimRight, SwimDown, SwimUp;

        /// <summary>
        /// The Sprites that show Link closes/opens his eyes.
        /// </summary>
        public readonly Sprite ClosingEyesDown, ClosedEyesDown, ClosingEyesLeft, ClosedEyesLeft,
                      ClosingEyesRight, ClosedEyesRight;

        /// <summary>
        /// The  that shows link playing his ocarina.
        /// </summary>
        public readonly Sprite PlayOcarinaStand;

        /// <summary>
        /// The ation displayed when Link dies.
        /// </summary>
        public readonly SpriteAnimation Dieing;

        /*
        /// <summary>
        /// The ation displayed when Link is falling down.
        /// </summary>
        public readonly SpriteAnimation Fall;
        */

        /// <summary>
        /// The melee attack SpriteAnimations.
        /// </summary>
        public readonly SpriteAnimation MeleeDown, MeleeUp, MeleeLeft, MeleeRight;

        /// <summary>
        /// The ranged attack SpriteAnimations.
        /// </summary>
        public readonly SpriteAnimation RangedDown, RangedUp, RangedLeft, RangedRight;

        /// <summary>
        /// The casting SpriteAnimations.
        /// </summary>
        public readonly SpriteAnimation CastUp, CastDown, CastLeft, CastRight;

        /// <summary>
        /// The SpriteAnimation of the Whirlwind special ation.
        /// </summary>
        public readonly SpriteAnimation AtkWhirlwind;

        /// <summary>
        /// The sword s of the whirlwind/bladestorm attacks.
        /// </summary>
        public readonly AnimatedSprite SwordWhirlwind, SwordBladestorm;

        /// <summary>
        /// The SpriteAnimation of the Bladestorm special ation.
        /// </summary>
        public readonly SpriteAnimation AtkBladestorm;

        /// <summary>
        /// Gets the color tint applied to Link's sprites.
        /// </summary>
        public LinkSpriteColorTint ColorTint
        {
            get
            {
                return this.colorTint;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkSprites"/> class.
        /// </summary>
        /// <param name="spriteLoader">
        /// The loader that is responsible for loading the sprites.
        /// </param>
        public LinkSprites( ISpriteLoader spriteLoader )
        {
            // Standing
            this.StandLeft = LoadSprite( "Stand_Left", spriteLoader );
            this.StandRight = LoadSprite( "Stand_Right", spriteLoader );
            this.StandUp = LoadSprite( "Stand_Up", spriteLoader );
            this.StandDown = LoadSprite( "Stand_Down", spriteLoader );

            // Movement
            this.MoveLeft = LoadAnimation( "Move_Left", spriteLoader );
            this.MoveRight = LoadAnimation( "Move_Right", spriteLoader );
            this.MoveUp = LoadAnimation( "Move_Up", spriteLoader );
            this.MoveDown = LoadAnimation( "Move_Down", spriteLoader );

            // Swimming (Stand)
            this.SwimStandLeft = LoadAnimation( "Swim_Left_Stand", spriteLoader );
            this.SwimStandRight = LoadAnimation( "Swim_Right_Stand", spriteLoader );
            this.SwimStandUp = LoadAnimation( "Swim_Up_Stand", spriteLoader );
            this.SwimStandDown = LoadAnimation( "Swim_Down_Stand", spriteLoader );

            // Swimming
            this.SwimLeft = LoadAnimation( "Swim_Left", spriteLoader );
            this.SwimRight = LoadAnimation( "Swim_Right", spriteLoader );
            this.SwimUp = LoadAnimation( "Swim_Up", spriteLoader );
            this.SwimDown = LoadAnimation( "Swim_Down", spriteLoader );

            // Eye Candy
            this.ClosedEyesLeft = LoadSprite( "ClosedEyes_Left", spriteLoader );
            this.ClosedEyesRight = LoadSprite( "ClosedEyes_Right", spriteLoader );
            this.ClosedEyesDown = LoadSprite( "ClosedEyes_Down", spriteLoader );

            this.ClosingEyesLeft = LoadSprite( "ClosingEyes_Left", spriteLoader );
            this.ClosingEyesRight = LoadSprite( "ClosingEyes_Right", spriteLoader );
            this.ClosingEyesDown = LoadSprite( "ClosingEyes_Down", spriteLoader );

            // Ocarina
            this.PlayOcarinaStand = LoadSprite( "PlayOcarina_0", spriteLoader );

            // Dieing
            this.Dieing = LoadAnimation( "Dieing", spriteLoader );

            // Melee Attack
            this.MeleeLeft = LoadAnimation( "Attack_Melee_Left", spriteLoader );
            this.MeleeRight = LoadAnimation( "Attack_Melee_Right", spriteLoader );
            this.MeleeUp = LoadAnimation( "Attack_Melee_Up", spriteLoader );
            this.MeleeDown = LoadAnimation( "Attack_Melee_Down", spriteLoader );

            // Ranged Attack
            this.RangedLeft = LoadAnimation( "Attack_Ranged_Left", spriteLoader );
            this.RangedRight = LoadAnimation( "Attack_Ranged_Right", spriteLoader );
            this.RangedUp = LoadAnimation( "Attack_Ranged_Up", spriteLoader );
            this.RangedDown = LoadAnimation( "Attack_Ranged_Down", spriteLoader );

            // Casting
            this.CastLeft = LoadAnimation( "Casting_Left", spriteLoader );
            this.CastRight = LoadAnimation( "Casting_Right", spriteLoader );
            this.CastUp = LoadAnimation( "Casting_Up", spriteLoader );
            this.CastDown = LoadAnimation( "Casting_Down", spriteLoader );

            // Special Attacks
            this.AtkWhirlwind = LoadAnimation( "Attack_Whirlwind", spriteLoader );
            this.AtkBladestorm = LoadAnimation( "Attack_Whirlwind_Turn", spriteLoader );

            this.SwordWhirlwind = spriteLoader.LoadAnimatedSprite( "Sword1_Whirlwind" );
            this.SwordBladestorm = spriteLoader.LoadAnimatedSprite( "Sword1_Whirlwind_Turn" );

            // Texture for color replacement tinting
            this.textureOriginalData = StandDown.Texture.GetColorData();
        }

        private SpriteAnimation LoadAnimation( string spriteName, ISpriteLoader spriteLoader )
        {
            return spriteLoader
                .LoadAnimatedSprite( GetSpriteAssetName( spriteName ) )
                .CreateInstance();
        }

        private Sprite LoadSprite( string spriteName, ISpriteLoader spriteLoader )
        {
            return spriteLoader.LoadSprite( GetSpriteAssetName( spriteName ) );
        }

        private string GetSpriteAssetName( string spriteName )
        {
            return "LinkGreen_" + spriteName;
        }

        /// <summary>
        /// Gets the standing sprite for the given direction.
        /// </summary>
        /// <param name="direction">
        /// The direction Link is facing.
        /// </param>
        /// <returns>
        /// The sprite for the given direction.
        /// </returns>
        public Sprite GetStand( Direction4 direction )
        {
            switch( direction )
            {
                case Direction4.Up:
                    return StandUp;

                case Direction4.Down:
                    return StandDown;

                case Direction4.Left:
                    return StandLeft;

                case Direction4.Right:
                    return StandRight;

                default:
                    return null;
            }
        }
        
        /// <summary>
        /// Gets the movement sprite animation for the given direction.
        /// </summary>
        /// <param name="direction">
        /// The direction Link is facing.
        /// </param>
        /// <returns>
        /// The sprite animation for the given direction.
        /// </returns>
        public SpriteAnimation GetMove( Direction4 direction )
        {
            switch( direction )
            {
                case Direction4.Up:
                    return MoveUp;

                case Direction4.Down:
                    return MoveDown;

                case Direction4.Left:
                    return MoveLeft;

                case Direction4.Right:
                    return MoveRight;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Changes the color tinting of Link's sprites.
        /// </summary>
        /// <param name="colorTint">
        /// The new color tint to apply.
        /// </param>
        public void SetColorTint( LinkSpriteColorTint colorTint )
        {
            if( colorTint.ClothMain == this.colorTint.ClothMain && colorTint.ClothHighlight == this.colorTint.ClothHighlight &&
                colorTint.HairMain == this.colorTint.HairMain && colorTint.HairHighlight == this.colorTint.HairHighlight )
            {
                // Nothing has changed, skip
                return;
            }

            Xna.Color[] data = (Xna.Color[])textureOriginalData.Clone();
            var clothColorSecondary = new Xna.Color( colorTint.ClothMain.R - 12, colorTint.ClothMain.G - 11, colorTint.ClothMain.B - 5 );

            for( int i = 0; i < data.Length; ++i )
            {
                Xna.Color color = data[i];

                // Main color
                if( color == ColorDefaults.ClothMain )
                    data[i] = colorTint.ClothMain;

                // Secondary main color
                else if( color == ColorDefaults.ClothSecondary )
                    data[i] = clothColorSecondary;

                // Hightlight color
                else if( color == ColorDefaults.ClothHighlight )
                    data[i] = colorTint.ClothHighlight;

                // Hair Main color
                else if( color == ColorDefaults.HairMain )
                    data[i] = colorTint.HairMain;

                // Hair Hightlight color
                else if( color == ColorDefaults.HairHighlight )
                    data[i] = colorTint.HairHighlight;
            }

            // All relevant sprites are in the same texture
            StandDown.Texture.SetData( data );
            this.colorTint = colorTint;
        }

        /// <summary>
        /// Reapplies the current color tinting to the link's texture.
        /// </summary>
        public void ReapplyTint()
        {
            // Force reapply by resetting to defaults.
            LinkSpriteColorTint colorTint = this.colorTint;
            this.colorTint = LinkSpriteColorTint.Default;
            SetColorTint( colorTint );
        }

        /// <summary>
        /// Resets the color tint of the sprites to their default values.
        /// </summary>
        public void ResetTint()
        {
            SetColorTint( LinkSpriteColorTint.Default );
        }

        /// <summary>
        /// Sets the animation speed of all movement animations.
        /// </summary>
        /// <param name="animationSpeed">
        /// The new animation speed.
        /// </param>
        public void SetMoveAnimSpeed( float animationSpeed )
        {
            this.MoveLeft.AnimationSpeed = animationSpeed;
            this.MoveRight.AnimationSpeed = animationSpeed;
            this.MoveUp.AnimationSpeed = animationSpeed;
            this.MoveDown.AnimationSpeed = animationSpeed;
        }

        /// <summary>
        /// The currently active color replacement of the clothes and hair.
        /// </summary>
        private LinkSpriteColorTint colorTint;

        /// <summary>
        /// The original color data of link's sprites. Used as a template when color tinting
        /// links clothes and hair.
        /// </summary>
        private readonly Xna.Color[] textureOriginalData;
    }
}
