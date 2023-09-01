sampler mainTexture : register(s0);

matrix worldViewProjection;

float uTime;
float AngularVelocity;
int duration;

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
    float xDistFromCentre = abs(0.5 - input.TextureCoordinates.x);
    float value = pow(2.718, 14. * xDistFromCentre - 7.) + 0.1;
    //float3 col = float3(value, value, value);
    
    float opacity = 1;
    
    //if (uTime < duration / 10)
    //{
    //    opacity = opacity * lerp(0, 1, uTime / (duration / 10));
    //}
    
    //if (uTime > duration / 10 * 9)
    //{
    //    opacity = opacity * lerp(0, 1, (duration - uTime) / (duration / 10));
    //}
    
    return (value, value, value, 1);
    
}

Technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
