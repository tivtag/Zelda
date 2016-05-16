// LightCompositor.fx
// This effects combines the FullBrightScene RenderTarget and the Light RenderTarget.

uniform extern float4x4 WorldViewProj : WORLDVIEWPROJECTION;
float4 color;
texture tex0;
texture tex1;

// Note: since we're doing 1:1 mapping, we may as
// well use point sampling instead of bilinear,
// to try to save some texture bandwidth.
sampler samp0 = sampler_state
{ 
    Texture   = (tex0);
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
};

sampler samp1 = sampler_state
{ 
    Texture   = (tex1);
    MipFilter = None;
    MinFilter = Point;
    MagFilter = Point;
};

struct VS_OUTPUT
{
    float4 position : POSITION;
    float2 tex : TEXCOORD0;
};

VS_OUTPUT ComposeVertexShader(float4 Pos  : POSITION,  float2 Tex : TEXCOORD0)
{
    VS_OUTPUT Out = (VS_OUTPUT)0;

	Out.position = mul(Pos,WorldViewProj);
	Out.tex = Tex;

    return Out;
}


float4 ComposePixelShader(float2 tex : TEXCOORD0) : COLOR0
{
	float4 sample0 = tex2D(samp0, float2(tex.x, tex.y));
	float4 sample1 = tex2D(samp1, float2(tex.x, tex.y));
	
	float4 finalColor = sample0 * sample1;
	finalColor.a = 1;
	return finalColor;
}


float4 FullBrightPixelShader( float2 tex : TEXCOORD0 ) : COLOR0
{
	float4 sample0 = tex2D(samp0, float2(tex.x, tex.y));
	
	float4 finalColor = sample0;
	finalColor.a = 1;
	return finalColor;
}


float4 LightOnlyPixelShader( float2 tex : TEXCOORD0 ) : COLOR0
{
	float4 sample1 = tex2D(samp1, float2(tex.x, tex.y));
	
	float4 finalColor = sample1;
	finalColor.a = 1;
	return finalColor;
}


// This technique combines the bright scene, with the lights
technique Normal
{
    pass Pass1
    {
		AlphaBlendEnable = true;
		SrcBlend         = SrcAlpha;
		DestBlend        = InvSrcAlpha;
        vertexShader     = compile vs_1_1 ComposeVertexShader();
        pixelShader      = compile ps_2_0 ComposePixelShader();
    }
}

// This technique renders the full scene, without any lights
technique FullBright
{
    pass Pass1
    {
		AlphaBlendEnable = true;
		SrcBlend         = SrcAlpha;
		DestBlend        = InvSrcAlpha;
        vertexShader     = compile vs_1_1 ComposeVertexShader();
        pixelShader      = compile ps_2_0 FullBrightPixelShader();
    }
}

// This technique only renders the Lights
technique LightOnly
{
    pass Pass1
    {
		AlphaBlendEnable = true;
		SrcBlend         = SrcAlpha;
		DestBlend        = InvSrcAlpha;
        vertexShader     = compile vs_1_1 ComposeVertexShader();
        pixelShader      = compile ps_2_0 LightOnlyPixelShader();
    }
}
