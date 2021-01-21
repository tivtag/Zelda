// <copyright file="LightningBoltEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.Spells.Lightning.LightningBoltEntity class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Casting.Spells.Lightning
{
    using Atom;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Effects;
    using Atom.Xna.Effects.Lightning;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Entities;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a single independent LightningBolt that damages everything
    /// that crosses its path.
    /// </summary>
    public sealed class LightningBoltEntity : DamageEffectEntity, IZeldaSetupable, ILight
    {
        /// <summary>
        /// Gets or sets the jittering range on the x-axis of the start point.
        /// </summary>
        public FloatRange StartJitterX
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the jittering range on the y-axis of the start point.
        /// </summary>
        public FloatRange StartJitterY
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the jittering range on the x-axis of the end point.
        /// </summary>
        public FloatRange EndJitterX
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the jittering range on the y-axis of the end point.
        /// </summary>
        public FloatRange EndJitterY
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the start point of the LightningBolt.
        /// </summary>
        public Vector2 Start
        {
            get
            {
                return this.start;
            }
        }

        /// <summary>
        /// Gets the end point of the LightningBolt.
        /// </summary>
        public Vector2 End
        {
            get
            {
                return this.end;
            }
        }
        
        /// <summary>
        /// Gets or sets the width of the collision area of the LightningBolt.
        /// </summary>
        public float Width
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the LightningSettings instance that allows to change the settings of the LightningBolt.
        /// </summary>
        public LightningSettings Settings
        {
            get
            {
                return this.settings;
            }

            set
            {
                this.settings = value;

                if( this.bolt != null )
                {
                    this.bolt.Settings = value;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the lightning effect
        /// of the bolt is enabled.
        /// </summary>
        public bool IsLightningEnabled 
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether only the DrawLight method of this ILight is called
        /// during the light drawing pass;
        /// -or- also the Draw method during the normal drawing pass.
        /// </summary>
        public bool IsLightOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the LightningBoltEntity class.
        /// </summary>
        public LightningBoltEntity()
        {
            this.FloorRelativity = EntityFloorRelativity.IsBelow;
            this.MeleeAttack.Limiter = new Attacks.Limiter.TimedAttackLimiter( 0.75f );
        }

        /// <summary>
        /// Sets the start and end position of the LightningBolt.
        /// </summary>
        /// <param name="start">
        /// The start position.
        /// </param>
        /// <param name="end">
        /// The end. position.
        /// </param>
        public void SetLocation( Vector2 start, Vector2 end )
        {
            this.start = start;
            this.start.Y = -this.start.Y;
            this.end = end;
            this.end.Y = -this.end.Y;

            this.area = OrientedRectangleF.FromLine( new FastLineSegment2( start, end ), this.Width );

            var rect = RectangleF.FromOrientedRectangle( this.area );
            this.Collision.Size = rect.Size;
            this.Transform.Position = rect.Position;

            this.texture = null;
            this.lightTexture = null;
        }
        
        /// <summary>
        /// Setups this LightningBoltEntity.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            IEffectLoader effectLoader = serviceProvider.GetService<IEffectLoader>();
            IGraphicsDeviceService deviceService = serviceProvider.GetService<IGraphicsDeviceService>();
            IRenderTarget2DFactory renderTargetFactory = serviceProvider.GetService<IRenderTarget2DFactory>();

            GraphicsDevice device = deviceService.GraphicsDevice;
            this.rand = serviceProvider.Rand;
            this.bolt = new LightningBolt( Vector3.Zero, Vector3.Zero, this.settings, null, serviceProvider.Rand );

            this.bolt.LoadContent(
                effectLoader,
                renderTargetFactory,
                device
            );

            this.offscreenTarget = renderTargetFactory.Create();
        }

        /// <summary>
        /// Setups the post-processing effects required for the light pass.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void SetupPostProcess( IZeldaServiceProvider serviceProvider )
        {
            IEffectLoader effectLoader = serviceProvider.GetService<IEffectLoader>();
            IGraphicsDeviceService deviceService = serviceProvider.GetService<IGraphicsDeviceService>();

            //this.glow = new Atom.Xna.Effects.PostProcess.Glow( effectLoader, renderTargetFactory, deviceService );
            //this.glow.LoadContent();

            this.blur = new Atom.Xna.Effects.PostProcess.GaussianBlur9x9( effectLoader, deviceService );
            this.blur.StandardDeviation = 0.1f;
            this.blur.LoadContent();
        }

        /// <summary>
        /// Updates the position at which the LightningBolt is rendered.
        /// </summary>
        private void UpdateBoltPositions()
        {
            Vector2 scroll = this.Scene.Camera.Scroll;
            var size = new Point2( 360, 240 );

            this.bolt.Source = new Vector3(
                this.start.X + this.StartJitterX.GetRandomValue( this.rand ) - scroll.X - size.X / 2,
                this.start.Y + this.StartJitterY.GetRandomValue( this.rand ) + scroll.Y + size.Y / 2,
                0.0f );

            this.bolt.Destination = new Vector3(
                this.end.X + this.EndJitterX.GetRandomValue( this.rand ) - scroll.X - size.X / 2,
                this.end.Y + this.EndJitterY.GetRandomValue( this.rand ) + scroll.Y + size.Y / 2,
                0.0f );
        }

        /// <summary>
        /// Resets this LightningBoltEntity.
        /// </summary>
        public void Reset()
        {
            this.texture = null;
            this.lightTexture = null;
        }

        /// <summary>
        /// Draws this LightningBoltEntity.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public override void Draw( ZeldaDrawContext drawContext )
        {
            if( !this.IsVisible )
            {
                return;
            }

            Vector2 delta = (this.area.LowerLeft - this.area.UpperRight);
            float deltaLength = delta.Length;
            
            if( this.texture != null && deltaLength > 21.0f )
            {
                const float BlendInEnd = 32.0f;
                float alpha = deltaLength <= BlendInEnd ? (1.0f - ((BlendInEnd - deltaLength) / BlendInEnd)) : 1.0f;

                Vector2 scroll = drawContext.Camera.Scroll;

                drawContext.Batch.Draw(
                    texture,
                    new Microsoft.Xna.Framework.Rectangle( (int)scroll.X, (int)scroll.Y, texture.Width, texture.Height ),
                    Xna.Color.White.WithAlpha( (byte)(alpha * byte.MaxValue) )
                );
            }

            if( !this.bolt.IsSupported )
            {
                drawContext.Batch.DrawLineRect(
                    this.area,
                    Xna.Color.White.WithAlpha( 50 )
                );
            }
        }

        /// <summary>
        /// Draws this Light. This method is called during the "Light-Drawing-Pass".
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void DrawLight( ZeldaDrawContext drawContext )
        {
            if( !this.IsVisible || !this.IsLightningEnabled || this.lightTexture == null )
            {
                return;
            }

            Atom.Xna.Batches.IComposedSpriteBatch batch = drawContext.Batch;
            Vector2 scroll = drawContext.Camera.Scroll;

            drawContext.Batch.Draw(
                this.lightTexture,
                new Microsoft.Xna.Framework.Rectangle( (int)scroll.X, (int)scroll.Y, texture.Width, texture.Height ),
                Xna.Color.White.WithAlpha( 100 )
            );
        }

        /// <summary>
        /// Updates this LightningBoltEntity.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.UpdateBoltPositions();
            this.bolt.Update( updateContext );
            base.Update( updateContext );
        }

        /// <summary>
        /// Called before drawing anything is drawn.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public override void PreDraw( ZeldaDrawContext drawContext )
        {
            if( this.IsVisible && this.bolt.IsSupported )
            {
                this.UpdateBoltPositions();

                this.bolt.GenerateTexture( drawContext );
                this.texture = this.bolt.ResolveTexture();

                if( this.IsLightningEnabled && this.blur != null )
                {
                    GraphicsDevice device = drawContext.Device;
                    RenderTarget2D oldRenderTarget = device.GetRenderTarget2D();
                    blur.PostProcess( this.texture, this.offscreenTarget, drawContext );
                    device.SetRenderTarget( oldRenderTarget );

                    this.lightTexture = this.offscreenTarget;
                }
            }
        }

        /// <summary>
        /// Tests for collesion against enemy objects.
        /// </summary>
        protected override void TestCollision()
        {
            if( !this.MeleeAttack.IsReady || this.Creator == null )
            {
                return;
            }

            RectangleF rectangle = this.Collision.Rectangle;
            foreach( ZeldaEntity target in this.Scene.VisibleEntities )
            {
                if( this.FloorNumber == target.FloorNumber )
                {
                    if( target.Collision.Intersects( ref rectangle ) )
                    {
                        if( this.area.Intersects( target.Collision.Rectangle ) )
                        {
                            this.TryAttack( target );
                        }
                    }
                }
            }
        }

        private Atom.Xna.Effects.PostProcess.GaussianBlur9x9 blur;
        private RenderTarget2D offscreenTarget;
        private Texture2D lightTexture;

        /// <summary>
        /// The actual bolt effect that drives everything.
        /// </summary>
        private LightningBolt bolt;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private IRand rand;

        /// <summary>
        /// Captures the current source position of the LightningBolt.
        /// </summary>
        private Vector2 start;

        /// <summary>
        /// Captures the current destionation position of the LightningBolt.
        /// </summary>
        private Vector2 end;

        /// <summary>
        /// The oriented collision area the bolt takes up.
        /// </summary>
        private OrientedRectangleF area;

        /// <summary>
        /// Stores the Lightning that has been rendered.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// The LightningSettings the LightningBolt uses.
        /// </summary>
        private LightningSettings settings;
    }
}
