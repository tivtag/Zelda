
namespace Zelda
{
    using System.Collections.Generic;
    using System.Linq;
    using Atom;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Zelda.Entities;
    using Zelda.UI;
    using Xna = Microsoft.Xna.Framework;

    public sealed class BubbleTextManager : IZeldaUpdateable
    {
        private static readonly Xna.Color ColorFont = Xna.Color.White;

        public int ActiveTextCount
        {
            get
            {
                return this.texts.Count;
            }
        }

        public void LoadContent()
        {
            this.splitter = new TextBlockSplitter( UIFonts.Tahoma10, 120.0f );
        }

        public void ShowText( string text, float duration, ZeldaEntity speaker )
        {
            if( speaker == null )
                return;

            BubbleText bubbleText = this.FindTextOf( speaker );

            if( bubbleText == null )
            {
                bubbleText = new BubbleText() {
                    Entity = speaker
                };

                this.texts.Add( bubbleText );
                this.SetupText( text, duration, bubbleText );
            }
            else
            {
                bubbleText.ForceBlendOut();
                bubbleText.Ended = (sender, e) => this.ShowText( text, duration, speaker );
            }
        }

        private void SetupText( string text, float duration, BubbleText bubbleText )
        {
            bubbleText.Text = new Text( UIFonts.Tahoma7, TextAlign.Center, ColorFont, this.splitter ) {
                TextString = text
            };
            bubbleText.SetDuration( duration );
        }

        public BubbleText FindTextOf( ZeldaEntity speaker )
        {
            return this.texts.FirstOrDefault( t => t.Entity == speaker );
        }

        public void Update( ZeldaUpdateContext updateContext )
        {
            for( int i = 0; i < this.texts.Count; ++i )
            {
                BubbleText text = this.texts[i];
                text.TimeLeft -= updateContext.FrameTime;

                if( text.TimeLeft <= 0.0f || text.Entity.Scene == null )
                {
                    this.texts.RemoveAt( i );
                    text.Ended.Raise( text );
                    --i;
                }
                else
                {
                    text.Text.Update( updateContext );
                }
            }
        }

        public void Draw( ZeldaDrawContext drawContext )
        {
            for( int i = 0; i < this.texts.Count; ++i )
            {
                this.texts[i].Draw( drawContext );
            }
        }

        public void Clear()
        {
            this.texts.Clear();
        }

        private readonly List<BubbleText> texts = new List<BubbleText>();
        private ITextBlockSplitter splitter;
    }
}
