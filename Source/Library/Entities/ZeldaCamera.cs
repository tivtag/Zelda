// <copyright file="ZeldaCamera.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.ZeldaCamera class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using System;
    using System.Diagnostics;
    using Atom;
    using Atom.Math;
    using Atom.Scene.Tiles;
    
    /// <summary>
    /// A camera controls what the player sees in the <see cref="ZeldaScene"/>. 
    /// </summary>
    /// <remarks>
    /// The camera in TLoZ - BC supports beside translating also zooming and rotating
    /// for special StatusEffects.
    /// </remarks>
    public sealed class ZeldaCamera
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the <see cref="Transform"/> of this ZeldaCamera has changed.
        /// </summary>
        public event SimpleEventHandler<ZeldaCamera> TransformChanged;

        /// <summary>
        /// Fired when the <see cref="Scroll"/> value of this ZeldaCamera has changed.
        /// </summary>
        public event RelaxedEventHandler<ChangedValue<Vector2>> ScrollChanged;

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeldaCamera"/> class.
        /// </summary>
        /// <param name="scene">
        /// The Scene that owns the new <see cref="ZeldaCamera"/>.
        /// </param>
        internal ZeldaCamera( ZeldaScene scene )
        {
            Debug.Assert( scene != null );
            Debug.Assert( scene.Map != null );

            this.map = scene.Map;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the entity this Camera follows.
        /// </summary>
        public ZeldaEntity EntityToFollow
        {
            //get
            //{
            //    return this.entityToFollow; 
            //}

            set 
            {
                this.entityToFollow = value;
                this.Update();
            }
        }

        /// <summary>
        /// Gets or sets the value which represents the translation of the scene
        /// archived by the Camera.
        /// </summary>
        public Vector2 Scroll
        {
            get
            {

                return this.scroll;
            }

            set
            { 
                // Limit the given scroll value
                // to be in allowed range.
                int mapWidth  = this.map.Width * 16;
                int mapHeight = this.map.Height * 16;

                if( value.X < 0.0f )
                    value.X = 0.0f;
                else if( value.X + viewSize.X > mapWidth )
                    value.X = mapWidth - viewSize.X;

                if( value.Y < 0.0f )
                    value.Y = 0.0f;
                else if( value.Y + viewSize.Y > mapHeight )
                    value.Y = mapHeight - viewSize.Y;

                if( scroll == value )
                    return;

                var oldValue = scroll;
                this.scroll  = value;

                UpdateCachedTransform();

                if( ScrollChanged != null )
                    ScrollChanged( this, new ChangedValue<Vector2>( oldValue, value ) );
            }
        }

        /// <summary>
        /// Gets or sets the size of the viewable area.
        /// </summary>
        public Point2 ViewSize
        {
            get
            {
                return this.viewSize;
            }

            set
            {
                this.viewSize = value;
                this.halfViewSize = this.viewSize / 2;
            }
        }

        /// <summary>
        /// Gets the area that is currently visible to the player.
        /// </summary>
        public Rectangle ViewArea
        {
            get
            {
                return new Rectangle( (int)scroll.X, (int)scroll.Y, viewSize.X, viewSize.Y );
            }
        }

        /// <summary>
        /// Gets the transformation matrix the camera applies to the Scene.
        /// </summary>
        /// <remarks>
        /// This is a <see cref=" Microsoft.Xna.Framework.Matrix"/> rather than an <see cref="Atom.Math.Matrix4"/>
        /// because the Matrix is immiedietly used by Xna. 
        /// Using an Atom matrix would add a small overhead in converting the matrix.
        /// </remarks>
        public Microsoft.Xna.Framework.Matrix Transform
        {
            get
            {
                return this.transform;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="ZeldaCamera"/>.
        /// </summary>
        public void Update()
        {
            if( this.entityToFollow != null )
            {
                var entityPosition = this.entityToFollow.Transform.Position;

                // Center the scroll value:
                this.Scroll = new Vector2(
                    entityPosition.X - this.halfViewSize.X,
                    entityPosition.Y - this.halfViewSize.Y
                );
            }
        }

        /// <summary>
        /// Receives a value that indicates whether the given <see cref="ZeldaEntity"/>
        /// intersects with the view area of this <see cref="ZeldaCamera"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity to test against. Can be null.
        /// </param>
        /// <returns>
        /// true if the given entity is within vision of the camera;
        /// otherwise false.
        /// </returns>
        public bool IsInVision( ZeldaEntity entity )
        {
            if( entity == null )
                return false;

            return this.ViewArea.Intersects( entity.Collision.Rectangle );
        }

        /// <summary>
        /// Updates the cached transform matrix
        /// the Camera applies to the Scene.
        /// </summary>
        private void UpdateCachedTransform()
        {
            this.transform = Microsoft.Xna.Framework.Matrix.CreateTranslation( -(int)Math.Floor( this.scroll.X ), -(int)Math.Floor( this.scroll.Y ), 0.0f );

            if( this.TransformChanged != null )
            {
                this.TransformChanged( this );
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Represents the translation of the scene.
        /// </summary>
        private Vector2 scroll;

        /// <summary>
        /// Stores the size of the area the player is able to see.
        /// </summary>
        private Point2 viewSize, halfViewSize;

        /// <summary>
        /// The cached transform matrix.
        /// </summary>
        private Microsoft.Xna.Framework.Matrix transform = Microsoft.Xna.Framework.Matrix.Identity;

        /// <summary>
        /// The entity this <see cref="ZeldaCamera"/> automatically follows.
        /// </summary>
        private ZeldaEntity entityToFollow;

        /// <summary>
        /// Reference of the TileMap object of the Scene
        /// that owns this <see cref="ZeldaCamera"/>.
        /// </summary>
        private readonly TileMap map;

        #endregion
    }
}
