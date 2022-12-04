using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

// TODO - Analyze whether there is real gain when using generic types in core structures

namespace Age.Core.Math;

[Serializable]
[DataContract]
internal record struct Basis<T> where T : notnull, INumber<T>, IRootFunctions<T>
{
    [DataMember]
    private readonly Vector3<T>[] rows = new Vector3<T>[3];

    /// <summary>
    /// Indexer for the components of this vector.
    /// </summary>
    /// <param name="i">The component to select. Zero based.</param>
    [IgnoreDataMember]
    public Vector3<T> this[int i]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.rows[i];
    }

    [IgnoreDataMember]
    public T this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.rows[x][y];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Basis(Vector3<T> x, Vector3<T> y, Vector3<T> z) => (this.rows[0], this.rows[1], this.rows[2]) = (x, y, z);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Basis<T> Inverse()
    {
        var rows = this.rows;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        T factory(int row1, int col1, int row2, int col2) =>
            rows[row1][col1] * rows[row2][col2] - rows[row1][col2] * rows[row2][col1];

        var co = new[] { factory(1, 1, 2, 2), factory(1, 2, 2, 0), factory(1, 0, 2, 1) };
        var det =
            this.rows[0][0] * co[0] +
            this.rows[0][1] * co[1] +
            this.rows[0][2] * co[2];

        // #ifdef MATH_CHECKS
        //     ERR_FAIL_COND(det == 0);
        // #endif

        var s = (T)(object)1.0f / det;

        return new
        (
            new Vector3<T>(co[0] * s, factory(0, 2, 2, 1) * s, factory(0, 1, 1, 2) * s),
			new Vector3<T>(co[1] * s, factory(0, 0, 2, 2) * s, factory(0, 2, 1, 0) * s),
			new Vector3<T>(co[2] * s, factory(0, 1, 2, 0) * s, factory(0, 0, 1, 1) * s)
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Basis<T> Transposed()
    {
        var rows = new[,]
        {
            { this.rows[0][0], this.rows[0][1], this.rows[0][2] },
            { this.rows[1][0], this.rows[1][1], this.rows[1][2] },
            { this.rows[2][0], this.rows[2][1], this.rows[2][2] },
        };

        (rows[0, 1], rows[1, 0]) = (rows[1, 0], rows[0, 1]);
        (rows[0, 2], rows[2, 0]) = (rows[2, 0], rows[0, 2]);
        (rows[1, 2], rows[2, 1]) = (rows[2, 1], rows[1, 2]);

        return new(
            new Vector3<T>(rows[0, 0], rows[0, 1], rows[0, 2]),
            new Vector3<T>(rows[1, 0], rows[1, 1], rows[1, 2]),
            new Vector3<T>(rows[2, 0], rows[2, 1], rows[2, 2])
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Vector3<T> XForm(Vector3<T> vector) =>
        new(
            this.rows[0].Dot(vector),
            this.rows[1].Dot(vector),
            this.rows[2].Dot(vector)
        );
}
