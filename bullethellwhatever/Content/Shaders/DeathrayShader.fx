sampler mainTexture : register(s0);

texture noiseMap;
sampler noiseMapSampler : register(s1);

matrix worldViewProjection;

float uTime;
int duration;
int direction; // 1 or -1
//float timeToStartFadingOut = duration / 10f * 9f;

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

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, worldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{   
    float2 uv = input.TextureCoordinates;
    
    // Calculate how far from the centre of the beam the co-ordinate is.
    
    float distanceFromCentre = abs(uv.x - 0.5);
    
    // Calculate the opacity of the point using an exponential function to make the opacity decrease drastically.
    // A sine is used to vary the exponent to produce a pulsing effect.
    
    float opacity = pow(1 - distanceFromCentre, sin(uTime / 2 - uv.y * 20) + 5);
    
    // Adjust red and white values to achieve the desired effect.
    
    float red = 1.4 - distanceFromCentre;
    float white = 0.75 - distanceFromCentre * 2;
    
    if (uTime < duration / 10)
    {
        opacity = opacity * lerp(0, 1, uTime / (duration / 10));
    }
    
    if (uTime > duration / 10 * 9)
    {       
        opacity = opacity * lerp(0, 1, (duration - uTime) / (duration / 10));
    }
    
    return float4(red, white, white, 1) * opacity;
    
}

Technique Technique1
{
    pass ShaderPass
    {
        //VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
        
    }
}
