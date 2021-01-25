//======================================================================
//
//	HDRSample
//
//		by MJP
//		09/20/08
//
//======================================================================
//
//	File:		pp_Blur.fx
//
//	Desc:		Implements several variants of post-processing blur
//				techiques.
//
//======================================================================

#include "Common.fxh"
#include "LogLuv.fxh"

float g_fSigma = 0.5f;

float CalcGaussianWeight( int iSamplePoint )
{
    float g = 1.0f / sqrt(2.0f * 3.14159 * g_fSigma * g_fSigma);  
    return (g * exp(-(iSamplePoint * iSamplePoint) / (2 * g_fSigma * g_fSigma)));
}

float4 HGaussianBlurShader( in float2 in_vTexCoord : TEXCOORD0,
                            uniform int iRadius,
                            uniform int bEncodeLogLuv ) : COLOR0
{
    float4 vColor = 0;
    float2 vTexCoord = in_vTexCoord;

    for( int i = -iRadius; i < iRadius; i++ )
    {
        float fWeight = CalcGaussianWeight(i);
        vTexCoord.x = in_vTexCoord.x + (i / g_vSourceDimensions.x);

        float4 vSample = tex2D(PointSampler0, vTexCoord);
        if( bEncodeLogLuv )
        {
            vSample = float4(LogLuvDecode(vSample), 1.0f);
        }

        vColor += vSample * fWeight;
    }

    if( bEncodeLogLuv )
    {
        vColor = LogLuvEncode(vColor.rgb);
    }

    return vColor;
}

float4 VGaussianBlurShader( in float2 in_vTexCoord : TEXCOORD0,
                            uniform int iRadius,
                            uniform int bEncodeLogLuv ) : COLOR0
{
    float4 vColor = 0;
    float2 vTexCoord = in_vTexCoord;

    for( int i = -iRadius; i < iRadius; i++ )
    {
        float fWeight = CalcGaussianWeight(i);
        vTexCoord.y = in_vTexCoord.y + (i / g_vSourceDimensions.y);
        float4 vSample = tex2D(PointSampler0, vTexCoord);

        if( bEncodeLogLuv )
        {
            vSample = float4(LogLuvDecode(vSample), 1.0f);
        }

        vColor += vSample * fWeight;
    }

    return vColor;
}

technique GaussianBlurH
{
    pass p0
    {
        VertexShader = compile vs_4_0 PostProcessVS();
        PixelShader = compile ps_4_0 HGaussianBlurShader(6, false);
        
        ZEnable = false;
        ZWriteEnable = false;
        StencilEnable = false;
        AlphaBlendEnable = false;
    }
}

technique GaussianBlurV
{
    pass p0
    {
        VertexShader = compile vs_4_0 PostProcessVS();
        PixelShader = compile ps_4_0 VGaussianBlurShader(6, false);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        StencilEnable = false;
    }
}

technique GaussianBlurEncodeH
{
    pass p0
    {
        VertexShader = compile vs_4_0 PostProcessVS();
        PixelShader = compile ps_4_0 HGaussianBlurShader(6, true);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        StencilEnable = false;
    }
}

technique GaussianBlurEncodeV
{
    pass p0
    {
        VertexShader = compile vs_4_0 PostProcessVS();
        PixelShader = compile ps_4_0 VGaussianBlurShader(6, true);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        StencilEnable = false;
    }
}
