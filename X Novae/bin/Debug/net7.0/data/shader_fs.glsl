#version 460 core

#extension GL_ARB_bindless_texture : require

out vec4 FragColor;

in VS_OUT
{
	vec2 texcoord;
} fs_in;

layout(std430, binding = 4) buffer MaterialData
{
	sampler2D diffuse;
};

void main()
{
	vec4 diffuseColor = texture(diffuse, fs_in.texcoord);
	FragColor = diffuseColor;
}