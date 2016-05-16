// <copyright file="LightningSparkEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.Spells.Lightning.LightningSparkEntity class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Casting.Spells.Lightning
{
    using Atom.Math;
    using Atom.Scene.Tiles;
    using Atom.Xna;
    using Atom.Xna.Effects.Lightning;
    using Zelda.Attacks;
    using Zelda.Entities;
    using Xna = Microsoft.Xna.Framework;
    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///    *
    ///    |
    /// *--X--*
    ///    |
    ///    *
    /// </remarks>
    public sealed class LightningSparkEntity : ZeldaEntity, IZeldaSetupable
    {
        /// <summary>
        /// The length in pixels that is taken per step while ray-casting the lightning bolts against the tilemap.
        /// </summary>
        private const float RayWalkStepSize = 10.0f;

        /// <summary>
        /// Gets or sets the maximum length of a single Lightning Bolt.
        /// </summary>
        public float MaximumLength
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the offset from the position of this LightningSparkEntity to the
        /// start of the lightning bolts.
        /// </summary>
        public float Offset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the statable component of the entity that
        /// has created the sparks.
        /// </summary>
        public Status.Statable Creator 
        {
            get
            {
                return this.bolts[0].Creator;
            }

            set
            {
                for( int i = 0; i < this.bolts.Length; ++i )
                {
                    this.bolts[i].Creator = value;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the method that is used to calculate thed amage
        /// done by the spark.
        /// </summary>
        public AttackDamageMethod DamageMethod
        {
            get
            {
                return this.bolts[0].MeleeAttack.DamageMethod;
            }

            set
            {
                for( int i = 0; i < this.bolts.Length; ++i )
                {
                    this.bolts[i].MeleeAttack.DamageMethod = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this LightningSparkEntity
        /// is automatically rebuild when its transformation changes.
        /// </summary>
        /// <value>
        /// The default value is true.
        /// </value>
        public bool IsAutomaticallyRebuild 
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the LightningSparkEntity class.
        /// </summary>
        public LightningSparkEntity()
        {
            this.IsAutomaticallyRebuild = true;
            this.MaximumLength = 85.0f;
            this.Offset = 16.0f;
            this.Transform.RelativePosition = new Vector2( 5.0f, 8.0f );
            this.Transform.InheritsRotation = false;

            this.settings = new LightningSettings() {
                FrameLength = 0.125f,
                IsGlowEnabled = true,
                BaseWidth = 2.8f,
                JitterDeviationRadius = 20.0f,
                InteriorColor = new Xna.Color( 2, 149, 217, 0 ),
                ExteriorColor = Xna.Color.Blue.WithAlpha(200)
            };

            var topology = this.settings.Topology;
            topology.Clear();
            topology.Add( LightningSubdivisionOp.Jitter );
            topology.Add( LightningSubdivisionOp.JitterAndFork );
            topology.Add( LightningSubdivisionOp.Jitter );
            topology.Add( LightningSubdivisionOp.Jitter );

            for( int i = 0; i < this.bolts.Length; ++i)
            {
                this.bolts[i] = new LightningBoltEntity() {
                    Settings = this.settings,
                    Width = 20.0f                    
                };
            }

            this.Transform.Changed += this.OnTransformChanged;
            this.Added += this.OnAddedToScene;
            this.Removed += this.OnRemovedFromScene;
        }
        
        /// <summary>
        /// Called when this LightningSparkEntity was added to the specified scene.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="scene">
        /// The scene to which this LightningSparkEntity has been added to.
        /// </param>
        private void OnAddedToScene( object sender, ZeldaScene scene )
        {
            for( int i = 0; i < this.bolts.Length; ++i )
            {
                scene.Add( this.bolts[i] );
            }
        }

        /// <summary>
        /// Called when this LightningSparkEntity was removed to the specified scene.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="scene">
        /// The scene to which this LightningSparkEntity has been removed from.
        /// </param>
        private void OnRemovedFromScene( object sender, ZeldaScene scene )
        {
            for( int i = 0; i < this.bolts.Length; ++i )
            {
                scene.RemoveEntity( this.bolts[i] );
            }
        }

        /// <summary>
        /// Setups this LightningSparkEntity.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            for( int i = 0; i < this.bolts.Length; ++i )
            {
                this.bolts[i].Setup( serviceProvider );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        private void OnTransformChanged( Atom.Components.Transform.ITransform2 sender )
        {
            if( this.IsAutomaticallyRebuild )
            {
                this.Rebuild();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateContext"></param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.Transform.UpdateTransform();
            base.Update( updateContext );
        }

        /// <summary>
        /// Updates the starting and ending positions of the lightning bolts this lightning sparks consists of. 
        /// </summary>
        public void Rebuild()
        {
            float angle = this.Transform.Rotation;
            float angleDelta = Constants.TwoPi / this.bolts.Length;
            Vector2 position = this.Transform.Position;

            TileMapDataLayer actionLayer = null;

            if( this.Scene != null )
            {
                actionLayer = this.Scene.Map.GetFloor( this.FloorNumber ).ActionLayer;
            }

            for( int i = 0; i < this.bolts.Length; ++i, angle += angleDelta )
            {
                var bolt = this.bolts[i];
                Vector2 direction = Vector2.FromAngle( angle );

                float length = this.MaximumLength;                
                Vector2 start = position + direction * this.Offset;

                if( actionLayer != null )
                {
                    length = actionLayer.RayWalk( 
                        new Ray2( start, direction ), 
                        this.MaximumLength, 
                        RayWalkStepSize,
                        StaticRangedTileHandler.Instance,
                        this
                    );
                }

                if( this.MaximumLength == 0.0f )
                {
                    bolt.IsVisible = false;
                    length = 1.0f;
                }
                else
                {
                    bolt.IsVisible = true;
                }

                Vector2 end = start + direction * length;
                bolt.SetLocation( start, end );
            }
        }

        public void Reset()
        {
            foreach( var bolt in this.bolts )
            {
                bolt.Reset();
            }
        }
        
        /// <summary>
        /// The settings taht are applied to the bolts.
        /// </summary>
        private readonly LightningSettings settings;

        /// <summary>
        /// The bolts this lightning sparks consists of. 
        /// </summary>
        private readonly LightningBoltEntity[] bolts = new LightningBoltEntity[4];
    }
}
