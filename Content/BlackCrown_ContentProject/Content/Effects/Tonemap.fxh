//=========================================================================
//
//	HDRSample
//
//		by MJP
//		09/20/08
//
//=========================================================================
//
//	File:		pp_Tonemap.fx
//
//	Desc:		Contains tone mapping routines used during post-processing.
//				The operator is derived from "Photographic Tone 
//				Reproduction for Digital Images" by Eric Reinhard.
//
//=========================================================================

float g_fMiddleGrey = 0.6f;
float g_fMaxLuminance = 16.0f;

static const float3 LUM_CONVERT = float3(0.299f, 0.587f, 0.114f);

float3 DoToneMap(float3 vColor)
{
	// Get the calculated average luminance 
	float fLumAvg = tex2D(PointSampler1, float2(0.5f, 0.5f)).r;	
	
	// Calculate the luminance of the current pixel
	float fLumPixel = dot(vColor, LUM_CONVERT);	
	
	// Apply the modified operator (Eq. 4)
	float fLumScaled = (fLumPixel * g_fMiddleGrey) / fLumAvg;	
	float fLumCompressed = (fLumScaled * (1 + (fLumScaled / (g_fMaxLuminance * g_fMaxLuminance)))) / (1 + fLumScaled);
	return fLumCompressed * vColor;
}