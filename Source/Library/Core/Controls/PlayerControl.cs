// <copyright file="PlayerControl.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Controls.PlayerControl class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Core.Controls
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Entities;
    using Zelda.Entities.Drawing;

    /// <summary>
    /// Transforms actual keyboard input into player action.
    /// </summary>
    internal sealed class PlayerControl
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerControl"/> class.
        /// </summary>
        /// <param name="player">
        /// The <see cref="PlayerEntity"/> that gets controlled by the new PlayerControl.
        /// </param>
        /// <param name="keySettings">
        /// The <see cref="KeySettings"/> instance that defines what keys map onto what actions.
        /// </param>
        public PlayerControl( PlayerEntity player, KeySettings keySettings )
        {
            Contract.Requires<ArgumentNullException>( player != null );
            Contract.Requires<ArgumentNullException>( keySettings != null );

            this.player      = player;
            this.keySettings = keySettings;
            
            this.moveable       = player.Moveable;
            this.drawData = player.DrawDataAndStrategy;

            this.player.Equipment.WeaponHandChanged += OnPlayerWeaponChanged;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="PlayerControl"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current <see cref="ZeldaUpdateContext"/>.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.CanMove() )
                return;

            KeyboardState keyState = Keyboard.GetState();
            bool moveLeft  = keyState.IsKeyDown( keySettings.MoveLeft1  ) || keyState.IsKeyDown( keySettings.MoveLeft2 );
            bool moveRight = keyState.IsKeyDown( keySettings.MoveRight1 ) || keyState.IsKeyDown( keySettings.MoveRight2 );
            bool moveUp    = keyState.IsKeyDown( keySettings.MoveUp1    ) || keyState.IsKeyDown( keySettings.MoveUp2 );
            bool moveDown  = keyState.IsKeyDown( keySettings.MoveDown1  ) || keyState.IsKeyDown( keySettings.MoveDown2 );

            if( moveLeft && moveRight )
            {
                moveLeft  = false;
                moveRight = false;
            }

            if( moveUp && moveDown )
            {
                moveUp   = false;
                moveDown = false;
            }

            this.Move( moveLeft, moveRight, moveUp, moveDown, updateContext );

            this.wasMoveLeft  = moveLeft;
            this.wasMoveRight = moveRight;
            this.wasMoveUp    = moveUp;
            this.wasMoveDown  = moveDown;
        }

        /// <summary>
        /// Moves the player entity into the specified directions.
        /// </summary>
        /// <param name="moveLeft">
        /// States whether the player should be moved left/west.
        /// </param>
        /// <param name="moveRight">
        /// States whether the player should be moved right/east.
        /// </param>
        /// <param name="moveUp">
        /// States whether the player should be moved up/north.
        /// </param>
        /// <param name="moveDown">
        /// States whether the player should be moved down/south.
        /// </param>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void Move( bool moveLeft, bool moveRight, bool moveUp, bool moveDown, ZeldaUpdateContext updateContext )
        {
            if( moveLeft )
            {
                this.MoveLeft( moveUp, moveDown, updateContext );
            }
            else if( moveRight )
            {
                this.MoveRight( moveUp, moveDown, updateContext );
            }
            else if( moveUp )
            {
                moveable.MoveUp( updateContext.FrameTime );

                this.ResetDiagonalMovementStates();
                this.ResetSpecialAnimation();
            }
            else if( moveDown )
            {
                moveable.MoveDown( updateContext.FrameTime );

                this.ResetDiagonalMovementStates();
                this.ResetSpecialAnimation();
            }
            else
            {
                this.ResetDiagonalMovementStates();
            }
        }
        
        /// <summary>
        /// Moves the player entity left/west and additionally, if specified, up or down.
        /// </summary>
        /// <param name="moveUp">
        /// States whether the player should be moved up/north.
        /// </param>
        /// <param name="moveDown">
        /// States whether the player should be moved down/south.
        /// </param>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void MoveLeft( bool moveUp, bool moveDown, ZeldaUpdateContext updateContext )
        {
            float frameTime = updateContext.FrameTime;

            if( moveUp )
            {
                if( wasMoveLeftUp || (wasMoveLeft && !wasMoveUp) )
                {
                    moveable.MoveLeftUp( frameTime );
                    wasMoveLeftUp = true;
                }
                else if( wasMoveUpLeft || (!wasMoveLeft && wasMoveUp) )
                {
                    moveable.MoveUpLeft( frameTime );
                    wasMoveUpLeft = true;
                }
                else
                {
                    moveable.MoveUpLeft( frameTime );
                    wasMoveUpLeft = true;
                }
            }
            else if( moveDown )
            {
                if( wasMoveLeftDown || (wasMoveLeft && !wasMoveDown) )
                {
                    moveable.MoveLeftDown( frameTime );
                    wasMoveLeftDown = true;
                }
                else if( wasMoveDownLeft || (!wasMoveLeft && wasMoveDown) )
                {
                    moveable.MoveDownLeft( frameTime );
                    wasMoveDownLeft = true;
                }
                else
                {
                    moveable.MoveDownLeft( frameTime );
                    wasMoveDownLeft = true;
                }
            }
            else
            {
                moveable.MoveLeft( frameTime );
                this.ResetDiagonalMovementStates();
            }

            this.ResetSpecialAnimation();
        }

        /// <summary>
        /// Moves the player entity right/east and additionally, if specified, up or down.
        /// </summary>
        /// <param name="moveUp">
        /// States whether the player should be moved up/north.
        /// </param>
        /// <param name="moveDown">
        /// States whether the player should be moved down/south.
        /// </param>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void MoveRight( bool moveUp, bool moveDown, ZeldaUpdateContext updateContext )
        {
            float frameTime = updateContext.FrameTime;

            if( moveUp )
            {
                if( wasMoveRightUp || (wasMoveRight && !wasMoveUp) )
                {
                    moveable.MoveRightUp( frameTime );
                    wasMoveRightUp = true;
                }
                else if( wasMoveUpRight || (!wasMoveRight && wasMoveUp) )
                {
                    moveable.MoveUpRight( frameTime );
                    wasMoveUpRight = true;
                }
                else
                {
                    moveable.MoveUpRight( frameTime );
                    wasMoveUpRight = true;
                }
            }
            else if( moveDown )
            {
                if( wasMoveRightDown || (wasMoveRight && !wasMoveDown) )
                {
                    moveable.MoveRightDown( frameTime );
                    wasMoveRightDown = true;
                }
                else if( wasMoveDownRight || (!wasMoveRight && wasMoveDown) )
                {
                    moveable.MoveDownRight( frameTime );
                    wasMoveDownRight = true;
                }
                else
                {
                    moveable.MoveDownRight( frameTime );
                    wasMoveDownRight = true;
                }
            }
            else
            {
                moveable.MoveRight( frameTime );
                this.ResetDiagonalMovementStates();
            }

            this.ResetSpecialAnimation();
        }

        /// <summary>
        /// Gets a value indicating whether the player is allowed to move.
        /// </summary>
        /// <returns>
        /// Returns true if the player is allowed to move;
        /// otherwise false.
        /// </returns>
        private bool CanMove()
        {
            if( !this.moveable.CanMove || this.player.IsCasting )
                return false;

            // This code disallows moving if the player attacks with his melee weapon.
            if( this.drawData.SpecialAnimation == PlayerSpecialAnimation.AttackMelee )
            {
                return this.drawData.IsMeleeAttackAnimationDone( this.timeFactorMovementAfterMeleeAttack );
            }

            // Experimental, feels aquard. 
            // else if( this.drawData.SpecialAnimation == PlayerSpecialAnimation.AttackRanged )
            // {
            //     return this.drawData.IsRangedAttackAnimationDone( 0.3f );
            // }
            return true;
        }

        /// <summary>
        /// Disables the special animation of the Player.
        /// </summary>
        private void ResetSpecialAnimation()
        {
            var specialAnimation = this.drawData.SpecialAnimation;
            
            if( specialAnimation != PlayerSpecialAnimation.None &&
                specialAnimation != PlayerSpecialAnimation.AttackBladestorm )
            {
                this.drawData.DisableCurrentSpecialAnimation();
            }
        }

        /// <summary>
        /// Resets all boolean flags that capture the movement
        /// of the player to <see langword="false"/>.
        /// </summary>
        private void ResetDiagonalMovementStates()
        {
            this.wasMoveLeftUp = false;
            this.wasMoveUpLeft = false;
            this.wasMoveRightUp = false;
            this.wasMoveUpRight = false;
            this.wasMoveLeftDown = false;
            this.wasMoveDownLeft = false;
            this.wasMoveRightDown = false;
            this.wasMoveDownRight = false;
        }

        /// <summary>
        /// Gets called when the WeaponHand of the player changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contain the event data.</param>
        private void OnPlayerWeaponChanged( object sender, EventArgs e )
        {
            this.RefreshMovementTimeFactor();
        }

        /// <summary>
        /// Refreshes the time factors that specify when after attacking the player is allowed to move again.
        /// </summary>
        private void RefreshMovementTimeFactor()
        {
            // Wearing a dagger allows the player to move again
            // faster after attacking.
            if( player.Equipment.IsWearingDagger )
            {
                this.timeFactorMovementAfterMeleeAttack = 0.75f;
            }
            else
            {
                this.timeFactorMovementAfterMeleeAttack = 0.8f;
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Descripes what keys map onto what actions.
        /// </summary>
        private readonly KeySettings keySettings;

        /// <summary>
        /// Identifies the PlayerEntity.
        /// </summary>
        private readonly PlayerEntity player;

        /// <summary>
        /// Identifies the Moveable component of the Player.
        /// </summary>
        private readonly Zelda.Entities.Components.Moveable moveable;

        /// <summary>
        /// Controls and contains the draw data and strategy of the player.
        /// </summary>
        private readonly PlayerDrawDataAndStrategy drawData;

        /// <summary>
        /// States the % of time required before moving after an attack is allowed again.
        /// </summary>
        private float timeFactorMovementAfterMeleeAttack = 0.8f;

        /// <summary>
        /// The state of the keyboard one frame ago.
        /// </summary>
        private bool wasMoveLeft, wasMoveRight, wasMoveUp, wasMoveDown;

        /// <summary>
        /// These state variables are used to reproduce the original Zelda diagonal-movement.
        /// </summary>
        private bool wasMoveLeftUp, wasMoveUpLeft, wasMoveRightUp, wasMoveUpRight,
                     wasMoveLeftDown, wasMoveDownLeft, wasMoveRightDown, wasMoveDownRight;

        #endregion
    }
}
