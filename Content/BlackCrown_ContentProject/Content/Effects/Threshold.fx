//======================================================================
//
//	HDRSample
//
//		by MJP
//		09/20/08
//
//======================================================================
//
//	File:		pp_Threshold.fx
//
//	Desc:		Applies a threshold, typically used to apply bloom.
//
//======================================================================

#include "Common.fxh"
#include "LogLuv.fxh"
#include "Tonemap.fxh"


float g_fThreshold = 0.7f;


float4 ThresholdPS (	in float2 in_vTexCoord			: TEXCOORD0,
						uniform bool bEncodeLogLuv		)	: COLOR0 
{
	float4 vSample = tex2D(PointSampler0, in_vTexCoord);
		
	if (bEncodeLogLuv)
		vSample = float4(LogLuvDecode(vSample), 1.0f);
		
	vSample = float4(DoToneMap(vSample.rgb), 1.0f);
		
    vSample -= g_fThreshold;
    vSample = max(vSample, 0.0f);    
	
	if (bEncodeLogLuv)
	 	vSample = LogLuvEncode(vSample.rgb);
	
	return vSample;
}


technique Threshold
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 ThresholdPS(false);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
       // AlphaTestEnable = false;
        StencilEnable = false;
    }
}

technique ThresholdEncode
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 ThresholdPS(true);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
       // AlphaTestEnable = false;
        StencilEnable = false;
    }
}
