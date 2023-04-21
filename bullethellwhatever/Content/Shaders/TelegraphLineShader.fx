sampler mainTexture : register(s0);

matrix worldViewProjection;

float uTime;
float AngularVelocity;

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
    float distanceFromCentre = abs(input.TextureCoordinates.x - 0.5);
    
    float opacity = pow(1 - distanceFromCentre, 7);
    
    return (1, 1, 0, 1) * opacity * 0.35;
    
}

Technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
