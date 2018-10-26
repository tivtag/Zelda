// <copyright file="Moveable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Components.Moveable class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Components
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Atom.Components;
    using Atom.Math;
    using Atom.Scene.Tiles;

    /// <summary>
    /// Defines the <see cref="Component"/> that allows an Entity
    /// to move around the Scene. This class can't be inherited.
    /// </summary>
    public sealed class Moveable : ZeldaComponent
    {
        /// <summary>
        /// Raised when the Entity has moved against a tile that has stopped his movement.
        /// </summary>
        public event SimpleEventHandler<Moveable> MapCollisionOccurred;

        /// <summary>
        /// Raised when the movement speed of the moveable entity has changed.
        /// </summary>
        public event SimpleEventHandler<Moveable> SpeedChanged;
        
        /// <summary>
        /// Gets or sets the <see cref="IZeldaTileHandler"/> the <see cref="Moveable"/> component uses 
        /// to interact with the Tile Map of the Scene.
        /// </summary>
        /// <exception cref="ArgumentNullException">Set: If the given value is null.</exception>
        public IZeldaTileHandler TileHandler
        {
            get
            {
                return this.tileHandler;
            }

            set
            {
                Contract.Requires<ArgumentNullException>( value != null );

                this.tileHandler = value;
            }
        }

        /// <summary>
        /// Gets or sets the direction the Entity is facing.
        /// </summary>
        private Atom.Math.Direction4 Direction
        {
            get 
            {
                return this.transform.Direction; 
            }

            set
            {
                this.transform.Direction = value;
            }
        }

        public float SlideOffset
        {
            get { return this.slideOffset; }
            set { this.slideOffset = value; }
        }        

        #region > Swimming <

        /// <summary>
        /// Gets or sets a value indicating whether the Entity can swim in deep water.
        /// </summary>
        /// <value>The default value is false.</value>
        public bool CanSwim
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the moveable Entity can currently swim,
        /// taking its current state into account.
        /// </summary>
        public bool CanCurrentlySwim
        {
            get
            {
                return this.CanSwim && (CanCurrentlySwimStateFunction != null ? CanCurrentlySwimStateFunction() : true);
            }
        }

        /// <summary>
        /// Gets or sets the function that can be used to state
        /// whether the moveable Entity can currently swim. 
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public Func<bool> CanCurrentlySwimStateFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the Entity is currently swimming in deep water.
        /// </summary>
        public bool IsSwimming
        {
            get
            {
                var actionLayer = this.Owner.ActionLayer;
                if( actionLayer == null )
                    return false;

                int tile = actionLayer.GetTileAtSafe(
                    (int)(collision.Center.X / 16.0f),
                    (int)((collision.Center.Y - 2.0f) / 16.0f)
                );

                return ((ActionTileId)tile) == ActionTileId.WaterSwimable;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Entity currently is in shallow (moveable) water.
        /// </summary>
        public bool IsInShallowWater
        {
            get
            {
                var actionLayer = this.Owner.ActionLayer;
                if( actionLayer == null )
                    return false;

                int tile = actionLayer.GetTileAtSafe(
                    (int)(collision.Center.X / 16.0f),
                    (int)((collision.Center.Y - 2.0f) / 16.0f)
                );

                return ((ActionTileId)tile) == ActionTileId.WaterWalkable;
            }
        }

        #endregion

        #region > Pushing <
                
        /// <summary>
        /// Gets or sets a value indicating whether the Entity can be pushed.
        /// </summary>
        /// <value>The default value is true.</value>
        public bool CanBePushed
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the value all pushing forces 
        /// applied to this moveable Entity are multiplied by.
        /// </summary>
        /// <value>The default value is 1.</value>
        public float PushingForceModifier       
        {
            get { return this.pushingForceModifier; }
            set { this.pushingForceModifier = value; }
        }

        #endregion
        
        /// <summary>
        /// Gets or sets a value indicating whether the Entity is solid
        /// and as such collides with the Tile Map of the Scene.
        /// </summary>
        /// <value>The default value is true.</value>
        public bool CollidesWithMap 
        { 
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Entity can currently move.
        /// </summary>
        /// <value>The default value is true.</value>
        public bool CanMove
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Entity can slide
        /// around the edges of a tile.
        /// </summary>
        /// <value>The default value is true.</value>
        public bool CanSlide
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Entity can change between TileMapFloors.
        /// </summary>
        /// <value>The default value is true.</value>
        public bool CanChangeFloor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current movement speed of the Entity
        /// </summary>
        public float Speed
        {
            get
            {
                return this.speed;
            }

            set 
            {
                if( this.speed != value )
                {
                    this.speed = value;
                    this.SpeedChanged.Raise( this );
                }
            }
        }

        /// <summary>
        /// Gets or sets the base(non modified) movement speed of the Entity.
        /// </summary>   
        public float BaseSpeed
        {
            get { return this.baseSpeed; }
            set { this.baseSpeed = value; }
        }

        /// <summary>
        /// Gets the movement speed while the Entity is swimming in deep water.
        /// </summary>
        public float SpeedSwimming
        {
            get { return this.speed * 0.75f; }
        }
        
        /// <summary>
        /// Gets the movement speed of the moveable Entity,
        /// taking into account the current actions/state of the entity and the world.
        /// </summary>
        /// <returns>
        /// The current speed.
        /// </returns>
        private float GetStateModifiedSpeed()
        { 
            if( this.IsSwimming )
                return this.SpeedSwimming;
            return this.speed;
        }
        
        /// <summary>
        /// Gets a value indicating whether the Entity is currently standing.
        /// </summary>
        public bool IsStanding
        {
            get { return this.isStanding; }
        }

        /// <summary>
        /// Gets a value indicating whether the Entity was standing in the last frame.
        /// </summary>
        public bool WasStanding
        {
            get { return this.wasStanding; }
        }

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Moveable"/> class.
        /// </summary>
        public Moveable()
        {
            this.CanMove = true;
            this.CanSlide = true;
            this.CanBePushed = true;
            this.CanChangeFloor = true;
            this.CollidesWithMap = true;

            this.TileHandler = DefaultTileHandler.Instance;
        }

        /// <summary>
        /// Called when an IComponent has been removed or added to the <see cref="IEntity"/> that owns this IComponent.
        /// </summary>
        public override void InitializeBindings()
        {
            this.transform = this.Owner.Components.Get<ZeldaTransform>();
            this.collision = this.Owner.Components.Get<ZeldaCollision>();
        }

        #endregion

        #region [ Methods ]

        #region > Update <

        /// <summary>
        /// Updates this <see cref="Moveable"/> component.
        /// </summary>
        /// <param name="updateContext">
        /// The current Atom.IUpdateContext.
        /// </param>
        public override void Update( Atom.IUpdateContext updateContext )
        {
            this.UpdatePushing( updateContext );

            if( this.CollidesWithMap )
            {
                // Check collision with map.
                if( this.velocity.X != 0.0f || this.velocity.Y != 0.0f )
                {
                    this.UpdatePositionMap( updateContext );
                }
            }
            else
            {
                if( this.velocity.X != 0.0f || this.velocity.Y != 0.0f )
                {
                    this.transform.Position += this.velocity;
                }
            }

            // Updating of what tiles have last been
            // visisted is currently not implemented.
            // UpdateTilesVisited();
            this.velocity.X = 0.0f;
            this.velocity.Y = 0.0f;
            this.wasStanding = this.isStanding;
        }

        /// <summary>
        /// Gets called before <see cref="Update"/> is called.
        /// </summary>
        /// <param name="updateContext">
        /// The current Atom.IUpdateContext.
        /// </param>
        public override void PreUpdate( Atom.IUpdateContext updateContext )
        {
            this.isStanding = true;
        }

        /// <summary>
        /// Integrates the current velocity of the Entity,
        /// checking for collision with tiles.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        private void UpdatePositionMap( Atom.IUpdateContext updateContext )
        {
            var entity = (ZeldaEntity)this.Owner;
            var scene  = entity.Scene;
            if( scene == null )
                return;

            // 1.) First cache some often used values:
            #region Cache

            float frameTime = updateContext.FrameTime;

            Vector2 collisionOffset = collision.Offset;
            int width  = (int)collision.Width;
            int height = (int)collision.Height;

            float oldX = transform.X;
            float oldY = transform.Y;
            Vector2 collisionPosition = new Vector2( oldX + collisionOffset.X, oldY + collisionOffset.Y );

            #endregion

            // 2.) Get the action layer we operate on.
            var floor = scene.Map.GetFloor( entity.FloorNumber );
            if( floor == null )
                return;

            var actionLayer = floor.ActionLayer;

            // 3.) Now we are going to test the object against the map
            //     and move it accordingly.
            int tileCoordX, tileCoordY;
            
            // 4.) Horizontal Movement
            #region if( velocity.X > 0.0f )

            if( velocity.X > 0.0f )
            {
                if( actionLayer.TestCollisionVertical<Moveable>(
                    (int)collisionPosition.X + width,
                    (int)collisionPosition.Y,
                    height - 1,
                    velocity.X,
                    out tileCoordX, 
                    out tileCoordY, 
                    tileHandler, 
                    this ) )
                {
                    collisionPosition.X = (tileCoordX * 16) - width;
                    transform.X         = collisionPosition.X - collisionOffset.X;
                    velocity.X          = 0.0f;


                    #region Sliding

                    if( this.CanSlide && velocity.Y == 0.0f )
                    {
                        bool freeUp = tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX, tileCoordY - 1 ), this )
                                   && tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX - 1, tileCoordY - 1 ), this );

                        bool freeDown = tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX, tileCoordY + 1 ), this )
                                     && tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX - 1, tileCoordY + 1 ), this );
                        
                        if( freeUp || freeDown )
                        {
                            int tilePixelY = tileCoordY * 16;

                            if( freeUp && collisionPosition.Y - slideOffset < tilePixelY )
                            {
                                velocity.Y = -tileSlideSpeed * frameTime;
                                UpdatePositionMapSlide( actionLayer );
                            }
                            else if( freeDown && collisionPosition.Y > tilePixelY + slideOffset )
                            {
                                velocity.Y = tileSlideSpeed * frameTime;
                                UpdatePositionMapSlide( actionLayer );
                            }
                        }
                    }

                    #endregion

                    OnMapCollisionOccurred();
                }
                else
                {
                    collisionPosition.X += velocity.X;
                    transform.X         += velocity.X;
                    velocity.X           = 0.0f;
                }
            }

            #endregion
            #region else if( velocity.X < 0.0f )
            else if( velocity.X < 0.0f )
            {
                if( actionLayer.TestCollisionVertical<Moveable>(
                    (int)(collisionPosition.X + velocity.X),
                    (int)collisionPosition.Y, 
                    height - 1,
                    velocity.X,
                    out tileCoordX, 
                    out tileCoordY,
                    tileHandler, 
                    this ) )
                {
                    collisionPosition.X = (tileCoordX + 1) * 16;
                    transform.X = collisionPosition.X - collisionOffset.X;
                    velocity.X  = 0.0f;

                    #region Sliding

                    if( this.CanSlide && velocity.Y == 0.0f )
                    {
                        bool freeUp = tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX, tileCoordY - 1 ), this )
                               && tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX + 1, tileCoordY - 1 ), this );

                        bool freeDown = tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX, tileCoordY + 1 ), this )
                                 && tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX + 1, tileCoordY + 1 ), this );

                        if( freeUp || freeDown )
                        {
                            int tilePixelY = tileCoordY * 16;
                            if( freeUp && collisionPosition.Y - slideOffset < tilePixelY )
                            {
                                float s = tileSlideSpeed * frameTime;
                                velocity.Y = -s;

                                UpdatePositionMapSlide( actionLayer );
                            }
                            else if( freeDown && collisionPosition.Y > tilePixelY + slideOffset )
                            {
                                float s = tileSlideSpeed * frameTime;
                                velocity.Y = s;

                                UpdatePositionMapSlide( actionLayer );
                            }
                        }
                    }

                    #endregion

                    OnMapCollisionOccurred();
                }
                else
                {
                    collisionPosition.X += velocity.X;
                    transform.X += velocity.X;
                   // transform.X = collisionPosition.X - collisionOffset.X; // really?
                    velocity.X  = 0.0f;
                }
            }
            #endregion

            // 5.) Vertical Movement
            #region if( velocity.Y > 0.0f )
            if( velocity.Y > 0.0f )
            {
                if( actionLayer.TestCollisionHorizontal<Moveable>(
                    (int)collisionPosition.X,
                    (int)(collisionPosition.Y + velocity.Y + height),
                    width - 2, 
                    out tileCoordX,
                    out tileCoordY, 
                    tileHandler, 
                    this ) )
                {
                    collisionPosition.Y = (tileCoordY * 16) - height;
                    transform.Y         = collisionPosition.Y - collisionOffset.Y;
                    velocity.Y          = 0.0f;

                    #region Sliding

                    if( this.CanSlide && velocity.X == 0.0f )
                    {
                        bool freeLeft  = tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX - 1, tileCoordY ), this )
                                      && tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX - 1, tileCoordY - 1 ), this );

                        bool freeRight = tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX + 1, tileCoordY ), this )
                                      && tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX + 1, tileCoordY - 1 ), this );
                        
                        if( freeLeft || freeRight )
                        {
                            int tilePixelX = tileCoordX * 16;

                            if( collisionPosition.X + slideOffset < tilePixelX && freeLeft )
                            {
                                velocity.X = -tileSlideSpeed * frameTime;
                                UpdatePositionMapSlide( actionLayer );
                            }
                            else if( collisionPosition.X - slideOffset > tilePixelX && freeRight )
                            {
                                velocity.X = tileSlideSpeed * frameTime;
                                UpdatePositionMapSlide( actionLayer );
                            }
                        }
                    }

                    #endregion

                    OnMapCollisionOccurred();
                }
                else
                {
                    collisionPosition.Y += velocity.Y;
                    transform.Y         += velocity.Y;
                    velocity.Y           = 0.0f;
                }
            }
            #endregion
            #region else if( velocity.Y < 0.0f )
            else if( velocity.Y < 0.0f )
            {
                if( actionLayer.TestCollisionHorizontal<Moveable>(
                    (int)collisionPosition.X, 
                    (int)(collisionPosition.Y + velocity.Y),
                    width - 2,
                    out tileCoordX, 
                    out tileCoordY, 
                    tileHandler, 
                    this ) )
                {
                    collisionPosition.Y = (tileCoordY + 1) * 16;
                    transform.Y = collisionPosition.Y - collisionOffset.Y;
                    velocity.Y = 0.0f;

                    #region Sliding

                    if( this.CanSlide && velocity.X == 0.0f )
                    {
                        bool freeLeft  = tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX - 1, tileCoordY ), this )
                                      && tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX - 1, tileCoordY + 1 ), this );

                        bool freeRight = tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX + 1, tileCoordY ), this )
                                      && tileHandler.IsWalkable( actionLayer.GetTileAtSafe( tileCoordX + 1, tileCoordY + 1 ), this );

                        if( freeLeft || freeRight )
                        {
                            int tilePixelX = tileCoordX * 16;

                            if( freeLeft && ((collisionPosition.X + slideOffset) < tilePixelX) )
                            {
                                float s = tileSlideSpeed * frameTime;
                                velocity.X = -s;

                                UpdatePositionMapSlide( actionLayer );
                            }
                            else if( freeRight && ((collisionPosition.X - slideOffset) > tilePixelX) )
                            {
                                float s = tileSlideSpeed * frameTime;
                                velocity.X = s;

                                UpdatePositionMapSlide( actionLayer );
                            }
                        }
                    }

                    #endregion

                    OnMapCollisionOccurred();
                }
                else
                {
                    transform.Y += velocity.Y;
                    velocity.Y   = 0.0f;
                }
            }

            #endregion
        }

        /// <summary>
        /// Updates the position of the player by sliding along a tile.
        /// </summary>
        /// <param name="actionLayer">
        /// The current Action Layer.
        /// </param>
        private void UpdatePositionMapSlide( TileMapDataLayer actionLayer )
        {
            Vector2 collisionOffset   = this.collision.Offset;
            Vector2 collisionSize     = this.collision.Size;
            Vector2 collisionPosition = transform.Position + collisionOffset;

            int tileCoordX, tileCoordY;
            #region if( velocity.X > 0.0f )
            if( velocity.X > 0.0f )
            {
                if( actionLayer.TestCollisionVertical<Moveable>(
                    (int)(collisionPosition.X + collisionSize.X),
                    (int)collisionPosition.Y,
                    (int)collisionSize.Y - 1,
                    velocity.X,
                    out tileCoordX, 
                    out tileCoordY, 
                    tileHandler, 
                    this ) )
                {
                    collisionPosition.X = (tileCoordX * 16) - collisionSize.X;
                    transform.X         = collisionPosition.X - collisionOffset.X;
                }
                else
                {
                    collisionPosition.X += velocity.X;
                    transform.X += velocity.X;
                }
            }
            #endregion
            #region else if( velocity.X < 0.0f )
            else if( velocity.X < 0.0f )
            {
                if( actionLayer.TestCollisionVertical<Moveable>(
                    (int)(collisionPosition.X + velocity.X),
                    (int)collisionPosition.Y,
                    (int)collisionSize.Y - 1,
                    velocity.X,
                    out tileCoordX,
                    out tileCoordY, 
                    tileHandler, 
                    this ) )
                {
                    collisionPosition.X = (tileCoordX + 1) * 16;
                    transform.X         = collisionPosition.X - collisionOffset.X;
                    return;
                }
                else
                {
                    collisionPosition.X += velocity.X;
                    transform.X          = collisionPosition.X - collisionOffset.X;
                    return;
                }
            }
            #endregion

            #region if( velocity.y > 0.0f )
            if( velocity.Y > 0.0f )
            {
                if( actionLayer.TestCollisionHorizontal<Moveable>(
                    (int)collisionPosition.X,
                    (int)(collisionPosition.Y + velocity.Y + collisionSize.Y),
                    (int)collisionSize.X - 2, 
                    out tileCoordX,
                    out tileCoordY,
                    tileHandler,
                    this ) )
                {
                    collisionPosition.Y = (tileCoordY * 16) - collisionSize.Y;
                    transform.Y         = collisionPosition.Y - collisionOffset.Y;
                }
                else
                {
                    transform.Y += velocity.Y;
                }
            }

            #endregion
            #region else if( velocity.Y < 0.0f )
            else if( velocity.Y < 0.0f )
            {
                if( actionLayer.TestCollisionHorizontal<Moveable>(
                    (int)collisionPosition.X,
                    (int)(collisionPosition.Y + velocity.Y),
                    (int)collisionSize.X - 2, 
                    out tileCoordX, 
                    out tileCoordY, 
                    tileHandler,
                    this ) )
                {
                    collisionPosition.Y = (tileCoordY + 1) * 16;
                    transform.Y         = collisionPosition.Y - collisionOffset.Y;
                }
                else
                {
                    transform.Y += velocity.Y;
                }
            }

            #endregion
        }

        /// <summary>
        /// Updates the Entity's active pushing effect.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        private void UpdatePushing( Atom.IUpdateContext updateContext )
        {
            if( isPushed )
            {
                Vector2 toPush = pushingValue * updateContext.FrameTime;

                pushingValue -= toPush;
                velocity     += toPush;

                bool finishedPushingX = false, finishedPushingY = false;

                if( pushingValue.X < 0.0f )
                {
                    if( pushingValue.X >= -1.0f )
                        finishedPushingX = true;
                }
                else
                {
                    if( pushingValue.X <= 1.0f )
                        finishedPushingX = true;
                }

                if( pushingValue.Y < 0.0f )
                {
                    if( pushingValue.Y >= -1.0f )
                        finishedPushingY = true;
                }
                else
                {
                    if( pushingValue.Y <= 1.0f )
                        finishedPushingY = true;
                }

                if( finishedPushingX && finishedPushingY )
                {
                    pushingValue = Vector2.Zero;
                    isPushed = false;
                }
            }
        }

        #endregion

        #region > Movement <

        #region Move

        /// <summary> 
        /// Moves the Entity without altering its this.Direction.
        /// </summary>
        /// <param name="speedX"> The movement speed on the x-axis. </param>
        /// <param name="speedY"> The movement speed on the y-axis. </param>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void Move( float speedX, float speedY, float frameTime )
        {
            this.velocity.X += speedX * frameTime;
            this.velocity.Y += speedY * frameTime;
        }

        /// <summary>
        /// Moves the Entity without altering its this.Direction.
        /// </summary>
        /// <param name="speed"> The movement speed. </param>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void Move( Vector2 speed, float frameTime )
        {
            this.velocity.X += speed.X * frameTime;
            this.velocity.Y += speed.Y * frameTime;
        }

        /// <summary>
        /// Moves the Entity without altering its this.Direction.
        /// </summary>
        /// <param name="speed"> The movement speed. </param>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        /// <param name="change">
        /// Will store the change in velocity.
        /// </param>
        public void Move( Vector2 speed, float frameTime, out Vector2 change )
        {
            change.X = speed.X * frameTime;
            change.Y = speed.Y * frameTime;

            this.velocity.X += change.X;
            this.velocity.Y += change.Y;
        }

        #endregion

        #region MoveDir

        #region MoveDir

        /// <summary>
        /// Moves the Entity into the specified this.Direction.
        /// </summary>
        /// <param name="direction">
        /// The this.Direction to move to. Must be a normalised this.Direction vector. 
        /// </param>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveDir( Vector2 direction, float frameTime )
        {
            this.velocity += direction * this.GetStateModifiedSpeed() * frameTime;

            if( velocity.Y > 0.0f )
                this.Direction = Direction4.Down;
            else if( velocity.Y < 0.0f )
                this.Direction = Direction4.Up;
            else if( velocity.X > 0.0f )
                this.Direction = Direction4.Right;
            else if( velocity.X < 0.0f )
                this.Direction = Direction4.Left;
        }

        #endregion

        #region MoveDir

        /// <summary>
        /// Moves this object into the specified this.Direction.
        /// </summary>
        /// <param name="direction">
        /// The this.Direction to move into.
        /// </param>
        /// <param name="frameTime">
        /// The time the last frame took in seconds.
        /// </param>
        public void MoveDir( Direction4 direction, float frameTime )
        {
            switch( direction )
            {
                case Direction4.Down:
                    MoveDown( frameTime );
                    break;

                case Direction4.Up:
                    MoveUp( frameTime );
                    break;

                case Direction4.Left:
                    MoveLeft( frameTime );
                    break;

                case Direction4.Right:
                    MoveRight( frameTime );
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region MoveDir

        /// <summary>
        /// Moves the specified Entity into the specified this.Direction.
        /// </summary>
        /// <param name="dir"> The this.Direction to move to. </param>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveDir( Direction8 dir, float frameTime )
        {
            switch( dir )
            {
                case Direction8.Down:
                    MoveDown( frameTime );
                    break;
                case Direction8.LeftDown:
                    MoveLeftDown( frameTime );
                    break;
                case Direction8.RightDown:
                    MoveRightDown( frameTime );
                    break;

                case Direction8.Up:
                    MoveUp( frameTime );
                    break;
                case Direction8.LeftUp:
                    MoveLeftUp( frameTime );
                    break;
                case Direction8.RightUp:
                    MoveRightUp( frameTime );
                    break;

                case Direction8.Left:
                    MoveLeft( frameTime );
                    break;
                case Direction8.Right:
                    MoveRight( frameTime );
                    break;

                case Direction8.None:
                    break;

                default:
                    break;
            }
        }

        #endregion

        #endregion

        #region MoveTo

        /// <summary>
        /// Moves the Entity on a straight line to the specified <paramref name="location"/>.
        /// </summary>
        /// <param name="location"> The location to move to. </param>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveTo( Vector2 location, float frameTime )
        {
            Vector2 direction = (location - this.transform.Position);
            direction.Normalize();

            MoveDir( direction, frameTime );
        }

        /// <summary>
        /// Moves the Entity on a straight line to the specified <paramref name="location"/>.
        /// </summary>
        /// <param name="location"> The location to move to. </param>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveToCenter( Vector2 location, float frameTime )
        {
            Vector2 direction = (location - this.collision.Center);
            direction.Normalize();

            MoveDir( direction, frameTime );
        }

        #endregion

        #region MoveLeft

        /// <summary>
        /// Moves the Entity left/west.
        /// </summary>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveLeft( float frameTime )
        {
            velocity.X -= this.GetStateModifiedSpeed() * frameTime;

            isStanding = false;
            this.Direction = Direction4.Left;
        }

        #endregion

        #region MoveRight

        /// <summary>
        /// Moves the Entity right/east.
        /// </summary>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveRight( float frameTime )
        {
            velocity.X += this.GetStateModifiedSpeed() * frameTime;

            isStanding = false;
            this.Direction  = Direction4.Right;
        }

        #endregion

        #region MoveUp

        /// <summary>
        /// Moves the Entity up/north.
        /// </summary>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveUp( float frameTime )
        {
            velocity.Y -= this.GetStateModifiedSpeed() * frameTime;

            isStanding = false;
            this.Direction = Direction4.Up;
        }

        #endregion

        #region MoveDown

        /// <summary>
        /// Moves the Entity down/south.
        /// </summary>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveDown( float frameTime )
        {
            this.velocity.Y += this.GetStateModifiedSpeed() * frameTime;

            this.isStanding = false;
            this.Direction  = Direction4.Down;
        }

        #endregion

        #region MoveLeftUp

        /// <summary>
        /// Moves the Entity left/west and up/north.
        /// </summary>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveLeftUp( float frameTime )
        {
            float speed = -(this.GetStateModifiedSpeed() * this.upliqueMovementMod) * frameTime;
            velocity += new Vector2( speed, speed );

            isStanding = false;
            this.Direction = Direction4.Left;
        }

        /// <summary>
        /// Moves the Entity up/north and left/west.
        /// </summary>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveUpLeft( float frameTime )
        {
            float speed = -(this.GetStateModifiedSpeed() * this.upliqueMovementMod) * frameTime;

            velocity += new Vector2( speed, speed );

            isStanding = false;
            this.Direction  = Direction4.Up;
        }

        #endregion

        #region MoveRightUp

        /// <summary>
        /// Moves the Entity right/east and up/north.
        /// </summary>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveRightUp( float frameTime )
        {
            float speed = (this.GetStateModifiedSpeed() * this.upliqueMovementMod) * frameTime;
            velocity += new Vector2( speed, -speed );

            isStanding = false;
            this.Direction = Direction4.Right;
        }

        /// <summary>
        /// Moves the Entity up/north and right/east.
        /// </summary>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveUpRight( float frameTime )
        {
            float speed = (this.GetStateModifiedSpeed() * this.upliqueMovementMod) * frameTime;
            velocity += new Vector2( speed, -speed );

            isStanding = false;
            this.Direction = Direction4.Up;
        }

        #endregion

        #region MoveLeftDown

        /// <summary>
        /// Moves the Entity left/west and down/south.
        /// </summary>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveLeftDown( float frameTime )
        {
            float speed  = (this.GetStateModifiedSpeed() * this.upliqueMovementMod) * frameTime;
            velocity += new Vector2( -speed, speed );

            isStanding = false;
            this.Direction = Direction4.Left;
        }

        /// <summary>
        /// Moves the Entity down/south and left/west.
        /// </summary>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveDownLeft( float frameTime )
        {
            float speed = (this.GetStateModifiedSpeed() * this.upliqueMovementMod) * frameTime;
            velocity += new Vector2( -speed, speed );

            isStanding = false;
            this.Direction = Direction4.Down;
        }

        #endregion

        #region MoveRightDown

        /// <summary>
        /// Moves the Entity right/east and down/south.
        /// </summary>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveRightDown( float frameTime )
        {
            float speed = (this.GetStateModifiedSpeed() * this.upliqueMovementMod) * frameTime;
            velocity += new Vector2( speed, speed );

            isStanding = false;
            this.Direction = Direction4.Right;
        }

        /// <summary>
        /// Moves the Entity down/south and right/east.
        /// </summary>
        /// <param name="frameTime">The time the last frame took in seconds.</param>
        public void MoveDownRight( float frameTime )
        {
            float speed = (this.GetStateModifiedSpeed() * this.upliqueMovementMod) * frameTime;
            velocity += new Vector2( speed, speed );

            isStanding = false;
            this.Direction = Direction4.Down;
        }

        #endregion

        #endregion

        #region > Push <

        /// <summary>
        /// Pushes the Entity using the given <paramref name="force"/>.
        /// </summary>
        /// <param name="force">The force to apply.</param>
        public void Push( Vector2 force )
        {
            if( this.CanBePushed )
            {
                this.isPushed      = true;
                this.pushingValue += (force * pushingForceModifier);
            }
        }

        /// <summary>
        /// Pushes the Entity with the given power into the given direction.
        /// </summary>
        /// <param name="power">The power of the push.</param>
        /// <param name="direction">The direction of the push.</param>
        public void Push( float power, Direction4 direction )
        {
            if(power == 0.0f)
                return;

            switch( direction )
            {
                case Direction4.Right:
                    this.Push( new Vector2( power, 0.0f ) );
                    break;

                case Direction4.Left:
                    this.Push( new Vector2( -power, 0.0f ) );
                    break;

                case Direction4.Up:
                    this.Push( new Vector2( 0.0f, -power ) );
                    break;

                case Direction4.Down:
                    this.Push( new Vector2( 0.0f, power ) );
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// Resets the current pushing value affecting the moveable ZeldaEntity.
        /// </summary>
        internal void ResetPush()
        {
            this.isPushed     = false;
            this.pushingValue = Vector2.Zero;
        }

        #endregion

        #region > Floor <

        /// <summary>
        /// Moves the Entity one TileMapFloor up.
        /// </summary>
        /// <returns>
        /// true if the entity has moved one floor up;
        /// otherwise false.
        /// </returns>
        public bool MoveFloorUp()
        {
            return this.ChangeFloor( this.Owner.FloorNumber + 1 );
        }

        /// <summary>
        /// Moves the Entity one TileMapFloor down.
        /// </summary>
        /// <returns>
        /// true if the entity has moved one floor down;
        /// otherwise false.
        /// </returns>
        public bool MoveFloorDown()
        {
            return this.ChangeFloor( this.Owner.FloorNumber - 1 );
        }

        /// <summary>
        /// Changes the floor of the Entity.
        /// </summary>
        /// <param name="newFloorNumber">
        /// The floor number to change to.
        /// </param>
        /// <returns>
        /// true if the entity has changed to the specified floor;
        /// otherwise false.
        /// </returns>
        private bool ChangeFloor( int newFloorNumber )
        {          
            var scene = this.Scene;            
            if( scene == null )
                return false;

            if( newFloorNumber < 0 || newFloorNumber >= scene.FloorCount )
                return false;

            this.Owner.FloorNumber = newFloorNumber;
            scene.NotifyVisabilityUpdateNeeded();
            return true;
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 3;
            context.Write( CurrentVersion );

            context.Write( this.baseSpeed );

            context.Write( this.CollidesWithMap );
            context.Write( this.CanMove );
            context.Write( this.CanSwim );
            context.Write( this.CanSlide );
            context.Write( this.CanBePushed );
            context.Write( this.CanChangeFloor );
            context.Write( this.PushingForceModifier ); // new int v3.
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
            const int CurrentVersion = 3;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 2, CurrentVersion, this.GetType() );

            this.baseSpeed = context.ReadSingle();
            this.speed     = baseSpeed;

            this.CollidesWithMap        = context.ReadBoolean();
            this.CanMove        = context.ReadBoolean();
            this.CanSwim        = context.ReadBoolean();
            this.CanSlide       = context.ReadBoolean();
            this.CanBePushed    = context.ReadBoolean();
            this.CanChangeFloor = context.ReadBoolean();

            if( version >= 3 )
            {
                this.PushingForceModifier = context.ReadSingle();
            }
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Setups the given <see cref="Moveable"/> object to be a clone of this <see cref="Moveable"/>.
        /// </summary>
        /// <param name="clone">
        /// The object to setup as a clone.
        /// </param>
        public void SetupClone( Moveable clone )
        {
            Contract.Requires<ArgumentNullException>( clone != null );

            clone.upliqueMovementMod = upliqueMovementMod;
            clone.tileSlideSpeed     = tileSlideSpeed;
            clone.baseSpeed          = baseSpeed;
            clone.speed              = speed;

            clone.CollidesWithMap     = this.CollidesWithMap;
            clone.CanMove     = this.CanMove;
            clone.CanSwim     = this.CanSwim;
            clone.CanBePushed = this.CanBePushed;
            clone.pushingForceModifier = this.pushingForceModifier;

            clone.TileHandler = this.TileHandler;
        }

        #endregion

        #region > Misc <
        
        /// <summary>
        /// Gets a value indicating whether the moveable entity could move at the given position on the given action layer.
        /// </summary>
        /// <param name="position">
        /// The world space position to check.
        /// </param>
        /// <param name="actionLayer">
        /// The action layer to check on.
        /// </param>
        /// <returns>
        /// True if the moveable entity could walk; -or- otherwise false.
        /// </returns>
        public bool IsWalkableAt( Vector2 position, TileMapDataLayer actionLayer )
        {
            return this.tileHandler.IsWalkable( actionLayer.GetTileAtSafe( (int)(position.X / 16), (int)(position.Y / 16) ), this );
        }

        /// <summary>
        /// Fires the <see cref="MapCollisionOccurred"/> event.
        /// </summary>
        private void OnMapCollisionOccurred()
        {
            if( this.MapCollisionOccurred != null )
            {
                this.MapCollisionOccurred( this );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The current movement velocity of the Entity.
        /// </summary>
        private Vector2 velocity;

        /// <summary>
        /// The IZeldaTileHandler used by the Moveable.
        /// </summary>
        private IZeldaTileHandler tileHandler;
        
        private float slideOffset = 5.0f;

        #region > Pushing <

        /// <summary>
        /// States whether the Entity is currently under a Pushing effect.
        /// </summary>
        private bool isPushed;

        /// <summary>
        /// Stores the currently active pushing value.
        /// </summary>
        private Vector2 pushingValue;

        /// <summary>
        /// The value all pushing forces applied to this moveable entity are multiplied by.
        /// </summary>
        private float pushingForceModifier = 1.0f;

        #endregion

        #region > Settings <

        /// <summary>
        /// The movement speed of the Entity.
        /// </summary>
        private float speed, baseSpeed;

        /// <summary>
        /// The movement speed when sliding against a solid tile.
        /// </summary>
        private float tileSlideSpeed = 10.0f;

        /// <summary>
        /// The modifier that is applied to uplique movement.
        /// </summary>
        private float upliqueMovementMod = 0.75f;

        #endregion

        #region > State <

        /// <summary>
        /// States whether the Entity is currently standing.
        /// </summary>
        private bool isStanding = true;

        /// <summary>
        /// States whether the Entity was standing in the last frame.
        /// </summary>
        private bool wasStanding = true;

        #endregion

        #region > Components <

        /// <summary>
        /// Identifies the ZeldaTransform component of the Entity that owns this <see cref="Moveable"/> component.
        /// </summary>
        private ZeldaTransform transform;
        
        /// <summary>
        /// Identifies the ZeldaTransform component of the Entity that owns this <see cref="Moveable"/> component.
        /// </summary>
        private ZeldaCollision collision;

        #endregion

        #endregion
    }
}