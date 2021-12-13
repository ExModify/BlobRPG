﻿#version 330

in vec2 pass_textureCoords;

out vec4 out_Color;

uniform vec3 color;
uniform sampler2D fontAtlas;

uniform float width;
uniform float edge;

uniform float borderWidth;
uniform float borderEdge;
uniform vec3 outlineColor;

uniform vec2 offset;

void main(void){

	float distance = 1.0 - texture(fontAtlas, pass_textureCoords).a;
	float alpha = 1 - smoothstep(width, width + edge, distance);

	float distance2 = 1.0 - texture(fontAtlas, pass_textureCoords + offset).a;
	float outlineAlpha = 1 - smoothstep(borderWidth, borderWidth + borderEdge, distance2);

	float overallAlpha = alpha + (1.0 - alpha) * outlineAlpha;
	vec3 overallColor = mix(outlineColor, color, alpha / overallAlpha);
	
	out_Color = vec4(overallColor, overallAlpha);
}