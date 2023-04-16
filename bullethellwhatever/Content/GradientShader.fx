sampler mainTexture : register(s0);

matrix worldViewProjection;
float uTime;
float bossHPRatio;
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
    float offsetDueToVelocity = input.TextureCoordinates.y / 10.0;
    
    float minimumWidth = lerp(0.1, 0.5, input.TextureCoordinates.y) - offsetDueToVelocity;
    float maximumWidth = lerp(0.9, 0.5, input.TextureCoordinates.y) - offsetDueToVelocity;
    
    if (input.TextureCoordinates.x > minimumWidth && input.TextureCoordinates.x < maximumWidth)
    {
        return float4(1 - input.TextureCoordinates.y, 0, 0, 0.3);
    }
    else
        return float4(0, 0, 0, 0);
    
}

Technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
