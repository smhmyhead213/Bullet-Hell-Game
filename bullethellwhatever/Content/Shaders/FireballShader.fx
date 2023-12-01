sampler mainTexture : register(s0);
//sampler2D noise : register(s1);
texture noiseMap;
texture randomNoiseMap;

sampler2D noiseSampler = sampler_state
{
    Texture = <noiseMap>;
};

sampler2D randomNoiseSampler = sampler_state
{
    Texture = <randomNoiseMap>;
};

matrix worldViewProjection;

float scrollSpeed;
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
    //mainTexture needs to be used to occupy register 0
    
    float2 dummy = tex2D(mainTexture, 0.3) * 0.001f;
    
    float2 uv = input.TextureCoordinates + dummy;
    
    float2 centre = float2(0.5, 0.5); // define the centre of the shader

    float2 noiseCoord = uv + float2(uTime * scrollSpeed, 0); // decide where to sample from the noisemap
    
    float distanceFromCentre = sqrt(pow(uv.x - centre.x, 2.) + pow(uv.y - centre.y, 2.)); // calculate how far we are from the centre

    float maxVariance = 0.2; // maximum variance in colour
    
    float colourVariance = tex2D(randomNoiseSampler, uv).r * (maxVariance * 2.) - maxVariance; // calculate the variance in colour
    
    float3 colour = float3(1, 0.65 + colourVariance, 0); // calculate the final colour
    
    float maxFadeDistanceVariance = 0.1; // maximum fade distance variance
    
    // multiply the random noise sample coord by time so the fireball "waves". a decimal is included to prevent integer wrapping when it goes to the same part of the texture after havinging looped back when the sample coord goes over 1
    
    distanceFromCentre = distanceFromCentre + tex2D(randomNoiseSampler, uv * (uTime * 0.35342)).r * (maxFadeDistanceVariance * 2.) - maxFadeDistanceVariance; // calculate the new distance from centre using the new fade distance variation
    
    float opacity = 1. - 2. * distanceFromCentre; // calculate opacity based on distance
    
    float4 noise = tex2D(noiseSampler, noiseCoord); // sample noise

    return (float4(colour, 1.) + noise) * opacity;   
}

Technique Technique1
{
    pass ShaderPass
    {
        //VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
        
    }
}
