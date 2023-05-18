#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
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
    float thickness = 1.5;
    float squish = 30;
    float frequency = 3.14 / 60000;
    float tolerance = 0.05;
    
    float sine = (thickness / 3. * sin(squish * uv.y - (frequency * uTime)) + 1.) / 2.;
    float negativeSine = (-thickness / 3. * sin(squish * uv.y - (frequency * uTime)) + 1.) / 2.;
    
    if (uv.x > sine - tolerance && uv.x < sine + tolerance)
    {
        return float4(1, 1, 1, 0);
    }
    else if (uv.x > negativeSine - tolerance && uv.x < negativeSine + tolerance)
    {
        return float4(1, 1, 1, 0);
    }
    else
        return float4(0, 0, 0, 0);    
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;
    float4 output = sineWaves(uv);
    
    if (output.r == 0 && output.g == 0 && output.b == 0 && output.a == 0)
    {
        float sineOscillation = sin(12. * uTime - 3. * uv.y);
        float distanceFromCentre = abs(uv.x - 0.5);
    
        // Calculate the opacity of the point using an exponential function to make the opacity decrease drastically.
        // A sine is used to vary the exponent to produce a pulsing effect.
    
        float opacity = pow(1. - distanceFromCentre, sineOscillation + 40.);
    
        // Adjust red and white values to achieve the desired effect.
    
        //float red =  7. * pow(4., uv.x);
        //float white = 1. - pow(5. * sin(uv.x) + 15., abs(uv.x - 0.5));
        
        float red = 2.5 - distanceFromCentre;
        float white = 0.15 * sineOscillation + 0.8 - distanceFromCentre * 2.;
    
        output = float4(red, white, white, 1) * opacity;
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