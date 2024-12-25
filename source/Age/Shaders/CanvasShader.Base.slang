#pragma once

import Common;

import "CanvasShader.Flags";
import "CanvasShader.BorderSide";
import "CanvasShader.PushConstant";

public struct VertexInput
{
    public float2 position : LOCATION0;
    public uint   index    : SV_VertexID;
}

public struct VertexOutput
{
    public float4 position : SV_POSITION;
    public float2 uv       : LOCATION0;
}

public struct FragmentInput
{
    public float2 uv : LOCATION0;
}

public struct FragmentOutput
{
    public float4 color : SV_Target;
}

[vk::binding(0)]
Sampler2D texture;

[vk::push_constant]
PushConstant data;

[shader("vertex")]
public VertexOutput main(in VertexInput input)
{
    var matrix = data.transform.toFloat3x3();

    var vertex = input.position / 2 + 0.5;
    vertex.y *= -1;
    vertex *= data.rect.size;
    vertex += data.rect.position;
    vertex = (mul(float3(vertex, 1), matrix)).xy;
    vertex.y *= -1;
    vertex /= data.viewport;
    vertex = vertex * 2 - 1;

    return
    {
        float4(vertex, 1, 1),
        data.uv[input.index]
    };
}
