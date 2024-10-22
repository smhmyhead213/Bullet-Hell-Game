sampler mainTexture : register(s0);

texture noiseMap;
sampler noiseMapSampler : register(s1);

matrix worldViewProjection;

float uTime;
int duration;
int direction; // 1 or -1
float3 colour;
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
    float centreX = 0.5;
    float centreY = 0.5;
    float distanceFromCenterX = abs(centreX - uv.x);
    float distanceFromCenterY = abs(centreY - uv.y);
    float colourContribX = centreX - distanceFromCenterX;
    // contribute less y as we reach top and bottom
    float colourContribY = centreY - distanceFromCenterY;
    colourContribX = pow(colourContribX + 0.5, 4);
    colourContribY = pow(colourContribY, 1);
    float saturation = colourContribX + colourContribY;
    return float4(saturation, saturation, saturation, 0);  
}

Technique Technique1
{
    pass ShaderPass
    {
        //VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
        
    }
}
