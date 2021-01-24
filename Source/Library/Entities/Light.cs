// <copyright file="Light.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Light class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Saving;
    using Xna = Microsoft.Xna.Framework;
    
    /// <summary>
    /// A <see cref="Light"/> lightens up OR darkens the Scene.
    /// </summary>
    /// <remarks>
    /// Lights use simple white and transparent Sprites that are tinted
    /// and the drawn to a LightMap using additive color blending.
    /// The drawing of Lights is seperated from the drawing of other objects.
    /// </remarks>
    public class Light : ZeldaEntity, Zelda.Entities.ILight
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="Xna.Color"/> of the <see cref="Light"/>.
        /// </summary>
        public Xna.Color Color
        {
            get 
            {
                return this.color;
            }

            set
            {
                this.color = value; 
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ISprite"/>
        /// that is used to visualize the <see cref="Light"/>.
        /// </summary>
        public ISprite Sprite
        {
            get 
            {
                return this.sprite;
            }

            set
            {
                this.sprite = value;
                this.UpdateCollisionBoundings();
            }
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
                return true;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Light"/> class.
        /// </summary>
        public Light()
        {
            this.Collision.IsSolid = false;
        }

        #endregion

        #region [ Methods ]

        #region > Draw <

        /// <summary>
        /// Draws this Light. This method is called during the "Light-Drawing-Pass".
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public void DrawLight( ZeldaDrawContext drawContext )
        {
            if( this.sprite != null )
            {
                var transform    = this.Transform;
                var drawPosition = transform.Position;
                drawPosition.X = (int)drawPosition.X;
                drawPosition.Y = (int)drawPosition.Y;

                sprite.Draw( drawPosition, this.color, transform.Rotation, transform.Origin, transform.Scale, drawContext.Batch );
            }
        }

        /// <summary>
        /// Overwridden to do nothing.
        /// </summary>
        /// <param name="drawContext">
        /// The current IDrawContext.
        /// </param>
        public override void Draw( ZeldaDrawContext drawContext )
        {
            // no op.
        }

        #endregion

        #region > Update <

        /// <summary>
        /// Updates the <see cref="Light"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            var updateable = this.sprite as Atom.IUpdateable;

            if( updateable != null )
            {
                updateable.Update( updateContext );
            }

            base.Update( updateContext );
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Creates a clone of this <see cref="Light"/>.
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            Light light = new Light();

            SetupClone( light );

            return light;
        }

        /// <summary>
        /// Setups the given <see cref="Light"/> object
        /// to be a clone of this <see cref="Light"/>.
        /// </summary>
        /// <param name="light">
        /// The entity to clone.
        /// </param>
        protected virtual void SetupClone( Light light )
        {
            base.SetupClone( light );

            light.color  = this.color;

            // The following line will update the light's bounding rectangle.
            light.Sprite = this.sprite == null ? null : sprite.CloneInstance();
        }

        #endregion

        /// <summary>
        /// Calculates the bounding rectangle of the <see cref="Light"/>.
        /// </summary>
        private void UpdateCollisionBoundings()
        {
            int width = 0, height = 0;

            if( sprite != null )
            {
                width  = sprite.Width;
                height = sprite.Height;
            }

            this.Transform.Origin = new Vector2( width / 2.0f, height / 2.0f );
            this.Collision.Size   = new Vector2( width, height );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The <see cref="ISprite"/> of the <see cref="Light"/>.
        /// </summary>
        private ISprite sprite;

        /// <summary>
        /// The color of the Light.
        /// </summary>
        private Xna.Color color = new Xna.Color( byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue );

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="Light"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<Light>
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
            public override void Serialize( Light entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                context.WriteDefaultHeader();

                // Write Sprite and SpriteType:
                if( entity.sprite == null )
                {
                    context.Write( string.Empty );
                    context.Write( string.Empty );
                }
                else
                {
                    context.Write( entity.sprite.Name );

                    if( entity.sprite is Sprite )
                    {
                        context.Write( ".spr" );
                    }
                    else if( entity.sprite is SpriteAnimation )
                    {
                        context.Write( ".aspr" );
                    }
                    else
                    {
                        throw new NotSupportedException(
                            string.Format(
                                System.Globalization.CultureInfo.CurrentCulture,
                                Resources.Error_InvalidSpriteTypeX,
                                entity.Sprite.GetType().ToString()
                            )
                        );
                    }
                }

                // Write Name:
                context.Write( entity.Name );

                // Write transform:
                var transform = entity.Transform;

                context.Write( transform.Position );
                context.Write( transform.Scale    );
                context.Write( transform.Rotation );

                // Write entity color:
                context.Write( entity.color );

                // Write Default Visability State:
                context.Write( entity.IsVisible );
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
            public override void Deserialize( Light entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                context.ReadDefaultHeader( this.GetType() );

                // Read Sprite
                string spriteAsset = context.ReadString();
                string spriteType  = context.ReadString();

                ISprite sprite = null;

                #region Load Sprite
                try
                {
                    if( spriteAsset.Length != 0 )
                    {
                        sprite = serviceProvider.SpriteLoader.Load( spriteAsset );
                    }
                }
                catch( Exception exc )
                {
                    var logService = Atom.GlobalServices.GetService<Atom.Diagnostics.ILogProvider>();
                    if( logService != null )
                        logService.Log.WriteLine( Atom.Diagnostics.LogSeverities.Error, exc.ToString() );
                }

                #endregion

                // Read Name
                entity.Name = context.ReadString();

                // Read Transform
                Vector2 position = context.ReadVector2();
                Vector2 scale = context.ReadVector2();
                float rotation = context.ReadSingle();

                // Read entity color:
                entity.Color = context.ReadColor();
                
                // Read Default Visability State:                
                entity.IsVisible = context.ReadBoolean();

                // Set Transform:
                entity.Transform.Position  = position;
                entity.Transform.Scale     = scale;
                entity.Transform.Rotation  = rotation;
                entity.Sprite              = sprite;
            }
        }

        #endregion
    }
}
