// <copyright file="Luminance.fx">
//     Copyright (c) federrot Software & Ziggyware. All rights reserved.
// </copyright>
// <summary>
// Implements the Add RGB pass of the High Dynamic Range effect.
// </summary>
// <author>
// Initial: Mahdi Khodadadi Fard( mahdi3466@yahoo.com)
// Changes: Paul Ennemoser
// </author>

texture2D SourceTexture1;
texture2D SourceTexture2;

sampler2D SourceTexture1Sampler = sampler_state
{
    Texture = <SourceTexture1>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    MaxAnisotropy = 1;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

sampler2D SourceTexture2Sampler = sampler_state
{
    Texture = <SourceTexture2>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    MaxAnisotropy = 1;
    AddressU = CLAMP;
    AddressV = CLAMP;
};

struct VS_OUTPUT
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT Common_VS(float4 Position : POSITION, float2 TexCoord : TEXCOORD0)
{
	VS_OUTPUT OUT;
	OUT.Position = Position;
	OUT.TexCoord = TexCoord;
	
	return OUT;
}

float4 AddRGB_PS(float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(SourceTexture1Sampler, texCoord);
	color.rgb += tex2D(SourceTexture2Sampler, texCoord).rgb;
	
	return color;
}

technique AddRGB
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 Common_VS();
        PixelShader = compile ps_2_0 AddRGB_PS();
    }
}