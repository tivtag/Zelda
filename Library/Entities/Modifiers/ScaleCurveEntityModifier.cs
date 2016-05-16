
namespace Zelda.Entities.Modifiers
{
    using Atom;
    using Atom.Math;
    using Zelda.Entities.Components;
    using Zelda.Saving;

    /// <summary>
    /// Scales an entity based on a curve over time.
    /// The scale on x and y-axis have the same value.
    /// </summary>
    public sealed class ScaleCurveEntityModifier : ZeldaComponent, IAttachedEntityModifier
    {
        public Curve Curve
        {
            get
            {
                return curve;
            }
        }

        public ScaleCurveEntityModifier()
        {
            curve.PostLoop = CurveLoopType.Cycle;
            curve.PreLoop = CurveLoopType.Cycle;
        }

        public void AddScaleKey( float position, float scaleValue )
        {
            curve.Keys.Add( new CurveKey( position, scaleValue ) );
        }

        public void ClearKeys()
        {
            curve.Keys.Clear();
        }

        public override void Update( IUpdateContext updateContext )
        {
            loopTime += updateContext.FrameTime;

            // Hack to prevent floating point overflow corruption
            if( loopTime > 1000000.0f )
            {
                loopTime = 0.0f;
            }

            float scale = curve.Evaluate( loopTime );
            this.Owner.Transform.Scale = new Vector2( scale, scale );
        }
        
        public void Serialize( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
            context.Write( this.curve );
        }

        public void Deserialize( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
            this.curve = context.ReadCurve();
        }

        private float loopTime;
        private Curve curve = new Curve();
    }
}
