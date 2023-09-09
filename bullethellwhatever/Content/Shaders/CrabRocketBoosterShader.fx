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
    float2 uv = input.TextureCoordinates;
    
    float xDistFromCentre = abs(0.5 - uv.x);
    //float thickness = 5. / (uv.y * 30.0);
    float thickness = 0.9 + (uv.y * 10.);
    float opacity = pow(1. - xDistFromCentre, thickness);
    
    //float value = 1. - pow(2.718, thickness * xDistFromCentre - (thickness / 2.));
    //float value  = 1. - uv.y;
    //value = value / 2.;
    
    float value = 1.;
    
    float3 col = float3(value, value / 2., 0.);

    return float4(col, 1) * opacity;
    
}

Technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
