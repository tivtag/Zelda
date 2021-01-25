// ***************************************
// *	    EDGE DETECTION SHADER		 *
// *									 *
// *								     *
// *								     *
// ***************************************


// ------- Parameters --------
Texture Texture;
float Thickness = 2.0f;
float Threshold = 0.5f;
float2 TextureSize = float2(640, 480);

// ------- Constants --------
const float K00 = -1;
const float K01 = -2;
const float K02 = -1;
const float K10 = 0;
const float K11 = 0;
const float K12 = 0;
const float K20 = 1;
const float K21 = 2;
const float K22 = 1;


// ------- Texture Samplers --------
sampler Sampler = sampler_state {
    Texture = <Texture>;
    
    magfilter = LINEAR; 
    minfilter = LINEAR;
    mipfilter = LINEAR; 
};


// ------- Vertex Shader related --------
struct VertexShaderInputOuput
{
    float4 Position      : POSITION0;
    float2 TextureCoords : TEXCOORD;
};

VertexShaderInputOuput VertexShaderFunction(VertexShaderInputOuput input)
{
    return input;   
}


// ------- Pixel Shader related --------
float getGray( float4 color )
{
    return dot(color.rgb,((0.33333).xxx));
}

float4 PixelShaderFunction( VertexShaderInputOuput input ) : COLOR0
{
    input.TextureCoords = input.TextureCoords;
    input.TextureCoords.y = 1.0f - input.TextureCoords.y;
    
    /*
    float2 texCoord = input.TextureCoords;	
    float offsetX = 1.0 / TextureSize.x;
    float offsetY = 1.0 / TextureSize.y;	

    float4 c    = tex2D(Sampler, texCoord);
    float4 edge =
        tex2D(Sampler, texCoord + float2(-offsetX, -offsetY)) +
        tex2D(Sampler, texCoord + float2(-offsetX, 0.0)) +
        tex2D(Sampler, texCoord + float2(-offsetX, offsetY)) +
        tex2D(Sampler, texCoord + float2( 0.0, offsetY)) +
        tex2D(Sampler, texCoord + float2( offsetX, offsetY)) +
        tex2D(Sampler, texCoord + float2( offsetY, 0.0)) +
        tex2D(Sampler, texCoord + float2( offsetY, -offsetY)) +
        tex2D(Sampler, texCoord + float2( 0.0, -offsetY));
    
    //return c;
    return c - edge / 8;
    */
    
    float4 Color = tex2D(Sampler, input.TextureCoords);	
    
    float2 ox = float2(1 / TextureSize.x, 0.0);
    float2 oy = float2(0.0, 1 / TextureSize.y);
    float2 uv = input.TextureCoords.xy;            
    
    float2 PP = uv - oy;
    float g00 = getGray( tex2D(Sampler, PP - ox) );    
    float g01 = getGray( tex2D(Sampler, PP ));
    float g02 = getGray( tex2D(Sampler, PP+ox));
    
    PP = uv;
    float g10 = getGray( tex2D(Sampler, PP - ox) );
    float g11 = getGray( tex2D(Sampler, PP) );    
    float g12 = getGray( tex2D(Sampler, PP + ox) );
    
    PP = uv + oy;
    float g20 = getGray(tex2D(Sampler, PP - ox));
    float g21 = getGray(tex2D(Sampler, PP));
    float g22 = getGray(tex2D(Sampler, PP + ox));
    
    float sx = g00 * K00;
    sx += g01 * K01;
    sx += g02 * K02;
    sx += g10 * K10;
    sx += g11 * K11;
    sx += g12 * K12;
    sx += g20 * K20;
    sx += g21 * K21;
    sx += g22 * K22; 
    
    float sy = g00 * K00;
    sy += g01 * K10;
    sy += g02 * K20;
    sy += g10 * K01;
    sy += g11 * K11;
    sy += g12 * K21;
    sy += g20 * K02;
    sy += g21 * K12;
    sy += g22 * K22; 
    
    float dist = sqrt(sx*sx + sy*sy);
    float result = 1;    
    
    if( dist > Threshold ) 
    {
		result = 1.0f / dist; 
	}
	
    float4 finalColor = Color * result.xxxx;
    
    finalColor.a = 1;
    
    return finalColor;
}


technique EdgeDetection
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader  = compile ps_4_0 PixelShaderFunction();
    }
}
