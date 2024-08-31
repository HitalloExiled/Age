using Age.Resources.Loaders.Wavefront;
using Age.Resources.Loaders.Wavefront.Exceptions;
using Age.Resources.Loaders.Wavefront.Parsers;

namespace Age.Tests.Loaders.Wavefront.Parsers;

public partial class ObjParserTest
{
    public record ValidScenario
    {
        public required string    Source    { get; set; }
        public required Data      Expected  { get; set; }
        public Material[]         Materials { get; set; } = [];
        public ObjParser.Options? Options   { get; set; }
        public bool               Skip      { get; set; }
    }

    public record InvalidScenario
    {
        public required string         Source   { get; set; }
        public required ParseException Expected { get; set; }
        public bool                    Skip     { get; set; }
    }

    public static class Scenarios
    {
        private const bool SKIP = false;

        private static readonly ValidScenario[] valid =
        [
            new()
            {
                Source   = "",
                Expected = new() { Objects = [new("test_object")] },
                Skip     = SKIP,
            },
            new()
            {
                Source   = "# this is a comment",
                Expected = new() { Objects = [new("test_object")] },
                Skip     = SKIP,
            },
            new()
            {
                Source =
                    """
                    v 1.0  0.0  0.0
                    v 0.0 -1.0  0.0
                    v 0.0  0.0  1.0  0.5
                    v 1.0  0.0  1.0  0.2  0.5  0.7
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        Colors =
                        {
                            new(0.2f, 0.5f, 0.7f),
                        },
                        Vertices =
                        {
                            new(1,  0,  0,    1),
                            new(0, -1,  0,    1),
                            new(0,  0,  1, 0.5f),
                            new(1,  0,  1,    1),
                        },
                    },
                    Objects =
                    [
                        new("test_object")
                        {
                            Mesh = new()
                            {
                                Vertices = [0, 1, 2, 3],
                            }
                        }
                    ]
                },
                Skip = SKIP,
            },
            new()
            {
                Source =
                    """
                    vt 1
                    vt 0.5 0.3
                    vt 0.3 0.4 1
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        TexCoords =
                        [
                            new(   1,    0, 0),
                            new(0.5f, 0.3f, 0),
                            new(0.3f, 0.4f, 1)
                        ],
                    },
                    Objects = [new("test_object")]
                },
                Skip = SKIP,
            },
            new()
            {
                Source =
                    """
                    vn 0.707 0.000 0.707
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        Normals  = [new(0.707f, 0, 0.707f)],
                    },
                    Objects = [new("test_object")]
                },
                Skip = SKIP
            },
            new()
            {
                Source =
                    """
                    v 1.0 0.0 0.0
                    v 0.0 1.0 0.0
                    l 1 2
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        Vertices =
                        [
                            new(1, 0, 0, 1),
                            new(0, 1, 0, 1),
                        ],
                    },
                    Objects =
                    [
                        new("test_object")
                        {
                            Mesh = new()
                            {
                                Lines = [new(0, 1)],
                            }
                        }
                    ]
                },
                Skip = SKIP
            },
            new()
            {
                Source =
                    """
                    v 1.0 0.0 0.0
                    v 0.0 1.0 0.0
                    l -1 2
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        Vertices =
                        [
                            new(1, 0, 0, 1),
                            new(0, 1, 0, 1),
                        ],
                    },
                    Objects =
                    [
                        new("test_object")
                        {
                            Mesh = new()
                            {
                                Lines = [new(0, 1)],
                            }
                        }
                    ]
                },
                Skip = SKIP
            },
            new()
            {
                Source =
                    """
                    v 1.0 0.0 0.0
                    v 0.0 1.0 0.0
                    v 0.0 0.0 1.0
                    l 1 2
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        Vertices =
                        [
                            new(1, 0, 0, 1),
                            new(0, 1, 0, 1),
                            new(0, 0, 1, 1),
                        ],
                    },
                    Objects =
                    [
                        new("test_object")
                        {
                            Mesh = new()
                            {
                                Lines    = [new(0, 1)],
                                Vertices = [2]
                            }
                        }
                    ]
                },
                Skip = SKIP
            },
            new()
            {
                Source =
                    """
                    v -1.0 0.0 0.0
                    v 1.0 0.0 1.0
                    v 1.0 0.0 -1.0
                    f 1 2 3
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        Vertices =
                        [
                            new(-1, 0,  0, 1),
                            new(1,  0,  1, 1),
                            new(1,  0, -1, 1),
                        ],
                    },
                    Objects =
                    [
                        new("test_object")
                        {
                            Mesh = new()
                            {
                                Faces =
                                [
                                    new()
                                    {
                                        Indices =
                                        [
                                            new(0, -1, -1, -1),
                                            new(1, -1, -1, -1),
                                            new(2, -1, -1, -1),
                                        ]
                                    }
                                ],
                            }
                        }
                    ]
                },
                Skip = SKIP
            },
            new()
            {
                Source =
                    """
                    v -1.0 0.0 0.0
                    v 1.0 0.0 1.0
                    v 1.0 0.0 -1.0
                    v 0 0 0
                    f 1 2 3
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        Vertices =
                        [
                            new(-1, 0,  0, 1),
                            new(1,  0,  1, 1),
                            new(1,  0, -1, 1),
                            new(0,  0,  0, 1),
                        ],
                    },
                    Objects =
                    [
                        new("test_object")
                        {
                            Mesh = new()
                            {
                                Faces =
                                [
                                    new()
                                    {
                                        Indices =
                                        [
                                            new(0, -1, -1, -1),
                                            new(1, -1, -1, -1),
                                            new(2, -1, -1, -1),
                                        ]
                                    }
                                ],
                                Vertices = [3]
                            }
                        }
                    ]
                },
                Skip = SKIP
            },
            new()
            {
                Source =
                    """
                    v -1.0 0.0 0.0
                    v 1.0 0.0 1.0
                    v 1.0 0.0 -1.0
                    v 0 0 0
                    f -3 -2 -1
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        Vertices =
                        [
                            new(-1, 0,  0, 1),
                            new(1,  0,  1, 1),
                            new(1,  0, -1, 1),
                            new(0,  0,  0, 1),
                        ],
                    },
                    Objects =
                    [
                        new("test_object")
                        {
                            Mesh = new()
                            {
                                Faces =
                                [
                                    new()
                                    {
                                        Indices =
                                        [
                                            new(0, -1, -1, -1),
                                            new(1, -1, -1, -1),
                                            new(2, -1, -1, -1),
                                        ]
                                    }
                                ],
                                Vertices = [3]
                            }
                        }
                    ]
                },
                Skip = SKIP
            },
            new()
            {
                Source =
                    """
                    v -1.0 0.0 0.0
                    v 1.0 0.0 1.0
                    v 1.0 0.0 -1.0
                    vt 0.0 0.0
                    f 1/1 2/1 3/1
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        TexCoords =
                        [
                            new(0, 0, 0)
                        ],
                        Vertices =
                        [
                            new(-1, 0,  0, 1),
                            new(1,  0,  1, 1),
                            new(1,  0, -1, 1),
                        ],
                    },
                    Objects =
                    [
                        new("test_object")
                        {
                            Mesh = new()
                            {
                                Faces =
                                [
                                    new()
                                    {
                                        Indices =
                                        [
                                            new(0, 0, -1, -1),
                                            new(1, 0, -1, -1),
                                            new(2, 0, -1, -1),
                                        ]
                                    }
                                ],
                            }
                        }
                    ]
                },
                Skip = SKIP
            },
            new()
            {
                Source =
                    """
                    v -1.0 0.0  0.0
                    v  1.0 0.0  1.0
                    v  1.0 0.0 -1.0
                    vt 1.0 0.0
                    vn 0.0 1.0 0.0
                    f 1/1/1 2/1/1 3/1/1
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        Normals   = [new(0, 1, 0)],
                        TexCoords = [new(1, 0, 0)],
                        Vertices =
                        [
                            new(-1, 0,  0, 1),
                            new(1,  0,  1, 1),
                            new(1,  0, -1, 1),
                        ],
                    },
                    Objects =
                    [
                        new("test_object")
                        {
                            Mesh = new()
                            {
                                Faces =
                                [
                                    new()
                                    {
                                        Indices =
                                        [
                                            new(0, 0, 0, -1),
                                            new(1, 0, 0, -1),
                                            new(2, 0, 0, -1),
                                        ]
                                    }
                                ],
                            }
                        }
                    ]
                },
                Skip = SKIP
            },
            new()
            {
                Source =
                    """
                    v -1.0 0.0  0.0
                    v  1.0 0.0  1.0
                    v  1.0 0.0 -1.0
                    vn 0.0 1.0 0.0
                    f 1//1 2//1 3//1
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        Normals   = [new(0, 1, 0)],
                        Vertices =
                        [
                            new(-1, 0,  0, 1),
                            new(1,  0,  1, 1),
                            new(1,  0, -1, 1),
                        ],
                    },
                    Objects =
                    [
                        new("test_object")
                        {
                            Mesh = new()
                            {
                                Faces =
                                [
                                    new()
                                    {
                                        Indices =
                                        [
                                            new(0, -1, 0, -1),
                                            new(1, -1, 0, -1),
                                            new(2, -1, 0, -1),
                                        ]
                                    }
                                ],
                            }
                        }
                    ]
                },
                Skip = SKIP
            },
            new()
            {
                Source =
                    """
                    mtllib foo.mtl
                    o foo
                    v -1.0 0.0  0.0
                    v  1.0 0.0  1.0
                    v  1.0 0.0 -1.0
                    vn 0.0 1.0 0.0
                    s 0
                    s off
                    s null
                    usemtl unknown_material
                    f 1//1 2//1 3//1
                    o bar
                    usemtl material
                    v -2.0 0.0  0.0
                    v  2.0 0.0  2.0
                    v  2.0 0.0 -2.0
                    vn 1.0 0.0 0.0
                    f 4//1 5//1 6//1
                    """,
                Materials = [new("material")],
                Options   = new() { SplitByObject = true },
                Expected  = new()
                {
                    Attributes = new()
                    {
                        Materials = [new("material")],
                        Normals   =
                        [
                            new(0, 1, 0),
                            new(1, 0, 0),
                        ],
                        Vertices =
                        [
                            new(-1, 0,  0, 1),
                            new(1,  0,  1, 1),
                            new(1,  0, -1, 1),
                            new(-2, 0,  0, 1),
                            new(2,  0,  2, 1),
                            new(2,  0, -2, 1),
                        ],
                    },
                    Objects =
                    [
                        new("foo")
                        {
                            Mesh = new()
                            {
                                Faces =
                                [
                                    new()
                                    {
                                        Indices =
                                        [
                                            new(0, -1, 0, -1),
                                            new(1, -1, 0, -1),
                                            new(2, -1, 0, -1),
                                        ]
                                    }
                                ],
                            }
                        },
                        new("bar")
                        {
                            Mesh = new()
                            {
                                Faces =
                                [
                                    new()
                                    {
                                        Indices =
                                        [
                                            new(3, -1, 0, -1),
                                            new(4, -1, 0, -1),
                                            new(5, -1, 0, -1),
                                        ],
                                        Material = 0,
                                    }
                                ],
                            }
                        }
                    ]
                },
                Skip = SKIP
            },
            new()
            {
                Source =
                    """
                    g Plane_Plane.001
                    v -1.0 0.0 0.0
                    v  1.0 0.0 1.0
                    v  1.0 0.0 -1.0
                    vn -0.0000 1.0000 -0.0000
                    vt 0.0 0.0
                    vt 1.0 0.0
                    vt 1.0 1.0
                    f 1/1/1 2/2/1 3/3/1
                    """,
                Expected = new()
                {
                    Attributes = new()
                    {
                        Groups    =
                        [
                            new("Plane_Plane.001")
                            {
                                Faces =
                                [
                                    new()
                                    {
                                        Group   = 0,
                                        Indices =
                                        [
                                            new(0, 0, 0, -1),
                                            new(1, 1, 0, -1),
                                            new(2, 2, 0, -1),
                                        ]
                                    }
                                ],
                                Vertices = [0, 1, 2],
                            }
                        ],
                        Normals   = [new(0, 1, 0)],
                        TexCoords =
                        [
                            new(0, 0, 0),
                            new(1, 0, 0),
                            new(1, 1, 0),
                        ],
                        Vertices =
                        [
                            new(-1, 0,  0, 1),
                            new(1,  0,  1, 1),
                            new(1,  0, -1, 1),
                        ],
                    },
                    Objects =
                    [
                        new("test_object")
                        {
                            Mesh = new()
                            {
                                Faces =
                                [
                                    new()
                                    {
                                        Group = 0,
                                        Indices =
                                        [
                                            new(0, 0, 0, -1),
                                            new(1, 1, 0, -1),
                                            new(2, 2, 0, -1),
                                        ],

                                    }
                                ],
                            },
                        }
                    ]
                },
                Skip = SKIP
            },
            new()
            {
                Source =
                    """
                    g Plane_Plane.001
                    v -1.0 0.0 0.0 1.0 0.0 0.0
                    v  1.0 0.0 1.0
                    v  1.0 0.0 -1.0
                    vn -0.0000 1.0000 -0.0000
                    vt 0.0 0.0
                    vt 1.0 0.0
                    vt 1.0 1.0
                    s 0
                    g off
                    f 1/1/1 2/2/1 3/3/1
                    """,
                Options = new() { SplitByGroup = true },
                Expected = new()
                {
                    Attributes = new()
                    {
                        Colors    = [new(1, 0, 0)],
                        Groups    =
                        [
                            new("Plane_Plane.001")
                            {
                                Faces    = [],
                                Vertices = [0, 1, 2]
                            },
                        ],
                        Normals   = [new(0, 1, 0)],
                        TexCoords =
                        [
                            new(0, 0, 0),
                            new(1, 0, 0),
                            new(1, 1, 0),
                        ],
                        Vertices =
                        [
                            new(-1, 0,  0, 1),
                            new(1,  0,  1, 1),
                            new(1,  0, -1, 1),
                        ],
                    },
                    Objects =
                    [
                        new("off")
                        {
                            Mesh = new()
                            {
                                Faces =
                                [
                                    new()
                                    {
                                        Indices =
                                        [
                                            new(0, 0, 0,  0),
                                            new(1, 1, 0, -1),
                                            new(2, 2, 0, -1),
                                        ],
                                    }
                                ],
                            },
                        }
                    ]
                },
                Skip = SKIP,
            },
            new()
            {
                Source =
                """
                mtllib foo.mtl
                mtllib "bar.mtl"
                """,
                Materials = [new("bar"), new("baz")],
                Expected  = new()
                {
                    Attributes = new()
                    {
                        Materials = [new("bar"), new("baz")],
                    },
                    Objects = [new("test_object")]
                }
            }
        ];

        private static readonly InvalidScenario[] invalid =
        [
            new()
            {
                Source   = "l",
                Expected = new("Unexpected end of expression", 1, 2, 1),
                Skip     = SKIP,
            },
            new()
            {
                Source   = "1",
                Expected = new("Unexpected number", 1, 1, 0),
                Skip     = SKIP,
            },
            new()
            {
                Source   = "foo",
                Expected = new("Unexpected token foo", 1, 1, 0),
                Skip     = SKIP,
            },
            new()
            {
                Source   = "s x",
                Expected = new("Unexpected token x", 1, 3, 2),
                Skip     = SKIP,
            },
            new()
            {
                Source   = "l x",
                Expected = new("Unexpected token x", 1, 3, 2),
                Skip     = SKIP,
            },
            new()
            {
                Source   = "vt -1",
                Expected = new("Value out of range 0..1", 1, 4, 3),
                Skip     = SKIP,
            },
            new()
            {
                Source   = "vt 2",
                Expected = new("Value out of range 0..1", 1, 4, 3),
                Skip     = SKIP,
            },
            new()
            {
                Source   = "vt x",
                Expected = new("Unexpected token x", 1, 4, 3),
                Skip     = SKIP,
            },
            new()
            {
                Source   = "vt 1 1 1 1",
                Expected = new("Unexpected number", 1, 10, 9),
                Skip     = SKIP,
            },
        ];

        public static TheoryData<ValidScenario>   Valid   => new(valid);
        public static TheoryData<InvalidScenario> Invalid => new(invalid);
    }
}
