sampler mainTexture : register(s0);

texture noiseMap;
sampler noiseMapSampler : register(s1);

matrix worldViewProjection;

float uTime;
int direction; // 1 or -1

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
    //direction = 1;
    
    //float2 uv = input.TextureCoordinates;

    ////float opacity = tex2D(noiseMap, float2(frac(uv.x + uTime * direction), uv.y)).b;

    ////return float4(1, 0, 0, 1) * opacity;
    
    //return tex2D(noiseMapSampler, float2(frac(uv.x + uTime * direction), uv.y));
    
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;
    // Get the pixel from the provided streak/fade map.
    float4 fadeMapColor = tex2D(noiseMapSampler, float2(frac(coords.x * 5 - uTime * 2.5), coords.y));
    
    // Define the shape fadeout.
    float bloomFadeout = pow(sin(coords.y * 3.141), 6);

    // Calcuate the grayscale version of the pixel and use it as the opacity.
    float opacity = (fadeMapColor.r + 0.5) * bloomFadeout;
    // Lerp between the base color, and the provided one.
    //float4 colorCorrected = lerp(color, float4(uColor, opacity), opacity);

    return opacity * 2.5;
    
    //return float4(1, 0, 0, 1);
    
}

Technique Technique1
{
    pass ShaderPass
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
