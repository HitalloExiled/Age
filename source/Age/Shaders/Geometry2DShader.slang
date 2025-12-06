#pragma once

import Modules.Common;

import "./Geometry2DShader.Flags";
import "./Geometry2DShader.BorderSide";
import "./Geometry2DShader.PushConstant";

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
    vertex *= data.size;
    vertex = (mul(float3(vertex, 1), matrix)).xy;
    vertex.y *= -1;
    vertex /= data.viewport;
    vertex = vertex * 2 - 1;

    var uv = data.uv; // For some reason the Intel GPU doesn't seem to support directly assigning push constant values ​​to the output of the shader stage

    return
    {
        float4(vertex, 1, 1),
        uv[input.index]
    };
}
