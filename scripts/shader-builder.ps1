# TODO Make dynamic
glslc source/Age.Playground/Shaders/shader.vert -o source/Age.Playground/Shaders/shader.vert.spv;
glslc source/Age.Playground/Shaders/shader.frag -o source/Age.Playground/Shaders/shader.frag.spv;
glslc source/Age.Rendering/Shaders/shader.vert -o source/Age.Rendering/Shaders/shader.vert.spv;
glslc source/Age.Rendering/Shaders/shader.frag -o source/Age.Rendering/Shaders/shader.frag.spv;

Write-Output "Shader compilation done!";
