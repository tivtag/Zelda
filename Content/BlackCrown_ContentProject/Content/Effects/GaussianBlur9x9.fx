// <copyright file="GaussianBlur9x9.fx">
//     Copyright (c) federrot Software & Ziggyware. All rights reserved.
// </copyright>
// <summary>
// Implements the Gaussian Blur 9x9 pass of the High Dynamic Range effect.
// </summary>
// <author>
// Initial: Mahdi Khodadadi Fard (mahdi3466@yahoo.com)
// Changes: Paul Ennemoser
// </author>


//
// Parameters
//

float Offsets[9];
float Weights[9];


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


//
// Vertex Shader
//

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


//
// Pixel Shader
//

float4 GaussianBlurH(float2 texCoord : TEXCOORD0) : COLOR
{
    float4 color = {0.0f, 0.0f, 0.0f, 0.0f};
    
    for(int i = 0; i < 9; i++)
    {
        color += (tex2D(SourceTextureSampler, texCoord + float2(Offsets[i], 0.0f)) * Weights[i]);
    }
        
    return float4( color.rgb, 1.0f );
}

float4 GaussianBlurV(float2 texCoord : TEXCOORD0) : COLOR
{
    float4 color = {0.0f, 0.0f, 0.0f, 0.0f};
    
    for(int i = 0; i < 9; i++)
    {
        color += (tex2D(SourceTextureSampler, texCoord + float2(0.0f, Offsets[i])) * Weights[i]);
    }
        
    return float4( color.rgb, 1.0f );
}


technique GaussianBlur9x9_H
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 Common_VS();
        PixelShader = compile ps_2_0 GaussianBlurH();
    }
}

technique GaussianBlur9x9_V
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 Common_VS();
        PixelShader = compile ps_2_0 GaussianBlurV();
    }
}

