// <copyright file="Plant.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Plant class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using Atom.Fmod;
    using Atom.Math;
    using Atom.Xna;
    using Zelda.Attacks;
    using Zelda.Entities.Components;
    using Zelda.Entities.Drawing;
    using Zelda.Status;

    /// <summary>
    /// A plant is un-moveable but killable entity that may drop loot.
    /// </summary>
    public class Plant : ZeldaEntity, IAttackableEntity
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="Statable"/> component of this <see cref="Plant"/> entity.
        /// </summary>
        public Statable Statable
        {
            get
            {
                return this.statable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Killable"/> component of this <see cref="Plant"/> entity.
        /// </summary>
        public Killable Killable
        {
            get
            {
                return this.killable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Lootable"/> component of this <see cref="Plant"/> entity.
        /// </summary>
        public Lootable Lootable
        {
            get
            {
                return this.lootable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Attackable"/> component of this <see cref="Plant"/> entity.
        /// </summary>
        public Attackable Attackable
        {
            get
            {
                return this.attackable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Respawnable"/> component of this <see cref="Plant"/> entity.
        /// </summary>
        public Respawnable Respawnable
        {
            get
            {
                return this.respawnable;
            }
        }

        /// <summary>
        /// Gets or sets the name of the special effect animation that is
        /// displayed when this Plant got cut.
        /// </summary>
        public string CutEffectAnimationName
        {
            get
            {
                return this.cutEffect.DrawDataAndStrategy.SpriteGroup;
            }

            set
            {
                this.cutEffect.DrawDataAndStrategy.SpriteGroup = value;

                if( string.IsNullOrEmpty( value ) == false )
                {
                    this.cutEffect.DrawDataAndStrategy.Load( serviceProvider );

                    // Hook onto the event that notifies us
                    // that the animation of the special effect has ended.
                    var animDDS = (TintedOneDirAnimDrawDataAndStrategy)this.cutEffect.DrawDataAndStrategy;

                    if( animDDS.Animation != null )
                    {
                        animDDS.Animation.ReachedEnd += this.OnCutEffectEnded;
                    }
                }
            }
        }

        public TintedOneDirAnimDrawDataAndStrategy CutEffectDrawStrategy
        {
            get
            {
                return (TintedOneDirAnimDrawDataAndStrategy)this.cutEffect.DrawDataAndStrategy;
            }
        }

        /// <summary>
        /// Gets or sets the Color of the Cut Effect Animation that 
        /// is shown when the player cuts the plant.
        /// </summary>
        public Microsoft.Xna.Framework.Color CutEffectAnimationColor
        {
            get
            {
                return this.CutEffectDrawStrategy.BaseColor;
            }

            set
            {
                this.CutEffectDrawStrategy.BaseColor = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this Plant will display a
        /// 'special' effect when she gets cut.
        /// </summary>
        private bool HasCutEffect
        {
            get
            {
                return this.cutEffect.DrawDataAndStrategy.SpriteGroup != null;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Plant"/> class.
        /// </summary>
        public Plant()
            : base( 8 )
        {
            // Create Components
            this.respawnable = new Respawnable();
            this.statable = new Statable() {
                Race = RaceType.Plant,
                CanBlock = false
            };

            // Plants are easy to hit!
            this.statable.ChanceToBe.BaseHit = 5.0f;
            this.statable.ChanceTo.SetAvoidance( 0.0f );

            this.lootable = new Lootable() {
                DropRange = 3.0f
            };

            this.killable = new Killable();
            this.attackable = new Attackable();

            // Add Components
            this.Components.BeginSetup();
            {
                this.Components.Add( respawnable );
                this.Components.Add( statable );
                this.Components.Add( attackable );
                this.Components.Add( killable );
                this.Components.Add( lootable );
            }
            this.Components.EndSetup();

            // Hook Events
            this.killable.Killed += this.OnKilled;
            this.statable.Damaged += this.OnDamaged;
            this.respawnable.Respawned += this.OnRespawned;

            // Create and setup the cut effect
            this.cutEffect = new ZeldaEntity() {
                Name = "Plant_CutEffect"
            };

            this.cutEffect.DrawDataAndStrategy = new TintedOneDirAnimDrawDataAndStrategy( cutEffect );

            // Set basic properties
            this.FloorRelativity = EntityFloorRelativity.IsBelow;
        }

        /// <summary>
        /// Setups this <see cref="Plant"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;

            this.statable.Setup( serviceProvider );
            this.lootable.Setup( serviceProvider );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Helper method that spawns the cut effect for this Plant.
        /// </summary>
        private void SpawnCutEffect()
        {
            if( this.HasCutEffect )
            {
                this.cutEffect.Transform.Position = this.Transform.Position;
                this.cutEffect.FloorNumber = this.FloorNumber;

                if( this.CutEffectDrawStrategy.Animation != null )
                {
                    this.CutEffectDrawStrategy.Animation.Reset();
                }

                this.cutEffect.AddToScene( this.Scene );

                //Sound sample = serviceProvider.AudioSystem.GetSample( "Grass_Cut.wav" );
                //var mode =
                //    Atom.Fmod.Native.MODE.SOFTWARE |
                //    Atom.Fmod.Native.MODE._3D |
                //    Atom.Fmod.Native.MODE._3D_LINEARROLLOFF;
                //sample.LoadAsSample( mode );

                //float volume = serviceProvider.Rand.RandomRange( 0.2f, 0.4f );
                //sample.PlayAt( this.Transform.Position, new FloatRange( 1, 240 ), volume );
            }
        }

        #region > Events <

        /// <summary>
        /// Called when this Plant has been destroyed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnKilled( Killable sender )
        {
            if( this.Scene == null )
                return;

            // Set the tile the Plant was placed on to be passable
            if( !ZeldaScene.EditorMode )
            {
                var map = this.Scene.Map;
                var floor = map.GetFloor( this.FloorNumber );
                var actionLayer = floor.ActionLayer;

                int tilePosX = (int)this.Transform.X / 16;
                int tilePosY = (int)this.Transform.Y / 16;
                actionLayer.SetTile( tilePosX, tilePosY, 0 /*ActionTileId.Normal*/ );
            }

            // Show special effect to notify the player
            this.SpawnCutEffect();

            // Drop Loot and start the respawn process
            this.lootable.DropLoot( serviceProvider.Rand );
            this.respawnable.NotifyRespawnNeeded();
        }

        /// <summary>
        /// Called when this Plant has been respawned.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>>
        private void OnRespawned( Respawnable sender )
        {
            // Regenerate
            this.statable.Life = this.statable.MaximumLife;

            if( !ZeldaScene.EditorMode )
            {
                // Set the tile the Plant was placed on to be not passable
                var map = this.Scene.Map;
                var floor = map.GetFloor( this.FloorNumber );
                var actionLayer = floor.ActionLayer;

                int tilePosX = (int)this.Transform.X / 16;
                int tilePosY = (int)this.Transform.Y / 16;
                actionLayer.SetTile( tilePosX, tilePosY, 1 /*ActionTileId.Solid*/ );
            }
        }

        /// <summary>
        /// Gets called when this Plant etity has been damaged.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="AttackDamageResult"/> that contains the event data.
        /// </param>
        private void OnDamaged( Statable sender, AttackDamageResult e )
        {
            if( this.Scene == null )
                return;

            var flyingTextManager = this.Scene.FlyingTextManager;
            var center = this.Collision.Center;

            Vector2 position;
            position.X = center.X;
            position.Y = center.Y - 10;

            switch( e.AttackReceiveType )
            {
                case Zelda.Attacks.AttackReceiveType.Hit:
                    flyingTextManager.FireAttackOnEnemyHit( position, e.Damage );
                    break;

                case Zelda.Attacks.AttackReceiveType.Crit:
                    flyingTextManager.FireAttackOnEnemyCrit( position, e.Damage );
                    break;

                case Zelda.Attacks.AttackReceiveType.PartialResisted:
                    flyingTextManager.FireAttackOnEnemyPartiallyResisted( position, e.Damage );
                    break;

                case Zelda.Attacks.AttackReceiveType.Miss:
                    flyingTextManager.FireAttackOnEnemyMissed( position );
                    break;

                case Zelda.Attacks.AttackReceiveType.Dodge:
                    flyingTextManager.FireAttackOnEnemyDodged( position );
                    break;

                case Zelda.Attacks.AttackReceiveType.Resisted:
                    flyingTextManager.FireAttackOnEnemyResisted( position );
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Called when the cut effect ended displaying.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnCutEffectEnded( SpriteAnimation sender )
        {
            this.cutEffect.RemoveFromScene();
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Setups the given <see cref="Plant"/> entity to be a clone of this <see cref="Plant"/> entity.
        /// </summary>
        /// <param name="plant">
        /// The Plant entity to setup as a clone of this Plant.
        /// </param>
        public void SetupClone( Plant plant )
        {
            base.SetupClone( plant );

            // Clone components:
            this.statable.SetupClone( plant.statable );
            this.killable.SetupClone( plant.killable );
            this.lootable.SetupClone( plant.lootable );
            this.respawnable.SetupClone( plant.respawnable );

            // Clone effect:
            plant.serviceProvider = this.serviceProvider;
            plant.CutEffectAnimationName = this.CutEffectAnimationName;
            plant.CutEffectAnimationColor = this.CutEffectAnimationColor;
        }

        /// <summary>
        /// Returns a clone of this <see cref="Plant"/> entity.
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            Plant clone = new Plant();

            this.SetupClone( clone );
            clone.Setup( this.serviceProvider );

            return clone;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the <see cref="Respawnable"/> component of this <see cref="Plant"/> entity.
        /// </summary>
        private readonly Respawnable respawnable;

        /// <summary>
        /// Identifies the <see cref="Statable"/> component of this <see cref="Plant"/> entity.
        /// </summary>
        private readonly Statable statable;

        /// <summary>
        /// Identifies the <see cref="Killable"/> component of this <see cref="Plant"/> entity.
        /// </summary>
        private readonly Killable killable;

        /// <summary>
        /// Identifies the <see cref="Lootable"/> component of this <see cref="Plant"/> entity.
        /// </summary>
        private readonly Lootable lootable;

        /// <summary>
        /// Identifies the <see cref="Attackable"/> component of this <see cref="Plant"/> entity.
        /// </summary>
        private readonly Attackable attackable;

        /// <summary>
        /// The effect object that is added to the scene
        /// once the player has cut the plant.
        /// </summary>
        private readonly ZeldaEntity cutEffect;

        /// <summary>
        /// Provides fast access to game related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="Plant"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<Plant>
        {
            /// <summary>
            /// Initializes a new instance of the ReaderWriter class.
            /// </summary>
            /// <param name="serviceProvider">
            /// Provides fast access to game-related services. 
            /// </param>
            public ReaderWriter( IZeldaServiceProvider serviceProvider )
                : base( serviceProvider )
            {
            }

            /// <summary>
            /// Serializes the given object using the given <see cref="System.IO.BinaryWriter"/>.
            /// </summary>
            /// <param name="entity">
            /// The entity to serialize.
            /// </param>
            /// <param name="context">
            /// The context under which the serialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Serialize( Plant entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                // First Write the header:
                const int Version = 2;
                context.Write( Version );

                // Write Name:
                context.Write( entity.Name );

                // Write Collision component
                entity.Collision.Serialize( context );

                // Write Killable component
                entity.killable.Serialize( context );

                // Write Statable component
                entity.statable.Serialize( context );

                // Write Lootable component
                entity.lootable.Serialize( context );

                // Write Respawnable component
                entity.respawnable.Serialize( context );

                // Write DDaS
                if( entity.DrawDataAndStrategy != null )
                {
                    context.Write( Drawing.DrawStrategyManager.GetName( entity.DrawDataAndStrategy ) );
                    entity.DrawDataAndStrategy.Serialize( context );
                }
                else
                {
                    context.Write( string.Empty );
                }

                // Write Cut Effect
                context.Write( entity.CutEffectAnimationName ?? string.Empty );
                context.Write( entity.CutEffectAnimationColor );
            }

            /// <summary>
            /// Deserializes the data in the given <see cref="System.IO.BinaryWriter"/> to initialize
            /// the given ZeldaEntity.
            /// </summary>
            /// <param name="entity">
            /// The ZeldaEntity to initialize.
            /// </param>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Deserialize( Plant entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                entity.Setup( serviceProvider );

                // Header
                const int CurrentVersion = 2;
                int version = context.ReadInt32();
                Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.EntityType );

                // Read Name
                entity.Name = context.ReadString();

                // Read Collision component
                entity.Collision.Deserialize( context );

                // Read Killable component
                entity.killable.Deserialize( context );

                // Read Statable component
                entity.statable.Deserialize( context );

                // Read Lootable component
                entity.lootable.Deserialize( context );

                // Read Respawnable component
                entity.respawnable.Deserialize( context );

                // Read draw data and strategy:
                string ddsName = context.ReadString();

                // -- need to refactor this --
                if( ddsName.Length != 0 )
                {
                    var dds = serviceProvider.DrawStrategyManager.GetStrategyClone( ddsName, entity );
                    dds.Deserialize( context );

                    try
                    {
                        dds.Load( serviceProvider );
                    }
                    catch( System.IO.FileNotFoundException exc )
                    {
                        serviceProvider.Log.WriteLine( Atom.Diagnostics.LogSeverities.Error, exc.ToString() );
                    }

                    entity.DrawDataAndStrategy = dds;
                }

                // Read and Load Cut Effect:
                entity.CutEffectAnimationName = context.ReadString();

                if( version >= 2 )
                {
                    entity.CutEffectAnimationColor = context.ReadColor();
                }
            }
        }

        #endregion
    }
}