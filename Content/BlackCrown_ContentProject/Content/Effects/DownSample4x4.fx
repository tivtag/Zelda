// <copyright file="DownSample4x4.fx">
//     Copyright (c) federrot Software & Ziggyware. All rights reserved.
// </copyright>
// <summary>
// Implements the Down Sample 4x4 pass of the High Dynamic Range effect.
// </summary>
// <author>
// Initial: Mahdi Khodadadi Fard (mahdi3466@yahoo.com)
// Changes: Paul Ennemoser
// </author>

float2 SourceSize = 1;
float2 TargetSize;
static const float Offsets[4] = {-1.5f, -0.5f, 0.5f, 1.5f};


struct VS_OUTPUT
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT Common_VS(float4 Position : POSITION, float2 TexCoord : TEXCOORD0)
{
	VS_OUTPUT OUT;
	OUT.Position = Position;
	OUT.TexCoord = TexCoord + (0.5f / TargetSize);
	
	return OUT;
}


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


float4 DownScale4Software_PS(float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 sum = 0;
	
	for (int x = 0; x < 4; x++)
	{
		for (int y = 0; y < 4; y++)
		{
			float2 vOffset = float2(Offsets[x], Offsets[y]) / SourceSize;
			sum += tex2D(SourceTextureSampler, texCoord + vOffset);
		}
	}

	sum /= 16.0f;	
	return sum;
}

technique DownScale4Software
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 Common_VS();
        PixelShader = compile ps_2_0 DownScale4Software_PS();
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        // AlphaTestEnable = false;
        StencilEnable = false;
    }
}