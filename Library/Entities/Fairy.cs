
namespace Zelda.Entities
{
    using System.Collections.Generic;
    using Atom;
    using Atom.Math;
    using Zelda.Entities.Behaviours;
    using Zelda.Entities.Components;
    using Zelda.Entities.Drawing;
    using Zelda.Entities.Modifiers;
    using Zelda.Status;

    public sealed class Fairy : ZeldaEntity, IMoveableEntity, IZeldaSetupable, ILight
    {
        public event SimpleEventHandler<Fairy> IsEnabledChanged;

        public const float DefaultMovementSpeed = 50.0f;

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                if( value == this.isEnabled )
                    return;

                this.isEnabled = value;

                if( value )
                {
                    this.TeleportToOwner();
                    this.AddToScene( this.owner.Scene );
                }
                else
                {
                    if( this.Scene != null )
                    {
                        this.RemoveFromScene();
                    }
                }

                this.IsEnabledChanged.Raise( this );
            }
        }

        public Moveable Moveable
        {
            get
            {
                return this.moveable;
            }
        }

        public PlayerEntity Owner
        {
            get
            {
                return this.owner;
            }
        }

        public bool IsLightOnly
        {
            get
            {
                return false;
            }
        }

        private new OneDirAnimDrawDataAndStrategy DrawDataAndStrategy
        {
            get
            {
                return (OneDirAnimDrawDataAndStrategy)base.DrawDataAndStrategy;
            }
        }

        public Fairy( PlayerEntity owner )
            : base( 5 )
        {
            this.owner = owner;
            this.FloorRelativity = EntityFloorRelativity.IsAbove;
            this.moveable.Speed = DefaultMovementSpeed;
            this.moveable.SlideOffset = 2.0f;
            this.moveable.CanSwim = true;
            this.moveable.TileHandler = FlyingTileHandler.Instance;

            this.Collision.IsSolid = false;
            this.Collision.Set( new Vector2( 3, 2 ), new Vector2( 5, 5 ) );

            base.DrawDataAndStrategy = new OneDirAnimDrawDataAndStrategy( this ) {
                SpriteGroup = "Fairy_Gray"
            };

            this.Components.BeginSetup();
            {
                this.Components.Add( this.moveable );
                this.Components.Add( this.behaveable );
            }
            this.Components.EndSetup();

            // Setup light
            this.Transform.AddChild( this.light.Transform );

            this.light.Transform.InheritsScale = false;
            this.light.Transform.RelativePosition = new Vector2( 6, 4 );
            this.light.Color = new Microsoft.Xna.Framework.Color( 255, 255, 255, 100 );
            this.light.Components.Add( new ScaleCurveEntityModifier() );

            this.Added += this.OnFairyAdded;
            this.Removed += this.OnFairyRemoved;
            this.owner.Spawnable.Spawned += this.OnOwnerSpawned;
            this.owner.Latern.RadiusRefreshed += this.OnLaternRadiusRefreshed;
        }

        private void OnLaternRadiusRefreshed( Latern latern )
        {
            float extraScale = latern.ExtraLightRadius / 60.0f;

            var scaleModifier = light.Components.Get<ScaleCurveEntityModifier>();
            scaleModifier.ClearKeys();
            scaleModifier.AddScaleKey( 0.0f, extraScale + 1.0f );
            scaleModifier.AddScaleKey( 3.0f, extraScale + 1.2f );
            scaleModifier.AddScaleKey( 6.0f, extraScale + 1.0f );
            scaleModifier.AddScaleKey( 8.0f, extraScale + 0.9f );
            scaleModifier.AddScaleKey( 10.0f, extraScale + 1.0f );
        }

        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.DrawDataAndStrategy.Load( serviceProvider );

            light.Sprite = serviceProvider.SpriteLoader.LoadSprite( "Light_Circle2_80px" );
            this.behaveable.Behaviour = new FairyMovementBehaviour( this, serviceProvider );
        }

        public void TeleportToOwner()
        {
            this.Transform.Position = this.owner.Collision.Center - this.Collision.Size / 2;
            this.FloorNumber = this.owner.FloorNumber;
        }

        public void Toggle()
        {
            if( owner.IsDead )
                return;

            this.IsEnabled = !this.isEnabled;
        }

        private void OnFairyRemoved( object sender, ZeldaScene e )
        {
            this.light.RemoveFromScene();
        }

        private void OnFairyAdded( object sender, ZeldaScene e )
        {
            this.light.AddToScene( e );
        }

        public override void Update( ZeldaUpdateContext updateContext )
        {
            base.Update( updateContext );

            var strategy = this.DrawDataAndStrategy;
            light.Transform.RelativePosition = new Vector2( 6, 4 );
        }

        public override void Draw( ZeldaDrawContext drawContext )
        {
            if( !this.IsVisible )
                return;

            if( this.DrawDataAndStrategy != null )
                this.DrawDataAndStrategy.Draw( tintColor, drawContext );
        }

        private void OnOwnerSpawned( Spawnable sender, Spawning.ISpawnPoint e )
        {
            if( this.isEnabled )
            {
                e.Spawn( this );
                TeleportToOwner();

                this.behaveable.Reset();
            }
        }

        public void DrawLight( ZeldaDrawContext drawContext )
        {
            this.DrawDataAndStrategy.Draw( lightColor, drawContext );
        }

        internal void RefreshColor()
        {
            Vector4 tintColor = new Vector4( 1.0f, 1.0f, 1.0f, 1.0f );

            List<StatusValueEffect> list;
            if( this.Owner.Statable.AuraList.GetEffects( ColorEffect.IdentifierString, out list ) )
            {
                foreach( ColorEffect effect in list )
                {
                    if( effect.Target == ColorEffect.ColorTarget.Fairy )
                    {
                        tintColor *= effect.Color;
                    }
                }
            }

            this.lightColor = new Microsoft.Xna.Framework.Color( tintColor.X, tintColor.Y, tintColor.Z, tintColor.W * 0.3f );
            this.tintColor = new Microsoft.Xna.Framework.Color( tintColor.X, tintColor.Y, tintColor.Z, tintColor.W );
        }

        private bool isEnabled = true;

        private Microsoft.Xna.Framework.Color tintColor = new Microsoft.Xna.Framework.Color( 255, 255, 255, 255 );
        private Microsoft.Xna.Framework.Color lightColor = new Microsoft.Xna.Framework.Color( 255, 255, 255, 100 );
        private readonly PlayerEntity owner;
        private readonly Light light = new Light();
        private readonly Moveable moveable = new Moveable();
        private readonly Behaveable behaveable = new Behaveable();
    }
}
