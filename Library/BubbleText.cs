
namespace Zelda
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Effects;
    using Atom.Xna.UI;
    using Zelda.Entities;
    using Xna = Microsoft.Xna.Framework;
    
    public sealed class BubbleText
    {
        private const float BlendOutTime = 0.8f;
        private const float AlphaFactor = 0.65f;
        private static readonly Xna.Color ColorBackground = Xna.Color.Black;

        public RectangleF Area
        {
            get
            {
                var sizeF = this.Text.TextBlockSize;
                var size = new Point2( (int)sizeF.X + 4, (int)sizeF.Y );
                int halfWidth = (int)(size.X / 2);
                
                var position = this.Entity.Transform.Position + new Vector2( this.Entity.Collision.Offset.X + this.Entity.Collision.Width / 2.0f - halfWidth, -size.Y - 1 );
                position.X = (float)Math.Floor( position.X );

                return new RectangleF( position, size );
            }
        }

        public Vector2 TextPosition
        {
            get
            {
                var size = this.Text.TextBlockSize;
                var position = this.Entity.Transform.Position + new Vector2( this.Entity.Collision.Offset.X + this.Entity.Collision.Width / 2.0f, -size.Y - 1 );
                position.X = (float)Math.Floor( position.X );

                return position;
            }
        }

        public ZeldaEntity Entity
        {
            get;
            set;
        }

        public Text Text
        { 
            get; 
            set; 
        }

        public float TimeLeft
        { 
            get; 
            set; 
        }

        public EventHandler Ended
        {
            get;
            set;
        }

        public void Draw( ZeldaDrawContext drawContext )
        {
            drawContext.Batch.DrawRect( this.Area, this.effect.Apply( ColorBackground ).MultiplyBy( 1.0f, 1.0f, 1.0f, AlphaFactor ) );
            this.Text.Draw( this.TextPosition, drawContext );
        }

        public void SetDuration( float duration )
        {
            this.TimeLeft = duration;
            
            if( this.effect != null )
            {
                this.Text.RemoveColorEffect( effect );
            }

            this.effect = new AlphaBlendInOutColorEffect( duration, 2, duration - BlendOutTime );
            this.Text.Color = Xna.Color.White.WithAlpha( 0 );

            this.Text.AddColorEffect( effect );
        }

        public void ForceBlendOut()
        {
            if( !this.effect.IsBlendingOut )
            {
                this.effect.Time = this.effect.EndMaxAlphaTime;
                this.TimeLeft = BlendOutTime;
            }
        }

        private Atom.Xna.Effects.AlphaBlendInOutColorEffect effect;
    }
}
