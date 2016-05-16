// <copyright file="OneDirMoveStandAttackAnimDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.OneDirMoveStandAttackAnimDrawDataAndStrategy class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Drawing
{
    using System;
    using Atom.Xna;

    /// <summary>
    /// Defines an <see cref="IDrawDataAndStrategy"/> that draws
    /// the same Animated Sprite for all actions and directions.
    /// </summary>
    /// <remarks>
    /// SpriteGroup format, where X is the SpriteGroup:
    /// 'X'
    /// </remarks>
    public sealed class OneDirMoveStandAttackAnimDrawDataAndStrategy : TintedDrawDataAndStrategy
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="SpriteAnimation"/> displayed by this <see cref="OneDirMoveStandAttackAnimDrawDataAndStrategy"/>.
        /// </summary>
        public SpriteAnimation Animation
        {
            get;
            set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDirMoveStandAttackAnimDrawDataAndStrategy"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity to visualize with the new <see cref="OneDirMoveStandAttackAnimDrawDataAndStrategy"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        public OneDirMoveStandAttackAnimDrawDataAndStrategy( ZeldaEntity entity )
        {
            if( entity == null )
                throw new ArgumentNullException( "entity" );

            this.entity = entity; 
            this.attackingEntity = entity as IAttackingEntity;

            if( this.attackingEntity == null )
            {
                throw new ArgumentException(
                    string.Format(
                       System.Globalization.CultureInfo.CurrentCulture,
                       Atom.ErrorStrings.ObjectXDoesntImplementY,   
                       "entity",
                       "Zelda.Entities.IAttackingEntity"
                   ),
                   "entity"
               );
            }

            this.moveable = entity.Components.Get<Components.Moveable>();
            Atom.ThrowHelper.IfComponentNull( this.moveable );
        }

        /// <summary>
        /// Initializes a new instance of the OneDirMoveStandAttackAnimDrawDataAndStrategy class.
        /// </summary>
        internal OneDirMoveStandAttackAnimDrawDataAndStrategy()
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( this.ShouldAnimateSprite() )
            {
                if( this.Animation != null )
                    this.Animation.Animate( updateContext.FrameTime );
            }

            base.Update( updateContext );
        }

        /// <summary>
        /// Gets a value indicating whether the sprite of 
        /// this TintedDrawDataAndStrategy should be animated.
        /// </summary>
        /// <returns>
        /// true if it should be animated;
        /// otherwise false.
        /// </returns>
        private bool ShouldAnimateSprite()
        {
            if( this.attackingEntity.IsAttacking )
            {
                return true;
            }

            return !this.moveable.IsStanding;
        }

        /// <summary>
        /// Draws this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public override void Draw( ZeldaDrawContext drawContext )
        {
            if( this.Animation != null )
            {
                var drawPosition = entity.Transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                this.Animation.Draw( drawPosition, this.FinalColor, drawContext.Batch );
            }
        }

        /// <summary>
        /// Loads the assets needed by this <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public override void Load( IZeldaServiceProvider serviceProvider )
        {
            if( string.IsNullOrEmpty( this.SpriteGroup ) )
            {
                this.Animation = null;
            }
            else
            {
                this.Animation = serviceProvider.SpriteLoader.LoadAnimatedSprite( this.SpriteGroup ).CreateInstance();
            }
        }

        /// <summary>
        /// Clones this <see cref="IDrawDataAndStrategy"/> for use by the specified object.
        /// </summary>
        /// <param name="newOwner">
        /// The new owner of the cloned <see cref="IDrawDataAndStrategy"/>.
        /// </param>
        /// <returns>
        /// The cloned <see cref="IDrawDataAndStrategy"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="newOwner"/> is null.
        /// </exception>
        public override IDrawDataAndStrategy Clone( ZeldaEntity newOwner )
        {
            var clone = new OneDirMoveStandAttackAnimDrawDataAndStrategy( newOwner ) {
                Animation = this.Animation != null ? this.Animation.Clone() : null
            };

            this.SetupClone( clone );
            return clone;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The entity that is visualized by the <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        private readonly ZeldaEntity entity;

        /// <summary>
        /// Provides a mechanism to receive a value that indicates whether
        /// the ZeldaEntity is currently attacking.
        /// </summary>
        private readonly IAttackingEntity attackingEntity;

        /// <summary>
        /// Identifies the moveable component of the <see cref="ZeldaEntity"/>.
        /// </summary>
        private readonly Zelda.Entities.Components.Moveable moveable;

        #endregion
    }
}
