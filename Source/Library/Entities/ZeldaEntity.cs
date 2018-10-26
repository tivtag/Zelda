// <copyright file="ZeldaEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.ZeldaEntity class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using System;
    using Atom;
    using Atom.Components;
    using Atom.Scene;
    using Zelda.Entities.Components;
    
    /// <summary>
    /// Represents an <see cref="Entity"/> that is composed of different <see cref="Component"/>s,
    /// including the following Components by default:
    /// <see cref="ZeldaTransform"/>, <see cref="ZeldaCollision"/> and <see cref="QuadTreeItem2"/>.
    /// </summary>
    public class ZeldaEntity : Entity, IZeldaEntity
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the <see cref="FloorNumber"/> of this <see cref="ZeldaEntity"/> has changed.
        /// </summary>
        public event RelaxedEventHandler<ChangedValue<int>> FloorNumberChanged;

        /// <summary>
        /// Fired when the <see cref="IsVisible"/> property of this <see cref="ZeldaEntity"/> has changed.
        /// </summary>
        public event EventHandler IsVisibleChanged;

        /// <summary>
        /// Fired when this ZeldaEntity has been added to a <see cref="ZeldaScene"/>.
        /// </summary>
        public event RelaxedEventHandler<ZeldaScene> Added;

        /// <summary>
        /// Fired when this ZeldaEntity has been removed from a <see cref="ZeldaScene"/>.
        /// </summary>
        public event RelaxedEventHandler<ZeldaScene> Removed;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="ZeldaScene"/> this <see cref="ZeldaEntity"/> is part of.
        /// </summary>
        public ZeldaScene Scene
        {
            get
            {
                return this.scene;
            }
            
            set
            { 
                this.scene = value;
                this.UpdateCachedDrawOrders();
            }
        }

        /// <summary>
        /// Gets the action layer this <see cref="ZeldaEntity"/> currently operates on.
        /// </summary>
        /// <value>
        /// The ActionLayer or null.
        /// </value>
        public Atom.Scene.Tiles.TileMapDataLayer ActionLayer
        {
            get
            {
                var scene = this.Scene;
                if( scene == null )
                    return null;

                var floor = scene.Map.GetFloor( this.FloorNumber );
                if( floor == null )
                    return null;

                return floor.ActionLayer;
            }
        }

        /// <summary>
        /// Gets or sets the floor this <see cref="ZeldaEntity"/> is part of.
        /// </summary>
        public int FloorNumber
        {
            get 
            {
                return this._floorNumber; 
            }

            set
            {
                if( value == this._floorNumber )
                    return;

                var oldValue = this._floorNumber;
                this._floorNumber = value;

                if( this.FloorNumberChanged != null )
                {
                    this.FloorNumberChanged( this, new ChangedValue<int>( oldValue, value ) );
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this <see cref="ZeldaEntity"/>
        /// is below or above other ZeldaEntities.
        /// </summary>
        public EntityFloorRelativity FloorRelativity
        {
            get 
            {
                return this._floorRelativity;
            }

            set
            {
                this._floorRelativity = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// this <see cref="ZeldaEntity"/> is saved into the SceneFile.
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        public bool IsSaved
        {
            get
            {
                return this._isSaved;
            }

            set
            {
                this._isSaved = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// this <see cref="ZeldaEntity"/> is manually editable by the user (in the editor).
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        public bool IsEditable
        {
            get
            {
                return this._isEditable;
            }

            set
            {
                this._isEditable = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZeldaEntity"/>
        /// is manually removeable by the user (in the editor).
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        public bool IsRemoveable
        {
            get
            {
                return this._isRemoveable;
            }

            set
            { 
                this._isRemoveable = value; 
            }
        }

        #region > Components <

        /// <summary>
        /// Gets the <see cref="ZeldaTransform"/> component of this <see cref="ZeldaEntity"/>.
        /// </summary>
        public ZeldaTransform Transform
        {
            get 
            {
                return this.transform;
            }
        }

        /// <summary>
        /// Gets the <see cref="ZeldaCollision"/> component of this <see cref="ZeldaEntity"/>.
        /// </summary>
        public ZeldaCollision Collision
        {
            get
            {
                return this.collision;
            }
        }

        /// <summary>
        /// Gets the <see cref="QuadTreeItem2"/> component of this <see cref="ZeldaEntity"/>.
        /// </summary>
        public QuadTreeItem2 QuadTreeItem
        {
            get 
            { 
                return this.quadTreeItem; 
            }
        }

        #endregion

        #region > Drawing <

        /// <summary>
        /// Gets the draw order of this <see cref="ZeldaEntity"/>
        /// relative to all other Entities on the same <see cref="FloorNumber"/>.
        /// </summary>
        public float RelativeDrawOrder
        {
            get { return this._relativeDrawOrder; }
        }

        /// <summary>
        /// Gets the secondary draw order value of this IZeldaFloorDrawable.
        /// </summary>
        /// <value>
        /// This value is used as a secondary sorting-value that is
        /// used when the RelativeDrawOrder of two IZeldaFloorDrawable is equal.
        /// </value>
        public virtual float SecondaryDrawOrder
        {
            get 
            {
                return this._secondaryDrawOrder;
            }
        }
    
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZeldaEntity"/> is visible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return this._isVisible;
            }

            set
            {
                if( value == this._isVisible )
                    return;

                this._isVisible = value;
                this.quadTreeItem.IsVisibleToQuadTree = value;

                if( this.Scene != null )
                    this.Scene.NotifyVisabilityUpdateNeeded();

                this.IsVisibleChanged.Raise( this );
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Zelda.Entities.Drawing.IDrawDataAndStrategy"/> of this <see cref="ZeldaEntity"/>.
        /// </summary>
        public Zelda.Entities.Drawing.IDrawDataAndStrategy DrawDataAndStrategy
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeldaEntity"/> class.
        /// </summary>
        /// <param name="componentCapacity">
        /// The number of components this entity can hold without reallocating memory.
        /// </param>
        public ZeldaEntity( int componentCapacity = 3 ) 
            : base( componentCapacity )
        {
            this.transform = new ZeldaTransform();
            this.collision = new ZeldaCollision();
            this.quadTreeItem = new QuadTreeItem2();

            this.Components.BeginSetup();
            {
                this.Components.Add( this.transform );
                this.Components.Add( this.collision );
                this.Components.Add( this.quadTreeItem );
            }
            this.Components.EndSetup();

            // Hook events.
            collision.Changed += this.OnCollisionChanged;
        }

        #endregion

        #region [ Methods ]

        #region > Draw <

        /// <summary>
        /// Called before drawing anything is drawn.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public virtual void PreDraw( ZeldaDrawContext drawContext )
        {
            // no op.
        }
        
        /// <summary>
        /// Draws this <see cref="ZeldaEntity"/> using its <see cref="DrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public virtual void Draw( ZeldaDrawContext drawContext )
        {
            if( !this.IsVisible )
                return;

            if( this.DrawDataAndStrategy != null )
                this.DrawDataAndStrategy.Draw( drawContext );
        }

        /// <summary>
        /// Draws this <see cref="ZeldaEntity"/> using its <see cref="DrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void Draw( IDrawContext drawContext )
        {
            this.Draw( (ZeldaDrawContext)drawContext );
        }

        #endregion

        #region > Update <

        /// <summary>
        /// Gets called before <see cref="Update"/> is called.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public virtual void PreUpdate( ZeldaUpdateContext updateContext )
        {
            this.Components.PreUpdate( updateContext );
        }

        /// <summary>
        /// Updates this <see cref="ZeldaEntity"/>, including all of its components.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
            this.Components.Update( updateContext );

            if( this.DrawDataAndStrategy != null )
                this.DrawDataAndStrategy.Update( updateContext );
        }

        /// <summary>
        /// Updates the cached relative draw order values.
        /// </summary>
        private void UpdateCachedDrawOrders()
        {
            var scene = this.Scene;

            if( scene == null )
            {
                _relativeDrawOrder = _secondaryDrawOrder = 0.0f;
                return;
            }

            var center = this.collision.Center;

            // Primarily sort by the Y position.
            float mapHeight = scene.HeightInPixels;
            float relativeY = center.Y / mapHeight;
            _relativeDrawOrder  = relativeY + (mapHeight * (int)this.FloorRelativity);

            // Secondarily sort by the X position.
            float mapWidth  = scene.WidthInPixels;
            float relativeX = center.X / mapWidth;
            _secondaryDrawOrder = relativeX + (mapWidth * (int)this.FloorRelativity);
        }

        #endregion

        #region > Events <

        /// <summary>
        /// Gets called when the collision shapes
        /// (and as such the collision center) of this ZeldaEntity has changed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnCollisionChanged( Atom.Components.Collision.ICollision2 sender )
        {
            this.UpdateCachedDrawOrders();
        }

        #endregion

        #region > Scene <

        /// <summary>
        /// Tries to add this <see cref="ZeldaEntity"/> to the specified <see cref="Atom.Scene.IScene"/>.
        /// </summary>
        /// <param name="scene">
        /// The <see cref="Atom.Scene.IScene"/> to add this <see cref="Entity"/> to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="scene"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If this <see cref="Entity"/> already is part of a <see cref="Atom.Scene.IScene"/>.
        /// </exception>
        public virtual void AddToScene( ZeldaScene scene )
        {
            if( scene == null )
            {
                throw new ArgumentNullException( "scene" );
            }

            if( this.scene != null )
            {
                throw new InvalidOperationException(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        ErrorStrings.EntityAlreadyAddedToScene,
                        this.ToString()
                    )
                );
            }

            // Add:
            scene.Add( this );
            this.scene = scene;

            if( this.Added != null )
                this.Added( this, (ZeldaScene)scene );
        }
               
        /// <summary>
        /// Tries to remove this <see cref="ZeldaEntity"/> from its <see cref="Atom.Scene.IScene"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If this <see cref="Entity"/> is not part of a <see cref="Atom.Scene.IScene"/>.
        /// </exception>
        public virtual void RemoveFromScene()
        {
            // Verify State
            if( this.scene == null )
            {
                throw new InvalidOperationException(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        ErrorStrings.EntityNotPartOfAScene,
                        this.ToString()
                    )
                );
            }

            // Remove
            var oldScene = this.Scene;
            this.scene.RemoveEntity( this );
            this.scene = null;

            if( this.Removed != null )
                this.Removed( this, oldScene );
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Creates a clone of this <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public virtual ZeldaEntity Clone()
        {
            var clone = new ZeldaEntity();

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given <see cref="ZeldaEntity"/> object
        /// to be a clone of this <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="clone">
        /// The entity to clone.
        /// </param>
        protected virtual void SetupClone( ZeldaEntity clone )
        {
            // Clone basics:
            clone.Name            = this.Name;
            clone.IsVisible       = this.IsVisible;
            clone.FloorNumber     = this.FloorNumber;
            clone.FloorRelativity = this.FloorRelativity;

            // Clone Transform:
            // NOTE NOTE NOTE
            // Future optimzations possible here by creating the following method:
            // entity.Transform.SetupClone( this.Transform );
            clone.Transform.Position = this.Transform.Position;
            clone.Transform.Rotation = this.Transform.Rotation;
            clone.Transform.Origin   = this.Transform.Origin;
            clone.Transform.Scale    = this.Transform.Scale;

            this.collision.SetupClone( clone.collision );

            // Clone DDS & Control:           
            clone.DrawDataAndStrategy = this.DrawDataAndStrategy != null ? this.DrawDataAndStrategy.Clone( clone ) : null;
        }

        #endregion
        
        #region > ISceneObject / ISceneProvider Members <

        /// <summary>
        /// Adds this ZeldaEntity to the given ZeldaScene.
        /// </summary>
        /// <param name="scene">
        /// The scene to add this ZeldaEntity to.
        /// </param>
        void Atom.Scene.ISceneObject.AddToScene( Atom.Scene.IScene scene )
        {
            this.AddToScene( (ZeldaScene)scene );
        }

        /// <summary>
        /// Gets or sets the ZeldaScene this ZeldaEntity is part of.
        /// </summary>
        Atom.Scene.IScene Atom.Scene.ISceneObject.Scene
        {
            get
            {
                return this.scene;
            }

            set
            {
                this.Scene = (ZeldaScene)value;
            }
        }

        /// <summary>
        /// Gets the ZeldaScene this ZeldaEntity is part of.
        /// </summary>
        Atom.Scene.IScene Atom.Scene.ISceneProvider.Scene
        {
            get
            {
                return this.scene;
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The number of the TileMapFloor the entity is part of.
        /// </summary>
        private int _floorNumber;

        /// <summary>
        /// Stores the current EntityFloorRelativity setting of the entity.
        /// </summary>
        private EntityFloorRelativity _floorRelativity = EntityFloorRelativity.Normal;

        /// <summary>
        /// Stores the current visability setting of the entity.
        /// </summary>
        private bool _isVisible = true;

        /// <summary>
        /// Caches the relative draw order of this ZeldaEntity.
        /// This value is updated once per frame.
        /// </summary>
        private float _relativeDrawOrder, _secondaryDrawOrder;

        /// <summary>
        /// Represents the storage fields of the <see cref="IsSaved"/>, <see cref="IsRemoveable"/> 
        /// and <see cref="IsEditable"/> properties.
        /// </summary>
        private bool _isSaved = true, _isRemoveable = true, _isEditable = true;

        /// <summary>
        /// The scene this ZeldaEntity is currently attached to.
        /// </summary>
        private ZeldaScene scene;

        /// <summary>
        /// Specifies the transform component of the entity.
        /// </summary>
        private readonly ZeldaTransform transform;

        /// <summary>
        /// Specifies the collision component of the entity.
        /// </summary>
        private readonly ZeldaCollision collision;

        /// <summary>
        /// Specifies the quad tree item component of the entity.
        /// </summary>
        private readonly QuadTreeItem2 quadTreeItem;

        #endregion
    }
}