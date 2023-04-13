float uTime;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    //float blue = lerp(0, 1, (sin(uTime * 0.1) + 1)/ 2);
    
    float3 red = float3(1, 0, 0);
    float3 green = float3(0, 1, 0);
    
    float3 test = lerp(red, green, coords.x);
    
    return float4(test, 1);
}

Technique Technique1
{
    pass ShaderPass
    {       
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
