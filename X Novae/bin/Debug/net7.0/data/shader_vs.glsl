#version 460 core

#extension GL_ARB_bindless_texture : require

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;
layout (location = 3) in vec3 aBinormal;
layout (location = 4) in vec3 aTangent;

layout(std430, binding = 3) buffer ShaderData
{
	mat4 model;
	mat4 projection;
	mat4 view;
};

out VS_OUT
{
	vec2 texcoord;
} vs_out;

void main()
{
	vs_out.texcoord = aTexCoord;
	gl_Position = model * vec4(aPos, 1.0);
}