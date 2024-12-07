import shader_b;

public struct A
{
    public B B;
    public float Value;
};

float4 main(float4 position : POSITION) : SV_POSITION
{
    var a = A();

    return mul(a.B.C.Value, position);
}
