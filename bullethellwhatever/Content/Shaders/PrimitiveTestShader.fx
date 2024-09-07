﻿sampler mainTexture : register(s0);

matrix worldViewProjection;

float uTime;
float AngularVelocity;
int duration;
float3 colour;

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
    float4 black = float4(0, 0, 0, 1);
    float4 red = float4(1, 0, 0, 1);
    float2 uv = input.TextureCoordinates;
    
    if (uv.x == 0 && uv.y == 0)
    {
        return red;
    }
    else
    {
        return black;
    }
}

Technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
