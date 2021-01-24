// <copyright file="Latern.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Latern class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using Atom;
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Status;
    using Zelda.Status.Auras;

    /// <summary>
    /// Represents a latern that emits light.
    /// </summary>
    public sealed class Latern
    {
        public event SimpleEventHandler<Latern> RadiusRefreshed;
        public event SimpleEventHandler<Latern> IsToggledChanged;

        /// <summary>
        /// The default scaling factor of the Light emitted by this Latern.
        /// </summary>
        private static readonly Vector2 DefaultLightScale = new Vector2( 3.6f, 3.5f );

        /// <summary>
        /// Gets or sets a value indicating whether this Latern
        /// is currently toggled on and as such visible.
        /// </summary>
        public bool IsToggled
        {
            get
            {
                return this.light.IsVisible;
            }

            set
            {
                if( value )
                {
                    this.light.IsVisible = true;
                    this.light.Transform.UpdateTransform();
                    this.player.Statable.AuraList.Add( this.manaDrainAura );
                }
                else
                {
                    this.light.IsVisible = false;
                    this.player.Statable.AuraList.Remove( this.manaDrainAura );
                }

                if( this.player.Scene != null )
                {
                    if( this.light.Scene == null )
                    {
                        this.light.AddToScene( this.player.Scene );
                    }
                    else
                    {
                        this.player.Scene.NotifyVisabilityUpdateNeeded();
                    }
                }

                this.IsToggledChanged.Raise( this );
            }
        }

        public float ExtraLightRadius
        {
            get 
            {
                return this.extraLightRadius; 
            }
        }

        /// <summary>
        /// Initializes a new instance of the Latern class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new Latern.
        /// </param>
        internal Latern( PlayerEntity player )
        {
            this.player = player;
            this.light = CreateLight();
            this.manaDrainAura = CreateManaDrainAura( player );

            player.Added += this.OnPlayerAddedToScene;
            player.Removed += this.OnPlayerRemovedFromScene;
            player.Transform.AddChild( this.light.Transform );
        }

        /// <summary>
        /// Loads the content required by this PlayerLatern.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal void LoadContent( IZeldaServiceProvider serviceProvider )
        {
            this.light.Sprite = serviceProvider.SpriteLoader.LoadSprite( "Light_Circle2_80px" );
        }

        /// <summary>
        /// Creates the Light entity shown when the 
        /// player has enabled the latern.
        /// </summary>
        /// <returns>
        /// A newly created Light instance.
        /// </returns>
        private static Light CreateLight()
        {
            var light = new Light() {
                Name      = "Light_Latern",
                Color     = new Microsoft.Xna.Framework.Color( 255, 215, 215, 200 ),
                IsVisible = false
            };

            light.Transform.InheritsScale = false;
            light.Transform.Scale            = DefaultLightScale;
            light.Transform.RelativePosition = new Vector2( 8.0f, -4.0f );

            return light;
        }

        /// <summary>
        /// Creates the Aura that is enabled when the 
        /// specified PlayerEntity has this PlayerLatern toggled on.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity whose mana is drained.
        /// </param>
        /// <returns>
        /// A newly created Aura.
        /// </returns>
        private static PermanentDamageOverTimeAura CreateManaDrainAura( PlayerEntity player )
        {
            return new PermanentDamageOverTimeAura( player ) {
                Name           = "LaternManaDrain",
                PowerType      = LifeMana.Mana,
                ManipType      = StatusManipType.Percental,
                DamageEachTick = 10,
                TickTime       = 10.0f
            };
        }

        /// <summary>
        /// Gets called when the PlayerEntity that owns this Latern has
        /// been added to a ZeldaScene.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        private void OnPlayerAddedToScene( object sender, ZeldaScene scene )
        {
            this.light.AddToScene( scene );
        }

        /// <summary>
        /// Gets called when the PlayerEntity that owns this Latern has
        /// been removed from a ZeldaScene.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        private void OnPlayerRemovedFromScene( object sender, ZeldaScene scene )
        {
            if( this.light.Scene != null )
            {
                this.light.RemoveFromScene();
            }
        }

        /// <summary>
        /// Toggles this Lantern on/off.
        /// </summary>
        public void Toggle()
        {
            if( player.IsDead )
                return;

            this.IsToggled = !this.IsToggled;
        }

        /// <summary>
        /// Refreshes the size of the Light emitted by this Latern.
        /// </summary>
        /// <seealso cref="LightRadiusEffect"/>
        internal void Refresh()
        {
            this.extraLightRadius = this.player.Statable.AuraList.GetFixedEffectValue( LightRadiusEffect.IdentifierString );
            
            Vector2 newLightScale = DefaultLightScale + extraLightRadius / 50.0f;
            this.light.Transform.Scale = newLightScale;
            
            this.RadiusRefreshed.Raise( this );
        }

        private float extraLightRadius;

        /// <summary>
        /// The <see cref="Light"/> entity that is used to visualize the light
        /// this Latern emits.
        /// </summary>
        private readonly Light light;

        /// <summary>
        /// The aura that is applied when this Latern is enabled.
        /// </summary>
        private readonly PermanentAura manaDrainAura;

        /// <summary>
        /// Identifies the <see cref="PlayerEntity"/> that owns this latern.
        /// </summary>
        private readonly PlayerEntity player;
    }
}