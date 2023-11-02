// using Age.Tests.Common;

// namespace Age.Tests.Loaders.WavefrontObj;

// public class LoaderTest
// {
//     private static readonly string cwd = Directory.GetCurrentDirectory();
//     private readonly VirtualPlatform virtualPlatform = new();

//     public LoaderTest()
//     {
//         var directory = new VirtualDirectory(
//             "Models",
//             null,
//             new VirtualFile[]
//             {
//                 new(
//                     "cube.obj",
//                     """
//                     # Simple Wavefront file
//                     v 0.0 0.0 0.0
//                     v 0.0 1.0 0.0
//                     v 1.0 0.0 0.0
//                     f 1 2 3
//                     """
//                 )
//             }
//         );

//         this.virtualPlatform.Setup(directory);
//     }

//     private static string FullPath(string path) =>
//         Path.GetFullPath(Path.Join(cwd, path));

//     [Fact]
//     public void Load()
//     { }
// }
