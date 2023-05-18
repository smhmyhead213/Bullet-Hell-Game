#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_4_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

matrix WorldViewProjection;
int uTime;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, WorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 sineWaves(in float2 uv)
{
    float thickness = 0.25;
    float squish = 30;
    float frequency = 0.523;
    float tolerance = 0.05;
    
    float sine = thickness * sin(squish * uv.y - (frequency * uTime));
    
    float positiveSine = sine + 0.5;
    float negativeSine = -sine + 0.5;
    
    float4 colour;
    float opacity;
    
    if (uv.x > positiveSine - tolerance && uv.x < positiveSine + tolerance)
    {
        float distanceFromSine = abs(positiveSine - uv.x);
        
        opacity = lerp(1, 0, distanceFromSine / tolerance);
        
        float white = 0.5 - distanceFromSine * 2;
        
        colour = float4(1, white, white, 1);
    }
    else if (uv.x > negativeSine - tolerance && uv.x < negativeSine + tolerance)
    {
        float distanceFromSine = abs(negativeSine - uv.x);
        
        opacity = lerp(1, 0, distanceFromSine / tolerance);
        
        float white = 0.5 - distanceFromSine * 2;
        
        colour = float4(1, white, white, 1);
    }
    else
    {
        opacity = 0;
        colour = float4(0, 0, 0, 0);
    }
    
    return colour * opacity;
}

float4 laser(in float2 uv)
{
    float sineOscillation = sin(12. * uTime - 3. * uv.y);
    float distanceFromCentre = abs(uv.x - 0.5);
    
        // Calculate the opacity of the point using an exponential function to make the opacity decrease drastically.
        // A sine is used to vary the exponent to produce a pulsing effect.
    
    float opacity = pow(1. - distanceFromCentre, sineOscillation + 40.);
    
        // Adjust red and white values to achieve the desired effect.

    float red = 2.5 - distanceFromCentre;
    float white = 0.15 * sineOscillation + 0.8 - distanceFromCentre * 2.;
    
    float output = float4(red, white, white, 1) * opacity;
    
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;
    float4 output = sineWaves(uv);
    
    float sineOscillation = sin(12. * uTime - 3. * uv.y);
    float distanceFromCentre = abs(uv.x - 0.5);
    
        // Calculate the opacity of the point using an exponential function to make the opacity decrease drastically.
        // A sine is used to vary the exponent to produce a pulsing effect.
    
    float opacity = pow(1. - distanceFromCentre, sineOscillation + 10.);
    
        // Adjust red and white values to achieve the desired effect.

    float red = 2.5 - distanceFromCentre;
    float white = 0.15 * sineOscillation + 0.8 - distanceFromCentre * 2.;
    
    output = float4(red, white, white, 1) * opacity;
    
    if (opacity < 0.3)
    {
        output = sineWaves(uv);
    }
    return output;

}



technique BasicColorDrawing
{
	pass P0
	{
		//VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};