#version 400

in vec2 textureCoords;
uniform sampler2D tex;

layout (location = 0) out vec4 out_Color;
layout (location = 1) out vec4 out_BrightColor;

void main(void) {

	out_Color = texture(tex, textureCoords);
	out_BrightColor = vec4(7.0);
}