// <copyright file="Luminance.fx">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
// Implements a Luminance effect.
// </summary>
// <author>
// Initial: Mahdi Khodadadi Fard
// Changes: Paul Ennemoser
// </author>

//
// Parameters
//

float2 SourceSize = 1;
float2 TargetSize;
int ShaderIndex = 0;


//
// Constants
//

static const float Offsets2x2[2] = {-0.5f, 0.5f};
static const float Offsets3x3[3] = {-0.5f, 0.0f, 0.5f};
static const float3 LUMINANCE = float3(0.299f, 0.587f, 0.114f);


//
// Vertex Shader
//

struct VS_OUTPUT
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT Luminance_VS(float4 Position : POSITION, float2 TexCoord : TEXCOORD0)
{
    VS_OUTPUT OUT;
    
    OUT.Position = Position;
    OUT.TexCoord = TexCoord + (0.5f / TargetSize);
    
    return OUT;
}


//
// Pixel Shader
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

float4 Luminance2x2_PS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float average = 0.0f;
    float maximum = -1e20;
    float4 color = 0.0f;
    
    for (int x = 0; x < 2; x++)
    {
        for (int y = 0; y < 2; y++)
        {
            float2 vOffset = float2(Offsets2x2[x], Offsets2x2[y])
                / SourceSize;
            color = tex2D(SourceTextureSampler, texCoord + vOffset);
            
            float GreyValue = dot(color.rgb, LUMINANCE);
            
            
            maximum = max(maximum, GreyValue);
            average += log(1e-5 + GreyValue) / 4.0f;
        }
    }
    
    average = exp(average);
    return float4(average, maximum, 0.0f, 1.0f);
}

float4 Luminance3x3_PS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float average = 0.0f;
    float maximum = -1e20;
    float4 color = 0.0f;
    
    for (int x = 0; x < 3; x++)
    {
        for (int y = 0; y < 3; y++)
        {
            float2 vOffset = float2(Offsets3x3[x], Offsets3x3[y])
                / SourceSize;
            color = tex2D(SourceTextureSampler, texCoord + vOffset);
                        
            float GreyValue = dot(color.rgb, LUMINANCE);            
            maximum = max(maximum, GreyValue);
            average += log(1e-5 + GreyValue) / 4.0f;            
            
          //  average += color.r;
           // maximum = max( maximum, color.g );
        }
    }
    
    average /= 9.0f;
    return float4( average, maximum, 0.0f, 1.0f );
}

technique Luminance2x2
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 Luminance_VS();
        PixelShader =  compile ps_4_0 Luminance2x2_PS();
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        //AlphaTestEnable = false;
        StencilEnable = false;
    }
}

technique Luminance3x3
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 Luminance_VS();
        PixelShader =  compile ps_4_0 Luminance3x3_PS();
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        //AlphaTestEnable = false;
        StencilEnable = false;
    }
}
