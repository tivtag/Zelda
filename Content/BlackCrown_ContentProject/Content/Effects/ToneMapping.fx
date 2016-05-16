// <copyright file="Luminance.fx">
//     Copyright (c) federrot Software & Ziggyware. All rights reserved.
// </copyright>
// <summary>
// Implements the Tone Mapping pass of the High Dynamic Range effect.
// </summary>
// <author>
// Initial: Mahdi Khodadadi Fard( mahdi3466@yahoo.com)
// Changes: Paul Ennemoser
// </author>

//
// Parameters
//

float Exposure = 1.0;


//
// Textures
//

texture2D SourceTexture;
sampler2D SourceTextureSampler = sampler_state
{
    Texture = <SourceTexture>;
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
    MaxAnisotropy = 1;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

texture2D LuminanceTexture;
sampler2D LuminanceTextureSampler = sampler_state
{
    Texture = <LuminanceTexture>;
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
    MaxAnisotropy = 1;
    AddressU = CLAMP;
    AddressV = CLAMP;
};


//
// Vertex Shader
//

struct VS_OUTPUT
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT Tonemap_VS(float4 Position : POSITION, float2 TexCoord : TEXCOORD0)
{
	VS_OUTPUT OUT;
	
	OUT.Position = Position;
	OUT.TexCoord = TexCoord;
	
	return OUT;
}


//
// Pixel Shader
//

float4 Tonemap_PS(float2 texcoord  : TEXCOORD0) : COLOR0
{
	float4 final	= tex2D(SourceTextureSampler, texcoord);
	float4 l		= tex2D(LuminanceTextureSampler, float2(0.5f, 0.5f));
	
	
	float Lp = (Exposure / l.r) * max(final.r, max(final.g, final.b));
	float LmSqr = (l.g * l.g) * (l.g * l.g);
	float toneScalar = ( Lp * ( 1.0f + ( Lp / ( LmSqr ) ) ) ) / ( 1.0f + Lp );

	final.rgb *= toneScalar;

	return final;
}

technique ToneMapping
{
	pass Pass0 
	{

		VertexShader = compile vs_1_1 Tonemap_VS();
		PixelShader = compile ps_2_0 Tonemap_PS();
	}
}