// <copyright file="Bloom.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Atom.Xna.Effects.PostProcess.Bloom class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Atom.Xna.Effects.PostProcess
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Implements a non-HDR bloom post-process effect.
    /// </summary>
    public sealed class DebugBloom : ManagedDisposable, IPostProcessEffect
    {
        #region [ Enums ]

        /// <summary>
        /// Enumerates what kind of intermediate buffers are shown
        /// that make up the Bloom effect.
        /// </summary>
        public enum IntermediateBuffer
        {
            /// <summary>
            /// The buffer before any bloom effect is applied.
            /// </summary>
            PreBloom,

            /// <summary>
            /// The buffer that contains the horizontally blurred scene.
            /// </summary>
            BlurredHorizontally,

            /// <summary>
            /// The buffer that contains the horizontally and vertically blurred scene.
            /// </summary>
            BlurredBothWays,

            /// <summary>
            /// The buffer that contains the result of the Bloom effect.
            /// </summary>
            FinalResult
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets what buffers of the Bloom effect are shown.
        /// </summary>
        public IntermediateBuffer ShowBuffer
        {
            get
            {
                return this.showBuffer;
            }

            set
            {
                this.showBuffer = value;
            }
        }

        /// <summary>
        /// Gets or sets the settings used by this Bloom effect.
        /// </summary>
        public BloomSettings Settings
        {
            get
            {
                return this.settings;
            }

            set
            {
                Contract.Requires<ArgumentNullException>( value != null );

                this.settings = value;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the Bloom class.
        /// </summary>        
        /// <param name="effectLoader">
        /// Provides a mechanism that allows loading of effect assets.
        /// </param>
        /// <param name="renderTargetFactory">
        /// Provides a mechanism that allows the creation of render targets.
        /// </param>
        /// <param name="deviceService">
        /// Provides access to the <see cref="GraphicsDevice"/>.
        /// </param>
        public DebugBloom( IEffectLoader effectLoader, IRenderTarget2DFactory renderTargetFactory, IGraphicsDeviceService deviceService )
        {
            Contract.Requires<ArgumentNullException>( effectLoader != null );
            Contract.Requires<ArgumentNullException>( renderTargetFactory != null );
            Contract.Requires<ArgumentNullException>( deviceService != null );

            this.effectLoader = effectLoader;
            this.renderTargetFactory = renderTargetFactory;
            this.deviceService = deviceService;
        }

        /// <summary>
        /// Loads the content required by this Bloom effect.
        /// </summary>
        public void LoadContent()
        {
            this.device = this.deviceService.GraphicsDevice;
            this.spriteBatch = new SpriteBatch( this.device );

            this.bloomExtractEffect = this.effectLoader.Load( "BloomExtract" );
            this.bloomCombineEffect = this.effectLoader.Load( "BloomCombine" );
            this.gaussianBlurEffect = this.effectLoader.Load( "GaussianBlur" );

            this.parameterWeights = this.gaussianBlurEffect.Parameters["SampleWeights"];
            this.parameterOffsets = this.gaussianBlurEffect.Parameters["SampleOffsets"];

            this.parameterBloomThreshold = this.bloomExtractEffect.Parameters["BloomThreshold"];
            this.parameterBloomIntensity = this.bloomCombineEffect.Parameters["BloomIntensity"];
            this.parameterBaseIntensity = this.bloomCombineEffect.Parameters["BaseIntensity"];
            this.parameterBloomSaturation = this.bloomCombineEffect.Parameters["BloomSaturation"];
            this.parameterBaseSaturation = this.bloomCombineEffect.Parameters["BaseSaturation"];

            // TODO: Those can be created at half the size!
            this.renderTarget1 = this.renderTargetFactory.Create();
            this.renderTarget2 = this.renderTargetFactory.Create();
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Applies this IPostProcessEffect.
        /// </summary>
        /// <param name="sourceTexture">
        /// The texture to post-process.
        /// </param>
        /// <param name="result">
        /// The RenderTarget to which to render the result of this PostProcessEffect.
        /// </param>
        /// <param name="drawContext">
        /// The context under which the drawing operation occurrs.
        /// </param>
        public void PostProcess( Texture2D sourceTexture, RenderTarget2D result, IXnaDrawContext drawContext )
        {
            // Pass 1: draw the scene into rendertarget 1, using a
            // shader that extracts only the brightest parts of the image.
            this.device.SamplerStates[1] = SamplerState.LinearClamp;
            this.parameterBloomThreshold.SetValue( this.settings.BloomThreshold );

            this.DrawQuad( sourceTexture, this.renderTarget1, this.bloomExtractEffect, IntermediateBuffer.PreBloom );

            // Pass 2: draw from rendertarget 1 into rendertarget 2,
            // using a shader to apply a horizontal gaussian blur filter.
            this.SetBlurEffectParameters( 1.0f / (float)renderTarget1.Width, 0 );
            this.DrawQuad(
                this.renderTarget1,
                this.renderTarget2,
                this.gaussianBlurEffect,
                IntermediateBuffer.BlurredHorizontally
            );

            // Pass 3: draw from rendertarget 2 back into rendertarget 1,
            // using a shader to apply a vertical gaussian blur filter.
            this.SetBlurEffectParameters( 0, 1.0f / (float)renderTarget1.Height );
            this.DrawQuad(
                this.renderTarget2,
                this.renderTarget1,
                this.gaussianBlurEffect,
                IntermediateBuffer.BlurredBothWays
            );

            // Pass 4: draw both rendertarget 1 and the original scene
            // image back into the main backbuffer, using a shader that
            // combines them to produce the final bloomed result.
            this.device.SetRenderTarget( result );

            this.parameterBloomIntensity.SetValue( this.settings.BloomIntensity );
            this.parameterBaseIntensity.SetValue( this.settings.BaseIntensity );
            this.parameterBloomSaturation.SetValue( this.settings.BloomSaturation );
            this.parameterBaseSaturation.SetValue( this.settings.BaseSaturation );

            this.device.Textures[1] = sourceTexture;
            Viewport viewport = this.device.Viewport;

            this.DrawQuad(
                this.renderTarget1,
                viewport.Width,
                viewport.Height,
                this.bloomCombineEffect,
                IntermediateBuffer.FinalResult
            );
        }

        /// <summary>
        /// Helper for drawing a texture into a rendertarget, using
        /// a custom shader to apply postprocessing effects.
        /// </summary>
        /// <param name="texture">
        /// The texture to draw.
        /// </param>
        /// <param name="renderTarget">
        /// The target of the drawing operation.
        /// </param>
        /// <param name="effect">
        /// The effect to apply when drawing the texture.
        /// </param>
        /// <param name="currentBuffer">
        /// States what buffer currently is drawn using this DrawQuad operation.
        /// </param>
        private void DrawQuad( Texture2D texture, RenderTarget2D renderTarget, Effect effect, IntermediateBuffer currentBuffer )
        {
            this.device.SetRenderTarget( renderTarget );
            this.DrawQuad( texture, renderTarget.Width, renderTarget.Height, effect, currentBuffer );
            this.device.SetRenderTarget( null );
        }

        /// <summary>
        /// Helper for drawing a texture into the current rendertarget,
        /// using a custom shader to apply postprocessing effects.
        /// </summary>
        /// <param name="texture">
        /// The texture to draw.
        /// </param>
        /// <param name="width">
        /// The width of the quad.
        /// </param>
        /// <param name="height">
        /// The height of the quad.
        /// </param>
        /// <param name="effect">
        /// The effect to apply when drawing the texture.
        /// </param>
        /// <param name="currentBuffer">
        /// States what buffer currently is drawn using this DrawQuad operation.
        /// </param>
        private void DrawQuad( Texture2D texture, int width, int height, Effect effect, IntermediateBuffer currentBuffer )
        {
            // If the user has selected one of the show intermediate buffer options,
            // we still draw the quad to make sure the image will end up on the screen,
            // but might need to skip applying the custom pixel shader.
            if( showBuffer < currentBuffer )
            {
                effect = null;
            }

            spriteBatch.Begin( 0, BlendState.Opaque, null, null, null, effect );
            spriteBatch.Draw( texture, new Rectangle( 0, 0, width, height ), Color.White );
            spriteBatch.End();
        }

        /// <summary>
        /// Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.
        /// </summary>
        private void SetBlurEffectParameters( float dx, float dy )
        {
            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = this.parameterWeights.Elements.Count;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian( 0 );
            sampleOffsets[0] = new Vector2( 0 );

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for( int i = 0; i < sampleCount / 2; ++i )
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian( i + 1 );
                int i2 = i * 2;

                sampleWeights[i2 + 1] = weight;
                sampleWeights[i2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i2 + 1.5f;

                Vector2 delta = new Vector2( dx, dy ) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i2 + 1] = delta;
                sampleOffsets[i2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for( int i = 0; i < sampleWeights.Length; ++i )
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            this.parameterWeights.SetValue( sampleWeights );
            this.parameterOffsets.SetValue( sampleOffsets );
        }

        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        /// <param name="n">
        /// The index of the entry to calculate.
        /// </param>
        /// <returns>
        /// The gaussian value for the specified index and
        /// current blurring settings.
        /// </returns>
        private float ComputeGaussian( float n )
        {
            float theta = this.settings.BlurAmount;
            return (float)((1.0 / Math.Sqrt( 2 * Math.PI * theta )) * Math.Exp( -(n * n) / (2 * theta * theta) ));
        }

        /// <summary>
        /// Releases all unmanaged resources.
        /// </summary>
        protected override void DisposeUnmanagedResources()
        {
            this.spriteBatch.Dispose();
            this.renderTarget1.Dispose();
            this.renderTarget2.Dispose();
        }

        /// <summary>
        /// Releases all managed resources.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            this.device = null;
            this.bloomExtractEffect = null;
            this.bloomCombineEffect = null;
            this.gaussianBlurEffect = null;
            this.renderTarget1 = null;
            this.spriteBatch = null;
            this.renderTarget1 = null;
            this.renderTarget2 = null;
            this.parameterWeights = null;
            this.parameterOffsets = null;
            this.parameterBloomThreshold = null;
            this.parameterBloomIntensity = null;
            this.parameterBaseIntensity = null;
            this.parameterBloomSaturation = null;
            this.parameterBaseSaturation = null;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The SpriteBatch that is used to draw the individual buffers
        /// of this Bloom effect.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The effect that is used to extract the area to bloom.
        /// </summary>
        private Effect bloomExtractEffect;

        /// <summary>
        /// The effect that is used to combine the input scene with the extracted bloom.
        /// </summary>
        private Effect bloomCombineEffect;

        /// <summary>
        /// The effect that is used to blur the bloomed result.
        /// </summary>
        private Effect gaussianBlurEffect;

        /// <summary>
        /// The parameter that sets the blurring weights.
        /// </summary>
        private EffectParameter parameterWeights;

        /// <summary>
        /// The parameter that sets the blurring offsets.
        /// </summary>
        private EffectParameter parameterOffsets;

        /// <summary>
        /// The parameter that sets the bloom treshold of this bloom effect.
        /// </summary>
        private EffectParameter parameterBloomThreshold;

        /// <summary>
        /// The parameter that sets the bloom intensity of this bloom effect.
        /// </summary>
        private EffectParameter parameterBloomIntensity;

        /// <summary>
        /// The parameter that sets the base intensity of this bloom effect.
        /// </summary>
        private EffectParameter parameterBaseIntensity;

        /// <summary>
        /// The parameter that sets the bloom saturation of this bloom effect.
        /// </summary>
        private EffectParameter parameterBloomSaturation;

        /// <summary>
        /// The parameter that sets the base saturation of this bloom effect.
        /// </summary>
        private EffectParameter parameterBaseSaturation;

        /// <summary>
        /// The second render target that is used when rendering this Bloom effect.
        /// </summary>
        private RenderTarget2D renderTarget1;

        /// <summary>
        /// The first render target that is used when rendering this Bloom effect.
        /// </summary>
        private RenderTarget2D renderTarget2;

        /// <summary>
        /// The current settings used by this Bloom effect.
        /// </summary>
        private BloomSettings settings = BloomSettings.Default;

        /// <summary>
        /// States what buffer this Bloom effect outputs to the screen.
        /// </summary>
        private IntermediateBuffer showBuffer = IntermediateBuffer.FinalResult;

        /// <summary>
        /// The GraphicsDevice used to draw this Bloom effect.
        /// </summary>
        private GraphicsDevice device;

        /// <summary>
        /// Provides a mechanism that loads Effects.
        /// </summary>
        private readonly IEffectLoader effectLoader;

        /// <summary>
        /// Provides a mechanism that creates new RenderTargets.
        /// </summary>
        private readonly IRenderTarget2DFactory renderTargetFactory;

        /// <summary>
        /// Provides access to the Xna GraphicsDevice.
        /// </summary>
        private readonly IGraphicsDeviceService deviceService;

        #endregion
    }
}
