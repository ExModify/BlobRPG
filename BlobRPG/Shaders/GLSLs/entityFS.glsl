﻿#version 400 core

in vec2 pass_textureCoords;

uniform sampler2D textureSampler;

out vec4 out_Color;

void main(void)
{
	out_Color = texture(textureSampler, pass_textureCoords);
}