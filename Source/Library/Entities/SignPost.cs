// <copyright file="SignPost.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.SignPost class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    /// <summary>
    /// Represents a simple sign post that shows some <see cref="LocalizableText"/> when the player reads it.
    /// </summary>
    public class SignPost : ZeldaEntity, IUseable
    {
        /// <summary>
        /// Gets the object that stores the text strings shown when the Player reads this SignPost.
        /// </summary>
        public LocalizableText Text
        {
            get 
            { 
                return this.text;
            }
        }
        
        /// <summary>
        /// Gets the <see cref="Zelda.Entities.Drawing.OneDirDrawDataAndStrategy"/> of this SignPost.
        /// </summary>
        public new Zelda.Entities.Drawing.OneDirDrawDataAndStrategy DrawDataAndStrategy
        {
            get 
            { 
                return (Zelda.Entities.Drawing.OneDirDrawDataAndStrategy)base.DrawDataAndStrategy;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignPost"/> class.
        /// </summary>
        public SignPost()
        {
            this.Collision.Size = new Atom.Math.Vector2( 16.0f, 16.0f );
            base.DrawDataAndStrategy = new Zelda.Entities.Drawing.OneDirDrawDataAndStrategy( this );
        }

        /// <summary>
        /// Uses this SignPost, trying to unlock it.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that tried to unlock this UnlockableDoorTileBlock.
        /// </param>
        /// <returns>
        /// Whether the UnlockableDoorTileBlock has been unlocked;
        /// and as such removed from the Scene.
        /// </returns>
        public bool Use( PlayerEntity user )
        {
            if( !this.Collision.IntersectsUnstrict( user.Collision ) )
                return false;
            if( !this.Transform.IsFacing( user.Transform ) )
                return false;

            this.OnRead( user );
            return true;
        }

        /// <summary>
        /// Called when this SignPost has been read.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that has been read.
        /// </param>
        protected virtual void OnRead( PlayerEntity user )
        {
            this.ShowText();
        }

        /// <summary>
        /// Shows the <see cref="Text"/> of this SignPost.
        /// </summary>
        protected void ShowText()
        {
            var userInterface = this.Scene.UserInterface;
            var dialog        = userInterface.Dialog;

            if( !dialog.IsVisible )
            {
                dialog.Show( this.text.LocalizedText );
            }
        }

        /// <summary>
        /// Creates a clone of this SignPost.
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            var clone = new SignPost();

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given SignPost to be a clone of this SignPost.
        /// </summary>
        /// <param name="clone">
        /// The SignPost to setup as a clone of this SignPost.
        /// </param>
        protected void SetupClone( SignPost clone )
        {
            base.SetupClone( clone );

            clone.text.Id = this.text.Id;
        }

        /// <summary>
        /// Stores the text strings shown when the Player reads this SignPost.
        /// </summary>
        private readonly LocalizableText text = new LocalizableText();
    }
}
